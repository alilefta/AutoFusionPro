using System.Windows;

namespace AutoFusionPro.UI.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfirmLogoutDialog.xaml
    /// </summary>
    public partial class ConfirmLogoutDialog : Window
    {
        public ConfirmLogoutDialog()
        {
            InitializeComponent();
        }

        private void ConfirmLogout(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }


        private void CancelLogout(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
