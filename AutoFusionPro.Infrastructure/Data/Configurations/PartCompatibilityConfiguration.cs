using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartCompatibilityConfiguration : IEntityTypeConfiguration<PartCompatibility>
    {
        public void Configure(EntityTypeBuilder<PartCompatibility> builder)
        {
            builder.ToTable("PartCompatibilities");

            builder.HasKey(pc => pc.Id);
            builder.Property(pc => pc.Id).ValueGeneratedOnAdd();

            builder.Property(pc => pc.Notes)
                .HasMaxLength(500);

            // Create a unique index to prevent duplicate compatibility records
            builder.HasIndex(pc => new { pc.PartId, pc.VehicleId }).IsUnique();

            // Relationships
            builder.HasOne(pc => pc.Part)
                .WithMany(p => p.CompatibleVehicles)
                .HasForeignKey(pc => pc.PartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pc => pc.Vehicle)
                .WithMany(v => v.CompatibleParts)
                .HasForeignKey(pc => pc.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}