using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id).ValueGeneratedOnAdd();

            builder.Property(oi => oi.QuantitySold)
                .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(oi => oi.DiscountPercentage)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0);

            builder.Property(oi => oi.LineTotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasOne(oi => oi.UnitOfMeasure)
            .WithMany() // UoM can be used by many order items
            .HasForeignKey(oi => oi.UnitOfMeasureId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

            // Relationships
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Part)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create a unique index to prevent duplicate items in an order
            builder.HasIndex(oi => new { oi.OrderId, oi.PartId }).IsUnique();
        }
    }
}