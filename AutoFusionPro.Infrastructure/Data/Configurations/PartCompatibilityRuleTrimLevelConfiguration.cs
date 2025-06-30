using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartCompatibilityRuleTrimLevelConfiguration : IEntityTypeConfiguration<PartCompatibilityRuleTrimLevel>
    {
        public void Configure(EntityTypeBuilder<PartCompatibilityRuleTrimLevel> builder)
        {
            builder.ToTable("PartCompatibilityRuleTrimLevels");

            // If PartCompatibilityRuleTrimLevel inherits BaseEntity (has its own Id PK)
            builder.HasKey(rt => rt.Id); // Primary key from BaseEntity
            builder.Property(rt => rt.Id).ValueGeneratedOnAdd();

            // Unique constraint for the combination of Rule and TrimLevel
            // This ensures a trim level isn't linked multiple times (e.g., once as include, once as exclude)
            // to the same rule. If IsExclusion can differentiate, this might be too strict or needs IsExclusion in the index.
            // For SQLite, this unique index will allow multiple (RuleId, TrimId) if one has IsExclusion=true and another IsExclusion=false.
            // If you want (RuleId, TrimId) to be absolutely unique regardless of IsExclusion, then:
            builder.HasIndex(rt => new { rt.PartCompatibilityRuleId, rt.TrimLevelId }).IsUnique();

            // Foreign Key to PartCompatibilityRule
            builder.HasOne(rt => rt.PartCompatibilityRule)
                .WithMany(r => r.ApplicableTrimLevels)
                .HasForeignKey(rt => rt.PartCompatibilityRuleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If the rule is deleted, these links are deleted

            // Foreign Key to TrimLevel
            builder.HasOne(rt => rt.TrimLevel)
                .WithMany() // Assuming TrimLevel doesn't have ICollection<PartCompatibilityRuleTrimLevel>
                .HasForeignKey(rt => rt.TrimLevelId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a TrimLevel if it's used in a rule
        }
    }
}
