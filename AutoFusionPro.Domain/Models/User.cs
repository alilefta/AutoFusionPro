// Ignore Spelling: Username

using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.SystemEnum;
using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }

        public bool IsAdmin { get; set; } = false;

        public string? ProfilePictureUrl { get; set; }


        public DateTime? DateOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Male;
        public string? Address { get; set; }
        public string? City { get; set; }


        public bool IsActive { get; set; } = true;


        public DateTime? LastLoginDate { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.Now;

        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswerHash { get; set; }

        public bool IsTwoFactorEnabled { get; set; } = false;
        public string? TwoFactorSecret { get; set; }

        public DateTime LastActive { get; set; } = DateTime.Now;

        // Change SystemRole to an enum
        public UserRole UserRole { get; set; } = UserRole.User;

        public Languages? PreferredLanguage { get; set; } = Languages.Arabic;

        // One-to-One (Optional): User can be associated with one Staff member (or none)
        //public Staff? Staff { get; set; } // Optional relationship - User may not be linked to Staff

        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = [];
        public virtual ICollection<Invoice> Invoices { get; set; } = [];
        public virtual ICollection<Purchase> Purchases { get; set; } = [];
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = [];
        public virtual ICollection<Payment> ProcessedPayments { get; set; } = [];

    }
}
