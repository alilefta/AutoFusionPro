using AutoFusionPro.Domain.Models.VehiclesInventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations.VehicleInventory
{
    public class VehicleDamageLogConfiguration : IEntityTypeConfiguration<VehicleDamageLog>
    {
        public void Configure(EntityTypeBuilder<VehicleDamageLog> builder)
        {
            builder.ToTable("VehicleDamageLogs");
            builder.HasKey(dl => dl.Id);
            builder.Property(dl => dl.Id).ValueGeneratedOnAdd();

            builder.Property(dl => dl.DateNoted).IsRequired();
            builder.Property(dl => dl.Description).IsRequired().HasMaxLength(500);
            builder.Property(dl => dl.Severity).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(dl => dl.IsRepaired).IsRequired().HasDefaultValue(false);
            builder.Property(dl => dl.EstimatedRepairCost).HasColumnType("decimal(18,2)");
            builder.Property(dl => dl.ActualRepairCost).HasColumnType("decimal(18,2)");
            builder.Property(dl => dl.RepairNotes).HasMaxLength(1000);

            // Relationship to Vehicle (configured in VehicleConfiguration)

            // One-to-Many: VehicleDamageLog to VehicleDamageImage
            builder.HasMany(dl => dl.DamageImages)
                .WithOne(di => di.VehicleDamageLog)
                .HasForeignKey(di => di.VehicleDamageLogId)
                .OnDelete(DeleteBehavior.Cascade); // If damage log deleted, its specific images are deleted
        }
    }
}