using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Services;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AutoFusionPro.UI.ViewModels.ViewNotification
{
    public partial class NotificationViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger<NotificationViewModel> _logger;
        private readonly ILocalizationService<FlowDirection> _localizationService;

        [ObservableProperty]
        private ObservableCollection<Notification> _notifications = new();

        [ObservableProperty]
        private bool _isNotificationPanelOpen;

        [ObservableProperty]
        private int _unreadCount;

        public bool HasUnreadNotifications => UnreadCount > 0;
        public bool HasNotifications => Notifications.Any();

        public ICommand ToggleNotificationPanelCommand { get; }
        public ICommand MarkAsReadCommand { get; }
        public ICommand MarkAllAsReadCommand { get; }
        public ICommand DeleteNotificationCommand { get; }
        public ICommand DeleteAllNotificationsCommand { get; }

        public NotificationViewModel(
            INotificationService notificationService,
            ISessionManager sessionManager,
            ILogger<NotificationViewModel> logger, ILocalizationService<FlowDirection> localizationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

            // Initialize commands
            ToggleNotificationPanelCommand = new RelayCommand(_ => ToggleNotificationPanel(), o => true);
            MarkAsReadCommand = new RelayCommand(async id => await MarkAsReadAsync((int)id), o => true);
            MarkAllAsReadCommand = new RelayCommand(async _ => await MarkAllAsReadAsync(), o => true);
            DeleteNotificationCommand = new RelayCommand(async id => await DeleteNotificationAsync((int)id), o => true);
            DeleteAllNotificationsCommand = new RelayCommand(async _ => await DeleteAllNotificationsAsync() , o => true);

            // Register for notification changes
            _notificationService.NotificationsChanged += OnNotificationsChanged;

            _localizationService.FlowDirectionChanged += OnCurrentFlowDirectionChanged;

            CurrentWorkFlow = _localizationService.CurrentFlowDirection;

            // Initial load
            _ = LoadNotificationsAsync();
        }

        private void OnCurrentFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }

        private async void OnNotificationsChanged(object sender, NotificationChangedEventArgs e)
        {
            // If the change affects the current user's role, reload notifications
            if (e.AffectedRole == _sessionManager.CurrentUser.UserRole || _sessionManager.CurrentUser.UserRole == UserRole.Admin)
            {
                await LoadNotificationsAsync();
            }
        }

        private void ToggleNotificationPanel()
        {
            IsNotificationPanelOpen = !IsNotificationPanelOpen;

            // If we're opening the panel, refresh the notifications
            if (IsNotificationPanelOpen)
            {
                _ = LoadNotificationsAsync();
            }
        }

        private async Task LoadNotificationsAsync()
        {
            try
            {
                // Get notifications for the current user role
                var userRole = _sessionManager.CurrentUser.UserRole;
                var notificationsList = await _notificationService.GetNotificationsAsync(userRole);

                Notifications = new ObservableCollection<Notification>(notificationsList);
                UnreadCount = await _notificationService.GetUnreadNotificationsCountAsync(userRole);

                OnPropertyChanged(nameof(HasUnreadNotifications));
                OnPropertyChanged(nameof(HasNotifications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
            }
        }

        private async Task MarkAsReadAsync(int notificationId)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(notificationId);

                // Update the local notification object
                var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;

                    // Decrement unread count
                    UnreadCount--;
                    OnPropertyChanged(nameof(HasUnreadNotifications));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
            }
        }

        private async Task MarkAllAsReadAsync()
        {
            try
            {
                await _notificationService.MarkAllAsReadAsync(_sessionManager.CurrentUser.UserRole);

                // Update all notifications in the collection
                foreach (var notification in Notifications)
                {
                    notification.IsRead = true;
                }

                // Reset unread count
                UnreadCount = 0;
                OnPropertyChanged(nameof(HasUnreadNotifications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
            }
        }

        private async Task DeleteNotificationAsync(int notificationId)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(notificationId);

                // Remove from local collection
                var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    Notifications.Remove(notification);

                    // If it was unread, decrement the counter
                    if (!notification.IsRead)
                    {
                        UnreadCount--;
                        OnPropertyChanged(nameof(HasUnreadNotifications));
                    }

                    OnPropertyChanged(nameof(HasNotifications));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
            }
        }

        private async Task DeleteAllNotificationsAsync()
        {
            try
            {
                await _notificationService.DeleteAllNotificationsAsync(_sessionManager.CurrentUser.UserRole);

                // Clear local collection
                Notifications.Clear();
                UnreadCount = 0;

                OnPropertyChanged(nameof(HasUnreadNotifications));
                OnPropertyChanged(nameof(HasNotifications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all notifications");
            }
        }

        // Clean up
        public void Cleanup()
        {
            _notificationService.NotificationsChanged -= OnNotificationsChanged;
        }
    }
}