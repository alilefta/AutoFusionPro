using System.Windows.Controls;

namespace AutoFusionPro.UI.Controls.Notifications.ToastNotifications
{
    public partial class ToastContainer : UserControl
    {
        public ToastContainer()
        {
            InitializeComponent();
        }

        public void AddNotification(ToastNotification notification)
        {
            NotificationsItemsControl.Items.Add(notification);
            notification.Closed += Notification_Closed;
        }

        private void Notification_Closed(object sender, System.EventArgs e)
        {
            if (sender is ToastNotification notification)
            {
                notification.Closed -= Notification_Closed;
                NotificationsItemsControl.Items.Remove(notification);
            }
        }
    }
}
