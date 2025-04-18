using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Core.Enums.UI;
using AutoFusionPro.UI.Controls.Notifications.ToastNotifications;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace AutoFusionPro.Application.Services
{
    public interface IWpfToastNotificationService : IToastNotificationService
    {
        void IntitializeContainer(ToastContainer container);

        bool IsInitialized { get; }
    }

    public class ToastNotificationService : IWpfToastNotificationService
    {
        private ToastContainer _container;
        private readonly TimeSpan _defaultDuration = TimeSpan.FromSeconds(4);
        private readonly ILogger<ToastNotificationService> _logger;
        private Dispatcher _dispatcher;

        public bool IsInitialized { get; private set; } = false;


        public ToastNotificationService(ILogger<ToastNotificationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void IntitializeContainer(ToastContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _dispatcher = container.Dispatcher;

            if(container == null)
            {
                _logger.LogError("Toast Container was null, continuing without Toast Notification Service");
                IsInitialized = false;
            }
            else
            {
                _logger.LogInformation("Toast Container initialized");
                IsInitialized = true;
            }



        }

        public void Show(string message, string? title = null, ToastType type = ToastType.Info, TimeSpan? duration = null)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            _dispatcher.BeginInvoke(new Action(() =>
            {
                var notification = new ToastNotification
                {
                    Title = title ?? GetDefaultTitle(type),
                    Message = message,
                    Icon = GetIcon(type),
                    ToastType = type
                };

                _container.AddNotification(notification);
                notification.Show(duration ?? _defaultDuration);
            }));
        }

        public void ShowSuccess(string message, string? title = null, TimeSpan? duration = null)
        {
            Show(message, title, ToastType.Success, duration);
        }

        public void ShowError(string message, string? title = null, TimeSpan? duration = null)
        {
            Show(message, title, ToastType.Error, duration);
        }

        public void ShowWarning(string message, string? title = null, TimeSpan? duration = null)
        {
            Show(message, title, ToastType.Warning, duration);
        }

        public void ShowInfo(string message, string? title = null, TimeSpan? duration = null)
        {
            Show(message, title, ToastType.Info, duration);
        }

        private SymbolRegular GetIcon(ToastType type)
        {
            return type switch
            {
                ToastType.Success => SymbolRegular.CheckmarkCircle48,
                ToastType.Error => SymbolRegular.DismissCircle48,
                ToastType.Warning => SymbolRegular.Warning28,
                ToastType.Info => SymbolRegular.Info28,
                ToastType.Primary => SymbolRegular.Info28,
                ToastType.Neutral => SymbolRegular.Info28,
                _ => SymbolRegular.Info28
            };
        }

        private string GetDefaultTitle(ToastType type)
        {
            return type switch
            {
                ToastType.Success => System.Windows.Application.Current.Resources["SuccessStr"] as string ?? "Success",
                ToastType.Error => System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                ToastType.Warning => System.Windows.Application.Current.Resources["WarningStr"] as string ?? "Warning",
                ToastType.Info => System.Windows.Application.Current.Resources["InfoStr"] as string ?? "Information",
                _ => string.Empty
            };
        }
    }
}
