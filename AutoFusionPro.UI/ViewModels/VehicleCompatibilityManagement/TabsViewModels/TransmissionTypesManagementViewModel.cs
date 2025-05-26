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
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.Transmissions;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.Transmissions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class TransmissionTypesManagementViewModel : BaseViewModel<TransmissionTypesManagementViewModel>, ITabViewModel
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
        private string _displayName = System.Windows.Application.Current.Resources["TransmissionTypesStr"] as string ?? "Transmission Types";

        [ObservableProperty]
        private string _icon = "";


        [ObservableProperty]
        private ObservableCollection<TransmissionTypeDto> _transmissionTypesCollection;

        [ObservableProperty]
        private TransmissionTypeDto _selectedTransmissionType = null;

        public TransmissionTypesManagementViewModel(IWpfToastNotificationService wpfToastNotificationService,
            ICompatibleVehicleService compatibleVehicleService,
            IDialogService dialogService,
            ILocalizationService localizationService,
            ILogger<TransmissionTypesManagementViewModel> logger) : base(localizationService, logger)
        {
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Initialize collections
            _transmissionTypesCollection = new ObservableCollection<TransmissionTypeDto>();

            LanguageDictionariesChanged += OnLanguageChanged;

            RegisterCleanup(() => LanguageDictionariesChanged -= OnLanguageChanged);

        }



        #region Initialize and Load

        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                if (!IsInititalized)
                {
                    await LoadTransmissionTypesAsync();
                    IsInititalized = true;
                }


            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadTransmissionTypesAsync()
        {
            try
            {
                TransmissionTypesCollection.Clear();

                var transmissionTypes = await _compatibleVehicleService.GetAllTransmissionTypesAsync();
                if (transmissionTypes != null)
                {
                    foreach (var make in transmissionTypes.OrderBy(m => m.Name))
                    {
                        TransmissionTypesCollection.Add(make);
                    }
                    _logger.LogInformation("Loaded {Count} Transmission Types.", TransmissionTypesCollection.Count);

                }
                else
                {
                    _logger.LogWarning("No Transmission Types exist!");
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _wpfToastNotificationService.ShowError("Failed to load Transmission Types. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(TransmissionTypesManagementViewModel), nameof(LoadTransmissionTypesAsync), MethodOperationType.LOAD_DATA, ex);

            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task ShowAddTransmissionTypeDialogAsync()
        {
            try
            {
                IsAdding = true;

                var results = await _dialogService.ShowDialogAsync<AddTransmissionTypeDialogViewModel, AddTransmissionTypeDialog>(null);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new transmission type has been added!");

                    await LoadTransmissionTypesAsync();

                    _wpfToastNotificationService.ShowSuccess($"New Transmission type {TransmissionTypesCollection.Last<TransmissionTypeDto>()} has been added!");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddTransmissionTypeDialog)}");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(TransmissionTypesManagementViewModel), nameof(ShowAddTransmissionTypeDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditTransmissionTypeDialogAsync(TransmissionTypeDto? ttyToEdit)
        {
            if (ttyToEdit == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditTransmissionTypeDialogViewModel, EditTransmissionTypeDialog>(ttyToEdit);

                if (results.HasValue && results.Value == true)
                {
                    await LoadTransmissionTypesAsync();

                    var updatedTT = TransmissionTypesCollection.First(make => make.Id == ttyToEdit.Id);

                    _wpfToastNotificationService.ShowSuccess($"Transmission Type '{ttyToEdit.Name}' changed to '{updatedTT.Name}' successfully!");

                    _logger.LogInformation($"Transmission Type {ttyToEdit.Name} changed to {updatedTT.Name} successfully!");
                }

            }
            catch (Exception ex)
            {

            }
        }

        [RelayCommand]
        public async Task ShowDeleteTransmissionTypeDialogAsync(TransmissionTypeDto transmissionTypeDto)
        {
            if (transmissionTypeDto == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {

                try
                {
                    await _compatibleVehicleService.DeleteTransmissionTypeAsync(transmissionTypeDto.Id);

                    await LoadTransmissionTypesAsync();

                    _wpfToastNotificationService.ShowSuccess($"Transmission Type has been deleted successfully!");

                    _logger.LogInformation("Transmission Type with ID={ID} has been deleted successfully!", transmissionTypeDto.Id);

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
                        $"Cannot delete {entityDisplayName} '{transmissionTypeDto.Name}' because it is associated with {dependentItemsText}. Please remove these associations first.",
                        "Deletion Blocked",
                        TimeSpan.FromSeconds(10) // Longer duration for important messages
                    );
                    return; // Important: Exit the method after handling this specific error.
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting a Transmission Item");
                    _wpfToastNotificationService.ShowError($"An error has occurred while deleting the Transmission {transmissionTypeDto.Name}");
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(TransmissionTypesManagementViewModel), nameof(ShowDeleteTransmissionTypeDialogAsync), MethodOperationType.DELETE_DATA, ex);


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
            DisplayName = System.Windows.Application.Current.Resources["TransmissionTypesStr"] as string ?? "Transmission Types";
        }


    }
}
