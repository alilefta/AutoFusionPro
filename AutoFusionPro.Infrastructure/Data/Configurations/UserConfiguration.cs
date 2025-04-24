using AutoFusionPro.Core.Enums.SystemEnum;
using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            // Required properties
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(u => u.Username).IsUnique();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(255);

            builder.Property(u => u.Salt)
                .HasMaxLength(255);

            // Optional properties with appropriate constraints
            builder.Property(u => u.ProfilePictureUrl)
                .HasMaxLength(500);


            builder.Property(u => u.Address)
                .HasMaxLength(255);

            builder.Property(u => u.City)
                .HasMaxLength(100);

            builder.Property(u => u.SecurityQuestion)
                .HasMaxLength(500);

            builder.Property(u => u.SecurityAnswerHash)
                .HasMaxLength(255);

            builder.Property(u => u.TwoFactorSecret)
                .HasMaxLength(100);

            builder.Property(u => u.PreferredLanguage)
                .HasDefaultValue(Languages.Arabic);

            // Date properties
            builder.Property(u => u.DateRegistered)
                .IsRequired();

            builder.Property(u => u.LastActive)
                .IsRequired();

            // Boolean properties
            builder.Property(u => u.IsAdmin)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.IsTwoFactorEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            // Enum property
            builder.Property(u => u.UserRole)
                .IsRequired()
                .HasDefaultValue(Core.Enums.ModelEnum.UserRole.User)
                .HasConversion<string>(); // Store enum as string in database

            // Relationships (inverse navigation properties)
            builder.HasMany(u => u.AuditLogs)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Additional indexes for performance
            builder.HasIndex(u => u.LastLoginDate);
            builder.HasIndex(u => u.UserRole);
            builder.HasIndex(u => u.IsActive);
        }
    }
}
