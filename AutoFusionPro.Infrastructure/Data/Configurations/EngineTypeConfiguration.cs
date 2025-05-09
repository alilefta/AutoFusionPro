using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class EngineTypeConfiguration : IEntityTypeConfiguration<EngineType>
    {
        public void Configure(EntityTypeBuilder<EngineType> builder)
        {
            builder.ToTable("EngineTypes");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);
            builder.HasIndex(e => e.Name).IsUnique(); // Engine type names should be unique

            builder.Property(e => e.Code)
                .HasMaxLength(50);

            // Corrected: Removed .IsClustered(false) for SQLite compatibility
            // If Code is not the INTEGER PRIMARY KEY, this index will be non-clustered by default in SQLite.
            // If Code *is* the INTEGER PRIMARY KEY, it becomes the alias for ROWID and is clustered.
            // Assuming 'Id' is the primary key here.
            builder.HasIndex(e => e.Code).IsUnique(); // Optional: unique engine code
        }
    }
}
