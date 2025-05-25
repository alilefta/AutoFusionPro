using AutoFusionPro.Application.DTOs;
using AutoFusionPro.Application.Events;
using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.UI;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using Microsoft.Extensions.Logging;

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


        public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(UserRole currentUserRole)
        {
            try
            {
                IEnumerable<Notification> notifications;
                // Use specific repository from UoW and FindAsync
                if (currentUserRole == UserRole.Admin)
                {
                    notifications = await _unitOfWork.Notifications.FindAsync(n => n.Role == UserRole.Admin || n.Role == UserRole.User);
                }
                else
                {
                    notifications = await _unitOfWork.Notifications.FindAsync(n => n.Role == currentUserRole);
                }

                // Order results in memory (consider adding OrderBy to repository if performance critical)
                return notifications
                    .OrderByDescending(n => n.Timestamp)
                    .Select(MapToDto); // Map to DTO
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for role {Role}", currentUserRole);
                // Avoid throwing generic Exception, consider a custom service exception
                // For now, re-throwing to indicate failure
                throw;
            }
        }

        public async Task<int> GetUnreadNotificationsCountAsync(UserRole currentUserRole)
        {
            try
            {
                // Use specific repository from UoW and CountAsync
                if (currentUserRole == UserRole.Admin)
                {
                    return await _unitOfWork.Notifications.CountAsync(n => (n.Role == UserRole.Admin || n.Role == UserRole.User) && !n.IsRead);
                }
                else // Corrected: Added 'else' for clarity
                {
                    return await _unitOfWork.Notifications.CountAsync(n => n.Role == currentUserRole && !n.IsRead);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count for role {Role}", currentUserRole);
                throw;
            }
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            try
            {
                // Use specific repository from UoW
                var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);

                if (notification != null && !notification.IsRead) // Only update if not already read
                {
                    notification.IsRead = true;
                    // No explicit Update call needed - EF Core tracks the change
                    await _unitOfWork.SaveChangesAsync(); // Save changes via UoW

                    _logger.LogInformation("Marked notification {Id} as read.", notificationId);
                    NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
                    {
                        AffectedRole = notification.Role,
                        ChangeType = NotificationChangeType.Read
                    });
                }
                else if (notification == null)
                {
                    _logger.LogWarning("Attempted to mark non-existent notification {Id} as read.", notificationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {Id} as read", notificationId);
                throw;
            }
        }

        public async Task MarkAllAsReadAsync(UserRole currentUserRole)
        {
            try
            {
                IEnumerable<Notification> notificationsToUpdate;
                // Use specific repository from UoW and FindAsync
                if (currentUserRole == UserRole.Admin)
                {
                    notificationsToUpdate = await _unitOfWork.Notifications.FindAsync(n => (n.Role == UserRole.Admin || n.Role == UserRole.User) && !n.IsRead);
                }
                else
                {
                    notificationsToUpdate = await _unitOfWork.Notifications.FindAsync(n => n.Role == currentUserRole && !n.IsRead);
                }

                bool changed = false;
                foreach (var notification in notificationsToUpdate)
                {
                    notification.IsRead = true;
                    changed = true;
                    // No explicit Update call needed
                }

                if (changed)
                {
                    await _unitOfWork.SaveChangesAsync(); // Save all changes at once
                    _logger.LogInformation("Marked all unread notifications as read for role {Role}.", currentUserRole);
                    NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
                    {
                        AffectedRole = currentUserRole, // Or determine more specific affected roles if needed
                        ChangeType = NotificationChangeType.Read
                    });
                }
                else
                {
                    _logger.LogInformation("No unread notifications found to mark as read for role {Role}.", currentUserRole);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for role {Role}", currentUserRole);
                throw;
            }
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            try
            {
                // Use specific repository from UoW
                var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);

                if (notification != null)
                {
                    var role = notification.Role; // Get role before deleting
                    _unitOfWork.Notifications.Delete(notification); // Use Delete method from IBaseRepository
                    await _unitOfWork.SaveChangesAsync(); // Save changes via UoW

                    _logger.LogInformation("Deleted notification {Id}.", notificationId);
                    NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
                    {
                        AffectedRole = role,
                        ChangeType = NotificationChangeType.Deleted
                    });
                }
                else
                {
                    _logger.LogWarning("Attempted to delete non-existent notification {Id}.", notificationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {Id}", notificationId);
                throw;
            }
        }

        public async Task DeleteAllNotificationsAsync(UserRole currentUserRole)
        {
            try
            {
                IEnumerable<Notification> notificationsToDelete;
                // Use specific repository from UoW and FindAsync
                if (currentUserRole == UserRole.Admin)
                {
                    // Careful: Should Admin delete ALL user notifications too? Clarify requirement.
                    // Assuming yes for now based on original code.
                    notificationsToDelete = await _unitOfWork.Notifications.FindAsync(n => n.Role == UserRole.Admin || n.Role == UserRole.User);
                }
                else
                {
                    notificationsToDelete = await _unitOfWork.Notifications.FindAsync(n => n.Role == currentUserRole);
                }

                var notificationsList = notificationsToDelete.ToList(); // Convert to list for RemoveRange
                if (notificationsList.Any())
                {
                    // Use RemoveRange for efficiency if supported by base repository
                    _unitOfWork.Notifications.RemoveRange(notificationsList);
                    await _unitOfWork.SaveChangesAsync(); // Save all deletions at once

                    _logger.LogInformation("Deleted {Count} notifications for role {Role}.", notificationsList.Count, currentUserRole);
                    NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
                    {
                        AffectedRole = currentUserRole,
                        ChangeType = NotificationChangeType.Deleted
                    });
                }
                else
                {
                    _logger.LogInformation("No notifications found to delete for role {Role}.", currentUserRole);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all notifications for role {Role}", currentUserRole);
                throw;
            }
        }

        public async Task AddNotificationAsync(string title, string message, UserRole targetRole, NotificationType type, string? relatedEntityId = null)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Attempted to add notification with empty title or message.");
                // Consider throwing ArgumentException instead of silently failing
                return;
            }

            try
            {
                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    Timestamp = DateTime.UtcNow, // Use UtcNow
                    IsRead = false,
                    RelatedEntityId = relatedEntityId,
                    Role = targetRole,
                    Type = type
                    // CreatedAt/ModifiedAt handled by BaseEntity/DbContext override
                };

                // Use specific repository from UoW
                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync(); // Save changes via UoW

                _logger.LogInformation("Added notification '{Title}' for role {Role}.", title, targetRole);
                NotificationsChanged?.Invoke(this, new NotificationChangedEventArgs
                {
                    AffectedRole = targetRole,
                    ChangeType = NotificationChangeType.Added
                    // Optionally include the created DTO: Notification = MapToDto(notification)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding notification: {Title}", title);
                throw;
            }
        }

        // --- Private Mapping Helper ---
        private NotificationDto MapToDto(Notification notification)
        {
            if (notification == null) return null; // Or throw? Depends on usage.

            return new NotificationDto(
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Timestamp, // Assuming Timestamp is already UTC or consistent
                notification.IsRead,
                notification.Type,
                notification.RelatedEntityId
            );
        }

    }
}
