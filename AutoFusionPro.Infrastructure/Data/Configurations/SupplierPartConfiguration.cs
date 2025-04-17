using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class SupplierPartConfiguration : IEntityTypeConfiguration<SupplierPart>
    {
        public void Configure(EntityTypeBuilder<SupplierPart> builder)
        {
            builder.ToTable("SupplierParts");

            builder.HasKey(sp => sp.Id);
            builder.Property(sp => sp.Id).ValueGeneratedOnAdd();

            builder.Property(sp => sp.SupplierPartNumber)
                .HasMaxLength(50);

            builder.Property(sp => sp.Cost)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Create a unique index to prevent duplicate supplier-part records
            builder.HasIndex(sp => new { sp.SupplierId, sp.PartId }).IsUnique();

            // Relationships
            builder.HasOne(sp => sp.Supplier)
                .WithMany(s => s.Parts)
                .HasForeignKey(sp => sp.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sp => sp.Part)
                .WithMany(p => p.Suppliers)
                .HasForeignKey(sp => sp.PartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}