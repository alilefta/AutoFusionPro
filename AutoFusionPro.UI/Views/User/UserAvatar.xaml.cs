using System.Windows;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Views.User
{
    /// <summary>
    /// Interaction logic for UserAvatar.xaml
    /// </summary>
    public partial class UserAvatar : UserControl
    {
        public UserAvatar()
        {
            InitializeComponent();
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            UserFlyout.IsOpen = !UserFlyout.IsOpen;
        }

        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            UserFlyout.IsOpen = !UserFlyout.IsOpen;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            UserFlyout.IsOpen = !UserFlyout.IsOpen;
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            UserFlyout.IsOpen = !UserFlyout.IsOpen;
        }

    }
}
