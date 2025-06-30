using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Models.PartCompatibilityRules
{
    public class PartCompatibilityRuleEngineType : BaseEntity
    {
        public int PartCompatibilityRuleId { get; set; }
        public virtual PartCompatibilityRule PartCompatibilityRule { get; set; } = null!;

        public int EngineTypeId { get; set; }
        public virtual EngineType EngineType { get; set; } = null!;

        public bool IsExclusion { get; set; } = false;
    }
}
