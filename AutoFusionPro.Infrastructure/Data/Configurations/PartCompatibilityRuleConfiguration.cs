using AutoFusionPro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartCompatibilityRuleConfiguration : IEntityTypeConfiguration<PartCompatibilityRule>
    {
        public void Configure(EntityTypeBuilder<PartCompatibilityRule> builder)
        {
            builder.ToTable("PartCompatibilityRules");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.Description).HasMaxLength(500);
            builder.Property(r => r.Notes).HasMaxLength(1000);

            // Foreign Key to Part
            builder.HasOne(r => r.Part)
                .WithMany(p => p.CompatibilityRules)
                .HasForeignKey(r => r.PartId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If Part is deleted, its compatibility rules are deleted

            // Foreign Keys to Taxonomy (Make, Model) - Optional relationships
            builder.HasOne(r => r.Make)
                .WithMany() // Assuming Make doesn't have a direct collection of PartCompatibilityRules
                .HasForeignKey(r => r.MakeId)
                .IsRequired(false) // Make can be null (rule applies to any make)
                .OnDelete(DeleteBehavior.SetNull); // If Make is deleted, set MakeId in rule to null

            builder.HasOne(r => r.Model)
                .WithMany() // Assuming Model doesn't have a direct collection of PartCompatibilityRules
                .HasForeignKey(r => r.ModelId)
                .IsRequired(false) // Model can be null
                .OnDelete(DeleteBehavior.SetNull); // If Model is deleted, set ModelId in rule to null


            builder.HasMany(r => r.ApplicableTrimLevels)
                .WithOne(atl => atl.PartCompatibilityRule) // Assuming PartCompatibilityRuleTrimLevel has this nav prop
                .HasForeignKey(atl => atl.PartCompatibilityRuleId)
                .OnDelete(DeleteBehavior.Cascade); // THIS IS KEY

            builder.HasMany(r => r.ApplicableEngineTypes)
                .WithOne(aet => aet.PartCompatibilityRule)
                .HasForeignKey(aet => aet.PartCompatibilityRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.ApplicableTransmissionTypes)
                .WithOne(aet => aet.PartCompatibilityRule)
                .HasForeignKey(aet => aet.PartCompatibilityRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.ApplicableBodyTypes)
                .WithOne(aet => aet.PartCompatibilityRule)
                .HasForeignKey(aet => aet.PartCompatibilityRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Year range properties (YearStart, YearEnd) are simple integers, configured by default.
            // Nullable int? will be handled correctly.

            // IsActive, IsTemplate are simple booleans.
            // CopiedFromRuleId is a nullable int.

            // Relationships to junction tables for applicable attributes
            // These are configured from the junction table side (WithOne PartCompatibilityRule)
            // builder.HasMany(r => r.ApplicableTrimLevels)... is already implied by the junction table config.

            // Consider an index if you often query rules by MakeId and/or ModelId
            builder.HasIndex(r => r.MakeId);
            builder.HasIndex(r => r.ModelId);
            builder.HasIndex(r => r.PartId); // Already an FK, but explicit index can be beneficial
            builder.HasIndex(r => r.IsTemplate); // If you query for templates often
        }
    }
}
