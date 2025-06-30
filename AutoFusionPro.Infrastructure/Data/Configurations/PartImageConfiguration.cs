using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartImageConfiguration : IEntityTypeConfiguration<PartImage>
    {
        public void Configure(EntityTypeBuilder<PartImage> builder)
        {
            builder.ToTable("PartImages");
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.Id).ValueGeneratedOnAdd();

            builder.Property(pi => pi.ImagePath).IsRequired().HasMaxLength(500);
            builder.Property(pi => pi.Caption).HasMaxLength(255);
            builder.Property(pi => pi.IsPrimary).IsRequired().HasDefaultValue(false);
            builder.Property(pi => pi.DisplayOrder).IsRequired().HasDefaultValue(0);

            builder.HasOne(pi => pi.Part)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PartId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If Part deleted, its images are deleted

            // Business rule: Only one IsPrimary = true per PartId.
            // This is hard to enforce with a simple DB constraint across all DBs.
            // Best handled by service layer logic (e.g., when setting one primary, unset others).
            // For SQL Server, a filtered unique index could be:
            // builder.HasIndex(pi => new { pi.PartId, pi.IsPrimary })
            //        .IsUnique()
            //        .HasFilter("[IsPrimary] = 1");
            // For SQLite, this filtered index isn't directly supported.
        }
    }
}
