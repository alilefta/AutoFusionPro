using AutoFusionPro.Domain.Models.VehiclesInventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations.VehicleInventory
{
    public class VehicleDocumentConfiguration : IEntityTypeConfiguration<VehicleDocument>
    {
        public void Configure(EntityTypeBuilder<VehicleDocument> builder)
        {
            builder.ToTable("VehicleDocuments");
            builder.HasKey(vd => vd.Id);
            builder.Property(vd => vd.Id).ValueGeneratedOnAdd();

            builder.Property(vd => vd.DocumentName).IsRequired().HasMaxLength(100);
            builder.Property(vd => vd.DocumentType).IsRequired().HasConversion<string>().HasMaxLength(50);
            builder.Property(vd => vd.FilePath).IsRequired().HasMaxLength(500);

            // Relationship to Vehicle (configured in VehicleConfiguration)
        }
    }
}
