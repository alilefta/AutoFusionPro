using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class TrimLevelConfiguration : IEntityTypeConfiguration<TrimLevel>
    {
        public void Configure(EntityTypeBuilder<TrimLevel> builder)
        {
            builder.ToTable("TrimLevels");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Unique constraint for Trim Name within a Model
            builder.HasIndex(t => new { t.ModelId, t.Name }).IsUnique();

            // Relationship: Many TrimLevels belong to One Model (already defined in ModelConfiguration)
        }
    }
}
