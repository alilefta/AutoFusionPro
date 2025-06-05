using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.Navigation;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.Navigation;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Categories.Dialogs;
using AutoFusionPro.UI.Views.Categories.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.ViewModels.Categories
{
    public partial class CategoriesViewModel : BaseViewModel<CategoriesViewModel>
    {
        #region Private Fields
        private INavigationService _navigationService;
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;
        private readonly IWpfToastNotificationService _wpfToastNotificationService;
        #endregion

        [ObservableProperty]
        private bool _isInititalized = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _isAdding = false;        
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ViewerIcon))]
        private bool _isCardViewActive = false;


        public SymbolRegular ViewerIcon => IsCardViewActive ? SymbolRegular.AppsList24 : SymbolRegular.Grid28;


        [ObservableProperty]
        private ObservableCollection<CategoryDto> _categoriesCollection;

        [ObservableProperty]
        private CategoryDto _selectedCategory = null;

        public CategoriesViewModel(ICategoryService categoryService, IDialogService dialogService, INavigationService navigationService, IWpfToastNotificationService wpfToastNotificationService, ILocalizationService localizationService, ILogger<CategoriesViewModel> logger) : base(localizationService, logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));

            _categoriesCollection = new ObservableCollection<CategoryDto>();


            _ = InitializeAsync();
        }

         
        #region Initializer & Loading

        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;

                if (!IsInititalized)
                {
                    await LoadTopCategoriesAsync();
                    IsInititalized = true;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadTopCategoriesAsync()
        {
            try
            {
                IsLoading = true;

                CategoriesCollection.Clear();

                IEnumerable<CategoryDto> categories;

                //filters ??= new CompatibleVehicleFilterCriteriaDto(); // Ensure filters is not null
                //CurrentlyAppliedFilters = filters; // Update the VM's current filters

                categories = await _categoryService.GetTopLevelCategoriesAsync(onlyActive: false);

                if (categories != null)
                {
                    foreach (var vehicle in categories) // compatibleVehicles is already IEnumerable<CompatibleVehicleSummaryDto>
                    {
                        CategoriesCollection.Add(vehicle);
                    }
                    _logger.LogInformation("Loaded {Count} Category.", CategoriesCollection.Count);
                }
                else
                {
                    _logger.LogWarning("GetTopLevelCategoriesAsync() returned null.");
                }

            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _wpfToastNotificationService.ShowError("Failed to load Categories. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(LoadTopCategoriesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task ShowAddCategoryDialogAsync()
        {
            try
            {
                IsAdding = true;

                var results = await _dialogService.ShowDialogAsync<AddRootCategoryDialogViewModel, AddRootCategoryDialog>(null);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new compatible vehicle has been added!");

                    await LoadTopCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemAddedSuccessfullyStr"] as string ?? "New Item Added Successfully!";

                    _wpfToastNotificationService.ShowSuccess(msg);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddRootCategoryDialog)}");
                var msg = System.Windows.Application.Current.Resources["CreationOperationFailedStr"] as string ?? "Creation Operation Has Failed!";
                _wpfToastNotificationService.ShowError(msg);

                // DEV ENV ONLY
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowAddCategoryDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditCategoryDialogAsync(CategoryDto? categoryToEdit)
        {
            if (categoryToEdit == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditRootCategoryDialogViewModel, EditRootCategoryDialog>(categoryToEdit);

                if (results.HasValue && results.Value == true)
                {
                    await  LoadTopCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemUpdatedSuccessfullyStr"] as string ?? "Category Updated Successfully!";

                    _wpfToastNotificationService.ShowSuccess(msg);

                    _logger.LogInformation("Transmission Type ID='{ID}' Updated Successfully!", categoryToEdit.Id);
                }

            }
            catch (Exception ex)
            {
                var msg = System.Windows.Application.Current.Resources["UpdateOperationFailedStr"] as string ?? "Update Operation Failed";
                _wpfToastNotificationService.ShowError(msg);
                return;
            }
        }

        [RelayCommand]
        public async Task OpenCategoryDetailsAsync(CategoryDto? categoryDto)
        {
            if (categoryDto == null) return;
            try
            {
                _navigationService.NavigateTo(Core.Enums.NavigationPages.ApplicationPage.CategoryDetails, categoryDto);

            }
            catch(NavigationException nex)
            {
                var msgTitle = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Category {categoryDto.Name} Has Failed";
                var msg = System.Windows.Application.Current.Resources["ReasonIsStr"] as string ?? $"Reason Is";
                _wpfToastNotificationService.ShowError($"{msg} {nex.Message}", msgTitle);
                throw nex;

            }
            catch (Exception ex)
            {
                var msg = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Category {categoryDto.Name} Has Failed";
                _wpfToastNotificationService.ShowError(msg);
                return;
            }
        }

        [RelayCommand]
        public async Task ShowDeleteCategoryDialogAsync(CategoryDto categoryToDelete)
        {
            if (categoryToDelete == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {

                try
                {
                    await _categoryService.DeleteCategoryAsync(categoryToDelete.Id);

                    await LoadTopCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemDeletedSuccessfullyStr"] as string ?? "Item Has Been Deleted Successfully!";


                    _wpfToastNotificationService.ShowSuccess(msg);

                    _logger.LogInformation("Category with ID={ID} has been deleted successfully!", categoryToDelete.Id);

                }
                catch (DeletionBlockedException dx)
                {
                    _logger.LogError(dx, "{ErrorMessage}. Entity: {EntityName}, Key: {EntityKey}, Dependents: {Dependents}",
                        ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, dx.EntityName, dx.EntityKey, string.Join(", ", dx.DependentEntityTypes ?? Enumerable.Empty<string>()));

                    // Construct a more user-friendly message
                    string entityDisplayName = dx.EntityName ?? "the item"; // Fallback
                    string dependentItemsText = "other items"; // Default
                    if (dx.DependentEntityTypes != null && dx.DependentEntityTypes.Any())
                    {
                        dependentItemsText = string.Join(" and ", dx.DependentEntityTypes.Select(FormatDependentTypeName));
                    }

                    var msgTitle = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";
                    var msg = System.Windows.Application.Current.Resources["BecauseTheFollowingItemsDependOnItStr"] as string ?? $"Because the following Items Depends on it:";


                    _wpfToastNotificationService.Show($"{msg}\n{dependentItemsText}", msgTitle, Core.Enums.UI.ToastType.Error,  TimeSpan.FromSeconds(10));
                    return;
                }
                catch (ServiceException ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                    var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                    _wpfToastNotificationService.ShowError(msg);

                    // FOR DEV ONLY
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowDeleteCategoryDialogAsync), MethodOperationType.DELETE_DATA, ex);


                }
                catch(Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                    var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                    _wpfToastNotificationService.ShowError(msg);

                    // FOR DEV ONLY
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowDeleteCategoryDialogAsync), MethodOperationType.DELETE_DATA, ex);
                }
            }
        }

        [RelayCommand]
        private void ChangeView()
        {
            IsCardViewActive = !IsCardViewActive;
        }

        #endregion


        #region Helpers

        private string FormatDependentTypeName(string entityType)
        {
            // Simple pluralization and spacing, can be more sophisticated with localization
            if (entityType.EndsWith("y"))
                return entityType.Substring(0, entityType.Length - 1) + "ies";
            if (entityType.EndsWith("s"))
                return entityType + "es"; // Or handle specific cases
            return entityType + "s";
        }

        #endregion
    }
}
