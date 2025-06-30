using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Models.PartCompatibilityRules
{
    public class PartCompatibilityRuleBodyType : BaseEntity
    {
        public int PartCompatibilityRuleId { get; set; }
        public virtual PartCompatibilityRule PartCompatibilityRule { get; set; } = null!;

        public int BodyTypeId { get; set; }
        public virtual BodyType BodyType { get; set; } = null!;

        public bool IsExclusion { get; set; } = false;
    }
}
