using AutoFusionPro.Domain.Models.VehiclesInventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations.VehicleInventory
{
    public class VehicleServiceHistoryConfiguration : IEntityTypeConfiguration<VehicleServiceHistory>
    {
        public void Configure(EntityTypeBuilder<VehicleServiceHistory> builder)
        {
            builder.ToTable("VehicleServiceHistories");
            builder.HasKey(sr => sr.Id);
            builder.Property(sr => sr.Id).ValueGeneratedOnAdd();

            builder.Property(sr => sr.ServiceDate).IsRequired();
            builder.Property(sr => sr.ServiceDescription).IsRequired().HasMaxLength(500);
            builder.Property(sr => sr.ServiceProviderName).HasMaxLength(255);
            builder.Property(sr => sr.Cost).HasColumnType("decimal(18,2)");
            builder.Property(sr => sr.Notes).HasMaxLength(1000);

            // Relationship to Vehicle (configured in VehicleConfiguration)
        }
    }
}
