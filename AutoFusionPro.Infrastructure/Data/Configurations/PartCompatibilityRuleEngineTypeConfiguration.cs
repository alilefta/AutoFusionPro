using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartCompatibilityRuleEngineTypeConfiguration : IEntityTypeConfiguration<PartCompatibilityRuleEngineType>
    {
        public void Configure(EntityTypeBuilder<PartCompatibilityRuleEngineType> builder)
        {
            builder.ToTable("PartCompatibilityRuleEngineTypes");

            builder.HasKey(rt => rt.Id); 
            builder.Property(rt => rt.Id).ValueGeneratedOnAdd();

            builder.HasIndex(rt => new { rt.PartCompatibilityRuleId, rt.EngineTypeId }).IsUnique();

            builder.HasOne(rt => rt.PartCompatibilityRule)
                .WithMany(r => r.ApplicableEngineTypes)
                .HasForeignKey(rt => rt.PartCompatibilityRuleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign Key to EngineType
            builder.HasOne(rt => rt.EngineType)
                .WithMany()
                .HasForeignKey(rt => rt.EngineTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
