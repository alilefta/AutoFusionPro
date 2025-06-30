using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartCompatibilityRuleBodyTypeConfiguration : IEntityTypeConfiguration<PartCompatibilityRuleBodyType>
    {
        public void Configure(EntityTypeBuilder<PartCompatibilityRuleBodyType> builder)
        {
            builder.ToTable("PartCompatibilityRuleBodyTypes");

            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id).ValueGeneratedOnAdd();

            builder.HasIndex(rt => new { rt.PartCompatibilityRuleId, rt.BodyTypeId }).IsUnique();

            // Foreign Key to PartCompatibilityRule
            builder.HasOne(rt => rt.PartCompatibilityRule)
                .WithMany(r => r.ApplicableBodyTypes)
                .HasForeignKey(rt => rt.PartCompatibilityRuleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign Key to BodyType
            builder.HasOne(rt => rt.BodyType)
                .WithMany()
                .HasForeignKey(rt => rt.BodyTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
