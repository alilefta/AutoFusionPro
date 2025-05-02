using System.Windows;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Views.Vehicles
{
    /// <summary>
    /// Interaction logic for VehiclesManagementView.xaml
    /// </summary>
    public partial class VehiclesView : UserControl
    {
        public VehiclesView()
        {
            InitializeComponent();
        }

        private void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            //if (DataContext is CasesViewModel casesViewModel && sender is CheckBox checkBox)
            //{
            //    if (checkBox.DataContext is CaseViewModel patient)
            //    {
            //        casesViewModel.ToggleSelection(patient);
            //    }
            //}
        }

        private void OnCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            //if (DataContext is CasesViewModel casesViewModel && sender is CheckBox checkBox)
            //{
            //    if (checkBox.DataContext is CaseViewModel patient)
            //    {
            //        casesViewModel.ToggleSelection(patient);
            //    }
            //}
        }
    }
}
