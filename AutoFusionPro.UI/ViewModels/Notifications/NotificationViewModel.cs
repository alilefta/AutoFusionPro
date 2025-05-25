using AutoFusionPro.Application.DTOs;
using AutoFusionPro.Application.Events;
using AutoFusionPro.Application.Interfaces.SessionManagement;
using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.ViewNotification
{
    public partial class NotificationViewModel : BaseViewModel<NotificationViewModel>
    {
        private readonly INotificationService _notificationService;
        private readonly ISessionManager _sessionManager;

        [ObservableProperty]
        private ObservableCollection<NotificationDto> _notifications = new();

        [ObservableProperty]
        private bool _isNotificationPanelOpen;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MarkAllAsReadCommand))] // Notify command when count changes
        [NotifyPropertyChangedFor(nameof(HasUnreadNotifications))] // Notify calculated property
        private int _unreadCount;

        public bool HasUnreadNotifications => UnreadCount > 0;
        public bool HasNotifications => Notifications.Any();

        public IRelayCommand ToggleNotificationPanelCommand { get; }
        public IRelayCommand<int> MarkAsReadCommand { get; }
        public IRelayCommand MarkAllAsReadCommand { get; }
        public IRelayCommand<int> DeleteNotificationCommand { get; }
        public IRelayCommand DeleteAllNotificationsCommand { get; }

        public NotificationViewModel(
            INotificationService notificationService,
            ISessionManager sessionManager,
            ILogger<NotificationViewModel> logger, ILocalizationService localizationService) : base(localizationService, logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));

            // Initialize commands
            // Initialize commands with CanExecute
            ToggleNotificationPanelCommand = new RelayCommand(ToggleNotificationPanel); // CanExecute always true
            MarkAsReadCommand = new RelayCommand<int>(async (id) => await MarkAsReadAsync(id), (id) => id > 0); // Basic check
            MarkAllAsReadCommand = new RelayCommand(async () => await MarkAllAsReadAsync(), () => HasUnreadNotifications); // Depends on state
            DeleteNotificationCommand = new RelayCommand<int>(async (id) => await DeleteNotificationAsync(id), (id) => id > 0); // Basic check
            DeleteAllNotificationsCommand = new RelayCommand(async () => await DeleteAllNotificationsAsync(), () => HasNotifications); // Depends on state

            // Register for notification changes
            _notificationService.NotificationsChanged += OnNotificationsChanged;

            RegisterCleanup(() => _notificationService.NotificationsChanged -= OnNotificationsChanged);

            // Initial load
            _ = LoadNotificationsAsync();

            // Need to react when collection changes for DeleteAllNotificationsCommand CanExecute
            Notifications.CollectionChanged += (s, e) => DeleteAllNotificationsCommand.NotifyCanExecuteChanged();
        }

        private async void OnNotificationsChanged(object sender, NotificationChangedEventArgs e)
        {
            var currentUser = _sessionManager.CurrentUser; // Check once
            if (currentUser == null) return;

            // Admin sees all user/admin changes, others only see their role changes
            if (e.AffectedRole == currentUser.UserRole || currentUser.UserRole == UserRole.Admin)
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
            if (!_sessionManager.Initialized.IsCompleted) return;

            var currentUser = _sessionManager.CurrentUser;
            if (currentUser == null)
            {
                _logger.LogWarning("Cannot load notifications: Current user is null.");
                Notifications.Clear(); // Clear list if user logs out
                UnreadCount = 0;
                // Manually trigger property changes and CanExecute updates if needed after clear
                OnPropertyChanged(nameof(HasNotifications));
                MarkAllAsReadCommand.NotifyCanExecuteChanged();
                DeleteAllNotificationsCommand.NotifyCanExecuteChanged();
                return;
            }

            try
            {
                var userRole = currentUser.UserRole;
                var notificationsList = await _notificationService.GetNotificationsAsync(userRole);
                var unread = await _notificationService.GetUnreadNotificationsCountAsync(userRole);

                // Update collection efficiently
                Notifications.Clear();
                if (notificationsList != null)
                {
                    foreach (var notification in notificationsList)
                    {
                        Notifications.Add(notification);
                    }
                }

                // Update properties - ObservableProperty handles OnPropertyChanged
                UnreadCount = unread;

                // No need to manually call OnPropertyChanged for calculated properties
                // if their dependencies are [ObservableProperty]

                // Explicitly notify commands relying on state
                MarkAllAsReadCommand.NotifyCanExecuteChanged();
                DeleteAllNotificationsCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications for role {UserRole}", currentUser.UserRole);
                // Optionally show error to user
                // _dialogService?.ShowError("Failed to load notifications.", ex.Message);
            }
        }

        private async Task MarkAsReadAsync(int notificationId)
        {
            // Find local item first
            var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification == null || notification.IsRead) // Already read or not found locally
            {
                return;
            }

            try
            {
                await _notificationService.MarkAsReadAsync(notificationId);

                // --- Update UI AFTER success ---
                // This assumes NotificationDto is a class or mutable record
                notification.IsRead = true;

                // Update count (use SetProperty to trigger dependent updates)
                UnreadCount = Math.Max(0, UnreadCount - 1); // Prevent going below zero

                // If the UI binds directly to IsRead within the item template,
                // the NotificationDto needs to support INotifyPropertyChanged
                // or be replaced entirely for the change to reflect reliably.
                // For simplicity here, we assume direct modification works or the list is rebound.

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
                // Optionally show error to user
            }
        }

        private async Task MarkAllAsReadAsync()
        {
            var currentUser = _sessionManager.CurrentUser;
            if (currentUser == null || UnreadCount == 0) return; // Nothing to do

            try
            {
                await _notificationService.MarkAllAsReadAsync(currentUser.UserRole);

                // --- Update UI AFTER success ---
                bool changed = false;
                foreach (var notification in Notifications)
                {
                    if (!notification.IsRead)
                    {
                        notification.IsRead = true; // Assumes mutable DTO
                        changed = true;
                        // If DTO implements INPC, UI updates. Otherwise, may need list refresh.
                    }
                }
                if (changed)
                {
                    UnreadCount = 0; // Directly sets _unreadCount via [ObservableProperty]
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for role {UserRole}", currentUser.UserRole);
                // Optionally show error to user
            }
        }

        private async Task DeleteNotificationAsync(int notificationId)
        {
            // Find local item first
            var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification == null) return; // Not found locally

            try
            {
                await _notificationService.DeleteNotificationAsync(notificationId);

                // --- Update UI AFTER success ---
                bool wasUnread = !notification.IsRead;
                Notifications.Remove(notification); // Remove from ObservableCollection

                if (wasUnread)
                {
                    UnreadCount = Math.Max(0, UnreadCount - 1);
                }
                // HasNotifications updates automatically via CollectionChanged/Linq

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", notificationId);
                // Optionally show error to user
            }
        }

        private async Task DeleteAllNotificationsAsync()
        {
            var currentUser = _sessionManager.CurrentUser;
            if (currentUser == null || !Notifications.Any()) return; // Nothing to do

            // Optional: Confirm with user before deleting all
            // var confirm = await _dialogService.ShowConfirmation("Delete all notifications?");
            // if (!confirm) return;

            try
            {
                await _notificationService.DeleteAllNotificationsAsync(currentUser.UserRole);

                // --- Update UI AFTER success ---
                Notifications.Clear();
                UnreadCount = 0;
                // HasNotifications updates automatically via CollectionChanged/Linq
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all notifications for role {UserRole}", currentUser.UserRole);
                // Optionally show error to user
            }
        }

    }
}