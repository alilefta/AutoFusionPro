using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.UI;
using AutoFusionPro.Domain.Models.Base;
using System.Security.Policy;

namespace AutoFusionPro.Domain.Models
{
    /// <summary>
    /// Notification: (For sending notifications to users - appointment reminders, alerts, etc.) 
    /// Relationships: 
    ///     Many-to-One with User
    /// </summary>
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string? RelatedEntityId { get; set; }

        public UserRole Role { get; set; }

        public NotificationType Type { get; set; }

        // Navigation Properties

        //// Many-to-One: Notification is for one User
        /////public int UserId { get; set; }
        //public User User { get; set; } = null!; // Required Relationship
    }
}
