using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.Filters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.Filters
{
    //public partial class VehicleCompatibilityFilterOptionsDialogViewModel : InitializableViewModel<VehicleCompatibilityFilterOptionsDialogViewModel>, IDialogViewModelWithResult<CompatibleVehicleFilterCriteriaDto>
    //{

    //    #region Fields
    //    /// <summary>
    //    /// Value Provided by DI container to manage Models.
    //    /// </summary>
    //    private readonly IVehicleTaxonomyService _compatibleVehicleService;
    //    /// <summary>
    //    /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
    //    /// </summary>
    //    private IDialogWindow _dialog = null!;
    //    #endregion

    //    #region Props

    //    [ObservableProperty]
    //    private bool _isSearching = false;

    //    [ObservableProperty]
    //    private bool _isLoadingMakes = false;

    //    [ObservableProperty]
    //    private bool _isLoadingModels = false;

    //    [ObservableProperty]
    //    private bool _isLoadingTrims = false;

    //    [ObservableProperty]
    //    private bool _isLoadingTransmissions = false;

    //    [ObservableProperty]
    //    private bool _isLoadingEngines = false;

    //    [ObservableProperty]
    //    private bool _isLoadingBodies = false;

    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
    //    [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
    //    [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
    //    private MakeDto? _selectedMake;

    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
    //    [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
    //    [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
    //    private ModelDto? _selectedModel;

    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
    //    [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
    //    [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
    //    private TrimLevelDto? _selectedTrimLevel;

    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
    //    [NotifyPropertyChangedFor(nameof(IsTransmissionTypeSelected))]
    //    [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
    //    private TransmissionTypeDto? _selectedTransmission;

    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
    //    [NotifyPropertyChangedFor(nameof(IsEngineTypeSelected))]
    //    [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
    //    private EngineTypeDto? _selectedEngineType;

    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
    //    [NotifyPropertyChangedFor(nameof(IsBodyTypeSelected))]
    //    [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
    //    private BodyTypeDto? _selectedBodyType;

    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
    //    [NotifyPropertyChangedFor(nameof(IsExactYearSelected))]
    //    [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
    //    private YearFilterItem? _selectedExactYear;

    //    [ObservableProperty]
    //    private ObservableCollection<MakeDto> _makesCollection;

    //    [ObservableProperty]
    //    private ObservableCollection<ModelDto> _modelsCollection;

    //    [ObservableProperty]
    //    private ObservableCollection<TrimLevelDto> _trimLevelsCollection;

    //    [ObservableProperty]
    //    private ObservableCollection<TransmissionTypeDto> _transmissionTypesCollection;

    //    [ObservableProperty]
    //    private ObservableCollection<BodyTypeDto> _bodyTypesCollection;

    //    [ObservableProperty]
    //    private ObservableCollection<EngineTypeDto> _engineTypesCollection;

    //    [ObservableProperty]
    //    private ObservableCollection<YearFilterItem> _yearsCollection;


    //    private CompatibleVehicleFilterCriteriaDto? _resultFilters;

    //    public bool IsMakeModelTrimFilterActive => (SelectedMake != null && SelectedMake.Id > 0) || (SelectedModel != null && SelectedModel.Id > 0) || (SelectedTrimLevel != null && SelectedTrimLevel.Id > 0);
    //    public bool IsTransmissionTypeSelected => SelectedTransmission != null && SelectedTransmission.Id > 0;
    //    public bool IsEngineTypeSelected => SelectedEngineType != null && SelectedEngineType.Id > 0;
    //    public bool IsBodyTypeSelected => SelectedBodyType != null && SelectedBodyType.Id > 0;
    //    public bool IsExactYearSelected => SelectedExactYear != null && SelectedExactYear.Year != null;

    //    public bool AnyFilterSelected => IsMakeModelTrimFilterActive || IsTransmissionTypeSelected || IsEngineTypeSelected || IsBodyTypeSelected || IsExactYearSelected;

    //    #endregion

    //    #region Constructor
    //    public VehicleCompatibilityFilterOptionsDialogViewModel(IVehicleTaxonomyService compatibleVehicleService, ILocalizationService localizationService, ILogger<VehicleCompatibilityFilterOptionsDialogViewModel> logger) : base(localizationService, logger)
    //    {
    //        _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));

    //        MakesCollection = new ObservableCollection<MakeDto>();
    //        ModelsCollection = new ObservableCollection<ModelDto>();
    //        TrimLevelsCollection = new ObservableCollection<TrimLevelDto>();
    //        TransmissionTypesCollection = new ObservableCollection<TransmissionTypeDto>();
    //        BodyTypesCollection = new ObservableCollection<BodyTypeDto>();
    //        EngineTypesCollection = new ObservableCollection<EngineTypeDto>();
    //        YearsCollection = new ObservableCollection<YearFilterItem>();
    //    }
    //    #endregion

    //    #region Initializer
    //    /// <summary>
    //    /// Inherited from <see cref="InitializableViewModel{TViewModel}"/>
    //    /// </summary>
    //    /// <param name="parameter"><see cref="null"/></param>
    //    /// <returns><see cref="void"/></returns>
    //    public override async Task InitializeAsync(object? parameter = null)
    //    {
    //        if (IsInitialized) return;

    //        await LoadMakesAsync();
    //        await LoadEngineTypesAsync();
    //        await LoadTransmissionTypesAsync();
    //        await LoadBodyTypesAsync();

    //        PopulateYearsCollection();

    //        // Populate fields if Parameter (CompatibleVehicleFilterCriteriaDto) already have filters.
    //        if (parameter is CompatibleVehicleFilterCriteriaDto currentFilters)
    //        {
    //            if (currentFilters.MakeId != null && currentFilters.MakeId.HasValue && currentFilters.MakeId.Value > 0)
    //                SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == currentFilters.MakeId.Value);
    //            else
    //                SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);
                

    //            if (SelectedMake != null && SelectedMake.Id > 0 && currentFilters.ModelId != null && currentFilters.ModelId.HasValue && currentFilters.ModelId.Value > 0)
    //            {
    //                SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == currentFilters.ModelId.Value);
    //            }
    //            else
    //            {
    //                SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == 0);
    //            }

    //            if (SelectedMake != null &&
    //                SelectedMake.Id > 0 &&
    //                SelectedModel != null &&
    //                SelectedModel.Id > 0 &&
    //                currentFilters.TrimLevelId != null &&
    //                currentFilters.TrimLevelId.HasValue &&
    //                currentFilters.TrimLevelId.Value > 0)
    //            {
    //                SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(m => m.Id == currentFilters.TrimLevelId.Value);
    //            }
    //            else
    //            {
    //                SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(m => m.Id == 0);
    //            }

    //            if (currentFilters.EngineTypeId != null && currentFilters.EngineTypeId.HasValue)
    //            {
    //                SelectedEngineType = EngineTypesCollection.FirstOrDefault(m => m.Id == currentFilters.EngineTypeId.Value);
    //            }
    //            else
    //            {
    //                SelectedEngineType = EngineTypesCollection.FirstOrDefault(m => m.Id == 0);
    //            }

    //            if (currentFilters.BodyTypeId != null && currentFilters.BodyTypeId.HasValue)
    //            {
    //                SelectedBodyType = BodyTypesCollection.FirstOrDefault(m => m.Id == currentFilters.BodyTypeId.Value);
    //            }
    //            else
    //            {
    //                SelectedBodyType = BodyTypesCollection.FirstOrDefault(m => m.Id == 0);
    //            }

    //            if (currentFilters.TransmissionTypeId != null && currentFilters.TransmissionTypeId.HasValue)
    //            {
    //                SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == currentFilters.TransmissionTypeId.Value);
    //            }
    //            else
    //            {
    //                SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
    //            }

    //            if (currentFilters.ExactYear != null)
    //            {
    //                SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == currentFilters.ExactYear.Value)
    //                                    ?? YearsCollection.FirstOrDefault(y => y.Year == null); // "All Years" item
    //            }
    //            else
    //            {
    //                SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null); // Default to "All Years"
    //            }
    //        }

    //        await base.InitializeAsync(parameter); // Call base to set IsInitialized and complete TaskCompletionSource
    //    }

    //    #endregion

    //    #region Initialization & Data Loading

    //    /// <summary>
    //    /// Loads the Car Makes
    //    /// </summary>
    //    /// <returns><see cref="Task"/></returns>
    //    /// <exception cref="ViewModelException">For Dev only</exception>
    //    private async Task LoadMakesAsync()
    //    {
    //        try
    //        {
    //            IsLoadingMakes = true;

    //            var makesData = await _compatibleVehicleService.GetAllMakesAsync();
    //            MakesCollection.Clear();

    //            var allMakesStr = System.Windows.Application.Current.Resources["AllMakesStr"] as string ?? "All Makes";
    //            MakesCollection.Add(new MakeDto(0, allMakesStr, null));

    //            foreach (var make in makesData.OrderBy(x => x.Id))
    //            {
    //                MakesCollection.Add(make);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // No longer throwing ViewModelException, handle here
    //            _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
    //            await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Makes Data, {ex.Message}", true, CurrentWorkFlow);

    //            // DEV ENV Only
    //            throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadMakesAsync), MethodOperationType.LOAD_DATA, ex);

    //        }
    //        finally
    //        {
    //            IsLoadingMakes = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Load Models Data for the selected <see cref="MakeDto"/>
    //    /// </summary>
    //    /// <returns><see cref="Task"/></returns>
    //    /// <exception cref="ViewModelException">For Dev only</exception>
    //    private async Task LoadModelsAsync(int makeId)
    //    {
    //        try
    //        {
    //            ModelsCollection.Clear();

    //            IsLoadingModels = true;


    //            var allModelsStr = System.Windows.Application.Current.Resources["AllModelsStr"] as string ?? "All Models";
    //            ModelsCollection.Add(new ModelDto(0, allModelsStr, 0, string.Empty, null));

    //            if (makeId != 0)
    //            {
    //                var modelsData = await _compatibleVehicleService.GetModelsByMakeIdAsync(makeId);

    //                foreach (var model in modelsData.OrderBy(x => x.Id))
    //                {
    //                    ModelsCollection.Add(model);
    //                }
    //            }

    //            SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == 0);
    //        }
    //        catch (Exception ex)
    //        {
    //            // No longer throwing ViewModelException, handle here
    //            _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
    //            await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Models Data, {ex.Message}", true, CurrentWorkFlow);

    //            // DEV ENV Only
    //            throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadModelsAsync), MethodOperationType.LOAD_DATA, ex);

    //        }
    //        finally
    //        {
    //            IsLoadingModels = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Load Trims Data for the selected <see cref="ModelDto"/>
    //    /// </summary>
    //    /// <param name="modelId"></param>
    //    /// <returns><see cref="Task"/></returns>
    //    /// <exception cref="ViewModelException">For Dev only</exception>
    //    private async Task LoadTrimLevelsAsync(int modelId)
    //    {
    //        try
    //        {
    //            IsLoadingTrims = true;

    //            TrimLevelsCollection.Clear();

    //            var allTrimsStr = System.Windows.Application.Current.Resources["AllTrimLevelsStr"] as string ?? "All Trim Levels";

    //            TrimLevelsCollection.Add(new TrimLevelDto(0, allTrimsStr, 0, string.Empty));

    //            if (modelId != 0)
    //            {
    //                var trimsData = await _compatibleVehicleService.GetTrimLevelsByModelIdAsync(modelId);

    //                foreach (var trim in trimsData.OrderBy(x => x.Id))
    //                {
    //                    TrimLevelsCollection.Add(trim);
    //                }
    //            }

    //            SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(t => t.Id == 0); ;

    //        }
    //        catch (Exception ex)
    //        {
    //            // No longer throwing ViewModelException, handle here
    //            _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
    //            await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Trim Levels Data, {ex.Message}", true, CurrentWorkFlow);

    //            // DEV ENV Only
    //            throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadTrimLevelsAsync), MethodOperationType.LOAD_DATA, ex);

    //        }
    //        finally
    //        {
    //            IsLoadingTrims = false;

    //        }
    //    }

    //    /// <summary>
    //    /// Loads Transmission Types
    //    /// </summary>
    //    /// <returns><see cref="Task"/></returns>
    //    /// <exception cref="ViewModelException">For Dev only</exception>
    //    private async Task LoadTransmissionTypesAsync()
    //    {
    //        try
    //        {
    //            IsLoadingTransmissions = true;

    //            var transmissionTypeDtos = await _compatibleVehicleService.GetAllTransmissionTypesAsync();
    //            TransmissionTypesCollection.Clear();

    //            var allTransTypes = System.Windows.Application.Current.Resources["AllTransmissionTypesStr"] as string ?? "All Transmission Types";

    //            TransmissionTypesCollection.Add(new TransmissionTypeDto(0, allTransTypes));

    //            foreach (var tty in transmissionTypeDtos.OrderBy(x => x.Id))
    //            {
    //                TransmissionTypesCollection.Add(tty);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // No longer throwing ViewModelException, handle here
    //            _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
    //            await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Transmission Types Data, {ex.Message}", true, CurrentWorkFlow);

    //            // DEV ENV Only
    //            throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadTransmissionTypesAsync), MethodOperationType.LOAD_DATA, ex);

    //        }
    //        finally
    //        {
    //            IsLoadingTransmissions = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Loads Engine Types
    //    /// </summary>
    //    /// <returns><see cref="Task"/></returns>
    //    /// <exception cref="ViewModelException">For Dev only</exception>
    //    private async Task LoadEngineTypesAsync()
    //    {
    //        try
    //        {
    //            IsLoadingEngines = true;

    //            var engineTypeDtos = await _compatibleVehicleService.GetAllEngineTypesAsync();
    //            EngineTypesCollection.Clear();

    //            var allEngineTypesStr = System.Windows.Application.Current.Resources["AllEngineTypesStr"] as string ?? "All Engine Types";

    //            EngineTypesCollection.Add(new EngineTypeDto(0, allEngineTypesStr, null));


    //            foreach (var engine in engineTypeDtos.OrderBy(x => x.Id))
    //            {
    //                EngineTypesCollection.Add(engine);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // No longer throwing ViewModelException, handle here
    //            _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
    //            await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Engine Types Data, {ex.Message}", true, CurrentWorkFlow);

    //            // DEV ENV Only
    //            throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadEngineTypesAsync), MethodOperationType.LOAD_DATA, ex);

    //        }
    //        finally
    //        {
    //            IsLoadingEngines = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Loads Transmission Types
    //    /// </summary>
    //    /// <returns><see cref="Task"/></returns>
    //    /// <exception cref="ViewModelException">For Dev only</exception>
    //    private async Task LoadBodyTypesAsync()
    //    {
    //        try
    //        {
    //            IsLoadingBodies = true;

    //            var bodyTypeDtos = await _compatibleVehicleService.GetAllBodyTypesAsync();
    //            BodyTypesCollection.Clear();


    //            var allBodyTypesStr = System.Windows.Application.Current.Resources["AllBodyTypesStr"] as string ?? "All Body Types";

    //            BodyTypesCollection.Add(new BodyTypeDto(0, allBodyTypesStr));


    //            foreach (var body in bodyTypeDtos.OrderBy(x => x.Id))
    //            {
    //                BodyTypesCollection.Add(body);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // Set an error message property for the UI
    //            // ValidationMessage = $"Failed to load data: {ex.Message}";
    //            // Or show a non-blocking toast
    //            // _wpfToastNotificationService.ShowError("Failed to load data. Please try again.");
    //            // Do NOT re-throw if the dialog is meant to stay open.
    //            _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
    //            await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Body Types Data, {ex.Message}", true, CurrentWorkFlow);

    //            // DEV ENV Only
    //            throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadBodyTypesAsync), MethodOperationType.LOAD_DATA, ex);

    //        }
    //        finally
    //        {
    //            IsLoadingBodies = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Populate Years collection with <see cref="YearFilterItem"/> with range for 30 years
    //    /// </summary>
    //    private void PopulateYearsCollection()
    //    {
    //        // Setup years
    //        YearsCollection.Clear();
    //        var allStr = System.Windows.Application.Current.Resources["AllStr"] as string ?? "All";

    //        YearsCollection.Add(new YearFilterItem(null, allStr));

    //        int currentYear = DateTime.Now.Year;
    //        for (int i = currentYear + 1; i >= currentYear - 30; i--)
    //        {
    //            YearsCollection.Add(
    //                new YearFilterItem(i, i.ToString())
    //            );
    //        }
    //    }

    //    #endregion

    //    #region Observable Props Customization (Makes, Models, Trims)

    //    // When SelectedMake changes, load models for it
    //    partial void OnSelectedMakeChanged(MakeDto? value)
    //    {
    //        if (value != null)
    //        {
    //            // Load models for the selected make
    //            _ = LoadModelsAsync(value.Id);
    //        }
    //        // Update CanExecute for commands if necessary
    //        ApplyFiltersCommand.NotifyCanExecuteChanged(); // Example
    //    }


    //    // When SelectedMake changes, load models for it
    //    partial void OnSelectedModelChanged(ModelDto? value)
    //    {
    //        if (value != null)
    //        {
    //            // Load models for the selected make
    //            _ = LoadTrimLevelsAsync(value.Id);
    //        }
    //        // Update CanExecute for commands if necessary
    //        ApplyFiltersCommand.NotifyCanExecuteChanged(); // Example
    //    }

    //    #endregion


    //    #region Commands

    //    [RelayCommand]
    //    private void ApplyFilters()
    //    {
    //        try
    //        {
    //            IsSearching = true;
    //            // Construct the DTO from current selections in the dialog
    //            _resultFilters = new CompatibleVehicleFilterCriteriaDto(
    //                MakeId: SelectedMake != null && SelectedMake.Id > 0 ? SelectedMake.Id : null,
    //                ModelId: SelectedModel != null && SelectedModel.Id > 0 ? SelectedModel.Id : null,
    //                ExactYear: SelectedExactYear?.Year,
    //                TrimLevelId: SelectedTrimLevel != null && SelectedTrimLevel.Id > 0 ? SelectedTrimLevel.Id : null,
    //                TransmissionTypeId: SelectedTransmission != null && SelectedTransmission.Id > 0 ? SelectedTransmission.Id : null,
    //                EngineTypeId: SelectedEngineType != null && SelectedEngineType.Id > 0 ? SelectedEngineType.Id : null,
    //                BodyTypeId: SelectedBodyType != null && SelectedBodyType.Id > 0 ? SelectedBodyType.Id : null
    //            );
    //            SetResultAndClose(true); // Signal success
    //        }
    //        finally
    //        {
    //            IsSearching = false; 
    //        }

    //    }

    //    [RelayCommand]
    //    private void ResetAndClearFilters()
    //    {
    //        SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);
    //        SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null);
    //        SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
    //        SelectedEngineType = EngineTypesCollection.FirstOrDefault(t => t.Id == 0);
    //        SelectedBodyType = BodyTypesCollection.FirstOrDefault(t => t.Id == 0);
    //    }

    //    [RelayCommand]
    //    private void Cancel()
    //    {
    //        SetResultAndClose(false);
    //    }

    //    [RelayCommand]
    //    private void ClearSelectedMake()
    //    {
    //        SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);

    //    }

    //    [RelayCommand]
    //    private void ClearSelectedYear()
    //    {
    //        SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null);

    //    }

    //    [RelayCommand]
    //    private void ClearSelectedTransmissionType()
    //    {
    //        SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
    //    }

    //    [RelayCommand]
    //    private void ClearSelectedBodyType()
    //    {
    //        SelectedBodyType = BodyTypesCollection.FirstOrDefault(t => t.Id == 0);
    //    }


    //    [RelayCommand]
    //    private void ClearSelectedEngineType()
    //    {
    //        SelectedEngineType = EngineTypesCollection.FirstOrDefault(t => t.Id == 0);
    //    }


    //    #endregion

    //    #region Dialog Specific Methods

    //    private void SetResultAndClose(bool res)
    //    {
    //        if (_dialog != null)
    //        {
    //            // Set the result first
    //            _dialog.DialogResult = res;

    //            // Then close with animation
    //            _dialog.Close();
    //        }

    //    }

    //    public void SetDialogWindow(IDialogWindow dialog)
    //    {
    //        if (dialog == null)
    //        {
    //            _logger.LogError("The Dialog window was null on {VM}", nameof(VehicleCompatibilityFilterOptionsDialog));
    //            return;
    //        }

    //        _dialog = dialog;
    //    }

    //    /// <summary>
    //    /// Set the Dialog Results. Inherited from <see cref="IDialogViewModelWithResult{TResult}"/> interface.
    //    /// </summary>
    //    /// <returns><see cref="CompatibleVehicleFilterCriteriaDto?"/></returns>
    //    /// <exception cref="NotImplementedException"></exception>
    //    public CompatibleVehicleFilterCriteriaDto? GetResult()
    //    {
    //        return _resultFilters; // Return the filters set by ApplyFilters
    //    }

    //    #endregion
    //}



}
