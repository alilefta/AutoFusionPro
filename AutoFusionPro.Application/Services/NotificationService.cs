using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.UI;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public event EventHandler<NotificationChangedEventArgs> NotificationsChanged;

        public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IEnumerable<Notification>> GetNotificationsAsync(UserRole currentUserRole)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadNotificationsCountAsync(UserRole currentUserRole)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsReadAsync(int notificationId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAllAsReadAsync(UserRole currentUserRole)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNotificationAsync(int notificationId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllNotificationsAsync(UserRole currentUserRole)
        {
            throw new NotImplementedException();
        }

        public Task AddNotificationAsync(string title, string message, UserRole targetRole, NotificationType type, string? relatedEntityId = null)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<Notification>> GetNotificationsAsync(UserRole currentUserRole)
        //{
        //    try
        //    {
        //        // If the user is an admin, get both admin notifications and all user-level notifications
        //        if (currentUserRole == UserRole.Admin)
        //        {
        //            return await _unitOfWork.Notifications
        //                .GetAllAsync(n => n.Role == UserRole.Admin || n.Role == UserRole.User,
        //                    orderBy: q => q.OrderByDescending(n => n.Timestamp));
        //        }

        //        // For other roles, only get notifications targeted at their role or general notifications
        //        return await _unitOfWork.Repository<Notification>()
        //            .GetAllAsync(n => n.Role == currentUserRole,
        //                orderBy: q => q.OrderByDescending(n => n.Timestamp));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting notifications for role {Role}", currentUserRole);
        //        throw;
        //    }
        //}

        //public async Task<int> GetUnreadNotificationsCountAsync(UserRole currentUserRole)
        //{
        //    try
        //    {
        //        if (currentUserRole == UserRole.Admin)
        //        {
        //            return await _unitOfWork.Repository<Notification>()
        //                .CountAsync(n => (n.Role == UserRole.Admin || n.Role == UserRole.User) && !n.IsRead);
        //        }

        //        return await _unitOfWork.Repository<Notification>()
        //            .CountAsync(n => n.Role == currentUserRole && !n.IsRead);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting unread notification count for role {Role}", currentUserRole);
        //        throw;
        //    }
        //}

        //public async Task MarkAsReadAsync(int notificationId)
        //{
        //    try
        //    {
        //        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(notificationId);

        //        if (notification != null)
        //        {
        //            notification.IsRead = true;
        //            await _unitOfWork.Repository<Notification>().UpdateAsync(notification);
        //            await _unitOfWork.SaveChangesAsync();

        //            NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
        //            {
        //                AffectedRole = notification.Role,
        //                ChangeType = NotificationChangeType.Read
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error marking notification {Id} as read", notificationId);
        //        throw;
        //    }
        //}

        //public async Task MarkAllAsReadAsync(UserRole currentUserRole)
        //{
        //    try
        //    {
        //        IEnumerable<Notification> notifications;

        //        if (currentUserRole == UserRole.Admin)
        //        {
        //            notifications = await _unitOfWork.Repository<Notification>()
        //                .GetAllAsync(n => (n.Role == UserRole.Admin || n.Role == UserRole.User) && !n.IsRead);
        //        }
        //        else
        //        {
        //            notifications = await _unitOfWork.Repository<Notification>()
        //                .GetAllAsync(n => n.Role == currentUserRole && !n.IsRead);
        //        }

        //        foreach (var notification in notifications)
        //        {
        //            notification.IsRead = true;
        //            await _unitOfWork.Repository<Notification>().UpdateAsync(notification);
        //        }

        //        await _unitOfWork.SaveChangesAsync();

        //        NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
        //        {
        //            AffectedRole = currentUserRole,
        //            ChangeType = NotificationChangeType.Read
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error marking all notifications as read for role {Role}", currentUserRole);
        //        throw;
        //    }
        //}

        //public async Task DeleteNotificationAsync(int notificationId)
        //{
        //    try
        //    {
        //        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(notificationId);

        //        if (notification != null)
        //        {
        //            var role = notification.Role;
        //            await _unitOfWork.Repository<Notification>().DeleteAsync(notificationId);
        //            await _unitOfWork.SaveChangesAsync();

        //            NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
        //            {
        //                AffectedRole = role,
        //                ChangeType = NotificationChangeType.Deleted
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting notification {Id}", notificationId);
        //        throw;
        //    }
        //}

        //public async Task DeleteAllNotificationsAsync(UserRole currentUserRole)
        //{
        //    try
        //    {
        //        IEnumerable<Notification> notifications;

        //        if (currentUserRole == UserRole.Admin)
        //        {
        //            notifications = await _unitOfWork.Repository<Notification>()
        //                .GetAllAsync(n => n.Role == UserRole.Admin || n.Role == UserRole.User);
        //        }
        //        else
        //        {
        //            notifications = await _unitOfWork.Repository<Notification>()
        //                .GetAllAsync(n => n.Role == currentUserRole);
        //        }

        //        foreach (var notification in notifications)
        //        {
        //            await _unitOfWork.Repository<Notification>().DeleteAsync(notification.Id);
        //        }

        //        await _unitOfWork.SaveChangesAsync();

        //        NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
        //        {
        //            AffectedRole = currentUserRole,
        //            ChangeType = NotificationChangeType.Deleted
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting all notifications for role {Role}", currentUserRole);
        //        throw;
        //    }
        //}

        //public async Task AddNotificationAsync(string title, string message, UserRole targetRole, NotificationType type, string? relatedEntityId = null)
        //{
        //    try
        //    {
        //        var notification = new Notification
        //        {
        //            Title = title,
        //            Message = message,
        //            Timestamp = DateTime.Now,
        //            IsRead = false,
        //            RelatedEntityId = relatedEntityId,
        //            Role = targetRole,
        //            Type = type
        //        };

        //        await _unitOfWork.Repository<Notification>().AddAsync(notification);
        //        await _unitOfWork.SaveChangesAsync();

        //        NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
        //        {
        //            AffectedRole = targetRole,
        //            ChangeType = NotificationChangeType.Added
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error adding notification: {Title}", title);
        //        throw;
        //    }
        //}
    }
}
