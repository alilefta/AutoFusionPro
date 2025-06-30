using System.Windows.Controls;

namespace AutoFusionPro.UI.Views.Parts.Dialogs.AddEditPartDialogTabs
{
    /// <summary>
    /// Interaction logic for VehicleCompatibilityTabView.xaml
    /// </summary>
    public partial class VehicleCompatibilityTabView : UserControl
    {
        public VehicleCompatibilityTabView()
        {
            InitializeComponent();
        }


        //private async void PreferredSupplierCheckBox_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is CheckBox checkBox && checkBox.DataContext is PartSupplierDto supplierLink)
        //    {
        //        if (DataContext is AddEditPartDialogViewModel viewModel)
        //        {
        //            // The CheckBox's IsChecked property will update the DTO binding
        //            // We just need to trigger the service call.
        //            // Temporarily uncheck to prevent visual glitch if service call fails or logic changes it back
        //            bool intendedState = checkBox.IsChecked ?? false;
        //            checkBox.IsChecked = !intendedState; // Show old state during async op

        //            await viewModel.TogglePreferredSupplierExecuteAsync(supplierLink, intendedState);
        //        }
        //    }
        //}
    }

}
