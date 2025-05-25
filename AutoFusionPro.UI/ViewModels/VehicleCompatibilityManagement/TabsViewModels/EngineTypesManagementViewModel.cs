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
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.EngineTypes;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.EngineTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{

    public partial class EngineTypesManagementViewModel : BaseViewModel<EngineTypesManagementViewModel>, ITabViewModel
    {
        private readonly IWpfToastNotificationService _wpfToastNotificationService;
        private readonly ICompatibleVehicleService _compatibleVehicleService;
        private readonly IDialogService _dialogService;
        [ObservableProperty]
        private bool _isVisible = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        private bool _isInititalized = false;

        [ObservableProperty]
        private string _displayName = System.Windows.Application.Current.Resources["EngineTypesStr"] as string ?? "Engine Types";

        [ObservableProperty]
        private string _icon = "";


        [ObservableProperty]
        private ObservableCollection<EngineTypeDto> _engineTypesCollection;

        [ObservableProperty]
        private EngineTypeDto _selectedEngineType = null;

        public EngineTypesManagementViewModel(IWpfToastNotificationService wpfToastNotificationService,
            ICompatibleVehicleService compatibleVehicleService,
            IDialogService dialogService,
            ILocalizationService localizationService,
            ILogger<EngineTypesManagementViewModel> logger) : base(localizationService, logger)
        {
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Initialize collections
            _engineTypesCollection = new ObservableCollection<EngineTypeDto>();

            LanguageDictionariesChanged += OnLanguageChanged;

            RegisterCleanup(() => LanguageDictionariesChanged -= OnLanguageChanged);

        }



        #region Initialize and Load

        public async Task InitializeAsync()
        {
            if (!IsInititalized)
            {
                await LoadEngineTypesAsync();
                IsInititalized = true;
            }
        }

        private async Task LoadEngineTypesAsync()
        {
            try
            {
                IsLoading = true;

                EngineTypesCollection.Clear();

                var engineTypes = await _compatibleVehicleService.GetAllEngineTypesAsync();
                if (engineTypes != null)
                {
                    foreach (var engine in engineTypes.OrderBy(m => m.Name))
                    {
                        EngineTypesCollection.Add(engine);
                    }
                    _logger.LogInformation("Loaded {Count} Engine Types.", EngineTypesCollection.Count);

                }
                else
                {
                    _logger.LogWarning("No Engine Types exist!");
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _wpfToastNotificationService.ShowError("Failed to load Engine Types. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(EngineTypesManagementViewModel), nameof(LoadEngineTypesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task ShowAddEngineTypeDialogAsync()
        {
            try
            {
                IsAdding = true;

                var results = await _dialogService.ShowDialogAsync<AddEngineTypeDialogViewModel, AddEngineTypeDialog>(null);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new engine type has been added!");

                    await LoadEngineTypesAsync();

                    _wpfToastNotificationService.ShowSuccess($"New Engine type {EngineTypesCollection.Last<EngineTypeDto>()} has been added!");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddEngineTypeDialog)}");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(EngineTypesManagementViewModel), nameof(ShowAddEngineTypeDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditEngineTypeDialogAsync(EngineTypeDto? engineToEdit)
        {
            if (engineToEdit == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditEngineTypeDialogViewModel, EditEngineTypeDialog>(engineToEdit);

                if (results.HasValue && results.Value == true)
                {
                    await LoadEngineTypesAsync();

                    var updatedEngineType = EngineTypesCollection.First(engine => engine.Id == engineToEdit.Id);

                    _wpfToastNotificationService.ShowSuccess($"Engine Type '{engineToEdit.Name}' changed to '{updatedEngineType.Name}' successfully!");

                    _logger.LogInformation($"Engine Type '{engineToEdit.Name}' changed to '{updatedEngineType.Name}' successfully!");
                }

            }
            catch (Exception ex)
            {

            }
        }

        [RelayCommand]
        public async Task ShowDeleteEngineTypeDialogAsync(EngineTypeDto engineTypeToDelete)
        {
            if (engineTypeToDelete == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {
                try
                {
                    await _compatibleVehicleService.DeleteEngineTypeAsync(engineTypeToDelete.Id);

                    await LoadEngineTypesAsync();

                    _wpfToastNotificationService.ShowSuccess($"Engine Type has been deleted successfully!");

                    _logger.LogInformation("Engine Type with ID={ID} has been deleted successfully!", engineTypeToDelete.Id);

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
                        $"Cannot delete {entityDisplayName} '{engineTypeToDelete.Name}' because it is associated with {dependentItemsText}. Please remove these associations first.",
                        "Deletion Blocked",
                        TimeSpan.FromSeconds(10) // Longer duration for important messages
                    );
                    return; // Important: Exit the method after handling this specific error.
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting a Engine Type Item");
                    _wpfToastNotificationService.ShowError($"An error has occurred while deleting the Engine Type {engineTypeToDelete.Name}");
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(EngineTypesManagementViewModel), nameof(ShowDeleteEngineTypeDialogAsync), MethodOperationType.DELETE_DATA, ex);


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

        #endregion

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            DisplayName = System.Windows.Application.Current.Resources["EngineTypesStr"] as string ?? "Engine Types";
        }


    }
}
