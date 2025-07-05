using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Enums.DTOEnums;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.Core.SharedDTOs.ComboboxFillDto;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.Filters;
using AutoFusionPro.UI.Views.Parts.Dialogs.Filters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs.Filters
{
    public partial class PartFilterOptionsDialogViewModel : InitializableViewModel<PartFilterOptionsDialogViewModel>, IDialogViewModelWithResult<PartFilterCriteriaDto>
    {

        #region Fields
        private readonly IVehicleTaxonomyService _vehicleTaxonomyService;
        private readonly ICategoryService _categoryService;
        //private readonly ISupplierService _supplierService;

        private IDialogWindow _dialog = null!;
        private PartFilterCriteriaDto? _resultFilters;

        #endregion

        #region General Props
        [ObservableProperty]
        private bool _isSearching = false;


        #endregion

        #region Part Related Props

        [ObservableProperty]
        private bool _isLoadingCategories = false;        
        
        
        //[ObservableProperty]
        //private bool _isLoadingSuppliers = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCategorySelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private CategorySelectionDto? _selectedCategory;

        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        //[NotifyPropertyChangedFor(nameof(IsSupplierSelected))]
        //private SupplierDto _selectedSupplier;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMinSellingPriceSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private decimal? _minSellingPrice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMaxSellingPriceSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private decimal? _maxSellingPrice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsManufacturerSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private string? _manufacturer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStockStatusSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private StockStatusComboboxSelectableItemDto? _selectedStockFilter;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSortBySelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private SortByComboboxSelectableItemDto? _sortBy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        [NotifyPropertyChangedFor(nameof(IsSortAscendingSelected))]
        private bool _isSortAscending = true;



        [ObservableProperty]
        private ObservableCollection<ComboboxSelectableItemDto> _showPartsByOriginalityFilterDtos = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsOriginalSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private ComboboxSelectableItemDto? _selectedOriginalityOption;


        #endregion


        #region Vehicle Taxonomy

        [ObservableProperty]
        private bool _isLoadingMakes = false;

        [ObservableProperty]
        private bool _isLoadingModels = false;

        [ObservableProperty]
        private bool _isLoadingTrims = false;

        [ObservableProperty]
        private bool _isLoadingTransmissions = false;

        [ObservableProperty]
        private bool _isLoadingEngines = false;

        [ObservableProperty]
        private bool _isLoadingBodies = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private MakeDto? _selectedMake;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private ModelDto? _selectedModel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private TrimLevelDto? _selectedTrimLevel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsTransmissionTypeSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private TransmissionTypeDto? _selectedTransmission;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsEngineTypeSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private EngineTypeDto? _selectedEngineType;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsBodyTypeSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private BodyTypeDto? _selectedBodyType;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsExactYearSelected))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private YearFilterItem? _selectedExactYear;

        [ObservableProperty]
        private ObservableCollection<MakeDto> _makesCollection = new();

        [ObservableProperty]
        private ObservableCollection<ModelDto> _modelsCollection = new();

        [ObservableProperty]
        private ObservableCollection<TrimLevelDto> _trimLevelsCollection = new();

        [ObservableProperty]
        private ObservableCollection<TransmissionTypeDto> _transmissionTypesCollection = new();

        [ObservableProperty]
        private ObservableCollection<BodyTypeDto> _bodyTypesCollection = new();

        [ObservableProperty]
        private ObservableCollection<EngineTypeDto> _engineTypesCollection = new();

        [ObservableProperty]
        private ObservableCollection<YearFilterItem> _yearsCollection = new();

        [ObservableProperty]
        private ObservableCollection<CategorySelectionDto> _categories = new();

        [ObservableProperty]
        private ObservableCollection<SortByComboboxSelectableItemDto> _sortByOptions = new();

        [ObservableProperty]
        private ObservableCollection<StockStatusComboboxSelectableItemDto> _stockStatusOptions = new();

        //[ObservableProperty]
        //private ObservableCollection<SupplierDto> _suppliers = new();

        //[ObservableProperty]
        //private ObservableCollection<SupplierDto> _suppliers = new();

        [ObservableProperty]
        private ObservableCollection<ComboboxSelectableItemDto> _showPartsByActivityFilterDtos = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsShowByActivityNotDefault))]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        private ComboboxSelectableItemDto? _selectedShowByActivityItem;       
        


        public bool IsMakeModelTrimFilterActive => (SelectedMake != null && SelectedMake.Id > 0) || (SelectedModel != null && SelectedModel.Id > 0) || (SelectedTrimLevel != null && SelectedTrimLevel.Id > 0);
        public bool IsTransmissionTypeSelected => SelectedTransmission != null && SelectedTransmission.Id > 0;
        public bool IsEngineTypeSelected => SelectedEngineType != null && SelectedEngineType.Id > 0;
        public bool IsBodyTypeSelected => SelectedBodyType != null && SelectedBodyType.Id > 0;
        public bool IsExactYearSelected => SelectedExactYear != null && SelectedExactYear.Year != null;
        public bool IsShowByActivityNotDefault => SelectedShowByActivityItem != null && SelectedShowByActivityItem.value != null;

        public bool IsOriginalSelected => SelectedOriginalityOption != null && SelectedOriginalityOption.value != null;
        public bool IsCategorySelected => SelectedCategory != null && SelectedCategory.Id > 0;
        public bool IsMinSellingPriceSelected => MinSellingPrice.HasValue && MinSellingPrice != null && MinSellingPrice.Value > 0;
        public bool IsMaxSellingPriceSelected => MaxSellingPrice.HasValue && MaxSellingPrice != null && MaxSellingPrice.Value > 0;
        public bool IsManufacturerSelected => !string.IsNullOrEmpty(Manufacturer);
        public bool IsSortBySelected => SortBy != null && SortBy.value != null;
        public bool IsStockStatusSelected => SelectedStockFilter != null && SelectedStockFilter.value != null;
        public bool IsSortAscendingSelected => IsSortAscending != true;

        //public bool IsSupplierSelected => SelectedSupplier != null && SelectedSupplier.Id > 0;

        public bool AnyFilterSelected => IsMakeModelTrimFilterActive || IsTransmissionTypeSelected ||
            IsEngineTypeSelected || IsBodyTypeSelected || IsExactYearSelected || IsShowByActivityNotDefault ||
            IsCategorySelected || IsManufacturerSelected || IsSortAscendingSelected || IsOriginalSelected ||
            IsMinSellingPriceSelected || IsMaxSellingPriceSelected || IsSortBySelected || IsStockStatusSelected;


        #endregion

        public PartFilterOptionsDialogViewModel( 
            IVehicleTaxonomyService vehicleTaxonomyService,
            ICategoryService categoryService,
            ILocalizationService localizationService, 
            ILogger<PartFilterOptionsDialogViewModel> logger) : base(localizationService, logger)
        {
            _vehicleTaxonomyService = vehicleTaxonomyService ?? throw new ArgumentNullException(nameof(vehicleTaxonomyService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        #region Initializer
        /// <summary>
        /// Inherited from <see cref="InitializableViewModel{TViewModel}"/>
        /// </summary>
        /// <param name="parameter"><see cref="null"/></param>
        /// <returns><see cref="void"/></returns>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (IsInitialized) return;

            await LoadCategoriesAsync();
            await LoadMakesAsync();
            await LoadEngineTypesAsync();
            await LoadTransmissionTypesAsync();
            await LoadBodyTypesAsync();

            PopulateShowByActivityCombobox();
            PopulateShowByOriginalityCombobox();
            PopulateSortByCombobox();
            PopulateStockStatusCombobox();
            PopulateYearsCollection();

            // Populate fields if Parameter (CompatibleVehicleFilterCriteriaDto) already have filters.
            if (parameter is PartFilterCriteriaDto currentFilters)
            {
                // Part Fields
                if (currentFilters.CategoryId != null && currentFilters.CategoryId.HasValue && currentFilters.CategoryId.Value > 0)
                    SelectedCategory = Categories.FirstOrDefault(m => m.Id == currentFilters.CategoryId.Value);
                else
                    SelectedCategory = Categories.FirstOrDefault(m => m.Id == 0);

                //if (currentFilters.SupplierId != null && currentFilters.SupplierId.HasValue && currentFilters.SupplierId.Value > 0)
                //    SelectedSupplier = Suppliers.FirstOrDefault(m => m.Id == currentFilters.SupplierId.Value);
                //else
                //    SelectedSupplier = Suppliers.FirstOrDefault(m => m.Id == 0);


                if (currentFilters.IsOriginal.HasValue)
                {
                    SelectedOriginalityOption = ShowPartsByOriginalityFilterDtos.FirstOrDefault(r => r.value == currentFilters.IsOriginal);
                }
                else
                {
                    SelectedOriginalityOption = ShowPartsByOriginalityFilterDtos.FirstOrDefault(r => r.value == null);
                }


                if (currentFilters.Manufacturer != null)
                {
                    Manufacturer = currentFilters.Manufacturer;
                }
                else
                {
                    Manufacturer = null;
                }

                if (currentFilters.MinSellingPrice.HasValue)
                {
                    MinSellingPrice = currentFilters.MinSellingPrice.Value;
                }
                else
                {
                    MinSellingPrice = null;
                }

                if (currentFilters.MaxSellingPrice.HasValue)
                {
                    MaxSellingPrice = currentFilters.MaxSellingPrice.Value;
                }
                else
                {
                    MaxSellingPrice = null;
                }

                if (currentFilters.SortBy.HasValue)
                {
                    SortBy = SortByOptions.FirstOrDefault(f => f.value == currentFilters.SortBy.Value);
                }
                else
                {
                    SortBy = SortByOptions.FirstOrDefault(f => f.value == null);
                }

                if (currentFilters.StockStatus.HasValue)
                {
                    SelectedStockFilter = StockStatusOptions.FirstOrDefault(f => f.value == currentFilters.StockStatus.Value);
                }
                else
                {
                    SelectedStockFilter = StockStatusOptions.FirstOrDefault(f => f.value == null);
                }

                if (currentFilters.IsSortAscending)
                {
                    IsSortAscending = currentFilters.IsSortAscending;
                }
                else
                {
                    IsSortAscending = true;
                }


                // Vehicle Taxonomy Fields
                if (currentFilters.MakeId != null && currentFilters.MakeId.HasValue && currentFilters.MakeId.Value > 0)
                    SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == currentFilters.MakeId.Value);
                else
                    SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);


                if (SelectedMake != null && SelectedMake.Id > 0 && currentFilters.ModelId != null && currentFilters.ModelId.HasValue && currentFilters.ModelId.Value > 0)
                {
                    SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == currentFilters.ModelId.Value);
                }
                else
                {
                    SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == 0);
                }

                if (SelectedMake != null &&
                    SelectedMake.Id > 0 &&
                    SelectedModel != null &&
                    SelectedModel.Id > 0 &&
                    currentFilters.TrimId != null &&
                    currentFilters.TrimId.HasValue &&
                    currentFilters.TrimId.Value > 0)
                {
                    SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(m => m.Id == currentFilters.TrimId.Value);
                }
                else
                {
                    SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(m => m.Id == 0);
                }

                if (currentFilters.EngineId != null && currentFilters.EngineId.HasValue)
                {
                    SelectedEngineType = EngineTypesCollection.FirstOrDefault(m => m.Id == currentFilters.EngineId.Value);
                }
                else
                {
                    SelectedEngineType = EngineTypesCollection.FirstOrDefault(m => m.Id == 0);
                }

                if (currentFilters.BodyTypeId != null && currentFilters.BodyTypeId.HasValue)
                {
                    SelectedBodyType = BodyTypesCollection.FirstOrDefault(m => m.Id == currentFilters.BodyTypeId.Value);
                }
                else
                {
                    SelectedBodyType = BodyTypesCollection.FirstOrDefault(m => m.Id == 0);
                }

                if (currentFilters.TransmissionId != null && currentFilters.TransmissionId.HasValue)
                {
                    SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == currentFilters.TransmissionId.Value);
                }
                else
                {
                    SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
                }

                if (currentFilters.SpecificYear != null)
                {
                    SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == currentFilters.SpecificYear.Value)
                                        ?? YearsCollection.FirstOrDefault(y => y.Year == null); // "All Years" item
                }
                else
                {
                    SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null); // Default to "All Years"
                }


                if (currentFilters.IsActive.HasValue)
                {
                    SelectedShowByActivityItem = ShowPartsByActivityFilterDtos.FirstOrDefault(r => r.value == currentFilters.IsActive.Value);
                }
                else
                {
                    SelectedShowByActivityItem = ShowPartsByActivityFilterDtos.FirstOrDefault(r => r.value == null);
                }
            }
            else
            {
                // Set default "All" selections if no filters are passed in
                SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);
                SelectedEngineType = EngineTypesCollection.FirstOrDefault(e => e.Id == 0);
                SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
                SelectedBodyType = BodyTypesCollection.FirstOrDefault(b => b.Id == 0);
                SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null);
                SelectedShowByActivityItem = ShowPartsByActivityFilterDtos.FirstOrDefault(r => r.value == null);

                SelectedOriginalityOption = ShowPartsByOriginalityFilterDtos.FirstOrDefault(r => r.value == null);
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == 0);
                //SelectedSupplier = Suppliers.FirstOrDefault(c => c.Id == 0);
                SelectedStockFilter = StockStatusOptions.FirstOrDefault(s => s.value == null);
                SortBy = SortByOptions.FirstOrDefault(s => s.value == null);
                Manufacturer = null;
                IsSortAscending = true;
                MinSellingPrice = null;
                MaxSellingPrice = null;

            }

            await base.InitializeAsync(parameter); // Call base to set IsInitialized and complete TaskCompletionSource
        }

        #endregion


        #region Initialization & Data Loading

        /// <summary>
        /// Loads the Car Makes
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadMakesAsync()
        {
            try
            {
                IsLoadingMakes = true;

                var makesData = await _vehicleTaxonomyService.GetAllMakesAsync();
                MakesCollection.Clear();

                var allMakesStr = System.Windows.Application.Current.Resources["AllMakesStr"] as string ?? "All Makes";
                MakesCollection.Add(new MakeDto(0, allMakesStr, null));

                foreach (var make in makesData.OrderBy(x => x.Id))
                {
                    MakesCollection.Add(make);
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Makes Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartCompatibilityRuleFilterOptionsDialogViewModel), nameof(LoadMakesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingMakes = false;
            }
        }

        /// <summary>
        /// Load Models Data for the selected <see cref="MakeDto"/>
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadModelsAsync(int makeId)
        {
            try
            {
                ModelsCollection.Clear();

                IsLoadingModels = true;


                var allModelsStr = System.Windows.Application.Current.Resources["AllModelsStr"] as string ?? "All Models";
                ModelsCollection.Add(new ModelDto(0, allModelsStr, 0, string.Empty, null));

                if (makeId != 0)
                {
                    var modelsData = await _vehicleTaxonomyService.GetModelsByMakeIdAsync(makeId);

                    foreach (var model in modelsData.OrderBy(x => x.Id))
                    {
                        ModelsCollection.Add(model);
                    }
                }

                SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == 0);
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Models Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartCompatibilityRuleFilterOptionsDialogViewModel), nameof(LoadModelsAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingModels = false;
            }
        }

        /// <summary>
        /// Load Trims Data for the selected <see cref="ModelDto"/>
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadTrimLevelsAsync(int modelId)
        {
            try
            {
                IsLoadingTrims = true;

                TrimLevelsCollection.Clear();

                var allTrimsStr = System.Windows.Application.Current.Resources["AllTrimLevelsStr"] as string ?? "All Trim Levels";

                TrimLevelsCollection.Add(new TrimLevelDto(0, allTrimsStr, 0, string.Empty));

                if (modelId != 0)
                {
                    var trimsData = await _vehicleTaxonomyService.GetTrimLevelsByModelIdAsync(modelId);

                    foreach (var trim in trimsData.OrderBy(x => x.Id))
                    {
                        TrimLevelsCollection.Add(trim);
                    }
                }

                SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(t => t.Id == 0); ;

            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Trim Levels Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartCompatibilityRuleFilterOptionsDialogViewModel), nameof(LoadTrimLevelsAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingTrims = false;

            }
        }

        /// <summary>
        /// Loads Transmission Types
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadTransmissionTypesAsync()
        {
            try
            {
                IsLoadingTransmissions = true;

                var transmissionTypeDtos = await _vehicleTaxonomyService.GetAllTransmissionTypesAsync();
                TransmissionTypesCollection.Clear();

                var allTransTypes = System.Windows.Application.Current.Resources["AllTransmissionTypesStr"] as string ?? "All Transmission Types";

                TransmissionTypesCollection.Add(new TransmissionTypeDto(0, allTransTypes));

                foreach (var tty in transmissionTypeDtos.OrderBy(x => x.Id))
                {
                    TransmissionTypesCollection.Add(tty);
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Transmission Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartCompatibilityRuleFilterOptionsDialogViewModel), nameof(LoadTransmissionTypesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingTransmissions = false;
            }
        }

        /// <summary>
        /// Loads Engine Types
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadEngineTypesAsync()
        {
            try
            {
                IsLoadingEngines = true;

                var engineTypeDtos = await _vehicleTaxonomyService.GetAllEngineTypesAsync();
                EngineTypesCollection.Clear();

                var allEngineTypesStr = System.Windows.Application.Current.Resources["AllEngineTypesStr"] as string ?? "All Engine Types";

                EngineTypesCollection.Add(new EngineTypeDto(0, allEngineTypesStr, null));


                foreach (var engine in engineTypeDtos.OrderBy(x => x.Id))
                {
                    EngineTypesCollection.Add(engine);
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Engine Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartCompatibilityRuleFilterOptionsDialogViewModel), nameof(LoadEngineTypesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingEngines = false;
            }
        }

        /// <summary>
        /// Loads Transmission Types
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadBodyTypesAsync()
        {
            try
            {
                IsLoadingBodies = true;

                var bodyTypeDtos = await _vehicleTaxonomyService.GetAllBodyTypesAsync();
                BodyTypesCollection.Clear();


                var allBodyTypesStr = System.Windows.Application.Current.Resources["AllBodyTypesStr"] as string ?? "All Body Types";

                BodyTypesCollection.Add(new BodyTypeDto(0, allBodyTypesStr));


                foreach (var body in bodyTypeDtos.OrderBy(x => x.Id))
                {
                    BodyTypesCollection.Add(body);
                }
            }
            catch (Exception ex)
            {
                // Set an error message property for the UI
                // ValidationMessage = $"Failed to load data: {ex.Message}";
                // Or show a non-blocking toast
                // _wpfToastNotificationService.ShowError("Failed to load data. Please try again.");
                // Do NOT re-throw if the dialog is meant to stay open.
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Body Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartCompatibilityRuleFilterOptionsDialogViewModel), nameof(LoadBodyTypesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingBodies = false;
            }
        }

        /// <summary>
        /// Populate Years collection with <see cref="YearFilterItem"/> with range for 30 years
        /// </summary>
        private void PopulateYearsCollection()
        {
            // Setup years
            YearsCollection.Clear();
            var allStr = System.Windows.Application.Current.Resources["AllStr"] as string ?? "All";

            YearsCollection.Add(new YearFilterItem(null, allStr));

            int currentYear = DateTime.Now.Year;
            for (int i = currentYear + 1; i >= currentYear - 30; i--)
            {
                YearsCollection.Add(
                    new YearFilterItem(i, i.ToString())
                );
            }
        }


        /// <summary>
        /// Loads the Categories
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadCategoriesAsync()
        {
            try
            {
                IsLoadingCategories = true;

                var cateData = await _categoryService.GetAllCategoriesForSelectionAsync();
                Categories.Clear();

                var allCategoriesStr = System.Windows.Application.Current.Resources["AllCategoriesStr"] as string ?? "All Categories";
                Categories.Add(new CategorySelectionDto(0, allCategoriesStr, null));

                foreach (var category in cateData.OrderBy(x => x.Id))
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Categories Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartFilterOptionsDialogViewModel), nameof(LoadCategoriesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingCategories = false;
            }
        }


        private void PopulateShowByActivityCombobox()
        {
            ShowPartsByActivityFilterDtos.Clear();
            var defaultItemStr = GetString("ShowAllPartsStr");
            var showOnlyActiveRulesStr = GetString("ShowOnlyActivePartsStr");
            var showOnlyInactiveRulesStr = GetString("ShowOnlyInactivePartsStr");


            var defaultNullItem = new ComboboxSelectableItemDto(defaultItemStr, null);
            var showOnlyActiveRulesItem = new ComboboxSelectableItemDto(showOnlyActiveRulesStr, true);
            var showOnlyInactiveRulesItem = new ComboboxSelectableItemDto(showOnlyInactiveRulesStr, false);

            ShowPartsByActivityFilterDtos.Add(defaultNullItem);
            ShowPartsByActivityFilterDtos.Add(showOnlyActiveRulesItem);
            ShowPartsByActivityFilterDtos.Add(showOnlyInactiveRulesItem);
        }

        private void PopulateShowByOriginalityCombobox()
        {
            ShowPartsByOriginalityFilterDtos.Clear();
            var defaultItemStr = GetString("ShowAllPartsStr");
            var showOnlyOriginalPartsStr = GetString("ShowOnlyOriginalPartsStr");
            var showOnlyAftermarketPartsStr = GetString("ShowOnlyAftermarketPartsStr");


            var defaultNullItem = new ComboboxSelectableItemDto(defaultItemStr, null);
            var showOnlyOriginalPartsItem = new ComboboxSelectableItemDto(showOnlyOriginalPartsStr, true);
            var showOnlyAftermarketPartsItem = new ComboboxSelectableItemDto(showOnlyAftermarketPartsStr, false);

            ShowPartsByOriginalityFilterDtos.Add(defaultNullItem);
            ShowPartsByOriginalityFilterDtos.Add(showOnlyOriginalPartsItem);
            ShowPartsByOriginalityFilterDtos.Add(showOnlyAftermarketPartsItem);
        }

        private void PopulateSortByCombobox()
        {
            SortByOptions.Clear();

            var defaultItemStr = GetString("ShowAllPartsStr");
            var sortByNameStr = GetString("SortByNameStr");
            var sortByPartNumberStr = GetString("SortByPartNumberStr");
            var sortBySellingPriceStr = GetString("SortBySellingPriceStr");
            var sortByCurrentStockStr = GetString("SortByCurrentStockStr");
            var sortByLastModifiedStr = GetString("SortByLastModifiedStr");

            var defaultNullItem = new SortByComboboxSelectableItemDto(defaultItemStr, null);
            var sortByNameStrItem = new SortByComboboxSelectableItemDto(sortByNameStr, PartSortBy.Name);
            var sortByPartNumberItem = new SortByComboboxSelectableItemDto(sortByPartNumberStr, PartSortBy.PartNumber);
            var sortBySellingPriceItem = new SortByComboboxSelectableItemDto(sortBySellingPriceStr, PartSortBy.SellingPrice);
            var sortByCurrentStockItem = new SortByComboboxSelectableItemDto(sortByCurrentStockStr, PartSortBy.CurrentStock);
            var sortByLastModifiedItem = new SortByComboboxSelectableItemDto(sortByLastModifiedStr, PartSortBy.LastModified);

            SortByOptions.Add(defaultNullItem);
            SortByOptions.Add(sortByNameStrItem);
            SortByOptions.Add(sortByPartNumberItem);
            SortByOptions.Add(sortBySellingPriceItem);
            SortByOptions.Add(sortByCurrentStockItem);
            SortByOptions.Add(sortByLastModifiedItem);

        }

        private void PopulateStockStatusCombobox()
        {
            StockStatusOptions.Clear();

            var defaultItemStr = GetString("ShowAllPartsStr");
            var allStockStr = GetString("AllStr");
            var inStockStr = GetString("InStockStr");
            var outOfStockStr = GetString("OutOfStockStr");
            var lowStockStr = GetString("LowStockStr");


            var defaultNullItem = new StockStatusComboboxSelectableItemDto(defaultItemStr, null);
            var allStockItem = new StockStatusComboboxSelectableItemDto(allStockStr, StockStatusFilter.All);
            var inStockItem = new StockStatusComboboxSelectableItemDto(inStockStr, StockStatusFilter.InStock);
            var outOfStockItem = new StockStatusComboboxSelectableItemDto(outOfStockStr, StockStatusFilter.OutOfStock);
            var lowStockItem = new StockStatusComboboxSelectableItemDto(lowStockStr, StockStatusFilter.LowStock);


            StockStatusOptions.Add(defaultNullItem);
            StockStatusOptions.Add(allStockItem);
            StockStatusOptions.Add(inStockItem);
            StockStatusOptions.Add(outOfStockItem);
            StockStatusOptions.Add(lowStockItem);

        }

        #endregion

        #region Commands

        [RelayCommand]
        private void ApplyFilters()
        {
            try
            {
                IsSearching = true;
                // Construct the DTO from current selections in the dialog
                _resultFilters = new PartFilterCriteriaDto(
                    SearchTerm: null,
                    CategoryId: SelectedCategory != null && SelectedCategory.Id > 0 ? SelectedCategory.Id : null,
                    Manufacturer: Manufacturer,
                    SupplierId: null,
                    MinSellingPrice: MinSellingPrice.HasValue ? MinSellingPrice.Value : null,
                    MaxSellingPrice: MaxSellingPrice.HasValue ? MaxSellingPrice.Value : null,
                    StockStatus: SelectedStockFilter?.value ?? null,
                    IsActive: SelectedShowByActivityItem?.value ?? null,
                    IsOriginal: SelectedOriginalityOption?.value ?? null,
                   
                    MakeId: SelectedMake != null && SelectedMake.Id > 0 ? SelectedMake.Id : null,
                    ModelId: SelectedModel != null && SelectedModel.Id > 0 ? SelectedModel.Id : null,
                    TrimId: SelectedTrimLevel != null && SelectedTrimLevel.Id > 0 ? SelectedTrimLevel.Id : null,
                    EngineId: SelectedEngineType != null && SelectedEngineType.Id > 0 ? SelectedEngineType.Id : null,
                    TransmissionId: SelectedTransmission != null && SelectedTransmission.Id > 0 ? SelectedTransmission.Id : null,
                    BodyTypeId: SelectedBodyType != null && SelectedBodyType.Id > 0 ? SelectedBodyType.Id : null,
                    SpecificYear: SelectedExactYear?.Year,
                    SortBy: SortBy?.value ?? null,
                    IsSortAscending: IsSortAscending


                );
                SetResultAndClose(true); // Signal success
            }
            finally
            {
                IsSearching = false;
            }

        }

        [RelayCommand]
        private void ResetAndClearFilters()
        {
            SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);
            SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null);
            SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
            SelectedEngineType = EngineTypesCollection.FirstOrDefault(t => t.Id == 0);
            SelectedBodyType = BodyTypesCollection.FirstOrDefault(t => t.Id == 0);
            SelectedShowByActivityItem = ShowPartsByActivityFilterDtos.FirstOrDefault(r => r.value == null);
            SelectedCategory = Categories.FirstOrDefault(r => r.Id == 0);
            //SelectedSupplier = Suppliers.FirstOrDefault(r => r.Id == 0);
            SelectedStockFilter = StockStatusOptions.FirstOrDefault(s => s.value == null);
            SortBy = SortByOptions.FirstOrDefault(s => s.value == null); 
            SelectedOriginalityOption = ShowPartsByOriginalityFilterDtos.FirstOrDefault(r => r.value == null);
            Manufacturer = null;
            MaxSellingPrice = null;
            MinSellingPrice = null;
            IsSortAscending = true;

        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }

        [RelayCommand]
        private void ClearSelectedMake()
        {
            SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);

        }

        [RelayCommand]
        private void ClearSelectedYear()
        {
            SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null);

        }

        [RelayCommand]
        private void ClearSelectedTransmissionType()
        {
            SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
        }

        [RelayCommand]
        private void ClearSelectedBodyType()
        {
            SelectedBodyType = BodyTypesCollection.FirstOrDefault(t => t.Id == 0);
        }


        [RelayCommand]
        private void ClearSelectedEngineType()
        {
            SelectedEngineType = EngineTypesCollection.FirstOrDefault(t => t.Id == 0);
        }

        [RelayCommand]
        private void ClearSelectedShowByActivity()
        {
            SelectedShowByActivityItem = ShowPartsByActivityFilterDtos.FirstOrDefault(r => r.value == null);
        }

        [RelayCommand]
        private void ClearSelectedShowByOriginality()
        {
            SelectedOriginalityOption = ShowPartsByOriginalityFilterDtos.FirstOrDefault(r => r.value == null);
        }


        [RelayCommand]
        private void ClearSelectedCategory()
        {
            SelectedCategory = Categories.FirstOrDefault(r => r.Id == 0);
        }

        [RelayCommand]
        private void ClearSelectedStockFilterStatus()
        {
            SelectedStockFilter = StockStatusOptions.FirstOrDefault(r => r.value == null);
        }

        [RelayCommand]
        private void ClearSelectedSortBy()
        {
            SortBy = SortByOptions.FirstOrDefault(r => r.value == null);
        }
        //[RelayCommand]
        //private void ClearSelectedSupplier()
        //{
        //    SelectedSupplier = Suppliers.FirstOrDefault(r => r.Id == 0);
        //}

        #endregion


        #region Observable Props Customization (Makes, Models, Trims)

        // When SelectedMake changes, load models for it
        partial void OnSelectedMakeChanged(MakeDto? value)
        {
            if (value != null)
            {
                // Load models for the selected make
                _ = LoadModelsAsync(value.Id);
            }
            // Update CanExecute for commands if necessary
            ApplyFiltersCommand.NotifyCanExecuteChanged(); // Example
        }


        // When SelectedMake changes, load models for it
        partial void OnSelectedModelChanged(ModelDto? value)
        {
            if (value != null)
            {
                // Load models for the selected make
                _ = LoadTrimLevelsAsync(value.Id);
            }
            // Update CanExecute for commands if necessary
            ApplyFiltersCommand.NotifyCanExecuteChanged(); // Example
        }

        #endregion


        #region Dialog Specific Methods

        private void SetResultAndClose(bool res)
        {
            if (_dialog != null)
            {
                // Set the result first
                _dialog.DialogResult = res;

                // Then close with animation
                _dialog.Close();
            }

        }

        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on {VM}", nameof(PartFilterOptionsDialog));
                return;
            }

            _dialog = dialog;
        }

        /// <summary>
        /// Set the Dialog Results. Inherited from <see cref="IDialogViewModelWithResult{TResult}"/> interface.
        /// </summary>
        /// <returns><see cref="CompatibleVehicleFilterCriteriaDto?"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PartFilterCriteriaDto? GetResult()
        {
            return _resultFilters; // Return the filters set by ApplyFilters
        }

        #endregion
    }
}
