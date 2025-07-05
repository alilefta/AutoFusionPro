using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.Navigation;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Enums.DTOEnums;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoFusionPro.UI.ViewModels.Parts
{
    /// <summary>
    /// ViewModel for managing and displaying a list of car parts.
    /// Handles data loading, filtering, pagination, and user interactions like adding, editing, and deleting parts.
    /// </summary>
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



        // --- Pagination Properties ---
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
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

        public bool IsFiltersActive => CurrentlyAppliedFilters != null &&
                                       (!string.IsNullOrWhiteSpace(CurrentlyAppliedFilters.SearchTerm) ||
                                        CurrentlyAppliedFilters.MakeId.HasValue ||
                                        CurrentlyAppliedFilters.ModelId.HasValue ||
                                        CurrentlyAppliedFilters.IsOriginal.HasValue ||
                                        CurrentlyAppliedFilters.IsActive.HasValue ||
                                        CurrentlyAppliedFilters.TrimId != null ||
                                        CurrentlyAppliedFilters.TransmissionId != null ||
                                        CurrentlyAppliedFilters.EngineId != null ||
                                        CurrentlyAppliedFilters.BodyTypeId != null ||
                                        CurrentlyAppliedFilters.SpecificYear != null ||
                                        !string.IsNullOrWhiteSpace(CurrentlyAppliedFilters.Manufacturer) ||
                                        CurrentlyAppliedFilters.MaxSellingPrice.HasValue ||
                                        CurrentlyAppliedFilters.MinSellingPrice.HasValue ||
                                        CurrentlyAppliedFilters.CategoryId.HasValue ||
                                        CurrentlyAppliedFilters.SupplierId.HasValue ||
                                        CurrentlyAppliedFilters.SortBy.HasValue || CurrentlyAppliedFilters.StockStatus.HasValue);


        /// <summary>
        /// Initializes a new instance of the <see cref="PartsViewModel"/> class.
        /// </summary>
        /// <param name="partService">Service for part-related data operations.</param>
        /// <param name="localizationService">Service for localization.</param>
        /// <param name="dialogService">Service for displaying dialogs.</param>
        /// <param name="wpfToastNotificationService">Service for showing toast notifications.</param>
        /// <param name="navigationService">Service for navigating between views.</param>
        /// <param name="categoryService">Service for category-related data operations.</param>
        /// <param name="partCompatibilityRuleService">Service for part compatibility rule operations.</param>
        /// <param name="vehicleTaxonomyService">Service for vehicle taxonomy data operations.</param>
        /// <param name="logger">Logger for logging view model activities.</param>
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
        /// Asynchronously loads a paginated and filtered list of part summaries from the data source.
        /// This is the primary method for fetching data and updates the Parts collection and pagination properties.
        /// </summary>
        private async Task LoadPartsDataAsync()
        {
            IsLoading = true;
            GoToNextPageCommand.NotifyCanExecuteChanged();
            GoToPreviousPageCommand.NotifyCanExecuteChanged();

            _logger.LogInformation("Loading Parts. Page: {Page}, Size: {Size}, Filters: {@Filters}",
                CurrentPage, PageSize, CurrentlyAppliedFilters);
            try
            {
                var filters = CurrentlyAppliedFilters with { SearchTerm = SearchQueryText };

                var pagedResult = await _partService.GetFilteredPartSummariesAsync(
                    filters,
                    CurrentPage,
                    PageSize
                );

                Parts.Clear();
                if (pagedResult?.Items != null)
                {
                    foreach (var part in pagedResult.Items)
                    {
                        Parts.Add(part);
                    }
                    TotalItems = pagedResult.TotalCount;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    if (TotalPages == 0) TotalPages = 1;
                }
                else
                {
                    TotalItems = 0;
                    TotalPages = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to load Parts data.");
                TotalItems = 0;
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

        /// <summary>
        /// Displays a dialog for adding a new part. If the operation is successful, the parts list is refreshed.
        /// </summary>
        [RelayCommand]
        public async Task ShowAddPartDialogAsync()
        {
            IsAddingPart = true;
            try
            {
                bool? result = await _dialogService.ShowDialogAsync<AddEditPartDialogViewModel, AddEditPartDialog>(null);

                if (result == true)
                {
                    await RefreshDataAsync();
                    _logger.LogInformation("AddEditPartDialog was called and returned results.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception happened while opening AddPartDialog.");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(ShowAddPartDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAddingPart = false;
            }
        }

        /// <summary>
        /// Deactivates the selected part after receiving confirmation from the user.
        /// </summary>
        /// <param name="partToDelete">The part to be deactivated.</param>
        [RelayCommand]
        public async Task DeleteSelectedPartAsync(PartSummaryDto? partToDelete)
        {
            if (partToDelete == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);
            if (results != true) return;

            IsRemovingPart = true;
            try
            {
                await _partService.DeactivatePartAsync(partToDelete.Id);
                await LoadPartsDataAsync();
                var msg = GetString("ItemDeletedSuccessfullyStr") ?? "Item Has Been Deleted Successfully!";
                _wpfToastNotificationService.ShowSuccess(msg);
                _logger.LogInformation("Part with ID={ID} has been deactivated successfully!", partToDelete.Id);
            }
            catch (DeletionBlockedException dx)
            {
                _logger.LogError(dx, "{ErrorMessage}. Entity: {EntityName}, Key: {EntityKey}, Dependents: {Dependents}",
                    ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, dx.EntityName, dx.EntityKey, string.Join(", ", dx.DependentEntityTypes ?? Enumerable.Empty<string>()));

                string entityDisplayName = dx.EntityName ?? "the item";
                string dependentItemsText = "other items";
                if (dx.DependentEntityTypes != null && dx.DependentEntityTypes.Any())
                {
                    dependentItemsText = string.Join(" and ", dx.DependentEntityTypes.Select(FormatDependentTypeName));
                }

                var msgTitle = GetString("DeleteOperationFailedStr") ?? "Cannot Delete the Item!";
                var msg = GetString("BecauseTheFollowingItemsDependOnItStr") ?? $"Because the following Items Depends on it:";
                _wpfToastNotificationService.Show($"{msg} {dependentItemsText}", msgTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(10));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "A service error occurred while deactivating part with ID={PartId}.", partToDelete.Id);
                var msg = GetString("DeleteOperationFailedStr") ?? "Cannot Delete the Item!";
                _wpfToastNotificationService.ShowError($"{msg}, {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deactivating part with ID={PartId}.", partToDelete.Id);
                var msg = GetString("DeleteOperationFailedStr") ?? "Cannot Delete the Item!";
                _wpfToastNotificationService.ShowError(msg);
                throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(DeleteSelectedPartAsync), MethodOperationType.DELETE_DATA, ex);
            }
            finally
            {
                IsRemovingPart = false;
            }
        }

        /// <summary>
        /// Displays a dialog for editing the selected part. If the operation is successful, the parts list is refreshed.
        /// </summary>
        /// <param name="partToEdit">The part to be edited.</param>
        [RelayCommand]
        public async Task ShowEditPartDialogAsync(PartSummaryDto? partToEdit)
        {
            if (partToEdit is null)
            {
                _logger.LogError("The Part to edit was null.");
                return;
            }

            IsEditingPart = true;
            try
            {
                var partDetails = await _partService.GetPartDetailsByIdAsync(partToEdit.Id);
                bool? result = await _dialogService.ShowDialogAsync<AddEditPartDialogViewModel, AddEditPartDialog>(partDetails);

                if (result == true)
                {
                    await RefreshDataAsync();
                    _logger.LogInformation("AddEditPartDialog was called and returned results for editing.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception happened while opening AddEditPartDialog for editing.");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(ShowEditPartDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsEditingPart = false;
            }
        }

        /// <summary>
        /// Navigates to the detailed view for the selected part.
        /// </summary>
        /// <param name="partToOpen">The part whose details should be displayed.</param>
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
                var msgTitle = GetString("NavigationFailedStr") ?? $"Navigation to Part {partToOpen.Name} Has Failed";
                var msg = GetString("ReasonIsStr") ?? $"Reason Is";
                _wpfToastNotificationService.ShowError($"{msg} {nex.Message}", msgTitle);
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(nex.Message, true, CurrentWorkFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while opening Part Details for part ID {PartId}.", partToOpen.Id);
                var msg = GetString("NavigationFailedStr") ?? $"Navigation to Part {partToOpen.Name} Has Failed";
                _wpfToastNotificationService.ShowError(msg);
            }
        }

        /// <summary>
        /// Resets the current page to 1 and reloads the parts data from the source.
        /// </summary>
        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            CurrentPage = 1;
            await LoadPartsDataAsync();
        }

        /// <summary>
        /// Opens the advanced filter dialog to allow the user to specify filter criteria.
        /// </summary>
        [RelayCommand]
        private async Task ShowFilterDialogAsync()
        {
            IsShowingFilters = true;
            try
            {
                var newFilters = await _dialogService.ShowDialogWithResultsAsync<PartFilterOptionsDialogViewModel, PartFilterOptionsDialog, PartFilterCriteriaDto>(CurrentlyAppliedFilters);
                if (newFilters != null)
                {
                    CurrentPage = 1;
                    CurrentlyAppliedFilters = newFilters with { SearchTerm = SearchQueryText };
                    await LoadPartsDataAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while showing the filter dialog.");
            }
            finally
            {
                IsShowingFilters = false;
            }
        }

        /// <summary>
        /// Removes a specific filter from the active filters and reloads the data.
        /// </summary>
        /// <param name="filterToRemove">The filter display item to remove.</param>
        [RelayCommand]
        private async Task RemoveActiveFilterAsync(ActiveFilterDisplayItem filterToRemove)
        {
            if (filterToRemove == null || CurrentlyAppliedFilters == null) return;

            _logger.LogInformation("Removing filter: Key='{Key}', Value='{Value}'", filterToRemove.CriterionKey, filterToRemove.FilterValueDisplay);

            var updatedFilters = CurrentlyAppliedFilters;
            bool needsReload = true;

            switch (filterToRemove.CriterionKey)
            {
                case nameof(PartFilterCriteriaDto.SearchTerm):
                    updatedFilters = updatedFilters with { SearchTerm = null };
                    SearchQueryText = null;
                    break;
                case nameof(PartFilterCriteriaDto.CategoryId):
                    updatedFilters = updatedFilters with { CategoryId = null };
                    break;
                case nameof(PartFilterCriteriaDto.Manufacturer):
                    updatedFilters = updatedFilters with { Manufacturer = null };
                    break;
                case nameof(PartFilterCriteriaDto.IsActive):
                    updatedFilters = updatedFilters with { IsActive = null };
                    break;
                case nameof(PartFilterCriteriaDto.IsOriginal):
                    updatedFilters = updatedFilters with { IsOriginal = null };
                    break;
                case nameof(PartFilterCriteriaDto.StockStatus):
                    updatedFilters = updatedFilters with { StockStatus = null };
                    break;
                case nameof(PartFilterCriteriaDto.SortBy):
                    updatedFilters = updatedFilters with { SortBy = null };
                    break;
                case nameof(PartFilterCriteriaDto.IsSortAscending):
                    updatedFilters = updatedFilters with { IsSortAscending = true }; // Revert to default
                    break;
                case "PriceRange":
                    updatedFilters = updatedFilters with { MinSellingPrice = null, MaxSellingPrice = null };
                    break;
                case nameof(PartFilterCriteriaDto.MakeId):
                    updatedFilters = updatedFilters with { MakeId = null, ModelId = null, TrimId = null, TransmissionId = null, EngineId = null, BodyTypeId = null, SpecificYear = null };
                    break;
                case nameof(PartFilterCriteriaDto.ModelId):
                    updatedFilters = updatedFilters with { ModelId = null };
                    break;
                case nameof(PartFilterCriteriaDto.SpecificYear):
                    updatedFilters = updatedFilters with { SpecificYear = null };
                    break;
                case nameof(PartFilterCriteriaDto.TrimId):
                    updatedFilters = updatedFilters with { TrimId = null };
                    break;
                case nameof(PartFilterCriteriaDto.TransmissionId):
                    updatedFilters = updatedFilters with { TransmissionId = null };
                    break;
                case nameof(PartFilterCriteriaDto.EngineId):
                    updatedFilters = updatedFilters with { EngineId = null };
                    break;
                case nameof(PartFilterCriteriaDto.BodyTypeId):
                    updatedFilters = updatedFilters with { BodyTypeId = null };
                    break;
                default:
                    _logger.LogWarning("Attempted to remove an unknown filter key: {Key}", filterToRemove.CriterionKey);
                    needsReload = false;
                    break;
            }

            if (needsReload)
            {
                CurrentPage = 1;
                CurrentlyAppliedFilters = updatedFilters;
                await LoadPartsDataAsync();
            }
        }

        /// <summary>
        /// Clears the text search query and reloads the data.
        /// </summary>
        [RelayCommand(CanExecute = nameof(HasTextSearchQuery))]
        private void ClearSearchQuery()
        {
            _searchDebounceTimer?.Dispose();
            SearchQueryText = string.Empty;
        }

        #endregion

        #region Commands (Implementation of Pagination)

        /// <summary>
        /// Navigates to the previous page of parts.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
        private async Task GoToPreviousPageAsync()
        {
            CurrentPage--;
            await LoadPartsDataAsync();
        }

        /// <summary>
        /// Navigates to the next page of parts.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
        private async Task GoToNextPageAsync()
        {
            CurrentPage++;
            await LoadPartsDataAsync();
        }

        #endregion


        #region Property Customization

        /// <summary>
        /// Called when the search query text changes. Triggers a debounced search.
        /// </summary>
        /// <param name="value">The new search text.</param>
        partial void OnSearchQueryTextChanged(string? value)
        {
            CurrentlyAppliedFilters = CurrentlyAppliedFilters with { SearchTerm = string.IsNullOrWhiteSpace(value) ? null : value };
            _searchDebounceTimer?.Dispose();
            _searchDebounceTimer = new Timer(async _ => await OnDebouncedSearch(), null, SearchDebounceDelayMs, Timeout.Infinite);
        }

        /// <summary>
        /// Called when the applied filters change. Updates the active filter display.
        /// </summary>
        /// <param name="value">The new filter criteria.</param>
        partial void OnCurrentlyAppliedFiltersChanged(PartFilterCriteriaDto value)
        {
            _ = UpdateActiveFiltersDisplayAsync();
            OnPropertyChanged(nameof(IsFiltersActive));
        }

        /// <summary>
        /// Called when the current page changes. Notifies commands to re-evaluate their CanExecute status.
        /// </summary>
        /// <param name="value">The new page number.</param>
        partial void OnCurrentPageChanged(int value)
        {
            GoToNextPageCommand.NotifyCanExecuteChanged();
            GoToPreviousPageCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Called when the total number of pages changes. Notifies commands to re-evaluate their CanExecute status.
        /// </summary>
        /// <param name="value">The new total page count.</param>
        partial void OnTotalPagesChanged(int value)
        {
            GoToNextPageCommand.NotifyCanExecuteChanged();
            GoToPreviousPageCommand.NotifyCanExecuteChanged();
        }

        #endregion

        /// <summary>
        /// Executes the search after a short delay to prevent excessive API calls while typing.
        /// </summary>
        private async Task OnDebouncedSearch()
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                CurrentPage = 1;
                await LoadPartsDataAsync();
                await UpdateActiveFiltersDisplayAsync();
            });
        }

        /// <summary>
        /// Updates the collection of active filter "pills" based on the state of CurrentlyAppliedFilters.
        /// This method fetches display names for IDs from services in parallel for better performance.
        /// </summary>
        private async Task UpdateActiveFiltersDisplayAsync()
        {
            var newActiveFilters = new List<ActiveFilterDisplayItem>();
            if (CurrentlyAppliedFilters == null)
            {
                if (ActiveFiltersDisplayCollection.Any())
                {
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(ActiveFiltersDisplayCollection.Clear);
                }
                return;
            }

            var filters = CurrentlyAppliedFilters;
            Task<CategoryDto?> categoryTask = filters.CategoryId.HasValue ? _categoryService.GetCategoryByIdAsync(filters.CategoryId.Value) : Task.FromResult<CategoryDto?>(null);
            Task<MakeDto?> makeTask = filters.MakeId.HasValue ? _vehicleTaxonomyService.GetMakeByIdAsync(filters.MakeId.Value) : Task.FromResult<MakeDto?>(null);
            Task<ModelDto?> modelTask = filters.ModelId.HasValue ? _vehicleTaxonomyService.GetModelByIdAsync(filters.ModelId.Value) : Task.FromResult<ModelDto?>(null);
            Task<EngineTypeDto?> engineTypeTask = filters.EngineId.HasValue ? _vehicleTaxonomyService.GetEngineTypeByIdAsync(filters.EngineId.Value) : Task.FromResult<EngineTypeDto?>(null);
            Task<BodyTypeDto?> bodyTypeTask = filters.BodyTypeId.HasValue ? _vehicleTaxonomyService.GetBodyTypeByIdAsync(filters.BodyTypeId.Value) : Task.FromResult<BodyTypeDto?>(null);
            Task<TrimLevelDto?> trimLevelTask = filters.TrimId.HasValue ? _vehicleTaxonomyService.GetTrimLevelByIdAsync(filters.TrimId.Value) : Task.FromResult<TrimLevelDto?>(null);
            Task<TransmissionTypeDto?> transmissionTypeTask = filters.TransmissionId.HasValue ? _vehicleTaxonomyService.GetTransmissionTypeByIdAsync(filters.TransmissionId.Value) : Task.FromResult<TransmissionTypeDto?>(null);

            await Task.WhenAll(categoryTask, makeTask, modelTask);

            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("SearchStr") ?? "Search", filters.SearchTerm, nameof(filters.SearchTerm), filters.SearchTerm));

            if (filters.CategoryId.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("CategoryStr") ?? "Category", (await categoryTask)?.Name ?? $"ID: {filters.CategoryId.Value}", nameof(filters.CategoryId), filters.CategoryId.Value));

            if (!string.IsNullOrWhiteSpace(filters.Manufacturer))
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("PartManufacturerStr") ?? "Manufacturer", filters.Manufacturer, nameof(filters.Manufacturer), filters.Manufacturer));

            if (filters.IsActive.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("StatusStr") ?? "Status", filters.IsActive.Value ? GetString("ActiveStr") ?? "Active" : GetString("InactiveStr") ?? "Inactive", nameof(filters.IsActive), filters.IsActive.Value));

            if (filters.IsOriginal.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("TypeStr") ?? "Type", filters.IsOriginal.Value ? GetString("OriginalPartStr") ?? "Original" : GetString("AftermarketPartStr") ?? "Aftermarket", nameof(filters.IsOriginal), filters.IsOriginal.Value));

            if (filters.StockStatus.HasValue)
            {
                var status = filters.StockStatus.Value;
                var statusDisplay = status switch
                {
                    StockStatusFilter.All => GetString("AllStr") ?? "All",
                    StockStatusFilter.InStock => GetString("InStockStr") ?? "In Stock",
                    StockStatusFilter.OutOfStock => GetString("OutOfStockStr") ?? "Out Of Stock",
                    StockStatusFilter.LowStock => GetString("LowStockStr") ?? "Low Stock",
                    _ => status.ToString()
                };
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("StockStatusStr") ?? "Stock Status", statusDisplay, nameof(filters.StockStatus), status));
            }

            if (filters.SortBy.HasValue)
            {
                var sort = filters.SortBy.Value;
                var sortDisplay = sort switch
                {
                    PartSortBy.Name => GetString("NameStr") ?? "Name",
                    PartSortBy.PartNumber => GetString("PartNumberStr") ?? "Part Number",
                    PartSortBy.SellingPrice => GetString("SellingPriceStr") ?? "Selling Price",
                    PartSortBy.CurrentStock => GetString("CurrentStockStr") ?? "Current Stock",
                    PartSortBy.LastModified => GetString("LastModifiedStr") ?? "Last Modified",
                    _ => sort.ToString()
                };
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("SortByStr") ?? "Sort By", sortDisplay, nameof(filters.SortBy), sort));
            }

            if (filters.IsSortAscending == false)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("SortDirectionStr") ?? "Sort Direction", GetString("DescendingStr") ?? "Descending", nameof(filters.IsSortAscending), false));

            if (filters.MinSellingPrice.HasValue || filters.MaxSellingPrice.HasValue)
            {
                string priceDisplay = (filters.MinSellingPrice, filters.MaxSellingPrice) switch
                {
                    (decimal min, decimal max) => $"{min:C} - {max:C}",
                    (decimal min, null) => $"> {min:C}",
                    (null, decimal max) => $"< {max:C}",
                    _ => ""
                };
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("SellingPriceStr") ?? "Price", priceDisplay, "PriceRange", null));
            }

            if (filters.MakeId.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("MakeStr") ?? "Make", (await makeTask)?.Name ?? $"ID: {filters.MakeId.Value}", nameof(filters.MakeId), filters.MakeId.Value));

            if (filters.ModelId.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("ModelStr") ?? "Model", (await modelTask)?.Name ?? $"ID: {filters.ModelId.Value}", nameof(filters.ModelId), filters.ModelId.Value));

            if (filters.SpecificYear.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("YearStr") ?? "Year", filters.SpecificYear.Value.ToString(), nameof(filters.SpecificYear), filters.SpecificYear.Value));

            if (filters.TrimId.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("TrimStr") ?? "Trim", (await trimLevelTask)?.Name ?? $"ID: {filters.TrimId.Value}", nameof(filters.TrimId), filters.TrimId.Value));

            if (filters.TransmissionId.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("TransmissionStr") ?? "Transmission", (await transmissionTypeTask)?.Name ?? $"ID: {filters.TransmissionId.Value}", nameof(filters.TransmissionId), filters.TransmissionId.Value));

            if (filters.EngineId.HasValue)
            {

                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("EngineStr") ?? "Engine", (await engineTypeTask)?.Name ?? $"ID: {filters.EngineId.Value}", nameof(filters.EngineId), filters.EngineId.Value));
            }

            if (filters.BodyTypeId.HasValue)
                newActiveFilters.Add(new ActiveFilterDisplayItem(GetString("BodyStr") ?? "Body", (await bodyTypeTask)?.Name ?? $"ID: {filters.BodyTypeId.Value}", nameof(filters.BodyTypeId), filters.BodyTypeId.Value));

            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ActiveFiltersDisplayCollection.Clear();
                foreach (var filter in newActiveFilters)
                {
                    ActiveFiltersDisplayCollection.Add(filter);
                }
                OnPropertyChanged(nameof(IsFiltersActive));
            });

            _logger.LogDebug("Active filters display updated. Count: {Count}", ActiveFiltersDisplayCollection.Count);
        }

        #region Helpers

        /// <summary>
        /// Formats an entity type name for display in user-facing messages (e.g., for deletion errors).
        /// </summary>
        /// <param name="entityType">The name of the entity type.</param>
        /// <returns>A pluralized, user-friendly string representation of the entity type.</returns>
        private string FormatDependentTypeName(string entityType)
        {
            if (string.IsNullOrEmpty(entityType)) return "items";
            if (entityType.EndsWith("y"))
                return entityType.Substring(0, entityType.Length - 1) + "ies";
            if (entityType.EndsWith("s"))
                return entityType + "es";
            return entityType + "s";
        }

        /// <summary>
        /// Public method to trigger a data refresh from external sources.
        /// Note: `async void` is used here for event-handler-like behavior, allowing fire-and-forget invocation from UI elements.
        /// </summary>
        public async void RefreshData()
        {
            await RefreshDataAsync();
        }

        #endregion

    }
}
