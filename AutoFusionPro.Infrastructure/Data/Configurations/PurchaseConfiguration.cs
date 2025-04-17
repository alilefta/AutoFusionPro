using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.PurchaseNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(p => p.PurchaseNumber).IsUnique();

            builder.Property(p => p.PurchaseDate)
                .IsRequired();

            builder.Property(p => p.Notes)
                .HasMaxLength(1000);

            builder.Property(p => p.Subtotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.TaxAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(p => p.ShippingCost)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(p => p.Total)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Supplier)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for faster querying
            builder.HasIndex(p => p.PurchaseDate);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.SupplierId);
        }
    }
}