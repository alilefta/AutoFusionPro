using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).ValueGeneratedOnAdd();

            builder.Property(v => v.Make)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Year)
                .IsRequired();

            builder.Property(v => v.Engine)
                .HasMaxLength(50);

            builder.Property(v => v.Transmission)
                .HasMaxLength(50);

            builder.Property(v => v.VIN)
                .HasMaxLength(17);

            // Create an index for faster vehicle lookup
            builder.HasIndex(v => new { v.Make, v.Model, v.Year });
        }
    }
}
