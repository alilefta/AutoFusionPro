using AutoFusionPro.Domain.Models;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations.VehicleInventory
{
    public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
    {
        public void Configure(EntityTypeBuilder<VehicleImage> builder)
        {
            builder.ToTable("VehicleImages");
            builder.HasKey(vi => vi.Id);
            builder.Property(vi => vi.Id).ValueGeneratedOnAdd();

            builder.Property(vi => vi.ImagePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(vi => vi.Caption).HasMaxLength(255);
            builder.Property(vi => vi.IsPrimary).IsRequired().HasDefaultValue(false);
            builder.Property(vi => vi.DisplayOrder).IsRequired().HasDefaultValue(0);

            // Foreign key to Vehicle is configured in VehicleConfiguration via HasMany
            // builder.HasOne(vi => vi.Vehicle)
            //     .WithMany(v => v.Images)
            //     .HasForeignKey(vi => vi.VehicleId)
            //     .IsRequired()
            //     .OnDelete(DeleteBehavior.Cascade);

            // Optional: Unique constraint for (VehicleId, IsPrimary = true) if only one primary image is allowed.
            // This is complex to enforce directly with a simple index if IsPrimary can be false for many.
            // Usually handled by business logic in the service layer.
            // If you wanted to try:
            // builder.HasIndex(vi => new { vi.VehicleId, vi.IsPrimary })
            //        .IsUnique()
            //        .HasFilter("[IsPrimary] = 1"); // SQL Server specific filter for unique index
            // SQLite doesn't support filtered unique indexes this way easily.
            // Service layer logic is better for this rule.
        }
    }
}
