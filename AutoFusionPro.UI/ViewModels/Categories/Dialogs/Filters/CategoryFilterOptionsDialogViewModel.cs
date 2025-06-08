using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Enums.UI.Categories;
using AutoFusionPro.UI.Helpers.Filtration;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.Categories.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Categories.Dialogs.Filters
{

    public partial class CategoryFilterOptionsDialogViewModel : InitializableViewModel<CategoryFilterOptionsDialogViewModel>, IDialogViewModelWithResult<CategoryFilterCriteriaDto>
    {

        #region Fields
        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;
        #endregion

        #region Props

        [ObservableProperty]
        private bool _isApplyingFilters = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        [NotifyPropertyChangedFor(nameof(IsFilterByIsActiveSelected))]
        private BooleanFilterOption? _selectedIsActiveFilter;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        [NotifyPropertyChangedFor(nameof(IsFilterBySubCategoriesSelected))]
        private BooleanFilterOption? _selectedHasSubcategoriesFilter;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        [NotifyPropertyChangedFor(nameof(IsFilterByPartsSelected))]
        private BooleanFilterOption? _selectedHasPartsFilter;

        [ObservableProperty]
        private ObservableCollection<BooleanFilterOption> _booleanFilterOptionsForHasSubcategories = new();

        [ObservableProperty]
        private ObservableCollection<BooleanFilterOption> _booleanFilterOptionsForHasParts = new();

        [ObservableProperty]
        private ObservableCollection<BooleanFilterOption> _booleanFilterOptionsForIsActive = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AnyFilterSelected))]
        [NotifyPropertyChangedFor(nameof(IsSortBySelected))]
        private KeyValuePair<CategorySortBy, string> _selectedSortByOption;

        [ObservableProperty]
        ObservableCollection<KeyValuePair<CategorySortBy, string>> _sortOptions = new();

        private CategoryFilterCriteriaDto? _resultFilters;

        public bool IsFilterBySubCategoriesSelected => SelectedHasSubcategoriesFilter?.Value != null;
        public bool IsFilterByPartsSelected => SelectedHasPartsFilter?.Value != null;
        public bool IsFilterByIsActiveSelected => SelectedIsActiveFilter?.Value != null;
        public bool IsSortBySelected => SelectedSortByOption.Key != CategorySortBy.NameAsc;

        public bool AnyFilterSelected => IsFilterBySubCategoriesSelected || IsFilterByPartsSelected || IsFilterByIsActiveSelected || IsSortBySelected;
        #endregion

        #region Constructor
        public CategoryFilterOptionsDialogViewModel(ILocalizationService localizationService, ILogger<CategoryFilterOptionsDialogViewModel> logger) : base(localizationService, logger)
        {

            PopulateBooleanFilterOptions();
            PopulateSortOptions();

            // Set initial default selections to "Any"
            SelectedIsActiveFilter = BooleanFilterOptionsForIsActive.FirstOrDefault(o => o.Value == null);
            SelectedHasSubcategoriesFilter = BooleanFilterOptionsForHasSubcategories.FirstOrDefault(o => o.Value == null);
            SelectedHasPartsFilter = BooleanFilterOptionsForHasParts.FirstOrDefault(o => o.Value == null);
            SelectedSortByOption = SortOptions.FirstOrDefault(o => o.Key == CategorySortBy.NameAsc);

        }

        #endregion


        #region Initializer
        /// <summary>
        /// Inherited from <see cref="InitializableViewModel{TViewModel}"/>
        /// </summary>
        /// <param name="parameter"><see cref="null"/></param>
        /// <returns><see cref="void"/></returns>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (IsInitialized) return;

            if (parameter is CategoryFilterCriteriaDto currentFilters)
            {
                SelectedIsActiveFilter = BooleanFilterOptionsForIsActive.FirstOrDefault(o => o.Value == currentFilters.IsActive)
                                        ?? BooleanFilterOptionsForIsActive.First(o => o.Value == null); // Default to "Any"
                SelectedHasSubcategoriesFilter = BooleanFilterOptionsForHasSubcategories.FirstOrDefault(o => o.Value == currentFilters.HasSubcategories)
                                        ?? BooleanFilterOptionsForHasSubcategories.First(o => o.Value == null);
                SelectedHasPartsFilter = BooleanFilterOptionsForHasParts.FirstOrDefault(o => o.Value == currentFilters.HasParts)
                                        ?? BooleanFilterOptionsForHasParts.First(o => o.Value == null);
                SelectedSortByOption = SortOptions.FirstOrDefault(o => o.Key == currentFilters.SortBy);
            }
            // No explicit await needed if base.InitializeAsync is not doing async work,
            // but good practice if it might in the future.
            await base.InitializeAsync(parameter);
        }

        private void PopulateBooleanFilterOptions()
        {
            // Populate Is Active
            var anyStr = System.Windows.Application.Current.Resources["AnyStr"] as string ?? "Any";
            var showActive = System.Windows.Application.Current.Resources["ShowOnlyActiveStr"] as string ?? "Show Only Active Categories";
            var showInactive = System.Windows.Application.Current.Resources["ShowOnlyInactiveStr"] as string ?? "Show Only Inactive Categories";

            BooleanFilterOptionsForIsActive.Add(new BooleanFilterOption(anyStr, null)); // "Any" or "Not Specified"
            BooleanFilterOptionsForIsActive.Add(new BooleanFilterOption(showActive, true));
            BooleanFilterOptionsForIsActive.Add(new BooleanFilterOption(showInactive, false));


            // Populate Has Subcategories
            var showWithSubcategories = System.Windows.Application.Current.Resources["ShowOnlyCategoryWithSubcategoriesStr"] as string ?? "Show Only Categories With Subcategories";
            var showWithoutSubcategories = System.Windows.Application.Current.Resources["ShowOnlyCategoryWithoutSubcategoriesStr"] as string ?? "Show Only Categories Without Subcategories";

            BooleanFilterOptionsForHasSubcategories.Add(new BooleanFilterOption(anyStr, null)); // "Any" or "Not Specified"
            BooleanFilterOptionsForHasSubcategories.Add(new BooleanFilterOption(showWithSubcategories, true));
            BooleanFilterOptionsForHasSubcategories.Add(new BooleanFilterOption(showWithoutSubcategories, false));


            // Populate Has Parts
            var showWithParts = System.Windows.Application.Current.Resources["ShowOnlyCategoryWithPartsStr"] as string ?? "Show Only Categories With Parts";
            var showWithoutParts = System.Windows.Application.Current.Resources["ShowOnlyCategoryWithoutPartsStr"] as string ?? "Show Only Categories Without Parts";

            BooleanFilterOptionsForHasParts.Add(new BooleanFilterOption(anyStr, null)); // "Any" or "Not Specified"
            BooleanFilterOptionsForHasParts.Add(new BooleanFilterOption(showWithParts, true));
            BooleanFilterOptionsForHasParts.Add(new BooleanFilterOption(showWithoutParts, false));


        }

        private void PopulateSortOptions()
        {
            SortOptions.Clear(); // Ensure it's clear before populating
            SortOptions.Add(new KeyValuePair<CategorySortBy, string>(CategorySortBy.NameAsc, System.Windows.Application.Current.Resources["NameAscStr"] as string ?? "Name (A-Z)"));
            SortOptions.Add(new KeyValuePair<CategorySortBy, string>(CategorySortBy.NameDesc, System.Windows.Application.Current.Resources["NameDescStr"] as string ?? "Name (Z-A)"));
            SortOptions.Add(new KeyValuePair<CategorySortBy, string>(CategorySortBy.LastUpdatedDesc, System.Windows.Application.Current.Resources["LastUpdatedDescStr"] as string ?? "Newest First"));
            SortOptions.Add(new KeyValuePair<CategorySortBy, string>(CategorySortBy.LastUpdatedAsc, System.Windows.Application.Current.Resources["LastUpdatedAscStr"] as string ?? "Oldest First"));
            SortOptions.Add(new KeyValuePair<CategorySortBy, string>(CategorySortBy.PartCountDesc, System.Windows.Application.Current.Resources["PartCountDescStr"] as string ?? "Most Parts"));
            SortOptions.Add(new KeyValuePair<CategorySortBy, string>(CategorySortBy.PartCountAsc, System.Windows.Application.Current.Resources["PartCountAscStr"] as string ?? "Fewest Parts"));
        }

        #endregion


        #region Commands

        [RelayCommand]
        private void ApplyFilters()
        {
            try
            {
                IsApplyingFilters = true;
                _resultFilters = new CategoryFilterCriteriaDto(
                    IsActive: SelectedIsActiveFilter?.Value,
                    HasSubcategories: SelectedHasSubcategoriesFilter?.Value,
                    HasParts: SelectedHasPartsFilter?.Value,
                    ParentId: null, // Assuming ParentId is not filtered in this specific dialog
                    SortBy: SelectedSortByOption.Key
                );
                SetResultAndClose(true);
            }
            finally
            {
                IsApplyingFilters = false;
            }
        }

        [RelayCommand]
        private void ResetAndClearFilters()
        {
            SelectedIsActiveFilter = BooleanFilterOptionsForIsActive.First(o => o.Value == null);
            SelectedHasSubcategoriesFilter = BooleanFilterOptionsForHasSubcategories.First(o => o.Value == null);
            SelectedHasPartsFilter = BooleanFilterOptionsForHasParts.First(o => o.Value == null);
            SelectedSortByOption = SortOptions.First(o => o.Key == CategorySortBy.NameAsc);
        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }

        #endregion

        #region Dialog Specific Methods

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

        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on {VM}", nameof(CategoryFilterOptionsDialog));
                return;
            }

            _dialog = dialog;
        }

        /// <summary>
        /// Set the Dialog Results. Inherited from <see cref="IDialogViewModelWithResult{TResult}"/> interface.
        /// </summary>
        /// <returns><see cref="CategoryFilterCriteriaDto?"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public CategoryFilterCriteriaDto? GetResult()
        {
            return _resultFilters; // Return the filters set by ApplyFilters
        }

        #endregion
    }
}
