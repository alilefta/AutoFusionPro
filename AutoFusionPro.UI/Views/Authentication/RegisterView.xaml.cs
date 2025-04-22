using AutoFusionPro.UI.ViewModels.Authentication;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Views.Authentication
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView(RegisterViewModel registerViewModel)
        {
            InitializeComponent();
            DataContext = registerViewModel;

        }
    }
}
