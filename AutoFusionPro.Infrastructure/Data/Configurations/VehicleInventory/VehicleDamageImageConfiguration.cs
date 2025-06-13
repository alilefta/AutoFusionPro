using AutoFusionPro.Domain.Models.VehiclesInventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations.VehicleInventory
{
    public class VehicleDamageImageConfiguration : IEntityTypeConfiguration<VehicleDamageImage>
    {
        public void Configure(EntityTypeBuilder<VehicleDamageImage> builder)
        {
            builder.ToTable("VehicleDamageImages");
            builder.HasKey(di => di.Id);
            builder.Property(di => di.Id).ValueGeneratedOnAdd();

            builder.Property(di => di.ImagePath).IsRequired().HasMaxLength(500);
            builder.Property(di => di.Caption).HasMaxLength(255);

            // Relationship to VehicleDamageLog (configured in VehicleDamageLogConfiguration)
        }
    }
}
