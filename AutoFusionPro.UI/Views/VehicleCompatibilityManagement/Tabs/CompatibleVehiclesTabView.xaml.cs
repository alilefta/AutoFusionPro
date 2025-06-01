using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Tabs
{
    /// <summary>
    /// Interaction logic for CompatibleVehiclesTabView.xaml
    /// </summary>
    public partial class CompatibleVehiclesTabView : UserControl
    {
        public CompatibleVehiclesTabView()
        {
            InitializeComponent();
        }

        #region Sorting Logic Using 3 Fallback Methods
        //How to Use:

        //    1. For sortable columns: Add local:CompatibleVehiclesTabView.SortMemberPath="PropertyName"
        //    2. For non-sortable columns: Add local:CompatibleVehiclesTabView.IsSortable="False" or simply omit the SortMemberPath
        //    3. Mixed approach: You can still use DisplayMemberBinding for simple columns if you prefer

        //    Migration Strategy:

        //    Immediate: Add SortMemberPath to columns that need sorting but don't have DisplayMemberBinding
        //    Gradual: You can keep existing DisplayMemberBinding columns as-is
        //    Future: Standardize on SortMemberPath for consistency

        //private void OnColumnHeaderClick(object sender, RoutedEventArgs e)
        //{
        //    if (sender is GridViewColumnHeader columnHeader)
        //    {
        //        var column = columnHeader.Column;
        //        var direction = ListSortDirection.Ascending;

        //        // Check if the column is already sorted
        //        if (columnHeader.Tag is ListSortDirection currentDirection)
        //        {
        //            direction = currentDirection == ListSortDirection.Ascending
        //                ? ListSortDirection.Descending
        //                : ListSortDirection.Ascending;
        //        }

        //        // Set the tag to the new direction
        //        columnHeader.Tag = direction;

        //        // Sort the items
        //        Sort(column, direction);
        //    }


        //}

        ///// <summary>
        ///// Enhanced Sort method that works with both DisplayMemberBinding and custom templates
        ///// </summary>
        ///// <param name="column"></param>
        ///// <param name="direction"></param>
        //private void Sort(GridViewColumn column, ListSortDirection direction)
        //{
        //    if (column == null)
        //        return;

        //    string sortBy = GetSortPropertyPath(column);

        //    if (!string.IsNullOrEmpty(sortBy))
        //    {
        //        var collectionView = CollectionViewSource.GetDefaultView(CompatibleVehicleListView.ItemsSource);
        //        collectionView.SortDescriptions.Clear();
        //        collectionView.SortDescriptions.Add(new SortDescription(sortBy, direction));
        //        collectionView.Refresh();
        //    }
        //    else
        //    {
        //        // Optional: Show message or just ignore silently for non-sortable columns
        //        // MessageBox.Show("This column is not sortable.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}

        ///// <summary>
        ///// Gets the sort property path from various sources
        ///// </summary>
        ///// <param name="column"></param>
        ///// <returns></returns>
        //private string GetSortPropertyPath(GridViewColumn column)
        //{
        //    // Method 1: Check DisplayMemberBinding first (existing behavior)
        //    if (column.DisplayMemberBinding is Binding displayBinding)
        //    {
        //        return displayBinding.Path.Path;
        //    }

        //    // Method 2: Check if column has a SortMemberPath attached property
        //    string sortMemberPath = GetSortMemberPath(column);
        //    if (!string.IsNullOrEmpty(sortMemberPath))
        //    {
        //        return sortMemberPath;
        //    }

        //    // Method 3: Try to get from header mapping (fallback)
        //    var gridView = CompatibleVehicleListView.View as GridView;
        //    if (gridView != null)
        //    {
        //        var columnIndex = gridView.Columns.IndexOf(column);
        //        if (columnIndex >= 0)
        //        {
        //            var header = GetColumnHeader(columnIndex);
        //            return GetSortPropertyFromHeaderContent(header);
        //        }
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// Gets column header content for mapping
        ///// </summary>
        ///// <param name="columnIndex"></param>
        ///// <returns></returns>
        //private string GetColumnHeader(int columnIndex)
        //{
        //    var gridView = CompatibleVehicleListView.View as GridView;
        //    if (gridView != null && columnIndex < gridView.Columns.Count)
        //    {
        //        var header = gridView.Columns[columnIndex].Header;

        //        // Handle resource references
        //        if (header is string headerString)
        //        {
        //            return headerString;
        //        }

        //        // Handle dynamic resources (this is a simplified approach)
        //        return header?.ToString();
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Maps header content to property names as fallback
        ///// </summary>
        ///// <param name="headerContent"></param>
        ///// <returns></returns>
        //private string GetSortPropertyFromHeaderContent(string headerContent)
        //{
        //    if (string.IsNullOrEmpty(headerContent)) return null;

        //    // Create a mapping dictionary - adjust these based on your actual headers
        //    var headerToPropertyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        //    {
        //        { "Make", "MakeName" },
        //        { "Model", "ModelName" },
        //        { "Start Year", "YearStart" },
        //        { "End Year", "YearEnd" },
        //        { "Engine", "EngineTypeName" },
        //        { "Transmission Type", "TransmissionTypeName" },
        //        { "Trim Level", "TrimLevelName" },
        //        { "Body Type", "BodyTypeName" },
        //        // Add more mappings as needed
        //    };

        //    // Try exact match first
        //    if (headerToPropertyMap.ContainsKey(headerContent))
        //    {
        //        return headerToPropertyMap[headerContent];
        //    }

        //    // Try partial match (for localized headers)
        //    foreach (var kvp in headerToPropertyMap)
        //    {
        //        if (headerContent.Contains(kvp.Key) || kvp.Key.Contains(headerContent))
        //        {
        //            return kvp.Value;
        //        }
        //    }

        //    return null;
        //}

        #region Attached Property for SortMemberPath

        /// <summary>
        /// Attached property to specify sort member path for columns without DisplayMemberBinding
        /// Usage: local:CompatibleVehiclesTabView.SortMemberPath="PropertyName"
        /// </summary>
        public static readonly DependencyProperty SortMemberPathProperty =
            DependencyProperty.RegisterAttached(
                "SortMemberPath",
                typeof(string),
                typeof(CompatibleVehiclesTabView),
                new PropertyMetadata(null));

        public static void SetSortMemberPath(DependencyObject element, string value)
        {
            element.SetValue(SortMemberPathProperty, value);
        }

        public static string GetSortMemberPath(DependencyObject element)
        {
            return (string)element.GetValue(SortMemberPathProperty);
        }

        /// <summary>
        /// Attached property to disable sorting for specific columns
        /// Usage: local:CompatibleVehiclesTabView.IsSortable="False"
        /// </summary>
        public static readonly DependencyProperty IsSortableProperty =
            DependencyProperty.RegisterAttached(
                "IsSortable",
                typeof(bool),
                typeof(CompatibleVehiclesTabView),
                new PropertyMetadata(true));

        public static void SetIsSortable(DependencyObject element, bool value)
        {
            element.SetValue(IsSortableProperty, value);
        }

        public static bool GetIsSortable(DependencyObject element)
        {
            return (bool)element.GetValue(IsSortableProperty);
        }

        #endregion

        #endregion
    }

}

