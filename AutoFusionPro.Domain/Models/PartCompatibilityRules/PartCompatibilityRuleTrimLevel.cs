// No BaseEntity needed if using composite key and no other audit fields on the junction itself.
// If you need CreatedAt/ModifiedAt on the link itself, then inherit BaseEntity and give it its own Id.
using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Models.PartCompatibilityRules
{
    public class PartCompatibilityRuleTrimLevel : BaseEntity // No BaseEntity if just a pure junction
    {
        public int PartCompatibilityRuleId { get; set; }
        public virtual PartCompatibilityRule PartCompatibilityRule { get; set; } = null!;

        public int TrimLevelId { get; set; }
        public virtual TrimLevel TrimLevel { get; set; } = null!;

        public bool IsExclusion { get; set; } = false; // True if this rule EXCLUDES this trim level
    }
}