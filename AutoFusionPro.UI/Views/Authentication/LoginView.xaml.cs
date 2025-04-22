using AutoFusionPro.UI.ViewModels.Authentication;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Views.Authentication
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView(LoginViewModel loginViewModel)
        {
            InitializeComponent();
            DataContext = loginViewModel;
        }
    }
}
