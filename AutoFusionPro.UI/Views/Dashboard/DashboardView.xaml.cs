using AutoFusionPro.UI.ViewModels.Dashboard;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView(DashboardViewModel dashboardViewModel)
        {
            InitializeComponent();
            DataContext = dashboardViewModel;
        }
    }
}
