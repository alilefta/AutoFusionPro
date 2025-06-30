using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Messages;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.Parts.Dialogs.AddEditPartDialogTabs.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEditPartDialogs
{
    public partial class DefineVehicleSpecificationDialogViewModel : BaseViewModel<DefineVehicleSpecificationDialogViewModel>, 
        IRecipient<SelectionChangedMessage<TrimLevelDto>>,
        IRecipient<SelectionChangedMessage<TransmissionTypeDto>>,
        IRecipient<SelectionChangedMessage<BodyTypeDto>>, 
        IRecipient<SelectionChangedMessage<EngineTypeDto>>, 
        IDialogViewModelWithResult<CreatePartCompatibilityRuleDto>
    {

        #region Fields

        /// <summary>
        /// Managing Parts backend service
        /// </summary>
        private readonly IVehicleTaxonomyService _vehicleTaxonomyService;

        private readonly IPartCompatibilityRuleService _partCompatibilityRuleService;

        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;

        private readonly IMessenger _messenger;


        private bool _isHandlingTrimSelectionChange = false; // Add a flag for each message type
        private bool _isHandlingTransmissionSelectionChange = false;
        private bool _isHandlingBodySelectionChange = false;
        private bool _isHandlingEngineSelectionChange = false;

        #endregion


        #region Props

        [ObservableProperty]
        private bool _isAdding = false;        
        
        [ObservableProperty]
        private bool _isLoadingExistingPartRules = false;

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
        [NotifyCanExecuteChangedFor(nameof(SaveCompatibilityRuleCommand))]
        private MakeDto? _selectedMake;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCompatibilityRuleCommand))]
        [NotifyPropertyChangedFor(nameof(IsTrimLevelEnabled))]
        private ModelDto? _selectedModel;


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCompatibilityRuleCommand))]
        private int _selectedStartYear;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCompatibilityRuleCommand))]
        private int _selectedEndYear;

        [ObservableProperty]
        private CreatePartCompatibilityRuleDto? _resultDto;



        [ObservableProperty]
        private PartCompatibilityRuleSummaryDto? _selectedExistingSpecFromList;

        [ObservableProperty]
        private string? _linkSpecNotes;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private bool _isActive = false;

        [ObservableProperty]
        private bool _setAsTemplate = false;


        [ObservableProperty]
        private ObservableCollection<PartCompatibilityRuleSummaryDto> _availableTemplates = new();

        [ObservableProperty]
        private PartCompatibilityRuleSummaryDto? _selectedTemplate;



        public bool IsTrimLevelEnabled => SelectedModel != null && SelectedModel.Id != 0;

        public bool IsAnyTrimLevel => TrimLevelsCollection.FirstOrDefault(item => item.DtoItem?.Id == 0)?.IsSelected ?? false;
        public bool IsAnyTransmissionType => TransmissionTypesCollection.FirstOrDefault(item => item.DtoItem?.Id == 0)?.IsSelected ?? false;
        public bool IsAnyEngineType => EngineTypesCollection.FirstOrDefault(item => item.DtoItem?.Id == 0)?.IsSelected ?? false;
        public bool IsAnyBodyType => BodyTypesCollection.FirstOrDefault(item => item.DtoItem?.Id == 0)?.IsSelected ?? false;




        #endregion

        #region Collections

        [ObservableProperty]
        private ObservableCollection<PartCompatibilityRuleSummaryDto> _existingMatchingSpecs = new();


        [ObservableProperty]
        private ObservableCollection<MakeDto> _makesCollection = new();

        [ObservableProperty]
        private ObservableCollection<ModelDto> _modelsCollection = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAnyTrimLevel))]
        private ObservableCollection<SelectableItemWrapper<TrimLevelDto>> _trimLevelsCollection = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAnyTransmissionType))]
        private ObservableCollection<SelectableItemWrapper<TransmissionTypeDto>> _transmissionTypesCollection = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAnyBodyType))]
        private ObservableCollection<SelectableItemWrapper<BodyTypeDto>> _bodyTypesCollection = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAnyEngineType))]
        private ObservableCollection<SelectableItemWrapper<EngineTypeDto>> _engineTypesCollection = new();

        [ObservableProperty]
        private ObservableCollection<int> _yearsCollection = new();


        [ObservableProperty]
        private ObservableCollection<PartCompatibilityRuleApplicableAttributeDto> _applicableTrimLevels = new();
        [ObservableProperty]
        private ObservableCollection<PartCompatibilityRuleApplicableAttributeDto> _applicableTransmissionTypes = new();
        [ObservableProperty]
        private ObservableCollection<PartCompatibilityRuleApplicableAttributeDto> _applicableEngineTypes = new();
        [ObservableProperty]
        private ObservableCollection<PartCompatibilityRuleApplicableAttributeDto> _applicableBodyTypes = new();


        #endregion

        public DefineVehicleSpecificationDialogViewModel(IVehicleTaxonomyService vehicleTaxonomyService, 
            IPartCompatibilityRuleService partCompatibilityRuleService, 
             ILocalizationService localizationService, 
             IMessenger messenger,
            ILogger<DefineVehicleSpecificationDialogViewModel> logger) : base(localizationService, logger)
        {
            _vehicleTaxonomyService = vehicleTaxonomyService ?? throw new ArgumentNullException(nameof(vehicleTaxonomyService));
            _partCompatibilityRuleService = partCompatibilityRuleService ?? throw new ArgumentNullException(nameof(partCompatibilityRuleService));
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));


            // Register to receive messages
            _messenger.RegisterAll(this);

            // Make sure to unregister when the VM is cleaned up
            // Your BaseViewModel should have a mechanism for this,
            // or you implement IDisposable
            RegisterCleanup(() => _messenger.UnregisterAll(this));



            _ = LoadInitialDataAsync();


        }


        #region Initialization & Data Loading

        private async Task LoadInitialDataAsync()
        {

            //await LoadExistingRuleTemplatesAsync();

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

        //private async Task LoadExistingRuleTemplatesAsync()
        //{
        //    try
        //    {
        //        IsLoadingExistingPartRules = true;

        //       // var existingRules = await _partCompatibilityRuleService.GetExit

        //    }
        //    finally
        //    {
        //        IsLoadingExistingPartRules = false;
        //    }
        //}

        /// <summary>
        /// Loads the Car Makes
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        /// <exception cref="ViewModelException">For Dev only</exception>
        private async Task LoadMakesAsync()
        {
            try
            {
                var makesData = await _vehicleTaxonomyService.GetAllMakesAsync();
                MakesCollection.Clear();

                var anyMakeStr = GetString("AnyMakeStr");

                MakesCollection.Add(new MakeDto(0, anyMakeStr, null));

                foreach (var make in makesData.OrderBy(x => x.Id))
                {
                    MakesCollection.Add(make);
                }

                // Optionally set a default selection if desired, but not required
                 SelectedMake = MakesCollection.FirstOrDefault(make => make.Id == 0);
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Makes Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(DefineVehicleSpecificationDialogViewModel), nameof(LoadMakesAsync), MethodOperationType.LOAD_DATA, ex);

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

                var anyModelStr = GetString("AnyModelStr");
                var defaultItem = new ModelDto(0, anyModelStr, 0, string.Empty, null);

                ModelsCollection.Clear();
                ModelsCollection.Add(defaultItem);


                if (makeId == 0)
                {
                    SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == 0);
                    OnPropertyChanged(nameof(IsTrimLevelEnabled));
                    return;
                }


                IsLoadingModels = true;

                var modelsData = await _vehicleTaxonomyService.GetModelsByMakeIdAsync(makeId);


                foreach (var model in modelsData.OrderBy(x => x.Id))
                {
                    ModelsCollection.Add(model);
                }

                // Optionally set a default selection if desired, but not required
                SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == 0);
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Models Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(DefineVehicleSpecificationDialogViewModel), nameof(LoadModelsAsync), MethodOperationType.LOAD_DATA, ex);

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
                var anyTrimlevelStr = GetString("AnyTrimLevelStr");
                var defaultItem = new TrimLevelDto(0, anyTrimlevelStr, 0, string.Empty);
                TrimLevelsCollection.Clear();
                TrimLevelsCollection.Add(new SelectableItemWrapper<TrimLevelDto>(defaultItem, true, _messenger));

                if (modelId == 0)
                {
                    OnPropertyChanged(nameof(IsTrimLevelEnabled));
                    return;
                }

                IsLoadingTrims = true;

                var trimsData = await _vehicleTaxonomyService.GetTrimLevelsByModelIdAsync(modelId);

                foreach (var trim in trimsData.OrderBy(x => x.Id))
                {
                    TrimLevelsCollection.Add(new SelectableItemWrapper<TrimLevelDto>(trim, false, _messenger));
                }

                OnPropertyChanged(nameof(IsTrimLevelEnabled));

            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Trim Levels Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(DefineVehicleSpecificationDialog), nameof(LoadTrimLevelsAsync), MethodOperationType.LOAD_DATA, ex);

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

                var anyTransmission = GetString("AnyTransmissionTypeStr");
                TransmissionTypesCollection.Add(new SelectableItemWrapper<TransmissionTypeDto>(new TransmissionTypeDto(0, anyTransmission), true, _messenger));

                foreach (var tty in transmissionTypeDtos.OrderBy(x => x.Id))
                {
                    TransmissionTypesCollection.Add(new SelectableItemWrapper<TransmissionTypeDto>(tty, false, _messenger));
                }

            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Transmission Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(DefineVehicleSpecificationDialogViewModel), nameof(LoadTransmissionTypesAsync), MethodOperationType.LOAD_DATA, ex);

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

                var anyEngineStr = GetString("AnyEngineTypeStr");
                EngineTypesCollection.Add(new SelectableItemWrapper<EngineTypeDto>(new EngineTypeDto(0, anyEngineStr, string.Empty), true, _messenger));

                foreach (var engine in engineTypeDtos.OrderBy(x => x.Id))
                {
                    EngineTypesCollection.Add(new SelectableItemWrapper<EngineTypeDto>(engine, false, _messenger));
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Failed to load data", $"an error occurred while loading Engine Types Data, {ex.Message}", true, CurrentWorkFlow);

                // DEV ENV Only
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(DefineVehicleSpecificationDialogViewModel), nameof(LoadEngineTypesAsync), MethodOperationType.LOAD_DATA, ex);

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

                var anyBodyType = GetString("AnyEngineTypeStr");
                BodyTypesCollection.Add(new SelectableItemWrapper<BodyTypeDto>(new BodyTypeDto(0, anyBodyType), true, _messenger));

                foreach (var body in bodyTypeDtos.OrderBy(x => x.Id))
                {
                    BodyTypesCollection.Add(new SelectableItemWrapper<BodyTypeDto>(body, false, _messenger));
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
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(DefineVehicleSpecificationDialogViewModel), nameof(LoadBodyTypesAsync), MethodOperationType.LOAD_DATA, ex);

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
            SaveCompatibilityRuleCommand.NotifyCanExecuteChanged(); // Example

            FormatCompatibilityName();
        }


        // When SelectedMake changes, load models for it
        partial void OnSelectedModelChanged(ModelDto? value)
        {
            TrimLevelsCollection.Clear();
            //SelectedTrimLevel = null; // Clear dependent selection

            if (value != null)
            {

                // Load models for the selected make
                _ = LoadTrimLevelsAsync(value.Id);
            }
            // Update CanExecute for commands if necessary
            SaveCompatibilityRuleCommand.NotifyCanExecuteChanged(); // Example

            FormatCompatibilityName();

        }

        partial void OnSelectedStartYearChanged(int value)
        {
            FormatCompatibilityName();
        }

        partial void OnSelectedEndYearChanged(int value)
        {
            if (SelectedEndYear < SelectedStartYear)
            {
                SelectedEndYear = SelectedStartYear;
            }
            FormatCompatibilityName();
        }


        partial void OnSelectedTemplateChanged(PartCompatibilityRuleSummaryDto? value)
        {
            if (value == null || value.Id == 0) return; // Ignore placeholder selection

            // User selected a template, now pre-fill the dialog
            _logger.LogInformation("Populating dialog from selected template: {TemplateName}", value.Name);
            // This requires a service call to get the *details* of the template rule
            _ = PopulateFromTemplateAsync(value.Id);
        }

        #endregion



        #region Command Methods


        private bool CanSaveCompatibilityRule()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   (SelectedMake != null || SelectedModel == null) && // Can't have model without make
                   (SelectedModel != null || (!TrimLevelsCollection.Any(t => t.IsSelected) || IsAnyTrimLevel)) && // Model implies Make was selected
                      SelectedStartYear >= 1900 && SelectedStartYear <= DateTime.Now.Year + 10 && // Basic year validation
                      SelectedEndYear >= 1900 && SelectedEndYear <= DateTime.Now.Year + 10 &&
                      SelectedEndYear >= SelectedStartYear &&
                      !IsAdding; // Prevent re-entrancy
        }

        [RelayCommand(CanExecute = nameof(CanSaveCompatibilityRule))]
        private async Task SaveCompatibilityRuleAsync()
        {

            // --- Perform final validation if needed ---
            if (!CanSaveCompatibilityRule()) // Re-check before proceeding
            {
                await MessageBoxHelper.ShowMessageWithoutTitleAsync("Please ensure the rule has a name and all selections are valid.", true, CurrentWorkFlow);
                return;
            }


            try
            {
                IsAdding = true;

                // --- 1. Gather Selections from UI ---

                // Gather applicable Trims
                var applicableTrims = IsAnyTrimLevel ? null : // If "Any Trim" is checked, the list is null (meaning "Any")
                    TrimLevelsCollection
                        .Where(w => w.IsSelected && w.DtoItem?.Id != 0) // Filter selected items, ignore placeholders
                        .Select(w => new PartCompatibilityRuleApplicableAttributeDto(w.DtoItem!.Id, w.DtoItem.Name, false)) // IsExclusion is false for now
                        .ToList();
                // If no specific trims are selected, the list will be empty, which also means "Any" for the service.

                // Gather applicable Engines
                var applicableEngines = IsAnyEngineType ? null :
                    EngineTypesCollection
                        .Where(w => w.IsSelected && w.DtoItem?.Id != 0)
                        .Select(w => new PartCompatibilityRuleApplicableAttributeDto(w.DtoItem!.Id, w.DtoItem.Name, false))
                        .ToList();

                // (Similar logic for Transmissions and BodyTypes)
                var applicableTransmissions = IsAnyTransmissionType ? null :
                    TransmissionTypesCollection
                        .Where(w => w.IsSelected && w.DtoItem?.Id != 0)
                        .Select(w => new PartCompatibilityRuleApplicableAttributeDto(w.DtoItem!.Id, w.DtoItem.Name, false))
                        .ToList();

                var applicableBodyTypes = IsAnyBodyType ? null :
                    BodyTypesCollection
                        .Where(w => w.IsSelected && w.DtoItem?.Id != 0)
                        .Select(w => new PartCompatibilityRuleApplicableAttributeDto(w.DtoItem!.Id, w.DtoItem.Name, false))
                        .ToList();

                // --- 2. Construct the SINGLE CreatePartCompatibilityRuleDto ---
                var ruleDto = new CreatePartCompatibilityRuleDto(
                    Name: Name.Trim(),
                    Description: string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    MakeId: (SelectedMake?.Id == 0) ? null : SelectedMake?.Id, // Handle "Any Make" selection
                    ModelId: (SelectedModel?.Id == 0) ? null : SelectedModel?.Id, // Handle "Any Model"
                    YearStart: SelectedStartYear,// Simplified year logic, needs a real ParseYearsFromInput helper
                    YearEnd: SelectedEndYear,
                    ApplicableTrimLevels: applicableTrims,
                    ApplicableEngineTypes: applicableEngines,
                    ApplicableTransmissionTypes: applicableTransmissions,
                    ApplicableBodyTypes: applicableBodyTypes,
                    Notes: string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                    IsActive: IsActive,
                    CreateAsTemplate: SetAsTemplate,
                    CopiedFromRuleId: null // Not handling copy in this flow
                );

                // --- 3. Set the result and close the dialog ---
                // The dialog's job is ONLY to construct this definition DTO.
                ResultDto = ruleDto;
                _logger.LogInformation("New Vehicle has been added successfully");

                await MessageBoxHelper.ShowMessageWithoutTitleAsync("New Vehicle has been added successfully", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while adding a new vehicle, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(DefineVehicleSpecificationDialogViewModel), nameof(SaveCompatibilityRuleAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

            }
            finally
            {
                IsAdding = false;
            }
        }


        [RelayCommand]
        private void Cancel()
        {
            ResultDto = null;
            SetResultAndClose(false);
        }
        #endregion

        #region Helpers

        private void FormatCompatibilityName()
        {

            if (SelectedMake == null || SelectedMake.Id == 0)
            {
                Name = GetString("GeneralCompatibilityRuleStr");
                return;
            }
            var nameParts = new List<string> { SelectedMake.Name };

            if (SelectedModel != null && SelectedModel.Id == 0)
            {
                nameParts.Add(GetString("AnyModelStr"));

            }

            if (SelectedModel != null && SelectedModel.Id != 0)
            {
                nameParts.Add(SelectedModel.Name);
            }



            if (SelectedEndYear >= SelectedStartYear)
            {
                nameParts.Add(SelectedStartYear == SelectedEndYear ? SelectedStartYear.ToString() : $"{SelectedStartYear}-{SelectedEndYear}");
            }
            // Now check the multi-select collections
            var selectedTrims = TrimLevelsCollection.Where(i => i.IsSelected && i.DtoItem?.Id != 0).Select(i => i.DtoItem!.Name).ToList();
            if (selectedTrims.Any())
            {
                nameParts.Add($"({string.Join(", ", selectedTrims)}) ");
            }


            var selectedTransmissions = TransmissionTypesCollection.Where(i => i.IsSelected && i.DtoItem?.Id != 0).Select(i => i.DtoItem!.Name).ToList();
            if (selectedTransmissions.Any())
            {
                nameParts.Add($" - ({string.Join(", ", selectedTransmissions)}) ");
            }

            var selectedEngines = EngineTypesCollection.Where(i => i.IsSelected && i.DtoItem?.Id != 0).Select(i => i.DtoItem!.Name).ToList();
            if (selectedEngines.Any())
            {
                nameParts.Add($" - ({string.Join(", ", selectedEngines)}) ");
            }

            var selectedBodies = BodyTypesCollection.Where(i => i.IsSelected && i.DtoItem?.Id != 0).Select(i => i.DtoItem!.Name).ToList();
            if (selectedBodies.Any())
            {
                nameParts.Add($" - ({string.Join(", ", selectedBodies)}) ");
            }


            Name = string.Join(" ", nameParts);

        }

        private async Task PopulateFromTemplateAsync(int ruleId)
        {
            var ruleDetails = await _partCompatibilityRuleService.GetRuleByIdAsync(ruleId);
            if (ruleDetails == null) return;

            await LoadInitialDataAsync();

            // Pre-fill all the selections in the dialog based on ruleDetails
            Name = $"Copy of {ruleDetails.Name}";
            Description = ruleDetails?.Description ?? string.Empty;
            SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == ruleDetails?.MakeId);
            SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == ruleDetails?.ModelId);
            SelectedStartYear = ruleDetails.YearStart ?? YearsCollection.FirstOrDefault();
            SelectedEndYear = ruleDetails.YearEnd ?? YearsCollection.FirstOrDefault();



            // Applicable Trim levels
            foreach (var selectedTrim in ApplicableTrimLevels)
            {
                if(selectedTrim.AttributeId == 0)
                {
                    var defaultItem = TrimLevelsCollection.FirstOrDefault(trim => trim.DtoItem?.Id == 0);
                    if(defaultItem != null) 
                        defaultItem.IsSelected = true;

                    return;
                }
                foreach (var item in TrimLevelsCollection)
                {
                    if (item.DtoItem?.Id == selectedTrim.AttributeId)
                    {
                        item.IsSelected = true;
                    }
                }
            }


                // ... wait for models to load, then select model...
                // ... select years ...
                // ... iterate through ruleDetails.ApplicableTrimLevels and set IsSelected=true on matching items in TrimLevelsCollection ...
                // ... and so on for all attributes ...
            }

        #endregion


            #region Dialog Methods

            /// <summary>
            /// Provided by <see cref="IDialogAware"/>
            /// </summary>
            /// <param name="dialog">Dialog will be provided internally by the service.</param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(DefineVehicleSpecificationDialog));
                return;
            }

            _dialog = dialog;
        }

        /// <summary>
        /// Helper for Setting results and Close
        /// </summary>
        /// <param name="res"></param>
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


        public CreatePartCompatibilityRuleDto? GetResult()
        {
            return ResultDto; // Return the filters set by ApplyFilters
        }


        #endregion

        #region Messaging Receivers

        public void Receive(SelectionChangedMessage<TrimLevelDto> message)
        {

            // If we are already in the middle of handling a change, ignore this new one to prevent loops.
            if (_isHandlingTrimSelectionChange) return;

            // Set the flag to indicate we are now processing.
            _isHandlingTrimSelectionChange = true;

            try
            {
                // Get the sender's DTO
                var senderDto = message.Sender.DtoItem;
                if (senderDto == null) return;

                if (senderDto.Id == 0) // The "Any Trim Level" item was changed
                {
                    if (message.IsSelected) // If "Any" was just CHECKED
                    {
                        // Uncheck all other items
                        foreach (var item in TrimLevelsCollection.Where(i => i.DtoItem?.Id != 0))
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else // A specific trim level was changed
                {
                    if (message.IsSelected) // If a specific trim was just CHECKED
                    {
                        // Find the "Any" item and uncheck it
                        var anyItem = TrimLevelsCollection.FirstOrDefault(i => i.DtoItem?.Id == 0);
                        if (anyItem != null)
                        {
                            anyItem.IsSelected = false;
                        }
                    }
                }
            }
            finally
            {
                _isHandlingTrimSelectionChange = false;
            }


            OnPropertyChanged(nameof(IsAnyTrimLevel));
            FormatCompatibilityName();
            SaveCompatibilityRuleCommand.NotifyCanExecuteChanged();
        }

        public void Receive(SelectionChangedMessage<TransmissionTypeDto> message)
        {
            if (_isHandlingTransmissionSelectionChange) return;

            _isHandlingTransmissionSelectionChange = true;

            try
            {
                var senderDto = message.Sender.DtoItem;
                if (senderDto == null) return;

                if (senderDto.Id == 0) // The "Any Trim Level" item was changed
                {
                    if (message.IsSelected) // If "Any" was just CHECKED
                    {
                        // Un-check all other items
                        foreach (var item in TransmissionTypesCollection.Where(i => i.DtoItem?.Id != 0))
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else // A specific trim level was changed
                {
                    if (message.IsSelected) // If a specific trim was just CHECKED
                    {
                        // Find the "Any" item and uncheck it
                        var anyItem = TransmissionTypesCollection.FirstOrDefault(i => i.DtoItem?.Id == 0);
                        if (anyItem != null)
                        {
                            anyItem.IsSelected = false;
                        }
                    }
                }
            }
            finally
            {
                _isHandlingTransmissionSelectionChange = false;
            }

            OnPropertyChanged(nameof(IsAnyTransmissionType));
            FormatCompatibilityName();
            SaveCompatibilityRuleCommand.NotifyCanExecuteChanged();
        }

        public void Receive(SelectionChangedMessage<BodyTypeDto> message)
        {

            if (_isHandlingBodySelectionChange) return;

            _isHandlingBodySelectionChange = true;

            try
            {

                // Get the sender's DTO
                var senderDto = message.Sender.DtoItem;
                if (senderDto == null) return;

                if (senderDto.Id == 0) // The "Any Trim Level" item was changed
                {
                    if (message.IsSelected) // If "Any" was just CHECKED
                    {
                        // Uncheck all other items
                        foreach (var item in BodyTypesCollection.Where(i => i.DtoItem?.Id != 0))
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else // A specific trim level was changed
                {
                    if (message.IsSelected) // If a specific trim was just CHECKED
                    {
                        // Find the "Any" item and uncheck it
                        var anyItem = BodyTypesCollection.FirstOrDefault(i => i.DtoItem?.Id == 0);
                        if (anyItem != null)
                        {
                            anyItem.IsSelected = false;
                        }
                    }
                }
            }
            finally
            {
                _isHandlingBodySelectionChange = false;
            }

            OnPropertyChanged(nameof(IsAnyBodyType));
            FormatCompatibilityName();
            SaveCompatibilityRuleCommand.NotifyCanExecuteChanged();
        }

        public void Receive(SelectionChangedMessage<EngineTypeDto> message)
        {
            if (_isHandlingEngineSelectionChange) return;

            _isHandlingEngineSelectionChange = true;


            try
            {
                // Get the sender's DTO
                var senderDto = message.Sender.DtoItem;
                if (senderDto == null) return;

                if (senderDto.Id == 0) // The "Any Trim Level" item was changed
                {
                    if (message.IsSelected) // If "Any" was just CHECKED
                    {
                        // Uncheck all other items
                        foreach (var item in EngineTypesCollection.Where(i => i.DtoItem?.Id != 0))
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else // A specific trim level was changed
                {
                    if (message.IsSelected) // If a specific trim was just CHECKED
                    {
                        // Find the "Any" item and uncheck it
                        var anyItem = EngineTypesCollection.FirstOrDefault(i => i.DtoItem?.Id == 0);
                        if (anyItem != null)
                        {
                            anyItem.IsSelected = false;
                        }
                    }
                }

            }
            finally
            {
                _isHandlingEngineSelectionChange = false;
            }
            OnPropertyChanged(nameof(IsAnyEngineType));
            FormatCompatibilityName();
            SaveCompatibilityRuleCommand.NotifyCanExecuteChanged();

        }
        #endregion
    }
}
