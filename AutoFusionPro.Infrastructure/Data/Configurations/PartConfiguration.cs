using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.ToTable("Parts");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.PartNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.PartNumber).IsUnique();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.Manufacturer)
                .HasMaxLength(100);

            builder.Property(p => p.CostPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.SellingPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Location)
                .HasMaxLength(50);

            builder.Property(p => p.ImagePath)
                .HasMaxLength(255);

            builder.Property(p => p.Notes)
                .HasMaxLength(1000);

            builder.HasOne(p => p.StockingUnitOfMeasure)
             .WithMany(uom => uom.PartsStockingUnit) // Updated inverse navigation
             .HasForeignKey(p => p.StockingUnitOfMeasureId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict); // Cannot delete UoM if parts use it as stocking unit

            builder.HasOne(p => p.SalesUnitOfMeasure)
                .WithMany(uom => uom.PartsSalesUnit) // Updated inverse navigation
                .HasForeignKey(p => p.SalesUnitOfMeasureId)
                .IsRequired(false) // Sales UoM can be optional (defaults to stocking UoM)
                .OnDelete(DeleteBehavior.SetNull); // If Sales UoM deleted, set Part.SalesUnitOfMeasureId to null

            builder.HasOne(p => p.PurchaseUnitOfMeasure)
                .WithMany(uom => uom.PartsPurchaseUnit) // Updated inverse navigation
                .HasForeignKey(p => p.PurchaseUnitOfMeasureId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.SalesConversionFactor).HasColumnType("decimal(18,4)"); // Allow precision
            builder.Property(p => p.PurchaseConversionFactor).HasColumnType("decimal(18,4)");

            // Relationships
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Parts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
