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
    public partial class VehicleCompatibilityFilterOptionsDialogViewModel : InitializableViewModel<VehicleCompatibilityFilterOptionsDialogViewModel>, IDialogViewModelWithResult<CompatibleVehicleFilterCriteriaDto>
    {

        #region Fields
        /// <summary>
        /// Value Provided by DI container to manage Models.
        /// </summary>
        private readonly ICompatibleVehicleService _compatibleVehicleService;
        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;
        #endregion

        #region Props

        [ObservableProperty]
        private bool _isSearching = false;

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
        private MakeDto? _selectedMake;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
        private ModelDto? _selectedModel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsMakeModelTrimFilterActive))]
        private TrimLevelDto? _selectedTrimLevel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsTransmissionTypeSelected))]
        private TransmissionTypeDto? _selectedTransmission;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsEngineTypeSelected))]
        private EngineTypeDto? _selectedEngineType;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsBodyTypeSelected))]
        private BodyTypeDto? _selectedBodyType;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFiltersCommand))]
        [NotifyPropertyChangedFor(nameof(IsExactYearSelected))]
        private int _selectedExactYear = 0;

        [ObservableProperty]
        private ObservableCollection<MakeDto> _makesCollection;

        [ObservableProperty]
        private ObservableCollection<ModelDto> _modelsCollection;

        [ObservableProperty]
        private ObservableCollection<TrimLevelDto> _trimLevelsCollection;

        [ObservableProperty]
        private ObservableCollection<TransmissionTypeDto> _transmissionTypesCollection;

        [ObservableProperty]
        private ObservableCollection<BodyTypeDto> _bodyTypesCollection;

        [ObservableProperty]
        private ObservableCollection<EngineTypeDto> _engineTypesCollection;

        [ObservableProperty]
        private ObservableCollection<int> _yearsCollection;

        private CompatibleVehicleFilterCriteriaDto? _resultFilters; // Store the filters to be returned

        public bool IsMakeModelTrimFilterActive => (SelectedMake != null && SelectedMake.Id > 0) || (SelectedModel != null && SelectedModel.Id > 0) || (SelectedTrimLevel != null && SelectedTrimLevel.Id > 0);
        public bool IsTransmissionTypeSelected => SelectedTransmission != null && SelectedTransmission.Id > 0;
        public bool IsEngineTypeSelected => SelectedEngineType != null && SelectedEngineType.Id > 0;
        public bool IsBodyTypeSelected => SelectedBodyType != null && SelectedBodyType.Id > 0;
        public bool IsExactYearSelected => SelectedExactYear != 0;

        #endregion

        #region Constructor
        public VehicleCompatibilityFilterOptionsDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<VehicleCompatibilityFilterOptionsDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));

            MakesCollection = new ObservableCollection<MakeDto>();
            ModelsCollection = new ObservableCollection<ModelDto>();
            TrimLevelsCollection = new ObservableCollection<TrimLevelDto>();
            TransmissionTypesCollection = new ObservableCollection<TransmissionTypeDto>();
            BodyTypesCollection = new ObservableCollection<BodyTypeDto>();
            EngineTypesCollection = new ObservableCollection<EngineTypeDto>();
            YearsCollection = new ObservableCollection<int>();
        }
        #endregion

        #region Initializer
        /// <summary>
        /// Inherited from <see cref="InitializableViewModel{TViewModel}"/>
        /// </summary>
        /// <param name="parameter"><see cref="null"/></param>
        /// <returns><see cref="void"/></returns>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (IsInitialized) return;

            await LoadMakesAsync();
            await LoadEngineTypesAsync();
            await LoadTransmissionTypesAsync();
            await LoadBodyTypesAsync();

            if (parameter is CompatibleVehicleFilterCriteriaDto currentFilters)
            {
                if (currentFilters.MakeId != null && currentFilters.MakeId.HasValue)
                {
                    SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == currentFilters.MakeId);
                }


                if (currentFilters.ModelId != null && currentFilters.ModelId.HasValue)
                {
                    SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == currentFilters.ModelId);
                }

                if (currentFilters.TrimLevelId != null && currentFilters.TrimLevelId.HasValue)
                {
                    SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(m => m.Id == currentFilters.TrimLevelId);
                }

                if (currentFilters.EngineTypeId != null && currentFilters.EngineTypeId.HasValue)
                {
                    SelectedEngineType = EngineTypesCollection.FirstOrDefault(m => m.Id == currentFilters.EngineTypeId);
                }

                if (currentFilters.BodyTypeId != null && currentFilters.BodyTypeId.HasValue)
                {
                    SelectedBodyType = BodyTypesCollection.FirstOrDefault(m => m.Id == currentFilters.BodyTypeId);
                }

                if (currentFilters.TransmissionTypeId != null && currentFilters.TransmissionTypeId.HasValue)
                {
                    SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == currentFilters.TransmissionTypeId);
                }
            }



            // Setup years
            YearsCollection.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear + 1; i >= currentYear - 30; i--)
            {
                YearsCollection.Add(i);
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
                var makesData = await _compatibleVehicleService.GetAllMakesAsync();
                MakesCollection.Clear();

                MakesCollection.Add(new MakeDto(0, string.Empty, string.Empty));

                foreach (var make in makesData.OrderBy(x => x.Id))
                {
                    MakesCollection.Add(make);
                }

                SelectedMake = MakesCollection.FirstOrDefault();
                // Optionally set a default selection if desired, but not required
                // SelectedMake = MakesCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Makes Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadMakesAsync), MethodOperationType.LOAD_DATA, ex);

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
                IsLoadingModels = true;

                var modelsData = await _compatibleVehicleService.GetModelsByMakeIdAsync(makeId);
                ModelsCollection.Clear();

                ModelsCollection.Add(new ModelDto(0, string.Empty, 0, string.Empty, null));


                foreach (var model in modelsData.OrderBy(x => x.Id))
                {
                    ModelsCollection.Add(model);
                }

                // Optionally set a default selection if desired, but not required
                SelectedModel = ModelsCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Models Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadModelsAsync), MethodOperationType.LOAD_DATA, ex);

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

                var trimsData = await _compatibleVehicleService.GetTrimLevelsByModelIdAsync(modelId);
                TrimLevelsCollection.Clear();

                TrimLevelsCollection.Add(new TrimLevelDto(0, string.Empty, 0, string.Empty));


                foreach (var trim in trimsData.OrderBy(x => x.Id))
                {
                    TrimLevelsCollection.Add(trim);
                }

                // Optionally set a default selection if desired, but not required
                SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Trim Levels Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadTrimLevelsAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingTrims = true;

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
                var transmissionTypeDtos = await _compatibleVehicleService.GetAllTransmissionTypesAsync();
                TransmissionTypesCollection.Clear();
                TransmissionTypesCollection.Add(new TransmissionTypeDto(0, string.Empty));


                foreach (var tty in transmissionTypeDtos.OrderBy(x => x.Id))
                {
                    TransmissionTypesCollection.Add(tty);
                }

                // Optionally set a default selection if desired, but not required
                SelectedTransmission = TransmissionTypesCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Transmission Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadTransmissionTypesAsync), MethodOperationType.LOAD_DATA, ex);

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

                var engineTypeDtos = await _compatibleVehicleService.GetAllEngineTypesAsync();
                EngineTypesCollection.Clear();

                EngineTypesCollection.Add(new EngineTypeDto(0, string.Empty, null));


                foreach (var engine in engineTypeDtos.OrderBy(x => x.Id))
                {
                    EngineTypesCollection.Add(engine);
                }

                // Optionally set a default selection if desired, but not required
                SelectedEngineType = EngineTypesCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Engine Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadEngineTypesAsync), MethodOperationType.LOAD_DATA, ex);

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

                var bodyTypeDtos = await _compatibleVehicleService.GetAllBodyTypesAsync();
                BodyTypesCollection.Clear();

                BodyTypesCollection.Add(new BodyTypeDto(0, string.Empty));


                foreach (var body in bodyTypeDtos.OrderBy(x => x.Id))
                {
                    BodyTypesCollection.Add(body);
                }

                // Optionally set a default selection if desired, but not required
                SelectedBodyType = BodyTypesCollection.FirstOrDefault();
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
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(VehicleCompatibilityFilterOptionsDialogViewModel), nameof(LoadBodyTypesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingBodies = false;
            }
        }

        #endregion


        #region Observable Props Customization (Makes, Models, Trims)

        // When SelectedMake changes, load models for it
        partial void OnSelectedMakeChanged(MakeDto? value)
        {
            ModelsCollection.Clear();
            SelectedModel = null;

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
            TrimLevelsCollection.Clear();
            SelectedTrimLevel = null;

            if (value != null)
            {
                // Load models for the selected make
                _ = LoadTrimLevelsAsync(value.Id);
            }
            // Update CanExecute for commands if necessary
            ApplyFiltersCommand.NotifyCanExecuteChanged(); // Example
        }

        #endregion


        #region Commands

        [RelayCommand]
        private void ApplyFilters()
        {
            // Construct the DTO from current selections in the dialog
            _resultFilters = new CompatibleVehicleFilterCriteriaDto(
                MakeId: SelectedMake != null && SelectedMake.Id > 0 ? SelectedMake.Id : 0,
                ModelId: SelectedModel != null && SelectedModel.Id > 0 ? SelectedModel.Id : 0,
                ExactYear: SelectedExactYear,
                TrimLevelId: SelectedTrimLevel != null && SelectedTrimLevel.Id > 0 ? SelectedTrimLevel.Id : 0,
                TransmissionTypeId: SelectedTransmission != null && SelectedTransmission.Id > 0 ? SelectedTransmission.Id : 0,
                EngineTypeId: SelectedEngineType != null && SelectedEngineType.Id > 0 ? SelectedEngineType.Id : 0,
                BodyTypeId: SelectedBodyType != null && SelectedBodyType.Id > 0 ? SelectedBodyType.Id : 0
            );
            SetResultAndClose(true); // Signal success
        }

        [RelayCommand]
        private void ResetAndClearFilters() // Renamed from just ResetFilters to be clear
        {
            SelectedMake = new MakeDto(0, string.Empty, null); // Cascading selection changes will clear Models, Trims
            SelectedExactYear = 0;
            SelectedTransmission = new TransmissionTypeDto(0, string.Empty);
            SelectedEngineType = new EngineTypeDto(0, string.Empty, null);
            SelectedBodyType = new BodyTypeDto(0, string.Empty); ;
            // Do NOT close the dialog on reset. Let the user "Apply" the cleared filters or "Cancel".
            // If you want Apply to happen on reset, then construct an empty DTO for _resultFilters.
        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
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
                _logger.LogError("The Dialog window was null on {VM}", nameof(VehicleCompatibilityFilterOptionsDialog));
                return;
            }

            _dialog = dialog;
        }

        /// <summary>
        /// Set the Dialog Results. Inherited from <see cref="IDialogViewModelWithResult{TResult}"/> interface.
        /// </summary>
        /// <returns><see cref="CompatibleVehicleFilterCriteriaDto?"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public CompatibleVehicleFilterCriteriaDto? GetResult()
        {
            return _resultFilters; // Return the filters set by ApplyFilters
        }

        #endregion
    }
}
