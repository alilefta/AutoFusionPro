using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions;
using AutoFusionPro.UI.ViewModels.Shell;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AutoFusionPro.UI.Views.Shell
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : UserControl
    {
        public ShellView(IWpfToastNotificationService toastService)
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            toastService.IntitializeContainer(ToastContainer);

        }

        /// <summary>
        /// Handled this way because sometimes the DataContext of ShellView, got assigned to MainWindowViewModel. Which is incorrect, we should set it into an initialized instance of ShellViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="DentaFusionProException"></exception>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && DataContext is not ShellViewModel) 
            {
                throw new AutoFusionProException($"Shell View should have valid Data Context, and should be initialized first. Currently it is {DataContext}");
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            //// Make sure ToastContainer exists before initializing
            //var toastContainer = this.FindName("ToastContainer") as ItemsControl;

            //if (toastContainer != null)
            //{
            //    // Initialize the toast notification service with the found container
            //    ToastNotificationService.Instance.Initialize(toastContainer);
            //    System.Diagnostics.Debug.WriteLine("Toast notification service initialized successfully.");
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("WARNING: ToastContainer not found in ShellView.");
            //}


            var culture = new CultureInfo("ar-IQ"); // or just "ar" for general Arabic
            TimeTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd - hh:mm tt", culture); // Show immediately

            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMinutes(1)
            };

            timer.Tick += (s, e) =>
            {
                TimeTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd - hh:mm tt", culture);
            };

            timer.Start();
        }
    }
}
