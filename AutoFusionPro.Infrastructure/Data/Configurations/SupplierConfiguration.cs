using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.ContactPerson)
                .HasMaxLength(100);

            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(s => s.Email)
                .HasMaxLength(100);

            builder.Property(s => s.Address)
                .HasMaxLength(255);

            builder.Property(s => s.Website)
                .HasMaxLength(255);

            builder.Property(s => s.PaymentTerms)
                .HasMaxLength(100);

            builder.Property(s => s.CurrentBalance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(s => s.Notes)
                .HasMaxLength(1000);

            // Create indexes for faster lookup
            builder.HasIndex(s => s.PhoneNumber);
            builder.HasIndex(s => s.Email);
        }
    }
}