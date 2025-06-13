using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class ModelConfiguration : IEntityTypeConfiguration<Model>
    {
        public void Configure(EntityTypeBuilder<Model> builder)
        {
            builder.ToTable("Models");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Unique constraint for Model Name within a Make
            builder.HasIndex(m => new { m.MakeId, m.Name }).IsUnique();

            builder.HasMany(m => m.TrimLevels)
                .WithOne(t => t.Model) // Correct: TrimLevel has a 'Model' navigation property
                .HasForeignKey(t => t.ModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
