using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.Core.Models;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.CompatibleVehicles;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.Filters;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.CompatibleVehicles;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.Filters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class CompatibleVehiclesViewModel : BaseViewModel<CompatibleVehiclesViewModel>, ITabViewModel
    {

        #region Private Fields

        private readonly ICompatibleVehicleService _compatibleVehicleService;
        private readonly IDialogService _dialogService;
        private readonly IWpfToastNotificationService _wpfToastNotificationService;

        #endregion

        [ObservableProperty]
        private bool _isAdding = false;       
        
        [ObservableProperty]
        private bool _isShowingFilters = false;

        [ObservableProperty]
        private bool _isInititalized = false;

        [ObservableProperty]
        private bool _isVisible = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _displayName = System.Windows.Application.Current.Resources["CompatibleVehicleStr"] as string ?? "Compatible Vehicles";
            
        [ObservableProperty]
        private string _icon = "";

        [ObservableProperty]
        private ObservableCollection<CompatibleVehicleSummaryDto> _compatibleVehiclesCollection;

        [ObservableProperty]
        private CompatibleVehicleSummaryDto _selectedCompatibleVehicle = null;

        [ObservableProperty]
        private CompatibleVehicleFilterCriteriaDto _currentlyAppliedFilters;

        #region Constructor
        public CompatibleVehiclesViewModel(
            ICompatibleVehicleService compatibleVehicleService,
            IWpfToastNotificationService wpfToastNotificationService,
            ILocalizationService localizationService,
            ILogger<CompatibleVehiclesViewModel> logger,
            IDialogService dialogService): base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Initialize collections
            _compatibleVehiclesCollection = new ObservableCollection<CompatibleVehicleSummaryDto>();

            LanguageDictionariesChanged += OnLanguageChanged;

            RegisterCleanup(() => LanguageDictionariesChanged -= OnLanguageChanged);
        }
        #endregion

        #region Initializer
        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;

                if (!IsInititalized)
                {
                    await LoadCompatibleVehiclesAsync();
                    IsInititalized = true;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion
        #region Loading

        private async Task LoadCompatibleVehiclesAsync(CompatibleVehicleFilterCriteriaDto? filters = null)
        {
            try
            {
                IsLoading = true;

                CompatibleVehiclesCollection.Clear();

                PagedResult<CompatibleVehicleSummaryDto> pagedResults;

                if (filters == null)
                {
                    var filterCriteria = new CompatibleVehicleFilterCriteriaDto();

                    pagedResults = await _compatibleVehicleService.GetFilteredCompatibleVehiclesAsync(filterCriteria, 0, 10);

                }
                else
                {
                    pagedResults = await _compatibleVehicleService.GetFilteredCompatibleVehiclesAsync(filters, 1, 10);

                }

                var compatibleVehicles = pagedResults.Items;

                if (compatibleVehicles != null)
                {
                    foreach (var vehicle in compatibleVehicles.OrderBy(v => v.Id))
                    {
                        CompatibleVehiclesCollection.Add(vehicle);
                    }
                    _logger.LogInformation("Loaded {Count} Compatible Vehicles.", CompatibleVehiclesCollection.Count);

                }
                else
                {
                    _logger.LogWarning("No Compatible Vehicles exist!");
                }
            }
            catch (Exception ex)
            {
                // No longer throwing ViewModelException, handle here
                _logger.LogError(ex, ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE);
                _wpfToastNotificationService.ShowError("Failed to load Compatible Vehicles. Please try again.", "Loading Error");

                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(CompatibleVehiclesViewModel), nameof(LoadCompatibleVehiclesAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task ShowAddCompatibleVehicleDialogAsync()
        {
            try
            {
                IsAdding = true;

                var results = await _dialogService.ShowDialogAsync<AddCompatibleVehicleDialogViewModel, AddCompatibleVehicleDialog>(null);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new compatible vehicle has been added!");

                    await LoadCompatibleVehiclesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemAddedSuccessfullyStr"] as string ?? "New Item Added Successfully!";

                    _wpfToastNotificationService.ShowSuccess(msg);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddCompatibleVehicleDialog)}");
                var msg = System.Windows.Application.Current.Resources["CreationOperationFailedStr"] as string ?? "Creation Operation Has Failed!";
                _wpfToastNotificationService.ShowError(msg);

                // DEV ENV ONLY
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(CompatibleVehiclesViewModel), nameof(ShowAddCompatibleVehicleDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditCompatibleVehicleDialogAsync(CompatibleVehicleSummaryDto? compatibleVehicleToEdit)
        {
            if (compatibleVehicleToEdit == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditCompatibleVehicleDialogViewModel, EditCompatibleVehicleDialog>(compatibleVehicleToEdit);

                if (results.HasValue && results.Value == true)
                {
                    await LoadCompatibleVehiclesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemUpdatedSuccessfullyStr"] as string ?? "Compatible Vehicle Updated Successfully!";

                    _wpfToastNotificationService.ShowSuccess(msg);

                    _logger.LogInformation("Transmission Type ID='{ID}' Updated Successfully!", compatibleVehicleToEdit.Id);
                }

            }
            catch (Exception ex)
            {
                var msg = System.Windows.Application.Current.Resources["UpdateOperationFailedStr"] as string ?? "Update Operation Failed";
                _wpfToastNotificationService.ShowError(msg);
                return;
            }
        }

        [RelayCommand]
        public async Task ShowDeleteCompatibleVehicleDialogAsync(CompatibleVehicleSummaryDto compatibleVehicleToDelete)
        {
            if (compatibleVehicleToDelete == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {

                try
                {
                    await _compatibleVehicleService.DeleteCompatibleVehicleAsync(compatibleVehicleToDelete.Id);

                    await LoadCompatibleVehiclesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemDeletedSuccessfullyStr"] as string ?? "Item Has Been Deleted Successfully!";


                    _wpfToastNotificationService.ShowSuccess(msg);

                    _logger.LogInformation("Transmission Type with ID={ID} has been deleted successfully!", compatibleVehicleToDelete.Id);

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
                        dependentItemsText = string.Join(" and ", dx.DependentEntityTypes.Select(FormatDependentTypeName));
                    }

                    var msgTitle = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";
                    var msg = System.Windows.Application.Current.Resources["BecauseTheFollowingItemsDependOnItStr"] as string ?? $"Because the following Items Depends on it:";
                    _wpfToastNotificationService.ShowError(msgTitle, $"{msg}\n{dependentItemsText}", TimeSpan.FromSeconds(10));
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting a Vehicle Compatibility Item");
                    var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                    _wpfToastNotificationService.ShowError(msg);
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(CompatibleVehiclesViewModel), nameof(ShowDeleteCompatibleVehicleDialogAsync), MethodOperationType.DELETE_DATA, ex);


                }
            }
        }

        [RelayCommand]
        public async Task ShowFilterOptionsDialogAsync()
        {
            try
            {
                IsShowingFilters = true;

                var newFilterCritieria = await _dialogService.ShowDialogWithResultsAsync<VehicleCompatibilityFilterOptionsDialogViewModel, VehicleCompatibilityFilterOptionsDialog, CompatibleVehicleFilterCriteriaDto>(CurrentlyAppliedFilters);

                if (newFilterCritieria is not null)
                {
                    _logger.LogInformation("A new compatible vehicle has been added!");

                    CurrentlyAppliedFilters = newFilterCritieria;

                    await LoadCompatibleVehiclesAsync(newFilterCritieria);

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync("Searched for Items", false, CurrentWorkFlow);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(VehicleCompatibilityFilterOptionsDialog)}");
                //var msg = System.Windows.Application.Current.Resources["CreationOperationFailedStr"] as string ?? "Creation Operation Has Failed!";
                //_wpfToastNotificationService.ShowError(msg);

                // DEV ENV ONLY
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(CompatibleVehiclesViewModel), nameof(ShowFilterOptionsDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsShowingFilters = false;
            }
        }

        #endregion


        #region Helpers

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            DisplayName = System.Windows.Application.Current.Resources["CompatibleVehicleStr"] as string ?? "Compatible Vehicles";
        }
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

    }
}
