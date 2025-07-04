using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class MakesModelsTrimsManagementViewModel : BaseViewModel<MakesModelsTrimsManagementViewModel>, ITabViewModel
    {
        private readonly IWpfToastNotificationService _wpfToastNotificationService;
        private readonly IVehicleTaxonomyService _vehicleTaxonomyService;
        private readonly IDialogService _dialogService;


        #region General Properties

        [ObservableProperty] private bool _isVisible = false;
        [ObservableProperty] private bool _isLoading = false;
        [ObservableProperty] private bool _isLoadingMakes = false;
        [ObservableProperty] private bool _isLoadingModels = false;
        [ObservableProperty] private bool _isLoadingTrims = false;

        [ObservableProperty] private bool _isAddingMake = false;
        [ObservableProperty] private bool _isAddingModel = false;
        [ObservableProperty] private bool _isAddingTrim = false;

        [ObservableProperty]
        private bool _isInititalized = false;




        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ModelsColumnWidth))]
        private bool _hasAnyMake = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TrimsColumnWidth))]
        private bool _hasAnyModel = false;

        public GridLength ModelsColumnWidth => HasAnyMake && HasSelectedMake ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);
        public GridLength TrimsColumnWidth => HasAnyModel && HasSelectedModel ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);

        [ObservableProperty]
        private string _displayName = System.Windows.Application.Current.Resources["VehicleStructureStr"] as string ?? "Vehicle Structure";

        [ObservableProperty]
        private string _icon = "";

        #endregion


        #region Collections

        [ObservableProperty]
        private ObservableCollection<MakeDto> _makesCollection;

        [ObservableProperty]
        private ObservableCollection<ModelDto> _modelsCollection;

        [ObservableProperty]
        private ObservableCollection<TrimLevelDto> _trimLevelsCollection;

        #endregion


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedMake))]
        [NotifyPropertyChangedFor(nameof(ModelsColumnWidth))]
        private MakeDto? _selectedMake;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedModel))]
        [NotifyPropertyChangedFor(nameof(TrimsColumnWidth))]

        private ModelDto? _selectedModel;

        [ObservableProperty]
        private TrimLevelDto? _selectedTrimLevel;

        public bool HasSelectedMake => SelectedMake != null;
        public bool HasSelectedModel => SelectedModel != null;


        public MakesModelsTrimsManagementViewModel(
            IWpfToastNotificationService wpfToastNotificationService,
            IVehicleTaxonomyService vehicleTaxonomyService,
            IDialogService dialogService,
            ILocalizationService localizationService,
            ILogger<MakesModelsTrimsManagementViewModel> logger) : base(localizationService, logger)
        {
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _vehicleTaxonomyService = vehicleTaxonomyService ?? throw new ArgumentNullException(nameof(vehicleTaxonomyService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Initialize collections
            _makesCollection = new ObservableCollection<MakeDto>();
            _modelsCollection = new ObservableCollection<ModelDto>();
            _trimLevelsCollection = new ObservableCollection<TrimLevelDto>();

            LanguageDictionariesChanged += OnLanguageChanged;

            RegisterCleanup(() => LanguageDictionariesChanged -= OnLanguageChanged);

        }


        #region Loading Data

        public async Task InitializeAsync()
        {
            if (!IsInititalized)
            {
                await LoadMakesDataAsync();
                IsInititalized = true;
            }
        }

        private async Task LoadMakesDataAsync()
        {
            try
            {
                HasAnyMake = false;

                IsLoadingMakes = true;

                MakesCollection.Clear(); // Clear before loading new data

                var makes = await _vehicleTaxonomyService.GetAllMakesAsync();
                if (makes != null) // Check for null before Any()
                {
                    foreach (var make in makes.OrderBy(m => m.Id)) // Ensure ordered
                    {
                        MakesCollection.Add(make);
                    }
                    _logger.LogInformation("Loaded {Count} makes.", MakesCollection.Count);

                    HasAnyMake = MakesCollection.Any();

                }
                else
                {
                    _logger.LogWarning("GetAllMakesAsync returned null.");
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _wpfToastNotificationService.ShowError("Failed to load Makes. Please try again.", "Loading Error");
        
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(LoadMakesDataAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingMakes = false;
            }
        }

        private async Task LoadModelsForMakeAsync(int makeId)
        {
            if (makeId <= 0) return;

            try
            {
                IsLoadingModels = true;

                HasAnyModel = false;

                ModelsCollection.Clear(); // Clear before loading

                var models = await _vehicleTaxonomyService.GetModelsByMakeIdAsync(makeId);
                if (models != null)
                {
                    foreach (var model in models.OrderBy(m => m.Id))
                    {
                        ModelsCollection.Add(model);
                    }

                    HasAnyModel = ModelsCollection.Any();

                    _logger.LogInformation("Loaded {Count} models for Make ID {MakeId}.", ModelsCollection.Count, makeId);
                }
                else
                {
                    _logger.LogWarning("GetModelsByMakeIdAsync returned null for Make ID {MakeId}.", makeId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage} for Make ID {MakeId}", ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, makeId);
                _wpfToastNotificationService.ShowError($"Failed to load Models for {SelectedMake?.Name}. Please try again.", "Loading Error");
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(LoadModelsForMakeAsync), MethodOperationType.LOAD_DATA, ex);
            }
            finally
            {
                IsLoadingModels = false;
            }
        }

        private async Task LoadTrimsForModelAsync(int modelId)
        {
            if (modelId <= 0) return;

            try
            {
                IsLoadingTrims = true;
                TrimLevelsCollection.Clear(); // Clear before loading

                var trimLevels = await _vehicleTaxonomyService.GetTrimLevelsByModelIdAsync(modelId);
                if (trimLevels != null)
                {
                    foreach (var trim in trimLevels.OrderBy(t => t.Name))
                    {
                        TrimLevelsCollection.Add(trim);
                    }
                    _logger.LogInformation("Loaded {Count} trim levels for Model ID {ModelId}.", TrimLevelsCollection.Count, modelId);
                }
                else
                {
                    _logger.LogWarning("GetTrimLevelsByModelIdAsync returned null for Model ID {ModelId}.", modelId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage} for Model ID {ModelId}", ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, modelId);
                _wpfToastNotificationService.ShowError($"Failed to load Trim Levels for {SelectedModel?.Name}. Please try again.", "Loading Error");
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(LoadTrimsForModelAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingTrims = false;
            }
        }

        #endregion


        #region Selection Change 

        partial void OnSelectedMakeChanged(MakeDto? oldValue, MakeDto newValue) // Use generated overload with oldValue and newValue
        {
            _logger.LogInformation("SelectedMake changed from '{OldMakeName}' to '{NewMakeName}' (ID: {NewMakeId})",
                oldValue?.Name, newValue?.Name, newValue?.Id);

            // Clear dependent collections
            ModelsCollection?.Clear(); // Use null-conditional operator
            TrimLevelsCollection?.Clear();
            SelectedModel = null; // This will trigger OnSelectedModelChanged, which also clears trims

            if (newValue != null)
            {
                _ = LoadModelsForMakeAsync(newValue.Id);
            }
            else
            {

                HasAnyMake = MakesCollection.Any(); // Re-evaluate if needed, though if makes are cleared, it's false.
                HasAnyModel = false; // Explicitly set if Models are cleared due to no Make selected
                                     // ModelsColumnWidth will update because HasAnyMake changed
                                     // TrimsColumnWidth will update because HasAnyModel changed

            }
            // Notify CanExecute for commands that depend on SelectedMake
            ShowAddModelDialogCommand.NotifyCanExecuteChanged();
            // Ensure Edit/Delete Make commands are also updated if their CanExecute depends on selection.
            ShowEditMakeDialogCommand.NotifyCanExecuteChanged(); // Assuming these are the correct command names
            ShowDeleteMakeDialogCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedModelChanged(ModelDto? oldValue, ModelDto newValue) // Use generated overload
        {
            _logger.LogInformation("SelectedModel changed from '{OldModelName}' to '{NewModelName}' (ID: {NewModelId})",
                oldValue?.Name, newValue?.Name, newValue?.Id);

            // Clear dependent collection
            TrimLevelsCollection?.Clear();
            SelectedTrimLevel = null;

            if (newValue != null)
            {
                _ = LoadTrimsForModelAsync(newValue.Id);
            }
            else
            {
                // Optionally, update UI state if no model is selected
            }
            // Notify CanExecute for commands that depend on SelectedModel
            ShowAddModelDialogCommand.NotifyCanExecuteChanged();
            // Ensure Edit/Delete Make commands are also updated if their CanExecute depends on selection.
            ShowEditMakeDialogCommand.NotifyCanExecuteChanged(); // Assuming these are the correct command names
            ShowDeleteMakeDialogCommand.NotifyCanExecuteChanged();
        }

        #endregion


        #region Commands

        [RelayCommand]
        public async Task ShowAddMakeDialogAsync()
        {
            try
            {
                IsAddingMake = true;


                // open dialog
                var results = await _dialogService.ShowDialogAsync<AddMakeDialogViewModel, AddMakeDialog>(null);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new make has been added!");

                    await LoadMakesDataAsync();

                    _wpfToastNotificationService.ShowSuccess($"New make {MakesCollection.Last<MakeDto>()} has been added!");

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening AddMakeDialog");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowAddMakeDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }finally
            {
                IsAddingMake = false;
            }
        }

        [RelayCommand]
        public async Task ShowAddModelDialogAsync()
        {
            try
            {
                IsAddingModel = true;   

                if(SelectedMake == null)
                {
                    _wpfToastNotificationService.ShowError("You should select a make, before adding a Model!");
                    return;
                }

                var results = await _dialogService.ShowDialogAsync<AddModelDialogViewModel, AddModelDialog>(SelectedMake);
                if(results.HasValue && results.Value == true)
                {
                    await LoadModelsForMakeAsync(SelectedMake.Id);

                    _wpfToastNotificationService.ShowSuccess($"New model {ModelsCollection.Last()} has been Added!");

                    _logger.LogInformation("A new model {last} has been added!", ModelsCollection.Last());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening AddModelDialog");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowAddModelDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAddingModel = false;
            }
        }

        [RelayCommand]
        public async Task ShowAddTrimLevelDialogAsync()
        {
            try
            {
                IsAddingTrim = true;


                if (SelectedModel == null)
                {
                    _wpfToastNotificationService.ShowError("You should select a model, before adding a trim level!");
                    return;
                }

                var results = await _dialogService.ShowDialogAsync<AddTrimLevelDialogViewModel, AddTrimLevelDialog>(SelectedModel);
                if (results.HasValue && results.Value == true)
                {
                    await LoadTrimsForModelAsync(SelectedModel.Id);

                    _wpfToastNotificationService.ShowSuccess($"New trim {TrimLevelsCollection.Last()} has been Added!");

                    _logger.LogInformation("A new trim level {last} has been added!", TrimLevelsCollection.Last());

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening AddTrimLevelDialog");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowAddTrimLevelDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAddingTrim = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditMakeDialogAsync(MakeDto? makeToEdit)
        {
            if (makeToEdit == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditMakeDialogViewModel, EditMakeDialog>(makeToEdit);

                if (results.HasValue && results.Value == true)
                {
                    await LoadMakesDataAsync();

                    var updatedMake = MakesCollection.First(make => make.Id == makeToEdit.Id);

                    _wpfToastNotificationService.ShowSuccess($"Make {makeToEdit.Name} changed to {updatedMake.Name} successfully!");

                    _logger.LogInformation($"Make {makeToEdit.Name} changed to {updatedMake.Name} successfully!");
                }

            }
            catch (Exception ex) { 
            
            }
        }

        [RelayCommand]
        public async Task ShowEditModelDialogAsync(ModelDto modelDto)
        {
            if (modelDto == null) return;

            try
            {
                var results = await _dialogService.ShowDialogAsync<EditModelDialogViewModel, EditModelDialog>(modelDto);

                if (results.HasValue && results.Value == true)
                {
                    await LoadModelsForMakeAsync(modelDto.MakeId);

                    var updatedModel = MakesCollection.First(make => make.Id == modelDto.Id);

                    _wpfToastNotificationService.ShowSuccess($"Make {modelDto.Name} changed to {updatedModel.Name} successfully!");

                    _logger.LogInformation($"Make {modelDto.Name} changed to {updatedModel.Name} successfully!");
                }

            }
            catch (Exception ex) { }
        }

        [RelayCommand]
        public async Task ShowEditTrimLevelDialogAsync(TrimLevelDto trimToBeUpdated)
        {
            if (trimToBeUpdated == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditTrimLevelDialogViewModel, EditTrimLevelDialog>(trimToBeUpdated);

                if (results.HasValue && results.Value == true)
                {
                    await LoadTrimsForModelAsync(trimToBeUpdated.ModelId);

                    var updatedTrim = TrimLevelsCollection.First(trim => trim.Id == trimToBeUpdated.Id);

                    _wpfToastNotificationService.ShowSuccess($"Make {trimToBeUpdated.Name} changed to {updatedTrim.Name} successfully!");

                    _logger.LogInformation($"Make {trimToBeUpdated.Name} changed to {updatedTrim.Name} successfully!");
                }

            }
            catch (Exception ex) { }
        }

        [RelayCommand]
        public async Task ShowDeleteMakeDialogAsync(MakeDto makeToBeDeleted)
        {
            if(makeToBeDeleted  == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true) {

                try
                {
                    await _vehicleTaxonomyService.DeleteMakeAsync(makeToBeDeleted.Id);

                    await LoadMakesDataAsync();

                    _wpfToastNotificationService.ShowSuccess($"Make has been deleted successfully!");

                    _logger.LogInformation("Make with ID={ID} has been deleted successfully!", makeToBeDeleted.Id);

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
                        // Make dependent types more readable (e.g., "Trim Levels", "Vehicle Specifications")
                        dependentItemsText = string.Join(" and ", dx.DependentEntityTypes.Select(FormatDependentTypeName));
                    }

                    _wpfToastNotificationService.ShowError(
                        $"Cannot delete {entityDisplayName} '{makeToBeDeleted.Name}' because it is associated with {dependentItemsText}. Please remove these associations first.",
                        "Deletion Blocked",
                        TimeSpan.FromSeconds(10) // Longer duration for important messages
                    );
                    return; // Important: Exit the method after handling this specific error.
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting a Make Item");
                    _wpfToastNotificationService.ShowError($"An error has occurred while deleting the make {makeToBeDeleted.Name}");
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowDeleteMakeDialogAsync), MethodOperationType.DELETE_DATA, ex);


                }
            }
        }


        [RelayCommand]
        public async Task ShowDeleteModelDialogAsync(ModelDto modelToBeDeleted)
        {
            if (modelToBeDeleted == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {

                try
                {
                    await _vehicleTaxonomyService.DeleteModelAsync(modelToBeDeleted.Id);

                    await LoadModelsForMakeAsync(modelToBeDeleted.MakeId);

                    _wpfToastNotificationService.ShowSuccess($"Model has been deleted successfully!");

                    _logger.LogInformation("Model with ID={ID} has been deleted successfully!", modelToBeDeleted.Id);

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
                        // Make dependent types more readable (e.g., "Trim Levels", "Vehicle Specifications")
                        dependentItemsText = string.Join(" and ", dx.DependentEntityTypes.Select(FormatDependentTypeName));
                    }

                    _wpfToastNotificationService.ShowError(
                        $"Cannot delete {entityDisplayName} '{modelToBeDeleted.Name}' because it is associated with {dependentItemsText}. Please remove these associations first.",
                        "Deletion Blocked",
                        TimeSpan.FromSeconds(10) // Longer duration for important messages
                    );
                    return; // Important: Exit the method after handling this specific error.
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting a Model Item");
                    _wpfToastNotificationService.ShowError($"An error has occurred while deleting the model {modelToBeDeleted.Name}");
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowDeleteModelDialogAsync), MethodOperationType.DELETE_DATA, ex);


                }
            }
        }

        [RelayCommand]
        public async Task ShowDeleteTrimLevelDialogAsync(TrimLevelDto trimToBeDeleted)
        {
            if (trimToBeDeleted == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {

                try
                {
                    await _vehicleTaxonomyService.DeleteTrimLevelAsync(trimToBeDeleted.Id);

                    await LoadTrimsForModelAsync(trimToBeDeleted.ModelId);

                    _wpfToastNotificationService.ShowSuccess($"Trim Level has been deleted successfully!");

                    _logger.LogInformation("Trim Level with ID={ID} has been deleted successfully!", trimToBeDeleted.Id);

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
                        // Make dependent types more readable (e.g., "Trim Levels", "Vehicle Specifications")
                        dependentItemsText = string.Join(" and ", dx.DependentEntityTypes.Select(FormatDependentTypeName));
                    }

                    _wpfToastNotificationService.ShowError(
                        $"Cannot delete {entityDisplayName} '{trimToBeDeleted.Name}' because it is associated with {dependentItemsText}. Please remove these associations first.",
                        "Deletion Blocked",
                        TimeSpan.FromSeconds(10) // Longer duration for important messages
                    );
                    return; // Important: Exit the method after handling this specific error.
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting a Trim Level Item");
                    _wpfToastNotificationService.ShowError($"An error has occurred while deleting the trim level {trimToBeDeleted.Name}");
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowDeleteTrimLevelDialogAsync), MethodOperationType.DELETE_DATA, ex);


                }
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

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            DisplayName = System.Windows.Application.Current.Resources["VehicleStructureStr"] as string ?? "Vehicle Structure";
        }

        #endregion
    }
}
