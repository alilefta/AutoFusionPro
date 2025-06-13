using System.Windows.Controls;

namespace AutoFusionPro.UI.Views.Settings.Tabs
{
    /// <summary>
    /// Interaction logic for GeneralSettingsTabItem.xaml
    /// </summary>
    public partial class GeneralSettingsTabItem : UserControl
    {
        public GeneralSettingsTabItem()
        {
            InitializeComponent();
        }

        //private void OnToggleButtonChecked(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is EditUserDialogViewModel viewModel)
        //    {
        //        viewModel.IsAdmin = true; // or false depending on the state of the toggle
        //    }
        //}

        //private void OnToggleButtonUnchecked(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is EditUserDialogViewModel viewModel)
        //    {
        //        viewModel.IsAdmin = false; // or true depending on the state of the toggle
        //    }
        //}
    }
}
