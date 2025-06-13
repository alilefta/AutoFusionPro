using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Timestamp).IsRequired();
            builder.Property(a => a.ActionType).IsRequired().HasMaxLength(50);
            builder.Property(a => a.EntityType).HasMaxLength(100);
            builder.Property(a => a.EntityId).HasMaxLength(50); // Or int if IDs are always int

            // For OldValues/NewValues, consider if they can be very long (e.g., JSON blobs)
            // For SQL Server: builder.Property(a => a.OldValues).HasColumnType("nvarchar(max)");
            // For SQLite, TEXT has a very large limit by default.
            builder.Property(a => a.OldValues); // Defaults to nvarchar(4000) in SQL Server if not specified otherwise.
            builder.Property(a => a.NewValues);

            builder.Property(a => a.Description).HasMaxLength(1000);

            builder.HasOne(a => a.User)
                .WithMany(u => u.AuditLogs) // Assumes User.AuditLogs collection exists
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete user if audit logs exist, or SetNull if User can be optional
        }
    }
}
