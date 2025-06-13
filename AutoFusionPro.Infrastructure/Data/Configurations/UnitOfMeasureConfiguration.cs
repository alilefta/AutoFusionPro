using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
    {
        public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
        {
            builder.ToTable("UnitOfMeasures");
            builder.HasKey(uom => uom.Id);
            builder.Property(uom => uom.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(uom => uom.Name).IsUnique();
            builder.Property(uom => uom.Symbol).IsRequired().HasMaxLength(10);
            builder.HasIndex(uom => uom.Symbol).IsUnique();
            builder.Property(uom => uom.Description).HasMaxLength(255);
        }
    }
}
