using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .HasMaxLength(100);

            builder.Property(c => c.Address)
                .HasMaxLength(255);

            builder.Property(c => c.DiscountPercentage)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0);

            builder.Property(c => c.CreditLimit)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(c => c.CurrentBalance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            // Create indexes for faster lookup
            builder.HasIndex(c => c.PhoneNumber);
            builder.HasIndex(c => c.Email);
        }
    }
}