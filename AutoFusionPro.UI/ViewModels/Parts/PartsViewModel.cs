using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.Navigation;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.Navigation;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Helpers.Filtration;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEdit;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs.Filters;
using AutoFusionPro.UI.Views.Parts.Dialogs.AddEdit;
using AutoFusionPro.UI.Views.Parts.Dialogs.Filters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Parts
{
    public partial class PartsViewModel : BaseViewModel<PartsViewModel>
    {
        #region Fields

        private readonly IPartService _partService;
        private readonly IDialogService _dialogService;
        private readonly IWpfToastNotificationService _wpfToastNotificationService;
        private readonly INavigationService _navigationService;
        private readonly ICategoryService _categoryService;
        //private readonly ISupplierService _supplierService; // You will need this for the filter dialog
        private readonly IVehicleTaxonomyService _vehicleTaxonomyService; // For vehicle filters
        private readonly IPartCompatibilityRuleService _partCompatibilityRuleService; // For advanced filtering


        private Timer? _searchDebounceTimer;
        private const int SearchDebounceDelayMs = 500;

        #endregion

        #region Props

        [ObservableProperty]
        private ObservableCollection<PartSummaryDto> _parts = new();

        [ObservableProperty]
        private PartSummaryDto? _selectedPart;

        [ObservableProperty]
        private bool _isLoading = false;                
        
        [ObservableProperty]
        private bool _isShowingFilters = false;        

        [ObservableProperty]
        private bool _isRemovingPart = false;        
        
        [ObservableProperty]
        private bool _isAddingPart = false;

        [ObservableProperty]
        private bool _isEditingPart = false;

        #endregion

        // --- Filter & Search Properties ---
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTextSearchQuery))]
        [NotifyPropertyChangedFor(nameof(IsFiltersActive))]
        private string? _searchQueryText;
        public bool HasTextSearchQuery => !string.IsNullOrWhiteSpace(SearchQueryText);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFiltersActive))]
        private PartFilterCriteriaDto _currentlyAppliedFilters = new(); // Initialize with defaults (e.g., IsActive = true)

        [ObservableProperty]
        private ObservableCollection<ActiveFilterDisplayItem> _activeFiltersDisplayCollection = new();
        public bool IsFiltersActive => ActiveFiltersDisplayCollection.Any(); // Simpler logic based on the display collection

        // --- Pagination Properties ---
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))] // Notify dependent command
        private int _currentPage = 1;

        [ObservableProperty] private int _pageSize = 25; // User can change this
        [ObservableProperty] private int _totalItems;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))] 
        private int _totalPages;

        // CanExecute logic for the commands
        public bool CanGoToPreviousPage => CurrentPage > 1 && !IsLoading;
        public bool CanGoToNextPage => CurrentPage < TotalPages && !IsLoading;



        public PartsViewModel(IPartService partService, 
            ILocalizationService localizationService, 
            IDialogService dialogService,
            IWpfToastNotificationService wpfToastNotificationService,
            INavigationService navigationService,
            ICategoryService categoryService,
            IPartCompatibilityRuleService partCompatibilityRuleService,
            IVehicleTaxonomyService vehicleTaxonomyService,
            ILogger<PartsViewModel> logger) : base(localizationService, logger)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _partCompatibilityRuleService = partCompatibilityRuleService ?? throw new ArgumentNullException(nameof(partCompatibilityRuleService));
            _vehicleTaxonomyService = vehicleTaxonomyService ?? throw new ArgumentNullException(nameof(vehicleTaxonomyService));

            _ = LoadPartsDataAsync();
        }

        #region Data Loading (and its interaction with pagination)

        /// <summary>
        /// The single source for loading paged and filtered parts data.
        /// Uses the current state of ViewModel properties like CurrentPage, PageSize, and CurrentlyAppliedFilters.
        /// </summary>
        private async Task LoadPartsDataAsync()
        {
            // Set IsLoading and IsEnabled on commands at the start
            IsLoading = true;
            GoToNextPageCommand.NotifyCanExecuteChanged();
            GoToPreviousPageCommand.NotifyCanExecuteChanged();

            _logger.LogInformation("Loading Parts. Page: {Page}, Size: {Size}, Filters: {@Filters}",
                CurrentPage, PageSize, CurrentlyAppliedFilters);
            try
            {
                // Ensure the search term from the UI is included in the criteria for the service call
                var filters = CurrentlyAppliedFilters with { SearchTerm = SearchQueryText };

                var pagedResult = await _partService.GetFilteredPartSummariesAsync(
                    filters,
                    CurrentPage,
                    PageSize
                );

                Parts.Clear(); // Clear existing items before adding new page's items
                if (pagedResult?.Items != null)
                {
                    foreach (var part in pagedResult.Items)
                    {
                        Parts.Add(part);
                    }
                    TotalItems = pagedResult.TotalCount;
                    // Calculate TotalPages based on TotalItems and PageSize
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    if (TotalPages == 0) TotalPages = 1; // Always have at least one page, even if empty
                }
                else // Handle null result from service
                {
                    TotalItems = 0;
                    TotalPages = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to load Parts data, {ex.Message}");
                // Optionally show a toast notification for the error
                // _wpfToastNotificationService.ShowError("Could not load parts. Please try again.", "Error");
                TotalItems = 0; // Reset counts on failure
                TotalPages = 1;
                Parts.Clear();
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(LoadPartsDataAsync), MethodOperationType.LOAD_DATA, ex);
                
            
            }
            finally
            {
                IsLoading = false;
                GoToNextPageCommand.NotifyCanExecuteChanged();
                GoToPreviousPageCommand.NotifyCanExecuteChanged();
            }
        }

        #endregion

        #region Commands 

        [RelayCommand]
        public async Task ShowAddPartDialogAsync() 
        {
            IsAddingPart = true;
            try
            {
                bool? result = await _dialogService.ShowDialogAsync<AddEditPartDialogViewModel, AddEditPartDialog>(null);
               
                if (result.HasValue && result.Value == true)
                {
                    RefreshData();
                    _logger.LogInformation("AddEditPartDialog was called and returned results.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while opening AddPartDialog, {ex.Message}");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(ShowAddPartDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }finally
            {
                IsAddingPart = false;
            }

        }

        [RelayCommand]
        public async Task DeleteSelectedPartAsync(PartSummaryDto? partToDelete)
        {
            if (partToDelete == null)
                return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            IsRemovingPart = true;


            if (results.HasValue && results.Value == true)
            {


                try
                {
                    await _partService.DeactivatePartAsync(partToDelete.Id);
                
                    await LoadPartsDataAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemDeletedSuccessfullyStr"] as string ?? "Item Has Been Deleted Successfully!";

                    _wpfToastNotificationService.ShowSuccess(msg);

                    _logger.LogInformation("Part with ID={ID} has been deleted successfully!", partToDelete.Id);

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


                    _wpfToastNotificationService.Show($"{msg}\n{dependentItemsText}", msgTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(10));
                    return;
                }
                catch (ServiceException ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting part");
                    var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                    _wpfToastNotificationService.ShowError($"{msg}, {ex.Message}");

                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                    var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                    _wpfToastNotificationService.ShowError(msg);

                    // FOR DEV ONLY
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(DeleteSelectedPartAsync), MethodOperationType.DELETE_DATA, ex);
                }
                finally
                {
                    IsRemovingPart = false;
                }
            }

        }

        [RelayCommand]
        public async Task ShowEditPartDialogAsync(PartSummaryDto? partToEdit)
        {
            if(partToEdit is null)
            {
                _logger.LogError("The Part Details was null");
                return;
            }

            IsEditingPart = true;
            try
            {

                var partDetails = await _partService.GetPartDetailsByIdAsync(partToEdit.Id);

                bool? result = await _dialogService.ShowDialogAsync<AddEditPartDialogViewModel, AddEditPartDialog>(partDetails);

                if (result.HasValue && result.Value == true)
                {
                    RefreshData();
                    _logger.LogInformation("AddEditPartDialog was called and returned results.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while opening AddPartDialog, {ex.Message}");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(ShowAddPartDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsEditingPart = false;
            }

        }

        [RelayCommand]
        public async Task OpenPartDetailsViewAsync(PartSummaryDto? partToOpen)
        {
            if (partToOpen == null) return;
            try
            {
                _navigationService.NavigateTo(Core.Enums.NavigationPages.ApplicationPage.PartDetails, partToOpen);

            }
            catch (NavigationException nex)
            {
                var msgTitle = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Part {partToOpen.Name} Has Failed";
                var msg = System.Windows.Application.Current.Resources["ReasonIsStr"] as string ?? $"Reason Is";
                _wpfToastNotificationService.ShowError($"{msg} {nex.Message}", msgTitle);
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(nex.Message, true, CurrentWorkFlow);

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while opening Part Details, {Message}", ex.Message);
                var msg = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Part {partToOpen.Name} Has Failed";
                _wpfToastNotificationService.ShowError(msg);
                return;
            }
        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            await LoadPartsDataAsync();
        }

        [RelayCommand]
        private async Task ShowFilterDialogAsync()
        {
            IsShowingFilters = true;
            try
            {
                var newFilters = await _dialogService.ShowDialogWithResultsAsync<PartFilterOptionsDialogViewModel, PartFilterOptionsDialog, PartFilterCriteriaDto>(CurrentlyAppliedFilters);
                if (newFilters != null)
                {
                    CurrentPage = 1; // Reset to first page
                    CurrentlyAppliedFilters = newFilters with { SearchTerm = SearchQueryText }; // Preserve search term
                    await LoadPartsDataAsync();
                }
            }
            // ... catch ...
            finally { IsShowingFilters = false; }
        }

        [RelayCommand]
        private async Task RemoveActiveFilterAsync(ActiveFilterDisplayItem filterToRemove)
        {
            if (filterToRemove == null || CurrentlyAppliedFilters == null) return;

            _logger.LogInformation("Removing filter: Key='{Key}', Value='{Value}'", filterToRemove.CriterionKey, filterToRemove.FilterValueDisplay);

            var updatedFilters = CurrentlyAppliedFilters; // Start with a copy
            bool needsReload = true;

            switch (filterToRemove.CriterionKey)
            {
                case nameof(PartFilterCriteriaDto.SearchTerm):
                    updatedFilters = updatedFilters with { SearchTerm = null };
                    SearchQueryText = null; // Clear the UI search box
                    break;
                case nameof(PartFilterCriteriaDto.CategoryId):
                    updatedFilters = updatedFilters with { CategoryId = null };
                    break;

                // ... (cases for Manufacturer, SupplierId, IsActive, IsOriginal, StockStatus) ...

                case "PriceRange": // Handle our custom key
                    updatedFilters = updatedFilters with { MinSellingPrice = null, MaxSellingPrice = null };
                    break;

                case nameof(PartFilterCriteriaDto.MakeId):
                    // Removing Make should also remove dependent Model filter
                    updatedFilters = updatedFilters with { MakeId = null, ModelId = null };
                    break;
                case nameof(PartFilterCriteriaDto.ModelId):
                    updatedFilters = updatedFilters with { ModelId = null };
                    break;
                case nameof(PartFilterCriteriaDto.SpecificYear):
                    updatedFilters = updatedFilters with { SpecificYear = null };
                    break;

                default:
                    _logger.LogWarning("Attempted to remove an unknown filter key: {Key}", filterToRemove.CriterionKey);
                    needsReload = false; // Don't reload if we don't know what to do
                    break;
            }

            if (needsReload)
            {
                CurrentPage = 1; // Reset to first page
                CurrentlyAppliedFilters = updatedFilters; // This will trigger OnPropertyChanged
                                                          // The OnCurrentlyAppliedFiltersChanged partial method should call UpdateActiveFiltersDisplayAsync
                await LoadPartsDataAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(HasTextSearchQuery))]
        private void ClearSearchQuery() // Renamed
        {
            _searchDebounceTimer?.Dispose();
            SearchQueryText = string.Empty; // This triggers OnChanged and debounced client search
        }

        #endregion

        #region Commands (Implementation of Pagination)

        /// <summary>
        /// Navigates to the previous page of parts.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
        private async Task GoToPreviousPageAsync()
        {
            _logger.LogInformation("Navigating to previous page. Current Page: {CurrentPage}", CurrentPage);

            // Decrement the page number
            CurrentPage--;

            // Reload data for the new page
            await LoadPartsDataAsync();
            _logger.LogInformation("Now on page {CurrentPage} of {TotalPages}", CurrentPage, TotalPages);
        }

        /// <summary>
        /// Navigates to the next page of parts.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
        private async Task GoToNextPageAsync()
        {
            _logger.LogInformation("Navigating to next page. Current Page: {CurrentPage}", CurrentPage);

            // Increment the page number
            CurrentPage++;

            // Reload data for the new page
            await LoadPartsDataAsync();
            _logger.LogInformation("Now on page {CurrentPage} of {TotalPages}", CurrentPage, TotalPages);
        }

        #endregion


        #region Property Customization

        partial void OnSearchQueryTextChanged(string? value)
        {
            // Update the SearchTerm in the applied filters immediately
            CurrentlyAppliedFilters = CurrentlyAppliedFilters with { SearchTerm = string.IsNullOrWhiteSpace(value) ? null : value };
            _searchDebounceTimer?.Dispose();
            _searchDebounceTimer = new Timer(async _ => await OnDebouncedSearch(), null, SearchDebounceDelayMs, Timeout.Infinite);
        }

        // Ensure OnCurrentlyAppliedFiltersChanged calls the update method
        partial void OnCurrentlyAppliedFiltersChanged(PartFilterCriteriaDto? oldValue, PartFilterCriteriaDto newValue)
        {
            _ = UpdateActiveFiltersDisplayAsync(); // Update the pills
            OnPropertyChanged(nameof(IsFiltersActive));
        }

        // OnCurrentPageChanged partial method to also update command state
        partial void OnCurrentPageChanged(int value)
        {
            GoToNextPageCommand.NotifyCanExecuteChanged();
            GoToPreviousPageCommand.NotifyCanExecuteChanged();
        }

        // OnTotalPagesChanged partial method to also update command state
        partial void OnTotalPagesChanged(int value)
        {
            GoToNextPageCommand.NotifyCanExecuteChanged();
            GoToPreviousPageCommand.NotifyCanExecuteChanged();
        }

        #endregion



        private async Task OnDebouncedSearch()
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                CurrentPage = 1; // Reset to first page for a new search
                await LoadPartsDataAsync();
                // Update active filters display if search term is shown as a pill
                await UpdateActiveFiltersDisplayAsync();
            });
        }

        /// <summary>
        /// Updates the collection of active filter "pills" based on the state of CurrentlyAppliedFilters.
        /// This method fetches display names for IDs from services.
        /// </summary>
        private async Task UpdateActiveFiltersDisplayAsync()
        {
            // For performance and to prevent flicker, build a new list and then update the collection.
            var newActiveFilters = new List<ActiveFilterDisplayItem>();
            if (CurrentlyAppliedFilters == null)
            {
                // If the criteria object is null, clear the display and return.
                if (ActiveFiltersDisplayCollection.Any())
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => ActiveFiltersDisplayCollection.Clear());
                }
                return;
            }

            // --- Generic Search Term ---
            if (!string.IsNullOrWhiteSpace(CurrentlyAppliedFilters.SearchTerm))
            {
                var searchStr = GetString("SearchStr") ?? "Search";
                newActiveFilters.Add(new ActiveFilterDisplayItem(searchStr, CurrentlyAppliedFilters.SearchTerm, nameof(PartFilterCriteriaDto.SearchTerm), CurrentlyAppliedFilters.SearchTerm));
            }

            // --- Category Filter ---
            if (CurrentlyAppliedFilters.CategoryId.HasValue && CurrentlyAppliedFilters.CategoryId.Value > 0)
            {
                var categoryStr = GetString("CategoryStr") ?? "Category";
                // Fetch the category name for display
                var category = await _categoryService.GetCategoryByIdAsync(CurrentlyAppliedFilters.CategoryId.Value);
                newActiveFilters.Add(new ActiveFilterDisplayItem(categoryStr, category?.Name ?? $"ID: {CurrentlyAppliedFilters.CategoryId.Value}", nameof(PartFilterCriteriaDto.CategoryId), CurrentlyAppliedFilters.CategoryId.Value));
            }

            // --- Manufacturer Filter ---
            if (!string.IsNullOrWhiteSpace(CurrentlyAppliedFilters.Manufacturer))
            {
                var manufacturerStr = GetString("PartManufacturerStr") ?? "Manufacturer";
                newActiveFilters.Add(new ActiveFilterDisplayItem(manufacturerStr, CurrentlyAppliedFilters.Manufacturer, nameof(PartFilterCriteriaDto.Manufacturer), CurrentlyAppliedFilters.Manufacturer));
            }

            // --- Supplier Filter ---
            //if (CurrentlyAppliedFilters.SupplierId.HasValue && CurrentlyAppliedFilters.SupplierId.Value > 0)
            //{
            //    var supplierStr = GetString("SupplierStr") ?? "Supplier";
            //    // Assuming ISupplierService exists and has GetSupplierByIdAsync
            //    var supplier = await _supplierService.GetSupplierByIdAsync(CurrentlyAppliedFilters.SupplierId.Value);
            //    newActiveFilters.Add(new ActiveFilterDisplayItem(supplierStr, supplier?.Name ?? $"ID: {CurrentlyAppliedFilters.SupplierId.Value}", nameof(PartFilterCriteriaDto.SupplierId), CurrentlyAppliedFilters.SupplierId.Value));
            //}

            // --- Active Status Filter ---
            if (CurrentlyAppliedFilters.IsActive.HasValue)
            {
                var statusStr = GetString("StatusStr") ?? "Status";
                var activeStr = GetString("ActiveStr") ?? "Active";
                var inactiveStr = GetString("InactiveStr") ?? "Inactive";
                newActiveFilters.Add(new ActiveFilterDisplayItem(statusStr, CurrentlyAppliedFilters.IsActive.Value ? activeStr : inactiveStr, nameof(PartFilterCriteriaDto.IsActive), CurrentlyAppliedFilters.IsActive.Value));
            }

            // --- Original Status Filter ---
            if (CurrentlyAppliedFilters.IsOriginal.HasValue)
            {
                var typeStr = GetString("TypeStr") ?? "Type";
                var originalStr = GetString("OriginalPartStr") ?? "Original";
                var aftermarketStr = GetString("AftermarketPartStr") ?? "Aftermarket";
                newActiveFilters.Add(new ActiveFilterDisplayItem(typeStr, CurrentlyAppliedFilters.IsOriginal.Value ? originalStr : aftermarketStr, nameof(PartFilterCriteriaDto.IsOriginal), CurrentlyAppliedFilters.IsOriginal.Value));
            }

            // --- Stock Status Filter ---
            if (CurrentlyAppliedFilters.StockStatus.HasValue)
            {
                var stockStatusStr = GetString("StockStatusStr") ?? "Stock Status";
                newActiveFilters.Add(new ActiveFilterDisplayItem(stockStatusStr, CurrentlyAppliedFilters.StockStatus.Value.ToString(), nameof(PartFilterCriteriaDto.StockStatus), CurrentlyAppliedFilters.StockStatus.Value));
            }

            // --- Selling Price Filter ---
            if (CurrentlyAppliedFilters.MinSellingPrice.HasValue || CurrentlyAppliedFilters.MaxSellingPrice.HasValue)
            {
                var priceStr = GetString("SellingPriceStr") ?? "Price";
                string priceDisplay;
                if (CurrentlyAppliedFilters.MinSellingPrice.HasValue && CurrentlyAppliedFilters.MaxSellingPrice.HasValue)
                {
                    priceDisplay = $"{CurrentlyAppliedFilters.MinSellingPrice.Value:C} - {CurrentlyAppliedFilters.MaxSellingPrice.Value:C}";
                }
                else if (CurrentlyAppliedFilters.MinSellingPrice.HasValue)
                {
                    priceDisplay = $"> {CurrentlyAppliedFilters.MinSellingPrice.Value:C}";
                }
                else
                {
                    var max = CurrentlyAppliedFilters.MaxSellingPrice.HasValue ? CurrentlyAppliedFilters.MaxSellingPrice.Value : 0;
                    priceDisplay = $"< {max:C}";
                }
                newActiveFilters.Add(new ActiveFilterDisplayItem(priceStr, priceDisplay, "PriceRange", null)); // Use a custom key for complex filters like ranges
            }

            // --- Vehicle Compatibility Filters ---
            if (CurrentlyAppliedFilters.MakeId.HasValue && CurrentlyAppliedFilters.MakeId.Value > 0)
            {
                var makeStr = GetString("MakeStr") ?? "Make";
                var make = await _vehicleTaxonomyService.GetMakeByIdAsync(CurrentlyAppliedFilters.MakeId.Value);
                newActiveFilters.Add(new ActiveFilterDisplayItem(makeStr, make?.Name ?? $"ID: {CurrentlyAppliedFilters.MakeId.Value}", nameof(PartFilterCriteriaDto.MakeId), CurrentlyAppliedFilters.MakeId.Value));
            }
            if (CurrentlyAppliedFilters.ModelId.HasValue && CurrentlyAppliedFilters.ModelId.Value > 0)
            {
                var modelStr = GetString("ModelStr") ?? "Model";
                var model = await _vehicleTaxonomyService.GetModelByIdAsync(CurrentlyAppliedFilters.ModelId.Value);
                newActiveFilters.Add(new ActiveFilterDisplayItem(modelStr, model?.Name ?? $"ID: {CurrentlyAppliedFilters.ModelId.Value}", nameof(PartFilterCriteriaDto.ModelId), CurrentlyAppliedFilters.ModelId.Value));
            }
            if (CurrentlyAppliedFilters.SpecificYear.HasValue)
            {
                var yearStr = GetString("YearStr") ?? "Year";
                newActiveFilters.Add(new ActiveFilterDisplayItem(yearStr, CurrentlyAppliedFilters.SpecificYear.Value.ToString(), nameof(PartFilterCriteriaDto.SpecificYear), CurrentlyAppliedFilters.SpecificYear.Value));
            }
            // ... (Add similar logic for TrimId, EngineId, etc. if you add them to the filter) ...


            // Update the UI collection on the UI thread
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ActiveFiltersDisplayCollection.Clear();
                foreach (var filter in newActiveFilters)
                {
                    ActiveFiltersDisplayCollection.Add(filter);
                }
                OnPropertyChanged(nameof(IsFiltersActive)); // Explicitly notify that IsFiltersActive may have changed
            });

            _logger.LogDebug("Active filters display updated. Count: {Count}", ActiveFiltersDisplayCollection.Count);
        }

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

        public async void RefreshData()
        {
            await LoadPartsDataAsync();
        }

        #endregion

    }
}
