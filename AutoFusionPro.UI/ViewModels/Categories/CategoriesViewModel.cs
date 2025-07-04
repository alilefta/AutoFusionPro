using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.Navigation;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Enums.UI.Categories;
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
using AutoFusionPro.UI.ViewModels.Categories.Dialogs;
using AutoFusionPro.UI.ViewModels.Categories.Dialogs.Filters;
using AutoFusionPro.UI.Views.Categories.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.ViewModels.Categories
{
    public partial class CategoriesViewModel : BaseViewModel<CategoriesViewModel>
    {
        #region Private Fields
        private INavigationService _navigationService;
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;
        private readonly IWpfToastNotificationService _wpfToastNotificationService;

        private Timer? _searchDebounceTimer;
        private const int SearchDebounceDelayMs = 500; // Adjust as needed (e.g., 300-750ms)
        #endregion


        #region General Props

        [ObservableProperty]
        private bool _isInititalized = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        private int _subCategoriesCount = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ViewerIcon))]
        private bool _isCardViewActive = false;

        public SymbolRegular ViewerIcon => IsCardViewActive ? SymbolRegular.AppsList24 : SymbolRegular.Grid28;

        #endregion

        #region Filter Props


        [ObservableProperty]
        private bool _isShowingFilters = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFiltersActive))]
        private CategoryFilterCriteriaDto _appliedFilterCriteria = new(); // Holds criteria from dialog

        [ObservableProperty]
        private ObservableCollection<ActiveFilterDisplayItem> _activeFiltersDisplayCollection = new();
        public bool IsFiltersActive => AppliedFilterCriteria != null &&
                                       (AppliedFilterCriteria.IsActive.HasValue ||
                                        AppliedFilterCriteria.HasSubcategories.HasValue ||
                                        AppliedFilterCriteria.HasParts.HasValue ||
                                        // ParentId filter not relevant if always showing top-level
                                        AppliedFilterCriteria.SortBy != CategorySortBy.NameAsc);

        #endregion

        #region Search Props

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ClearCategorySearchCommand))]
        [NotifyPropertyChangedFor(nameof(HasTextCategorySearchQuery))]
        private string? _searchQueryText;
        public bool HasTextCategorySearchQuery => !string.IsNullOrWhiteSpace(SearchQueryText);

        #endregion


        #region Category Props


        private List<CategoryDto> _pureCategoriesCollection = new();

        [ObservableProperty]
        private CategoryDto _selectedCategory = null;


        // This is what the UI (DataGrid/ItemsControl) binds to.
        [ObservableProperty]
        private ObservableCollection<CategoryDto> _displayedCategories = new ObservableCollection<CategoryDto>();


        #endregion







        public CategoriesViewModel(ICategoryService categoryService, IDialogService dialogService, INavigationService navigationService, IWpfToastNotificationService wpfToastNotificationService, ILocalizationService localizationService, ILogger<CategoriesViewModel> logger) : base(localizationService, logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));

            RegisterCleanup(() => _searchDebounceTimer?.Dispose());

            _ = InitializeAsync();
        }

         
        #region Initializer & Loading

        public async Task InitializeAsync()
        {

            if (!IsInititalized)
            {
                IsLoading = true;
                await LoadTopCategoriesAsync();
                IsInititalized = true;

                IsLoading = false;

            }
        }

        /// <summary>
        /// Fetches ALL top-level categories from the service and stores them in _pureCategoriesCollection.
        /// Then applies current filters and search to populate DisplayedCategories.
        /// </summary>
        private async Task LoadTopCategoriesAsync()
        {
            _logger.LogInformation("Loading master list of top-level categories from server.");
            IsLoading = true; // Could have a more specific IsMasterLoading flag
            _pureCategoriesCollection.Clear();
            try
            {
                // GetTopLevelCategoriesAsync should populate HasChildren and PartCount in CategoryDto correctly.
                // Pass onlyActive:false if you want to load all and filter by IsActive on client.
                // Or pass true if you mostly deal with active ones. For a filter dialog, loading all might be better.
                var categories = await _categoryService.GetTopLevelCategoriesAsync(onlyActive: false);

                if (categories != null)
                {
                    _pureCategoriesCollection.AddRange(categories);
                    _logger.LogInformation("Loaded {Count} master top-level categories.", _pureCategoriesCollection.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load master top-level categories from server.");
                _wpfToastNotificationService.ShowError("Failed to load categories. Please try again.", "Load Error");
            }
            finally
            {
                ApplyFiltersAndSearchToDisplay(); // Apply current filters/search to the newly loaded master list
                IsLoading = false;
            }
        }



        /// <summary>
        /// Applies the current _appliedFilterCriteria and _categorySearchQueryText
        /// to the _masterTopLevelCategories and updates the _displayedCategories collection.
        /// </summary>
        private void ApplyFiltersAndSearchToDisplay()
        {
            _logger.LogDebug("Applying client-side filters and search. Search: '{Search}', Filters: {@Filters}",
                SearchQueryText, AppliedFilterCriteria);

            DisplayedCategories.Clear();
            IEnumerable<CategoryDto> filteredItems = _pureCategoriesCollection;

            // 1. Apply filters from _appliedFilterCriteria DTO
            if (AppliedFilterCriteria != null)
            {
                if (AppliedFilterCriteria.IsActive.HasValue)
                {
                    filteredItems = filteredItems.Where(c => c.IsActive == AppliedFilterCriteria.IsActive.Value);
                }
                if (AppliedFilterCriteria.HasSubcategories.HasValue)
                {
                    filteredItems = filteredItems.Where(c => c.HasChildren == AppliedFilterCriteria.HasSubcategories.Value);
                }
                if (AppliedFilterCriteria.HasParts.HasValue)
                {
                    filteredItems = filteredItems.Where(c => (AppliedFilterCriteria.HasParts.Value ? c.PartCount > 0 : c.PartCount == 0));
                }
                // ParentId filter is not applicable here as we are dealing with top-level only.
            }

            // 2. Apply text search
            if (!string.IsNullOrWhiteSpace(SearchQueryText))
            {
                string searchTermLower = SearchQueryText.ToLowerInvariant();
                filteredItems = filteredItems.Where(c =>
                    c.Name.ToLowerInvariant().Contains(searchTermLower) ||
                    (c.Description?.ToLowerInvariant().Contains(searchTermLower) ?? false)
                );
            }

            // 3. Apply Sorting (from _appliedFilterCriteria)
            if (AppliedFilterCriteria != null)
            {
                switch (AppliedFilterCriteria.SortBy)
                {
                    case CategorySortBy.NameDesc:
                        filteredItems = filteredItems.OrderByDescending(c => c.Name);
                        break;
                    case CategorySortBy.LastUpdatedDesc:
                        filteredItems = filteredItems.OrderByDescending(c => c.ModifiedAt ?? c.CreatedAt); // Assuming BaseEntity props are in DTO or access entity
                        break;
                    case CategorySortBy.LastUpdatedAsc:
                        filteredItems = filteredItems.OrderBy(c => c.ModifiedAt ?? c.CreatedAt);
                        break;
                    case CategorySortBy.PartCountDesc:
                        filteredItems = filteredItems.OrderByDescending(c => c.PartCount);
                        break;
                    case CategorySortBy.PartCountAsc:
                        filteredItems = filteredItems.OrderBy(c => c.PartCount);
                        break;
                    case CategorySortBy.NameAsc: // Default
                    default:
                        filteredItems = filteredItems.OrderBy(c => c.Name);
                        break;
                }
            }
            else // Default sort if no criteria
            {
                filteredItems = filteredItems.OrderBy(c => c.Name);
            }


            // 4. Populate the UI-bound collection
            foreach (var item in filteredItems)
            {
                DisplayedCategories.Add(item);
            }
            _logger.LogInformation("DisplayedCategories updated. Count: {Count}", DisplayedCategories.Count);
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task ShowAddCategoryDialogAsync()
        {
            try
            {
                IsAdding = true;

                var results = await _dialogService.ShowDialogAsync<AddRootCategoryDialogViewModel, AddRootCategoryDialog>(null);

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new compatible vehicle has been added!");

                    await LoadTopCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemAddedSuccessfullyStr"] as string ?? "New Item Added Successfully!";

                    _wpfToastNotificationService.ShowSuccess(msg);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(AddRootCategoryDialog)}");
                var msg = System.Windows.Application.Current.Resources["CreationOperationFailedStr"] as string ?? "Creation Operation Has Failed!";
                _wpfToastNotificationService.ShowError(msg);

                // DEV ENV ONLY
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowAddCategoryDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditCategoryDialogAsync(CategoryDto? categoryToEdit)
        {
            if (categoryToEdit == null) return;
            try
            {
                var results = await _dialogService.ShowDialogAsync<EditRootCategoryDialogViewModel, EditRootCategoryDialog>(categoryToEdit);

                if (results.HasValue && results.Value == true)
                {
                    await  LoadTopCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemUpdatedSuccessfullyStr"] as string ?? "Category Updated Successfully!";

                    _wpfToastNotificationService.ShowSuccess(msg);

                    _logger.LogInformation("Transmission Type ID='{ID}' Updated Successfully!", categoryToEdit.Id);
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
        public async Task OpenCategoryDetailsAsync(CategoryDto? categoryDto)
        {
            if (categoryDto == null) return;
            try
            {
                _navigationService.NavigateTo(Core.Enums.NavigationPages.ApplicationPage.CategoryDetails, categoryDto);

            }
            catch(NavigationException nex)
            {
                var msgTitle = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Category {categoryDto.Name} Has Failed";
                var msg = System.Windows.Application.Current.Resources["ReasonIsStr"] as string ?? $"Reason Is";
                _wpfToastNotificationService.ShowError($"{msg} {nex.Message}", msgTitle);
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(nex.Message, true, CurrentWorkFlow);

            }
            catch (Exception ex)
            {
                var msg = System.Windows.Application.Current.Resources["NavigationFailedStr"] as string ?? $"Navigation to Category {categoryDto.Name} Has Failed";
                _wpfToastNotificationService.ShowError(msg);
                return;
            }
        }

        [RelayCommand]
        public async Task ShowDeleteCategoryDialogAsync(CategoryDto categoryToDelete)
        {
            if (categoryToDelete == null) return;

            var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

            if (results.HasValue && results.Value == true)
            {

                try
                {
                    await _categoryService.DeleteCategoryAsync(categoryToDelete.Id);

                    await LoadTopCategoriesAsync();

                    var msg = System.Windows.Application.Current.Resources["ItemDeletedSuccessfullyStr"] as string ?? "Item Has Been Deleted Successfully!";


                    _wpfToastNotificationService.ShowSuccess(msg);

                    _logger.LogInformation("Category with ID={ID} has been deleted successfully!", categoryToDelete.Id);

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


                    _wpfToastNotificationService.Show($"{msg}\n{dependentItemsText}", msgTitle, Core.Enums.UI.ToastType.Error,  TimeSpan.FromSeconds(10));
                    return;
                }
                catch (ServiceException ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                    var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                    _wpfToastNotificationService.ShowError(msg);

                    // FOR DEV ONLY
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowDeleteCategoryDialogAsync), MethodOperationType.DELETE_DATA, ex);


                }
                catch(Exception ex)
                {
                    _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                    var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                    _wpfToastNotificationService.ShowError(msg);

                    // FOR DEV ONLY
                    throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowDeleteCategoryDialogAsync), MethodOperationType.DELETE_DATA, ex);
                }
            }
        }

        [RelayCommand]
        private void ChangeView() // Renamed
        {
            IsCardViewActive = !IsCardViewActive;
            //ViewerIcon = IsCardViewActive ? SymbolRegular.AppsList24 : SymbolRegular.Grid28;

            // No data reload needed here if both views operate on the same DisplayedCategories
            // ApplyFiltersAndSearchToDisplay(); // Already done by OnAppliedFilterCriteriaChanged if filters were active
            // Or if sorting is different per view type, re-apply here.
        }



        #endregion

        #region Filter Commands

        [RelayCommand(CanExecute = nameof(HasTextCategorySearchQuery))]
        private void ClearCategorySearch() // Renamed
        {
            _searchDebounceTimer?.Dispose();
            SearchQueryText = string.Empty; // This triggers OnChanged and debounced client search
        }


        [RelayCommand]
        public async Task ShowFilterOptionsDialogAsync()
        {
            try
            {
                IsShowingFilters = true; // For button loading state if any
                var newFilters = await _dialogService.ShowDialogWithResultsAsync<CategoryFilterOptionsDialogViewModel, CategoryFilterOptionsDialog, CategoryFilterCriteriaDto>(AppliedFilterCriteria);

                if (newFilters != null)
                {
                    _logger.LogInformation("New category filter criteria applied from dialog.");
                    // Clear client-side text search when applying new server/complex filters,
                    // because the context of the search changes.
                    // CategorySearchQueryText = string.Empty; // Or preserve it and apply on top

                    AppliedFilterCriteria = newFilters; // This triggers OnAppliedFilterCriteriaChanged
                                                        // which calls 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening {nameof(CategoryFilterOptionsDialog)}");
                //var msg = System.Windows.Application.Current.Resources["CreationOperationFailedStr"] as string ?? "Creation Operation Has Failed!";
                //_wpfToastNotificationService.ShowError(msg);

                // DEV ENV ONLY
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(CategoriesViewModel), nameof(ShowFilterOptionsDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsShowingFilters = false;
            }
        }

        [RelayCommand]
        public async Task RemoveActiveFilterAsync(ActiveFilterDisplayItem filterToRemove)
        {
            if (filterToRemove == null || AppliedFilterCriteria == null) return;
            _logger.LogInformation("Removing client filter: Type='{FilterType}', Value='{FilterValue}'",
                filterToRemove.FilterType, // CORRECTED HERE
                filterToRemove.FilterValueDisplay);

            var updatedFilters = AppliedFilterCriteria;
            bool refreshNeeded = false; // To decide if we need to call ApplyFiltersAndSearchToDisplay

            switch (filterToRemove.CriterionKey)
            {
                case nameof(CategoryFilterCriteriaDto.IsActive):
                    updatedFilters = updatedFilters with { IsActive = null };
                    refreshNeeded = true;
                    break;
                case nameof(CategoryFilterCriteriaDto.HasSubcategories):
                    updatedFilters = updatedFilters with { HasSubcategories = null };
                    refreshNeeded = true;
                    break;
                case nameof(CategoryFilterCriteriaDto.HasParts):
                    updatedFilters = updatedFilters with { HasParts = null };
                    refreshNeeded = true;
                    break;
                case nameof(CategoryFilterCriteriaDto.SortBy):
                    // Reset SortBy to a default, e.g., NameAsc
                    updatedFilters = updatedFilters with { SortBy = CategorySortBy.NameAsc };
                    refreshNeeded = true;
                    break;
                    // SearchTerm is handled by ClearCategorySearchCommand directly modifying SearchQueryText
            }

            if (refreshNeeded)
            {
                AppliedFilterCriteria = updatedFilters; // This will trigger OnAppliedFilterCriteriaChanged
                                                        // which in turn calls ApplyFiltersAndSearchToDisplay
                                                        // and UpdateActiveFiltersDisplayAsync
            }
        }


        #endregion

        #region Property Changed Customization

        partial void OnSearchQueryTextChanged(string? value)
        {
            _searchDebounceTimer?.Dispose();
            _searchDebounceTimer = new Timer(_ => PerformDebouncedClientFilterAndSearch(), null, SearchDebounceDelayMs, Timeout.Infinite);
            ClearCategorySearchCommand.NotifyCanExecuteChanged();
        }

        // Called when _appliedFilterCriteria DTO is replaced (e.g., after dialog)
        partial void OnAppliedFilterCriteriaChanged(CategoryFilterCriteriaDto value)
        {
            _logger.LogInformation("Server-side filter criteria changed. Applying client-side filters and search.");
            // Search term is part of the criteria now, or separate
            // CategorySearchQueryText = value?.SearchTerm; // Sync if SearchTerm is part of DTO from dialog

            ApplyFiltersAndSearchToDisplay(); // Re-filter the master list
            _ = UpdateActiveFiltersDisplayAsync(); // Update UI "pills"
            OnPropertyChanged(nameof(IsFiltersActive));
        }

        #endregion

        private async Task UpdateActiveFiltersDisplayAsync() // Make it async if fetching names for IDs
        {
            ActiveFiltersDisplayCollection.Clear();
            if (AppliedFilterCriteria == null)
            {
                OnPropertyChanged(nameof(IsFiltersActive)); // Ensure IsFiltersActive updates
                return;
            }

            // For Search Term (if you want to display it as an active filter "pill")
            // This is client-side, so it's based on SearchQueryText, not AppliedFilterCriteria.SearchTerm
            // if you've decided server-side filtering for categories doesn't use SearchTerm.
            // If server-side *does* use it, then use AppliedFilterCriteria.SearchTerm
            if (!string.IsNullOrWhiteSpace(SearchQueryText)) // Using the ViewModel's direct search property
            {
                var searchStr = System.Windows.Application.Current.Resources["SearchStr"] as string ?? "Search";
                ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(searchStr, SearchQueryText, "ClientSearchTerm", SearchQueryText));
            }


            if (AppliedFilterCriteria.IsActive.HasValue)
            {
                var statusStr = System.Windows.Application.Current.Resources["StatusStr"] as string ?? "Status";
                var activeStr = System.Windows.Application.Current.Resources["ActiveStr"] as string ?? "Active";
                var inactiveStr = System.Windows.Application.Current.Resources["InactiveStr"] as string ?? "Inactive";
                ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(
                    statusStr,
                    AppliedFilterCriteria.IsActive.Value ? activeStr : inactiveStr,
                    nameof(CategoryFilterCriteriaDto.IsActive),
                    AppliedFilterCriteria.IsActive.Value));
            }

            if (AppliedFilterCriteria.HasSubcategories.HasValue)
            {
                var hasSubStr = System.Windows.Application.Current.Resources["HasSubcategoriesStr"] as string ?? "Has Subcategories";
                var yesStr = System.Windows.Application.Current.Resources["YesStr"] as string ?? "Yes";
                var noStr = System.Windows.Application.Current.Resources["NoStr"] as string ?? "No";
                ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(
                    hasSubStr,
                    AppliedFilterCriteria.HasSubcategories.Value ? yesStr : noStr,
                    nameof(CategoryFilterCriteriaDto.HasSubcategories),
                    AppliedFilterCriteria.HasSubcategories.Value));
            }

            if (AppliedFilterCriteria.HasParts.HasValue)
            {
                var hasPartsStr = System.Windows.Application.Current.Resources["HasPartsStr"] as string ?? "Has Parts";
                var yesStr = System.Windows.Application.Current.Resources["YesStr"] as string ?? "Yes";
                var noStr = System.Windows.Application.Current.Resources["NoStr"] as string ?? "No";
                ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(
                    hasPartsStr,
                    AppliedFilterCriteria.HasParts.Value ? yesStr : noStr,
                    nameof(CategoryFilterCriteriaDto.HasParts),
                    AppliedFilterCriteria.HasParts.Value));
            }

            if (AppliedFilterCriteria.SortBy != CategorySortBy.NameAsc) // Assuming NameAsc is default and not shown as "active"
            {
                var sortByStr = System.Windows.Application.Current.Resources["SortByStr"] as string ?? "Sort By";
                ActiveFiltersDisplayCollection.Add(new ActiveFilterDisplayItem(
                    sortByStr,
                    FormatSortByForDisplay(AppliedFilterCriteria.SortBy), // Helper to get user-friendly sort name
                    nameof(CategoryFilterCriteriaDto.SortBy),
                    AppliedFilterCriteria.SortBy));
            }

            OnPropertyChanged(nameof(IsFiltersActive)); // Re-evaluate IsFiltersActive
            _logger.LogDebug("Active filters display updated. Count: {Count}", ActiveFiltersDisplayCollection.Count);
        }


        #region Helpers

        private void PerformDebouncedClientFilterAndSearch()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _logger.LogInformation("Debounced client filter/search for: {SearchQuery} with filters: {@Filters}",
                    SearchQueryText, AppliedFilterCriteria);
                ApplyFiltersAndSearchToDisplay();
            });
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

        private string FormatSortByForDisplay(CategorySortBy sortBy)
        {
            // This could also use localized resources
            return sortBy switch
            {
                CategorySortBy.NameAsc => System.Windows.Application.Current.Resources["NameAscStr"] as string ?? "Name (A-Z)",
                CategorySortBy.NameDesc => System.Windows.Application.Current.Resources["NameDescStr"] as string ?? "Name (Z-A)",
                CategorySortBy.LastUpdatedDesc => System.Windows.Application.Current.Resources["LastUpdatedDescStr"] as string ?? "Newest First",
                CategorySortBy.LastUpdatedAsc => System.Windows.Application.Current.Resources["LastUpdatedAscStr"] as string ?? "Oldest First",
                CategorySortBy.PartCountDesc => System.Windows.Application.Current.Resources["PartCountDescStr"] as string ?? "Most Parts",
                CategorySortBy.PartCountAsc => System.Windows.Application.Current.Resources["PartCountAscStr"] as string ?? "Fewest Parts",
                _ => sortBy.ToString(),
            };
        }

        #endregion
    }
}
