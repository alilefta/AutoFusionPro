using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Categories.Dialogs;
using AutoFusionPro.UI.Views.Categories.Dialogs;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.CompatibleVehicles;
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

        public CategoriesViewModel(ICategoryService categoryService, IDialogService dialogService, IWpfToastNotificationService wpfToastNotificationService, ILocalizationService localizationService, ILogger<CategoriesViewModel> logger) : base(localizationService, logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
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
        public async Task ShowAddCategoryAsync()
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
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddCompatibleVehicleDialog)}");
                var msg = System.Windows.Application.Current.Resources["CreationOperationFailedStr"] as string ?? "Creation Operation Has Failed!";
                _wpfToastNotificationService.ShowError(msg);

                // DEV ENV ONLY
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowAddCategoryAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        private void ChangeView()
        {
            IsCardViewActive = !IsCardViewActive;
        }

        #endregion

    }
}
