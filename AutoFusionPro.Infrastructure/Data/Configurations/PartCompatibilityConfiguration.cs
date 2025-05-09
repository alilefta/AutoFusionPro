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

            builder.Property(pc => pc.Notes).HasMaxLength(500);

            // Unique index to prevent duplicate Part - CompatibleVehicle links
            builder.HasIndex(pc => new { pc.PartId, pc.CompatibleVehicleId }).IsUnique();

            // Relationships
            builder.HasOne(pc => pc.Part)
                .WithMany(p => p.CompatibleVehicles) // Assumes Part.CompatibleVehicles is ICollection<PartCompatibility>
                .HasForeignKey(pc => pc.PartId)
                .OnDelete(DeleteBehavior.Cascade); // If a Part is deleted, its compatibility links are removed

            builder.HasOne(pc => pc.CompatibleVehicle)
                .WithMany(cv => cv.PartCompatibilities) // Assumes CompatibleVehicle.PartCompatibilities exists
                .HasForeignKey(pc => pc.CompatibleVehicleId)
                .OnDelete(DeleteBehavior.Cascade); // If a CompatibleVehicle spec is deleted, links are removed
        }
    }
}