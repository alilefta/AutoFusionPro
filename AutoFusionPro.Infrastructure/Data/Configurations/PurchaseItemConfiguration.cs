using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.ToTable("PurchaseItems");

            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.Id).ValueGeneratedOnAdd();

            builder.Property(pi => pi.QuantityOrdered)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(pi => pi.UnitCost)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(pi => pi.LineTotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            // Consider HasComputedColumnSql if your DB supports it and LineTotal is always Qty * Cost

            // Relationships

            // Many-to-One with Purchase
            builder.HasOne(pi => pi.Purchase)
                .WithMany(p => p.PurchaseItems) // Assumes Purchase.PurchaseItems exists
                .HasForeignKey(pi => pi.PurchaseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a Purchase is deleted, its items are also deleted

            // Many-to-One with Part
            builder.HasOne(pi => pi.Part)
                .WithMany(p => p.PartPurchaseItems) // CORRECTED: Using the new collection property
                .HasForeignKey(pi => pi.PartId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete Part if it's on a purchase order item.

            // Many-to-One with UnitOfMeasure
            builder.HasOne(pi => pi.UnitOfMeasure)
                .WithMany() // A UnitOfMeasure can be used by many PurchaseItems.
                            // No inverse navigation property on UnitOfMeasure for PurchaseItems is strictly needed by EF Core.
                .HasForeignKey(pi => pi.UnitOfMeasureId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Cannot delete UoM if it's used.
        }
    }
}
