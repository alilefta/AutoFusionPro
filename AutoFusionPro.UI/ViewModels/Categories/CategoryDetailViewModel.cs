using AutoFusionPro.Application.DTOs.Breadcrumb;
using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.Part;
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
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Categories.Dialogs;
using AutoFusionPro.UI.Views.Categories.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Categories
{
    public partial class CategoryDetailViewModel : InitializableViewModel<CategoryDetailViewModel>
    {
        #region Private Fields
        private readonly IPartService _partService;
        private readonly INavigationService _navigationService;
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;
        private readonly IWpfToastNotificationService _toastNotificationService;
        #endregion

        #region Props

        [ObservableProperty]
        private bool _isAddingSubCategory = false;

        [ObservableProperty]
        private CategoryDto? _currentCategory;

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _subcategories;

        [ObservableProperty]
        private CategoryDto? _selectedSubcategory;

        [ObservableProperty]
        private ObservableCollection<PartSummaryDto> _partsInCategory;

        [ObservableProperty]
        private PartSummaryDto? _selectedPart;

        [ObservableProperty]
        private bool _isLoadingDetails = false;

        [ObservableProperty]
        private bool _isLoadingSubcategories = false;

        [ObservableProperty]
        private bool _isLoadingParts = false;       
        
        [ObservableProperty]
        private bool _isEditingCategory = false;       
        
        [ObservableProperty]
        private bool _isDeletingCategory = false;

        [ObservableProperty]
        private int _activePartsCount;

        [ObservableProperty]
        private int _lowStockPartsCount;

        [ObservableProperty]
        private ObservableCollection<CategoryBreadcrumbDTO> _breadcrumbItems = new();

        public bool HasSubcategories => Subcategories?.Any() ?? false;
        #endregion


        public CategoryDetailViewModel(ICategoryService categoryService, 
            IPartService partService, IDialogService dialogService, 
            IWpfToastNotificationService toastNotificationService, 
            INavigationService navigationService, 
            ILocalizationService localizationService, 
            ILogger<CategoryDetailViewModel> logger) : base(localizationService, logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _toastNotificationService = toastNotificationService ?? throw new ArgumentNullException(nameof(toastNotificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            Subcategories = new ObservableCollection<CategoryDto>();
            PartsInCategory = new ObservableCollection<PartSummaryDto>();

            Subcategories.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasSubcategories));

            RegisterCleanup(() => Subcategories.CollectionChanged -= (s, e) => OnPropertyChanged(nameof(HasSubcategories)));
        }

        #region Initialization and Loading
        /// <summary>
        /// Initialization method provided by <see cref="InitializableViewModel{TViewModel}"/> to load <see cref="CategoryDto"/> parameter required to allow <see cref="CategoryDetailViewModel"/> to work.
        /// </summary>
        /// <param name="parameter"><see cref="CategoryDto"/></param>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ArgumentNullException">If <see cref="CategoryDto"/> param was null</exception>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if(parameter is not CategoryDto categoryDto)
            {
                _logger.LogError("The parameter provided to CategoryDetailViewModel is null or not a CategoryDto.");
                // Potentially close dialog or show error immediately
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Initialization Error", "Cannot edit category: Invalid data provided.", true, CurrentWorkFlow);
                throw new ArgumentNullException(nameof(parameter), "CategoryDto parameter is required for editing.");
            }
            if (IsInitialized) return;

            CurrentCategory = categoryDto;

            await BuildBreadcrumbTrailAsync(categoryDto);

            await LoadSubCategoriesAsync();
            await LoadPartsAsync();

            await base.InitializeAsync(parameter);
        }


        private async Task LoadCurrentCategoryDetailsAsync()
        {
            try
            {
                if (CurrentCategory == null) return;

                IsLoadingDetails = true;

                var category = await _categoryService.GetCategoryByIdAsync(CurrentCategory.Id);

                if (category != null)
                {
                    CurrentCategory = category;
                }
                else
                {
                    _logger.LogWarning("GetCategoryByIdAsync() returned null.");
                    CurrentCategory = null;
                }

            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _toastNotificationService.ShowError("Failed to load category details. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(CategoryDetailViewModel), nameof(LoadCurrentCategoryDetailsAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingDetails = false;
            }
        }

        private async Task LoadSubCategoriesAsync()
        {
            try
            {
                if (CurrentCategory == null) return;

                IsLoadingSubcategories = true;

                Subcategories.Clear();

                IEnumerable<CategoryDto> subCategories;

                subCategories = await _categoryService.GetSubcategoriesAsync(CurrentCategory.Id, false);

                if (subCategories != null)
                {
                    foreach (var category in subCategories)
                    {
                        Subcategories.Add(category);
                    }
                    _logger.LogInformation("Loaded {Count} Category.", Subcategories.Count);
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
                _toastNotificationService.ShowError("Failed to load Categories. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(CategoryDetailViewModel), nameof(LoadSubCategoriesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingSubcategories = false;
            }
        }

        // TODO : Using IPartService
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ViewModelException"></exception>
        private async Task LoadPartsAsync()
        {
            try
            {
                if (CurrentCategory == null) return;

                IsLoadingParts = true;

                PartsInCategory.Clear();

                IEnumerable<PartSummaryDto> parts;

                ActivePartsCount = 10;
                LowStockPartsCount = 10;

                var filter = new PartFilterCriteriaDto(CategoryId: CurrentCategory.Id);

                var pagedResults = await _partService.GetFilteredPartSummariesAsync(filter, 0, 100);

                parts = pagedResults.Items;

                if (parts != null)
                {
                    foreach (var part in parts)
                    {
                        PartsInCategory.Add(part);
                    }
                    _logger.LogInformation("Loaded {Count} Parts.", PartsInCategory.Count);
                }
                else
                {
                    _logger.LogWarning("GetPartsByCategoryAsync() returned null.");
                }

            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _toastNotificationService.ShowError("Failed to load Parts in Category. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(CategoryDetailViewModel), nameof(LoadPartsAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingParts = false;
            }
        }

        /// <summary>
        /// Build the <see cref="BreadcrumbItems"/> Collection using Descendants <see cref="CategoryDto"/>
        /// 
        /// 1. Get the Parent View <see cref="CategoriesViewModel"/> which is All Categories
        /// 2. Traverse all the parents of <see cref="CategoryDto"/> and push them into Stack for reversing Order of <see cref="CategoryBreadcrumbDTO"/>
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        private async Task BuildBreadcrumbTrailAsync(CategoryDto initialCategoryDto)
        {
            BreadcrumbItems.Clear();

            // 1. Add the "All Categories" root link
            var allCategoriesStr = System.Windows.Application.Current.Resources["AllCategoriesStr"] as string ?? "All Categories";
            BreadcrumbItems.Add(new CategoryBreadcrumbDTO(allCategoriesStr, null)); // Null DTO signifies root navigation

            // 2. Build the path from the current category up to its root parent
            var pathStack = new Stack<CategoryBreadcrumbDTO>();
            CategoryDto? currentTraversalCategory = initialCategoryDto;
            int safetyBreak = 0; // Prevent infinite loops

            while (currentTraversalCategory != null && safetyBreak++ < 10) // Max 10 levels for breadcrumb
            {
                // Add current item to the front of the path (to be popped later for correct order)
                pathStack.Push(new CategoryBreadcrumbDTO(currentTraversalCategory.Name, currentTraversalCategory));

                if (currentTraversalCategory.ParentCategoryId.HasValue && currentTraversalCategory.ParentCategoryId.Value != 0)
                {
                    // Fetch the full parent DTO to get its name and its own ParentCategoryId
                    currentTraversalCategory = await _categoryService.GetCategoryByIdAsync(currentTraversalCategory.ParentCategoryId.Value);
                }
                else
                {
                    currentTraversalCategory = null; // Reached a root category (or error fetching parent)
                }
            }

            // 3. Add the path items to the BreadcrumbItems collection in the correct display order
            while (pathStack.Count > 0)
            {
                BreadcrumbItems.Add(pathStack.Pop());
            }
        }


        #endregion

        #region Category Commands

        [RelayCommand]
        public async Task ShowEditCategoryDialogAsync(CategoryDto? categoryToEdit)
        {
            if (categoryToEdit == null) return;
            try
            {
                IsEditingCategory = true;

                bool? results;
                // Check if the Category is Root or not
                if (categoryToEdit.ParentCategoryId == null)
                {
                    results = await _dialogService.ShowDialogAsync<EditRootCategoryDialogViewModel, EditRootCategoryDialog>(categoryToEdit);
                    _logger.LogInformation("Root Category ID='{ID}' Updated Successfully!", categoryToEdit.Id);

                }
                else
                {
                    results = await _dialogService.ShowDialogAsync<EditSubCategoryDialogViewModel, EditSubCategoryDialog>(categoryToEdit);
                    _logger.LogInformation("Sub Category ID='{ID}' Updated Successfully!", categoryToEdit.Id);
                }

                if (results.HasValue && results.Value == true)
                {
                    await LoadCurrentCategoryDetailsAsync();
                    await LoadSubCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemUpdatedSuccessfullyStr"] as string ?? "Category Updated Successfully!";

                    _toastNotificationService.ShowSuccess(msg);

                }

            }
            catch (Exception ex)
            {
                var msg = System.Windows.Application.Current.Resources["UpdateOperationFailedStr"] as string ?? "Update Operation Failed";
                _toastNotificationService.ShowError(msg);
                return;
            }
            finally
            {
                IsEditingCategory = false;

            }
        }

        [RelayCommand]
        public async Task ShowDeleteCategoryDialogAsync(CategoryDto categoryToDelete)
        {
            if (categoryToDelete == null) return;

            try
            {
                IsDeletingCategory = true;

                var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

                if (results.HasValue && results.Value == true)
                {

                    try
                    {
                        await _categoryService.DeleteCategoryAsync(categoryToDelete.Id);


                        var msg = System.Windows.Application.Current.Resources["ItemDeletedSuccessfullyStr"] as string ?? "Item Has Been Deleted Successfully!";


                        _toastNotificationService.ShowSuccess(msg);

                        _logger.LogInformation("Category with ID={ID} has been deleted successfully!", categoryToDelete.Id);

                        _navigationService.GoBack();


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


                        _toastNotificationService.Show($"{msg}\n{dependentItemsText}", msgTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(10));
                        return;
                    }
                    catch (ServiceException ex)
                    {
                        _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                        var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                        _toastNotificationService.ShowError(msg);

                        // FOR DEV ONLY
                        throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(CategoryDetailViewModel), nameof(ShowDeleteCategoryDialogAsync), MethodOperationType.DELETE_DATA, ex);


                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                        var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                        _toastNotificationService.ShowError(msg);

                        // FOR DEV ONLY
                        throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(CategoryDetailViewModel), nameof(ShowDeleteCategoryDialogAsync), MethodOperationType.DELETE_DATA, ex);
                    }
                }
            }
            finally
            {
                IsDeletingCategory = false;

            }

        }

        [RelayCommand]
        public void GoBack()
        {
            _navigationService.GoBack();
        }


        [RelayCommand]
        public void NavigateBackOnBreadcrumb(CategoryBreadcrumbDTO categoryBreadcrumbDTO)
        {
            try
            {

                if(categoryBreadcrumbDTO.CategoryItemDTO == null)
                {
                    _navigationService.NavigateTo(Core.Enums.NavigationPages.ApplicationPage.Categories);
                }
                else
                {
                    _navigationService.NavigateTo(Core.Enums.NavigationPages.ApplicationPage.CategoryDetails, categoryBreadcrumbDTO.CategoryItemDTO);

                }


            }
            catch(Exception ex)
            {
                // FOR DEV
                throw;
            }
        }

        #endregion

        #region Sub Category Commands
        [RelayCommand]
        private async Task ShowAddSubCategoryDialogAsync()
        {
            try
            {
                IsAddingSubCategory = true;

                var results = await _dialogService.ShowDialogAsync<AddSubCategoryDialogViewModel, AddSubCategoryDialog>(CurrentCategory);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new category has been added!");

                    await LoadSubCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemAddedSuccessfullyStr"] as string ?? "New Item Added Successfully!";

                    _toastNotificationService.ShowSuccess(msg);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddRootCategoryDialog)}");
                var msg = System.Windows.Application.Current.Resources["CreationOperationFailedStr"] as string ?? "Creation Operation Has Failed!";
                _toastNotificationService.ShowError(msg);

                // DEV ENV ONLY
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(CategoryDetailViewModel), nameof(ShowAddSubCategoryDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAddingSubCategory = false;
            }
        }

        [RelayCommand]
        public async Task OpenSubCategoryDetailsAsync(CategoryDto? categoryDto)
        {
            if (categoryDto == null) return;
            try
            {
                _navigationService.NavigateTo(Core.Enums.NavigationPages.ApplicationPage.CategoryDetails, categoryDto);

            }
            catch (NavigationException nex)
            {
                var msgTitle = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Category {categoryDto.Name} Has Failed";
                var msg = System.Windows.Application.Current.Resources["ReasonIsStr"] as string ?? $"Reason Is";
                _toastNotificationService.ShowError($"{msg} {nex.Message}", msgTitle);
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(nex.Message, true, CurrentWorkFlow);

            }
            catch (Exception ex)
            {
                var msg = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Category {categoryDto.Name} Has Failed";
                _toastNotificationService.ShowError(msg);
                return;
            }
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
