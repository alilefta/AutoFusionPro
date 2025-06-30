using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Configurations
{
    public class PartCompatibilityRuleTransmissionTypeConfiguration : IEntityTypeConfiguration<PartCompatibilityRuleTransmissionType>
    {
        public void Configure(EntityTypeBuilder<PartCompatibilityRuleTransmissionType> builder)
        {
            builder.ToTable("PartCompatibilityRuleTransmissionTypes");

            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id).ValueGeneratedOnAdd();

            builder.HasIndex(rt => new { rt.PartCompatibilityRuleId, rt.TransmissionTypeId }).IsUnique();

            // Foreign Key to PartCompatibilityRule
            builder.HasOne(rt => rt.PartCompatibilityRule)
                .WithMany(r => r.ApplicableTransmissionTypes)
                .HasForeignKey(rt => rt.PartCompatibilityRuleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign Key to TransmissionType
            builder.HasOne(rt => rt.TransmissionType)
                .WithMany()
                .HasForeignKey(rt => rt.TransmissionTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
