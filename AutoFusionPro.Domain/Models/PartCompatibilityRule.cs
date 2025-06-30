using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Domain.Models
{
    public class PartCompatibilityRule : BaseEntity
    {
        [Required]
        public int PartId { get; set; } // The Part this rule applies to
        public virtual Part Part { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty; // User-friendly name for this rule, e.g., "Nissan Altima 2019-2022 (S, SV)"

        [StringLength(500)]
        public string? Description { get; set; } // Optional further description of the rule

        // --- Core Vehicle Filters ---
        public int? MakeId { get; set; } // Nullable: If null, this rule applies to any make (if ModelId is also null)
        public virtual Make? Make { get; set; }

        public int? ModelId { get; set; } // Nullable: If null (and MakeId is set), applies to any model of that Make
        public virtual Model? Model { get; set; }

        // Year Range: Both should be set for a range, or both null for "any year".
        // Or, use specific start/end if rule is for a fixed range.
        // For "Any Year", both could be null, or use sentinel values like 0 or Min/Max if nulls are problematic for queries.
        // Let's assume YearStart/YearEnd define the applicable range.
        // If a part fits ALL years of a Model, you might omit these or use very broad values.
        public int? YearStart { get; set; } // e.g., 2019. If null, implies from beginning of model or no year restriction.
        public int? YearEnd { get; set; }   // e.g., 2022. If null, implies up to end of model or no year restriction.

        // --- Collections for Specific Attribute Selections (Many-to-Many via Junction Tables) ---
        // If a collection is empty, it means "ANY" for that attribute type (given the Make/Model/Year constraints).
        // If a collection has items, the rule applies ONLY to those specific items.
        public virtual ICollection<PartCompatibilityRuleTrimLevel> ApplicableTrimLevels { get; set; } = new List<PartCompatibilityRuleTrimLevel>();
        public virtual ICollection<PartCompatibilityRuleEngineType> ApplicableEngineTypes { get; set; } = new List<PartCompatibilityRuleEngineType>();
        public virtual ICollection<PartCompatibilityRuleTransmissionType> ApplicableTransmissionTypes { get; set; } = new List<PartCompatibilityRuleTransmissionType>();
        public virtual ICollection<PartCompatibilityRuleBodyType> ApplicableBodyTypes { get; set; } = new List<PartCompatibilityRuleBodyType>();

        [StringLength(1000)]
        public string? Notes { get; set; } // General notes for this entire rule

        public bool IsActive { get; set; } = true; // Rule can be disabled

        // Optional: For future template system
        public bool IsTemplate { get; set; } = false; // If this rule can be used as a template for others
        public int? CopiedFromRuleId { get; set; } // If this rule was copied from another rule/template
    }
}
