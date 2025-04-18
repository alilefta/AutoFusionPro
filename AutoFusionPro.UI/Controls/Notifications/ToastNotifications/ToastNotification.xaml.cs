using AutoFusionPro.Core.Enums.UI;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.Controls.Notifications.ToastNotifications
{
    public partial class ToastNotification : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ToastNotification), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(ToastNotification), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(SymbolRegular), typeof(ToastNotification), new PropertyMetadata(null));

        public static readonly DependencyProperty ToastTypeProperty =
            DependencyProperty.Register(nameof(ToastType), typeof(ToastType), typeof(ToastNotification),
                new PropertyMetadata(ToastType.Info, OnToastTypeChanged));

        public event EventHandler Closed;

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public SymbolRegular Icon
        {
            get => (SymbolRegular)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public ToastType ToastType
        {
            get => (ToastType)GetValue(ToastTypeProperty);
            set => SetValue(ToastTypeProperty, value);
        }

        private TimeSpan _duration = TimeSpan.FromSeconds(4);
        private System.Threading.Timer _timer;

        public ToastNotification()
        {
            InitializeComponent();

            var res = System.Windows.Application.Current.Resources;

            MainBorder.Background = res[$"Toast.Info.BackgroundBrush"] as Brush;
            Foreground = res[$"Toast.Info.ForegroundBrush"] as Brush;
            MainBorder.BorderBrush = res[$"Toast.Info.BorderBrush"] as Brush;
        }

        public void Show(TimeSpan duration)
        {
            _duration = duration;
            var storyboard = FindResource("ShowAnimation") as Storyboard;
            storyboard?.Begin();

            // Set up auto-hide timer
            _timer = new System.Threading.Timer(
                state => Dispatcher.Invoke(() => Hide()),
                null,
                _duration,
                System.Threading.Timeout.InfiniteTimeSpan);
        }

        public void Hide()
        {
            _timer?.Dispose();
            _timer = null;

            var storyboard = FindResource("HideAnimation") as Storyboard;
            storyboard?.Begin();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void HideAnimation_Completed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private static void OnToastTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToastNotification notification)
            {
                notification.UpdateAppearance((ToastType)e.NewValue);
            }
        }

        private void UpdateAppearance(ToastType toastType)
        {
            var res = System.Windows.Application.Current.Resources;

            MainBorder.Background = res[$"Toast.{toastType}.BackgroundBrush"] as Brush;
            Foreground = res[$"Toast.{toastType}.ForegroundBrush"] as Brush;
            MainBorder.BorderBrush = res[$"Toast.{toastType}.BorderBrush"] as Brush;
        }
    }

}
