using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class MakeConfiguration : IEntityTypeConfiguration<Make>
    {
        public void Configure(EntityTypeBuilder<Make> builder)
        {
            builder.ToTable("Makes");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(m => m.Name).IsUnique(); // Make names should be unique

            // Relationship: One Make has Many Models
            builder.HasMany(m => m.Models)
                .WithOne(mo => mo.Make)
                .HasForeignKey(mo => mo.MakeId)
                .OnDelete(DeleteBehavior.Cascade); // If a Make is deleted, its Models are also deleted
        }
    }
}
