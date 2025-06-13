using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(i => i.InvoiceNumber).IsUnique();

            builder.Property(i => i.InvoiceDate)
                .IsRequired();

            builder.Property(i => i.DueDate)
                .IsRequired();

            builder.Property(i => i.Notes)
                .HasMaxLength(1000);

            builder.Property(i => i.Subtotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(i => i.TaxAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(i => i.Total)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.AmountPaid)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            //builder.Property(i => i.Balance)
            //    .HasColumnType("decimal(18,2)")
            //    .HasComputedColumnSql("[Total] - [AmountPaid]");

            // Relationships
            builder.HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Order)
                .WithMany(o => o.Invoices)
                .HasForeignKey(i => i.OrderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for faster querying
            builder.HasIndex(i => i.InvoiceDate);
            builder.HasIndex(i => i.DueDate);
            builder.HasIndex(i => i.Status);
            builder.HasIndex(i => i.CustomerId);
        }
    }
}