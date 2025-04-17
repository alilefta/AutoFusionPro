using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable("InventoryTransactions");

            builder.HasKey(it => it.Id);
            builder.Property(it => it.Id).ValueGeneratedOnAdd();

            builder.Property(it => it.TransactionDate)
                .IsRequired();

            builder.Property(it => it.PreviousQuantity)
                .IsRequired();

            builder.Property(it => it.Quantity)
                .IsRequired();

            builder.Property(it => it.NewQuantity)
                .IsRequired();

            builder.Property(it => it.ReferenceNumber)
                .HasMaxLength(50);

            builder.Property(it => it.Notes)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(it => it.Part)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(it => it.User)
                .WithMany(u => u.InventoryTransactions)
                .HasForeignKey(it => it.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for faster querying
            builder.HasIndex(it => it.TransactionDate);
            builder.HasIndex(it => it.PartId);
            builder.HasIndex(it => it.TransactionType);
        }
    }
}