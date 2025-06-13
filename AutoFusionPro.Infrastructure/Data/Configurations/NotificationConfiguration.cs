using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id).ValueGeneratedOnAdd();

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200); // Adjust length as needed

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000); // Or more if messages can be long

            builder.Property(n => n.Timestamp)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.RelatedEntityId)
                .HasMaxLength(50); // Assuming IDs are generally not excessively long strings
                                   // If RelatedEntityId is always an int, you could change its type in the model

            // Configure Enum to String conversions
            builder.Property(n => n.Role) // UserRole
                .IsRequired()
                .HasConversion<string>() // Store UserRole enum as string
                .HasMaxLength(50);       // Max length for the string representation of the enum

            builder.Property(n => n.Type) // NotificationType
                .IsRequired()
                .HasConversion<string>() // Store NotificationType enum as string
                .HasMaxLength(50);       // Max length for the string representation of the enum

            // Indexes for common query patterns
            builder.HasIndex(n => n.Timestamp);
            builder.HasIndex(n => n.IsRead);
            builder.HasIndex(n => n.Role); // If you often query notifications by role
            builder.HasIndex(n => n.Type); // If you often query notifications by type

            // --- Optional: Relationship to User ---
            // If you uncomment the User relationship in your Notification model:
            // public int? UserId { get; set; }
            // public virtual User? User { get; set; }
            //
            // Then you would add this configuration:
            /*
            builder.HasOne(n => n.User)
                .WithMany() // Assuming User does not have a direct ICollection<Notification>
                            // If User has ICollection<Notification> Notifications, use .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .IsRequired(false) // Because a notification might be for a Role instead of a specific User
                .OnDelete(DeleteBehavior.SetNull); // Or Restrict, depending on desired behavior if user is deleted
                                                   // SetNull makes UserId null if user is deleted.
                                                   // Restrict prevents user deletion if they have notifications (unless also by Role).
            builder.HasIndex(n => n.UserId); // Index if UserId relationship is added and queried
            */
        }
    }
}
