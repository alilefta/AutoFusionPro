using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.Navigation;
using AutoFusionPro.Application.Services;
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
        [NotifyPropertyChangedFor(nameof(HasSubcategories))]
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
        private string _breadcrumbTrail;

        public bool HasSubcategories => Subcategories.Count > 0;

        [ObservableProperty]
        private int _activePartsCount;

        [ObservableProperty]
        private int _lowStockPartsCount;
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
        }


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

            await LoadSubCategoriesAsync();
            await LoadPartsAsync();

            await base.InitializeAsync(parameter);
        }

        //private async Task LoadCategoryDetailsAsync()
        //{
        //    try
        //    {
        //        if (CurrentCategory == null) return;

        //        IsLoadingDetails = true;
          
        //        var category = await _categoryService.GetCategoryByIdAsync(CurrentCategory.Id);

        //        if (category != null)
        //        {
        //            CurrentCategory = category;
        //            _logger.LogInformation("Loaded {Count} Category.", Subcategories.Count);

        //            await LoadSubCategoriesAsync();

        //        }
        //        else
        //        {
        //            _logger.LogWarning("GetCategoryByIdAsync() returned null.");
        //            CurrentCategory = null;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // No longer throwing ViewModelException, handle here
        //        _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
        //        _toastNotificationService.ShowError("Failed to load Categories. Please try again.", "Loading Error");

        //        throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(CategoryDetailViewModel), nameof(LoadCategoryDetailsAsync), MethodOperationType.LOAD_DATA, ex);

        //    }
        //    finally
        //    {
        //        IsLoadingDetails = false;
        //    }
        //}

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

                //parts = await _partService.GetPartsByCategoryAsync(CurrentCategory.Id, false);

                //if (parts != null)
                //{
                //    foreach (var part in parts)
                //    {
                //        PartsInCategory.Add(part);
                //    }
                //    _logger.LogInformation("Loaded {Count} Parts.", PartsInCategory.Count);
                //}
                //else
                //{
                //    _logger.LogWarning("GetPartsByCategoryAsync() returned null.");
                //}

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


        [RelayCommand]
        private async Task ShowAddSubCategoryDialogAsync()
        {
            try
            {
                IsAddingSubCategory = true;

                var results = await _dialogService.ShowDialogAsync<AddSubCategoryDialogViewModel, AddSubCategoryDialog>(null);

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
    }
}
