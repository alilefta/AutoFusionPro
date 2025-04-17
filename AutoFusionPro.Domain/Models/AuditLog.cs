using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class AuditLog : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Description { get; set; }

        // Navigation Properties

        // Many-to-One: AuditLog is performed by one User
        public User User { get; set; } = null!; // Required Relationship
    }
}
