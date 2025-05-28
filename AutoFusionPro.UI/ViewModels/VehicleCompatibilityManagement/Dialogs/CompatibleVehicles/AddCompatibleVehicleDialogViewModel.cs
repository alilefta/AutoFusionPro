using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Enums.UI.VehicleDialogs;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.CompatibleVehicles;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.CompatibleVehicles
{
    public partial class AddCompatibleVehicleDialogViewModel : BaseViewModel<AddCompatibleVehicleDialogViewModel>, IDialogAware
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
        private bool _isAdding = false;

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
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private MakeDto? _selectedMake;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private ModelDto? _selectedModel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private TrimLevelDto? _selectedTrimLevel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private TransmissionTypeDto? _selectedTransmission;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private EngineTypeDto? _selectedEngineType;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private BodyTypeDto? _selectedBodyType;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private int _selectedStartYear;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCompatibleVehicleCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private int _selectedEndYear;
        #endregion

        #region Collections

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

        #endregion

        #region UI Properties

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BasicInfoVisibility))]
        [NotifyPropertyChangedFor(nameof(TechnicalDetailsVisibility))]
        [NotifyPropertyChangedFor(nameof(BackButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(CancelButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(NextButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(FinishButtonVisibility))]
        private TwoStepWizard _currentStep = TwoStepWizard.BasicInfo;

        public Visibility BasicInfoVisibility => CurrentStep == TwoStepWizard.BasicInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility TechnicalDetailsVisibility => CurrentStep == TwoStepWizard.TechnicalDetails ? Visibility.Visible : Visibility.Collapsed;

        // Button Visibility
        public Visibility BackButtonVisibility => CurrentStep != TwoStepWizard.BasicInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CancelButtonVisibility => CurrentStep == TwoStepWizard.BasicInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NextButtonVisibility => CurrentStep == TwoStepWizard.BasicInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility FinishButtonVisibility => CurrentStep == TwoStepWizard.TechnicalDetails ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LoadingVisibility => IsAdding ? Visibility.Visible : Visibility.Collapsed;

        [ObservableProperty]
        private string _validationMessage = string.Empty;

        [ObservableProperty]
        private SymbolRegular _nextButtonIcon = SymbolRegular.ArrowRight48;

        #endregion

        #region Constructor

        public AddCompatibleVehicleDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<AddCompatibleVehicleDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));

            // Initialize Collections
            MakesCollection = new ObservableCollection<MakeDto>();
            ModelsCollection = new ObservableCollection<ModelDto>();
            TrimLevelsCollection = new ObservableCollection<TrimLevelDto>();
            TransmissionTypesCollection = new ObservableCollection<TransmissionTypeDto>();
            BodyTypesCollection = new ObservableCollection<BodyTypeDto>();
            EngineTypesCollection = new ObservableCollection<EngineTypeDto>();
            YearsCollection = new ObservableCollection<int>();

            NextButtonIcon = _localizationService.CurrentLangString != "ar-SA" ? SymbolRegular.ArrowRight48 : SymbolRegular.ArrowLeft48;

            LanguageDictionariesChanged += OnLanguageChanged;

            RegisterCleanup(() => LanguageDictionariesChanged -= OnLanguageChanged);

            _ = LoadInitialDataAsync();

        }

        #endregion

        #region Initialization & Data Loading

        private async Task LoadInitialDataAsync()
        {

            await LoadMakesAsync();
            await LoadEngineTypesAsync(); 
            await LoadTransmissionTypesAsync();
            await LoadBodyTypesAsync();

            // Setup years
            YearsCollection.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear + 1; i >= currentYear - 30; i--)
            {
                YearsCollection.Add(i);
            }

            // Set default year
            SelectedStartYear = currentYear;
            SelectedEndYear = currentYear;
        }

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

                foreach (var make in makesData.OrderBy(x => x.Id)) {
                    MakesCollection.Add(make);
                }

                // Optionally set a default selection if desired, but not required
                // SelectedMake = MakesCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Makes Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(AddCompatibleVehicleDialogViewModel), nameof(LoadMakesAsync), MethodOperationType.LOAD_DATA, ex);

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

                foreach (var model in modelsData.OrderBy(x => x.Id)) {
                    ModelsCollection.Add(model);
                }

                // Optionally set a default selection if desired, but not required
                // SelectecModel = ModelsCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Models Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(AddCompatibleVehicleDialogViewModel), nameof(LoadModelsAsync), MethodOperationType.LOAD_DATA, ex);

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

                foreach (var trim in trimsData.OrderBy(x => x.Id)) {
                    TrimLevelsCollection.Add(trim);
                }

                // Optionally set a default selection if desired, but not required
                // SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Trim Levels Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(AddCompatibleVehicleDialogViewModel), nameof(LoadTrimLevelsAsync), MethodOperationType.LOAD_DATA, ex);

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

                foreach (var tty in transmissionTypeDtos.OrderBy(x => x.Id))
                {
                    TransmissionTypesCollection.Add(tty);
                }

                // Optionally set a default selection if desired, but not required
                // SelectedTransmission = TransmissionTypesCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Transmission Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(AddCompatibleVehicleDialogViewModel), nameof(LoadTransmissionTypesAsync), MethodOperationType.LOAD_DATA, ex);

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

                foreach (var engine in engineTypeDtos.OrderBy(x => x.Id))
                {
                    EngineTypesCollection.Add(engine);
                }

                // Optionally set a default selection if desired, but not required
                // SelectedEngine = EngineTypesCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Engine Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(AddCompatibleVehicleDialogViewModel), nameof(LoadEngineTypesAsync), MethodOperationType.LOAD_DATA, ex);

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

                foreach (var body in bodyTypeDtos.OrderBy(x => x.Id))
                {
                    BodyTypesCollection.Add(body);
                }

                // Optionally set a default selection if desired, but not required
                // SelectedBody = BodyTypesCollection.FirstOrDefault();
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
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(AddCompatibleVehicleDialogViewModel), nameof(LoadBodyTypesAsync), MethodOperationType.LOAD_DATA, ex);

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
            SelectedModel = null; // Clear dependent selection

            if (value != null)
            {
                // Load models for the selected make
                _ = LoadModelsAsync(value.Id);
            }
            // Update CanExecute for commands if necessary
            AddCompatibleVehicleCommand.NotifyCanExecuteChanged(); // Example
        }


        // When SelectedMake changes, load models for it
        partial void OnSelectedModelChanged(ModelDto? value)
        {
            TrimLevelsCollection.Clear();
            SelectedTrimLevel = null; // Clear dependent selection

            if (value != null)
            {
                // Load models for the selected make
                _ = LoadTrimLevelsAsync(value.Id);
            }
            // Update CanExecute for commands if necessary
            AddCompatibleVehicleCommand.NotifyCanExecuteChanged(); // Example
        }

        #endregion

        #region Commands

        private bool CanAddCompatibleVehicle()
        {
            return CurrentStep == TwoStepWizard.TechnicalDetails &&
                      SelectedModel != null && // Model implies Make was selected
                      SelectedStartYear >= 1900 && SelectedStartYear <= DateTime.Now.Year + 10 && // Basic year validation
                      SelectedEndYear >= 1900 && SelectedEndYear <= DateTime.Now.Year + 10 &&
                      SelectedEndYear >= SelectedStartYear &&
                      !IsAdding; // Prevent re-entrancy
        }

        [RelayCommand(CanExecute = nameof(CanAddCompatibleVehicle))]
        private async Task AddCompatibleVehicleAsync()
        {
            try
            {
                IsAdding = true;

                var createVehicle = new CreateCompatibleVehicleDto(
                    SelectedModel!.Id, 
                    SelectedStartYear, 
                    SelectedEndYear, 
                    SelectedTrimLevel?.Id, 
                    SelectedTransmission?.Id, 
                    SelectedEngineType?.Id, 
                    SelectedBodyType?.Id, 
                    null);

                var newItem = await _compatibleVehicleService.CreateCompatibleVehicleAsync(createVehicle);
                _logger.LogInformation("New Vehicle has been added successfully");

                await MessageBoxHelper.ShowMessageWithoutTitleAsync("New Vehicle has been added successfully", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while creating new Compatible Vehicle, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"An error occurred while creating new Compatible Vehicle, {dx.Message}", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while adding a new vehicle, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddCompatibleVehicleDialogViewModel), nameof(AddCompatibleVehicleAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }

        private bool CanGoToNextStep()
        {
            if (CurrentStep == TwoStepWizard.BasicInfo)
            {
                // Check if Make, Model are filled and Year is valid
                return SelectedMake != null &&
                       SelectedModel != null &&
                       SelectedStartYear >= 1900 && SelectedStartYear <= 2100 &&
                       SelectedEndYear >= 1900 && SelectedEndYear <= 2100 &&
                       SelectedStartYear <= SelectedEndYear;
            }

            return false; // Cannot go next from last step
        }

        [RelayCommand(CanExecute = nameof(CanGoToNextStep))]
        private void GoToNextStep()
        {
            if (CurrentStep == TwoStepWizard.BasicInfo)
            {
                CurrentStep = TwoStepWizard.TechnicalDetails;
            }
            // Notify commands that depend on CurrentStep
            GoToNextStepCommand.NotifyCanExecuteChanged();
            GoToPreviousStepCommand.NotifyCanExecuteChanged();
            AddCompatibleVehicleCommand.NotifyCanExecuteChanged(); // Finish button visibility depends on step
        }


        [RelayCommand]
        private void GoToPreviousStep()
        {
            if (CurrentStep == TwoStepWizard.TechnicalDetails)
            {
                CurrentStep = TwoStepWizard.BasicInfo;
            }
            
            // Notify commands that depend on CurrentStep
            GoToNextStepCommand.NotifyCanExecuteChanged();
            GoToPreviousStepCommand.NotifyCanExecuteChanged();
            AddCompatibleVehicleCommand.NotifyCanExecuteChanged(); // Finish button visibility depends on step
        }

        #endregion

        #region Dialog Specific

        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on {VM}", nameof(AddCompatibleVehicleDialog));
                return;
            }

            _dialog = dialog;
        }

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

        #endregion

        #region Helpers

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            NextButtonIcon = _localizationService.CurrentLangString != "ar-SA" ? SymbolRegular.ArrowRight48 : SymbolRegular.ArrowLeft48;
        }

        #endregion

    }
}
