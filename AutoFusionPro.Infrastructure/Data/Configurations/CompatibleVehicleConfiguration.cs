using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class CompatibleVehicleConfiguration : IEntityTypeConfiguration<CompatibleVehicle>
    {
        public void Configure(EntityTypeBuilder<CompatibleVehicle> builder)
        {
            builder.ToTable("CompatibleVehicles");
            builder.HasKey(cv => cv.Id);

            builder.Property(cv => cv.YearStart).IsRequired();
            builder.Property(cv => cv.YearEnd).IsRequired();

            builder.Property(cv => cv.VIN)
                .HasMaxLength(20); // Adjust length as needed
            builder.HasIndex(cv => cv.VIN).IsUnique().HasFilter("[VIN] IS NOT NULL"); // Unique if VIN is provided

            // Composite index for uniqueness of a configuration
            // This combination should uniquely identify a vehicle spec for compatibility
            builder.HasIndex(cv => new {
                cv.ModelId,
                cv.YearStart,
                cv.YearEnd,
                cv.TrimLevelId,
                cv.TransmissionTypeId,
                cv.EngineTypeId,
                cv.BodyTypeId
            }).IsUnique().HasDatabaseName("IX_CompatibleVehicle_UniqueSpec");


            // Relationships
            builder.HasOne(cv => cv.Model)
                .WithMany(m => m.CompatibleVehicleSpecs)
                .HasForeignKey(cv => cv.ModelId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a Model if CompatibleVehicles reference it

            builder.HasOne(cv => cv.TrimLevel)
                .WithMany(t => t.CompatibleVehicleSpecs)
                .HasForeignKey(cv => cv.TrimLevelId)
                .IsRequired(false) // TrimLevel is optional
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cv => cv.TransmissionType)
                .WithMany(tt => tt.CompatibleVehicleSpecs)
                .HasForeignKey(cv => cv.TransmissionTypeId)
                .IsRequired(false) // TransmissionType is optional
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cv => cv.EngineType)
                .WithMany(et => et.CompatibleVehicleSpecs)
                .HasForeignKey(cv => cv.EngineTypeId)
                .IsRequired(false) // EngineType is optional
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cv => cv.BodyType)
                .WithMany(bt => bt.CompatibleVehicleSpecs)
                .HasForeignKey(cv => cv.BodyTypeId)
                .IsRequired(false) // BodyType is optional
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
