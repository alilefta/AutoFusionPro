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
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.BodyTypes;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.BodyTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class BodyTypesManagementViewModel : BaseViewModel<BodyTypesManagementViewModel>, ITabViewModel
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
        private string _displayName = System.Windows.Application.Current.Resources["BodyTypesStr"] as string ?? "Body Types";

        [ObservableProperty]
        private string _icon = "";


        [ObservableProperty]
        private ObservableCollection<BodyTypeDto> _bodyTypesCollection;

        [ObservableProperty]
        private BodyTypeDto _selectedBodyType = null;

        public BodyTypesManagementViewModel(IWpfToastNotificationService wpfToastNotificationService,
            ICompatibleVehicleService compatibleVehicleService,
            IDialogService dialogService,
            ILocalizationService localizationService,
            ILogger<BodyTypesManagementViewModel> logger) : base(localizationService, logger)
        {
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Initialize collections
            _bodyTypesCollection = new ObservableCollection<BodyTypeDto>();

            LanguageDictionariesChanged += OnLanguageChanged;

            RegisterCleanup(() => LanguageDictionariesChanged -= OnLanguageChanged);

        }



        #region Initialize and Load

        public async Task InitializeAsync()
        {
            if (!IsInititalized)
            {
                await LoadBodyTypesAsync();
                IsInititalized = true;
            }
        }

        private async Task LoadBodyTypesAsync()
        {
            try
            {
                IsLoading = true;

                BodyTypesCollection.Clear();

                var bodyTypes = await _compatibleVehicleService.GetAllBodyTypesAsync();
                if (bodyTypes != null)
                {
                    foreach (var body in bodyTypes.OrderBy(m => m.Name))
                    {
                        BodyTypesCollection.Add(body);
                    }
                    _logger.LogInformation("Loaded {Count} Body Types.", BodyTypesCollection.Count);

                }
                else
                {
                    _logger.LogWarning("No Body Types exist!");
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _wpfToastNotificationService.ShowError("Failed to load Body Types. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(BodyTypesManagementViewModel), nameof(LoadBodyTypesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task ShowAddBodyTypeDialogAsync()
        {
            try
            {
                IsAdding = true;

                var results = await _dialogService.ShowDialogAsync<AddBodyTypeDialogViewModel, AddBodyTypeDialog>(null);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new body type has been added!");

                    await LoadBodyTypesAsync();

                    _wpfToastNotificationService.ShowSuccess($"New Body type {BodyTypesCollection.Last<BodyTypeDto>()} has been added!");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddBodyTypeDialog)}");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(BodyTypesManagementViewModel), nameof(ShowAddBodyTypeDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditBodyTypeDialogAsync(BodyTypeDto? bodyToEdit)
        {
            if (bodyToEdit == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditBodyTypeDialogViewModel, EditBodyTypeDialog>(bodyToEdit);

                if (results.HasValue && results.Value == true)
                {
                    await LoadBodyTypesAsync();

                    var updatedTT = BodyTypesCollection.First(body => body.Id == bodyToEdit.Id);

                    _wpfToastNotificationService.ShowSuccess($"Body Type '{bodyToEdit.Name}' changed to '{updatedTT.Name}' successfully!");

                    _logger.LogInformation($"Body Type {bodyToEdit.Name} changed to {updatedTT.Name} successfully!");
                }

            }
            catch (Exception ex)
            {

            }
        }

        [RelayCommand]
        public async Task ShowDeleteBodyTypeDialogAsync(BodyTypeDto bodyTypeToDelete)
        {
            if (bodyTypeToDelete == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {

                try
                {
                    await _compatibleVehicleService.DeleteBodyTypeAsync(bodyTypeToDelete.Id);

                    await LoadBodyTypesAsync();

                    _wpfToastNotificationService.ShowSuccess($"Body Type has been deleted successfully!");

                    _logger.LogInformation("Body Type with ID={ID} has been deleted successfully!", bodyTypeToDelete.Id);

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
                        $"Cannot delete {entityDisplayName} '{bodyTypeToDelete.Name}' because it is associated with {dependentItemsText}. Please remove these associations first.",
                        "Deletion Blocked",
                        TimeSpan.FromSeconds(10) // Longer duration for important messages
                    );
                    return; // Important: Exit the method after handling this specific error.
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting a Body Type Item");
                    _wpfToastNotificationService.ShowError($"An error has occurred while deleting the Body Type {bodyTypeToDelete.Name}");
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(BodyTypesManagementViewModel), nameof(ShowDeleteBodyTypeDialogAsync), MethodOperationType.DELETE_DATA, ex);


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
            DisplayName = System.Windows.Application.Current.Resources["BodyTypesStr"] as string ?? "Body Types";
        }


    }
}
