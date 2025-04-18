﻿using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.UI;
using AutoFusionPro.Domain.Models;
using System.Collections.ObjectModel;

namespace AutoFusionPro.Application.Interfaces
{
    public interface INotificationService
    {
        // Get notifications for the current user role
        Task<IEnumerable<Notification>> GetNotificationsAsync(UserRole currentUserRole);

        // Get unread notifications count
        Task<int> GetUnreadNotificationsCountAsync(UserRole currentUserRole);

        // Mark a notification as read
        Task MarkAsReadAsync(int notificationId);

        // Mark all notifications as read for a specific role
        Task MarkAllAsReadAsync(UserRole currentUserRole);

        // Delete a specific notification
        Task DeleteNotificationAsync(int notificationId);

        // Delete all notifications for a specific role
        Task DeleteAllNotificationsAsync(UserRole currentUserRole);

        // Add a new notification
        Task AddNotificationAsync(string title, string message, UserRole targetRole, NotificationType type, string? relatedEntityId = null);

        // Event that fires when notifications change
        event EventHandler<NotificationChangedEventArgs> NotificationsChanged;
    }

    public class NotificationChangedEventArgs : EventArgs
    {
        public UserRole AffectedRole { get; set; }
        public NotificationChangeType ChangeType { get; set; }
    }

}
