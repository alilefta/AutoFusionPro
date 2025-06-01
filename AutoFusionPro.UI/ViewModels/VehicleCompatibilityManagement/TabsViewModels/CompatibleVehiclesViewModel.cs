using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
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
using System.ComponentModel;
using System.Windows.Data;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class CompatibleVehiclesViewModel : BaseViewModel<CompatibleVehiclesViewModel>, ITabViewModel
    {

        #region Private Fields

        private readonly ICompatibleVehicleService _compatibleVehicleService;
        private readonly IDialogService _dialogService;
        private readonly IWpfToastNotificationService _wpfToastNotificationService;

        private Timer? _searchDebounceTimer;
        private const int SearchDebounceDelayMs = 500; // Adjust as needed (e.g., 300-750ms)

        #endregion

        // Pagination is not required
        //[ObservableProperty] private int _pageSize = 10; // Default
        //[ObservableProperty] private int _totalItems;
        //[ObservableProperty] private int _totalPages;

        //[ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
        //[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
        //private int _currentPage = 1; // Start at page 1

        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        private bool _isShowingFilters = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTextSearchQuery))]
        [NotifyPropertyChangedFor(nameof(IsFiltersActive))]
        private string? _searchQueryText;

        [ObservableProperty]
        private bool _isInititalized = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _isVisible = false;



        [ObservableProperty]
        private string _displayName = System.Windows.Application.Current.Resources["CompatibleVehicleStr"] as string ?? "Compatible Vehicles";

        [ObservableProperty]
        private string _icon = "";

        //private ListCollectionView _vehiclesCollectionView;

        [ObservableProperty]
        private ObservableCollection<CompatibleVehicleSummaryDto> _compatibleVehiclesCollection;

        [ObservableProperty]
        private CompatibleVehicleSummaryDto _selectedCompatibleVehicle = null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFiltersActive))]
        private CompatibleVehicleFilterCriteriaDto _currentlyAppliedFilters;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFiltersActive))]
        private ObservableCollection<ActiveFilterDisplayItem> _activeFiltersDisplayCollection = new();


        public bool HasTextSearchQuery => SearchQueryText != null && SearchQueryText.Length > 0;

        public bool IsFiltersActive => CurrentlyAppliedFilters != null &&
            (CurrentlyAppliedFilters.MakeId != null || CurrentlyAppliedFilters.ModelId != null ||
            CurrentlyAppliedFilters.TrimLevelId != null || CurrentlyAppliedFilters.TransmissionTypeId != null ||
            CurrentlyAppliedFilters.EngineTypeId != null || CurrentlyAppliedFilters.BodyTypeId != null ||
            CurrentlyAppliedFilters.ExactYear != null || CurrentlyAppliedFilters.SearchTerm != null);



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

            //_vehiclesCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(_compatibleVehiclesCollection);
            //_vehiclesCollectionView.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));

            LanguageDictionariesChanged += OnLanguageChanged;

            RegisterCleanup(() => LanguageDictionariesChanged -= OnLanguageChanged);

            RegisterCleanup(() => _searchDebounceTimer?.Dispose()); // If using your BaseViewModel's 
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

                IEnumerable<CompatibleVehicleSummaryDto> compatibleVehicles;

                filters ??= new CompatibleVehicleFilterCriteriaDto(); // Ensure filters is not null
                CurrentlyAppliedFilters = filters; // Update the VM's current filters

                compatibleVehicles = await _compatibleVehicleService.GetAllFilteredCompatibleVehiclesAsync(filters);

                //CurrentPage = pagedResults.PageNumber; // Ensure this reflects the actual returned page
                //PageSize = pagedResults.PageSize;     // Update if service can change it
                //TotalItems = pagedResults.TotalCount;
                //TotalPages = pagedResults.TotalPages;

                if (compatibleVehicles != null)
                {
                    foreach (var vehicle in compatibleVehicles) // compatibleVehicles is already IEnumerable<CompatibleVehicleSummaryDto>
                    {
                        CompatibleVehiclesCollection.Add(vehicle);
                    }
                    _logger.LogInformation("Loaded {Count} Compatible Vehicles.", CompatibleVehiclesCollection.Count);
                }
                else
                {
                    _logger.LogWarning("GetAllFilteredCompatibleVehiclesAsync returned null.");
                }

                //_vehiclesCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(CompatibleVehiclesCollection);
                //_vehiclesCollectionView.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
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

        private async Task UpdateActiveFiltersDisplayAsync()
        {
            ActiveFiltersDisplayCollection.Clear();
            if (CurrentlyAppliedFilters == null) return;

            if (CurrentlyAppliedFilters.MakeId.HasValue)
            {
                var makeStr = System.Windows.Application.Current.Resources["MakeStr"] as string ?? "Make";
                var make = await _compatibleVehicleService.GetMakeByIdAsync(CurrentlyAppliedFilters.MakeId.Value);
                if (make != null)
                    ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(makeStr, make.Name, nameof(CompatibleVehicleFilterCriteriaDto.MakeId), make.Id));
            }
            if (CurrentlyAppliedFilters.ModelId.HasValue)
            {
                var modelStr = System.Windows.Application.Current.Resources["ModelStr"] as string ?? "Model";

                var model = await _compatibleVehicleService.GetModelByIdAsync(CurrentlyAppliedFilters.ModelId.Value);
                if (model != null)
                    ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(modelStr, model.Name, nameof(CompatibleVehicleFilterCriteriaDto.ModelId), model.Id));
            }


            if (CurrentlyAppliedFilters.TrimLevelId.HasValue)
            {
                var trimStr = System.Windows.Application.Current.Resources["TrimStr"] as string ?? "Trim";

                var trim = await _compatibleVehicleService.GetTrimLevelByIdAsync(CurrentlyAppliedFilters.TrimLevelId.Value);
                if (trim != null)
                    ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(trimStr, trim.Name, nameof(CompatibleVehicleFilterCriteriaDto.TrimLevelId), trim.Id));
            }

            if (CurrentlyAppliedFilters.TransmissionTypeId.HasValue)
            {
                var transmissionStr = System.Windows.Application.Current.Resources["TransmissionStr"] as string ?? "Transmission";

                var transmission = await _compatibleVehicleService.GetTransmissionTypeByIdAsync(CurrentlyAppliedFilters.TransmissionTypeId.Value);
                if (transmission != null)
                    ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(transmissionStr, transmission.Name, nameof(CompatibleVehicleFilterCriteriaDto.TransmissionTypeId), transmission.Id));
            }

            if (CurrentlyAppliedFilters.EngineTypeId.HasValue)
            {
                var engineStr = System.Windows.Application.Current.Resources["EngineStr"] as string ?? "Engine";

                var engine = await _compatibleVehicleService.GetEngineTypeByIdAsync(CurrentlyAppliedFilters.EngineTypeId.Value);
                if (engine != null)
                    ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(engineStr, engine.Name, nameof(CompatibleVehicleFilterCriteriaDto.EngineTypeId), engine.Id));
            }

            if (CurrentlyAppliedFilters.BodyTypeId.HasValue)
            {
                var bodyStr = System.Windows.Application.Current.Resources["BodyStr"] as string ?? "Body";

                var body = await _compatibleVehicleService.GetBodyTypeByIdAsync(CurrentlyAppliedFilters.BodyTypeId.Value);
                if (body != null)
                    ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(bodyStr, body.Name, nameof(CompatibleVehicleFilterCriteriaDto.BodyTypeId), body.Id));
            }

            if (CurrentlyAppliedFilters.ExactYear.HasValue)
            {
                var yearStr = System.Windows.Application.Current.Resources["YearStr"] as string ?? "Year";

                ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(yearStr, CurrentlyAppliedFilters.ExactYear.Value.ToString(), nameof(CompatibleVehicleFilterCriteriaDto.ExactYear), CurrentlyAppliedFilters.ExactYear.Value));
            }

            if (!string.IsNullOrWhiteSpace(CurrentlyAppliedFilters.SearchTerm))
            {
                var searchStr = System.Windows.Application.Current.Resources["SearchStr"] as string ?? "Search";
                ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(searchStr, CurrentlyAppliedFilters.SearchTerm, nameof(CompatibleVehicleFilterCriteriaDto.SearchTerm), CurrentlyAppliedFilters.SearchTerm));
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

                var filtersForDialog = CurrentlyAppliedFilters ?? new CompatibleVehicleFilterCriteriaDto();

                var newFilterCriteriaFromDialog = await _dialogService.ShowDialogWithResultsAsync<VehicleCompatibilityFilterOptionsDialogViewModel, VehicleCompatibilityFilterOptionsDialog, CompatibleVehicleFilterCriteriaDto>(filtersForDialog);

                if (newFilterCriteriaFromDialog is not null)
                {
                    _logger.LogInformation("New filter criteria applied from dialog.");

                    CurrentlyAppliedFilters = newFilterCriteriaFromDialog with { SearchTerm = this.SearchQueryText };


                    // The LoadCompatibleVehiclesAsync will be triggered by OnSearchQueryTextChanged's debouncer
                    // if SearchQueryText changed, OR we need to trigger it manually if only other filters changed.
                    // To be safe and ensure reload when dialog applies ANY change:
                    _searchDebounceTimer?.Dispose(); // Cancel any pending search from direct typing
                    await LoadCompatibleVehiclesAsync(CurrentlyAppliedFilters); // Load with the fully merged filters

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

        [RelayCommand]
        public async Task RemoveFilterAsync(ActiveFilterDisplayItem filterToRemove)
        {
            if (filterToRemove == null || CurrentlyAppliedFilters == null) return;

            _logger.LogInformation("Removing filter: Type={FilterType}, Value={FilterValue}", filterToRemove.FilterType, filterToRemove.FilterValueDisplay);

            bool refreshNeeded = false;
            var updatedFilters = CurrentlyAppliedFilters; // Work with a copy

            switch (filterToRemove.CriterionKey)
            {
                case nameof(CompatibleVehicleFilterCriteriaDto.MakeId):
                    updatedFilters = updatedFilters with { MakeId = null, ModelId = null, TrimLevelId = null };
                    refreshNeeded = true;
                    break;
                case nameof(CompatibleVehicleFilterCriteriaDto.ModelId):
                    updatedFilters = updatedFilters with { ModelId = null, TrimLevelId = null };
                    refreshNeeded = true;
                    break;
                case nameof(CompatibleVehicleFilterCriteriaDto.TrimLevelId):
                    updatedFilters = updatedFilters with { TrimLevelId = null };
                    refreshNeeded = true;
                    break;
                case nameof(CompatibleVehicleFilterCriteriaDto.ExactYear):
                    updatedFilters = updatedFilters with { ExactYear = null };
                    refreshNeeded = true;
                    break;
                case nameof(CompatibleVehicleFilterCriteriaDto.TransmissionTypeId):
                    updatedFilters = updatedFilters with { TransmissionTypeId = null };
                    refreshNeeded = true;
                    break;
                case nameof(CompatibleVehicleFilterCriteriaDto.EngineTypeId):
                    updatedFilters = updatedFilters with { EngineTypeId = null };
                    refreshNeeded = true;
                    break;
                case nameof(CompatibleVehicleFilterCriteriaDto.BodyTypeId):
                    updatedFilters = updatedFilters with { BodyTypeId = null };
                    refreshNeeded = true;
                    break;
                case nameof(CompatibleVehicleFilterCriteriaDto.SearchTerm):
                    // If removing the search term chip, also clear the main search box
                    SearchQueryText = null; // This will trigger OnSearchQueryTextChanged, which updates CurrentlyAppliedFilters.SearchTerm and debounces
                                            // The refresh will be handled by the debouncer from OnSearchQueryTextChanged.
                                            // We might not need `refreshNeeded = true` here if we let the debounce handle it.
                                            // However, to ensure the ActiveFiltersDisplayCollection updates immediately:
                    ActiveFiltersDisplayCollection.Remove(filterToRemove);
                    OnPropertyChanged(nameof(IsFiltersActive));
                    return; // Exit, let OnSearchQueryTextChanged's debounce handle the reload
            }

            if (refreshNeeded)
            {
                CurrentlyAppliedFilters = updatedFilters; // Update the main filter object
                                                          // ActiveFiltersDisplayCollection will be updated by OnCurrentlyAppliedFiltersChanged
                _searchDebounceTimer?.Dispose(); // Cancel any pending live search
                await LoadCompatibleVehiclesAsync(CurrentlyAppliedFilters);
            }
        }


        private bool CanClearSearch()
        {
            return !string.IsNullOrEmpty(SearchQueryText) || (CurrentlyAppliedFilters != null && !string.IsNullOrEmpty(CurrentlyAppliedFilters.SearchTerm));
        }

        [RelayCommand(CanExecute = nameof(CanClearSearch))]
        private async Task ClearSearchAndReloadAsync()
        {
            _searchDebounceTimer?.Dispose(); // Cancel any pending search from typing

            // Setting SearchQueryText to empty will trigger OnSearchQueryTextChanged.
            // OnSearchQueryTextChanged will update CurrentlyAppliedFilters.SearchTerm
            // and start a new debounced search with the empty term.
            SearchQueryText = string.Empty;

            // If you want absolutely immediate reload on clear, bypassing debounce:
            // IsLoading = true;
            // await LoadCompatibleVehiclesAsync(CurrentlyAppliedFilters); // CurrentlyAppliedFilters already updated by OnSearchQueryTextChanged
            // IsLoading = false;
        }


        #endregion

        #region Property Changed Customization

        partial void OnCurrentlyAppliedFiltersChanged(CompatibleVehicleFilterCriteriaDto value)
        {
            _ = UpdateActiveFiltersDisplayAsync(); // Load names asynchronously
                                                   // Also update IsFiltersActive
            OnPropertyChanged(nameof(IsFiltersActive));
        }

        partial void OnSearchQueryTextChanged(string? value) // MVVM Toolkit generates oldValue, newValue parameters if needed
        {
            // Update the SearchTerm in the applied filters immediately
            // This ensures that if another filter action happens before debounce triggers,
            // the latest search term is included.
            CurrentlyAppliedFilters = (CurrentlyAppliedFilters ?? new CompatibleVehicleFilterCriteriaDto())
                                          with
            { SearchTerm = string.IsNullOrWhiteSpace(value) ? null : value }; 
            
            ClearSearchAndReloadCommand.NotifyCanExecuteChanged();

            // Dispose existing timer if any
            _searchDebounceTimer?.Dispose();

            // Create and start a new timer
            // The timer will call PerformSearchAsync after the delay
            _searchDebounceTimer = new Timer(async (_) => await PerformDebouncedSearchAsync(), null, SearchDebounceDelayMs, Timeout.Infinite);
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


        /// <summary>
        /// Delays Search for about <see cref="SearchDebounceDelayMs"/>
        /// </summary>
        /// <returns></returns>
        private async Task PerformDebouncedSearchAsync()
        {
            _logger.LogInformation("Debounced search triggered with query: {SearchQuery}", CurrentlyAppliedFilters?.SearchTerm);
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                IsLoading = true; // Set on UI thread
                try
                {
                    // LoadCompatibleVehiclesAsync already uses CurrentlyAppliedFilters
                    // and will modify CompatibleVehiclesCollection
                    await LoadCompatibleVehiclesAsync(CurrentlyAppliedFilters);
                }
                catch (ViewModelException vmEx) // Catch specific exceptions if LoadCompatibleVehiclesAsync throws them
                {
                    // Logged within LoadCompatibleVehiclesAsync, toast shown there.
                    // Decide if further action is needed here. Usually not if already handled.
                    _logger.LogWarning(vmEx, "ViewModelException during debounced search execution.");
                }
                catch (Exception ex) // Catch any other unexpected exceptions
                {
                    _logger.LogError(ex, "Unexpected error during debounced search execution.");
                    // Show a generic error toast if LoadCompatibleVehiclesAsync didn't already.
                    _wpfToastNotificationService.ShowError("An error occurred during search.", "Search Error");
                }
                finally
                {
                    IsLoading = false; // Set on UI thread
                }
            });
        }

        #endregion
    }
}
