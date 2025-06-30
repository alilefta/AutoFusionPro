using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Models.PartCompatibilityRules
{
    public class PartCompatibilityRuleTransmissionType : BaseEntity
    {
        public int PartCompatibilityRuleId { get; set; }
        public virtual PartCompatibilityRule PartCompatibilityRule { get; set; } = null!;

        public int TransmissionTypeId { get; set; }
        public virtual TransmissionType TransmissionType { get; set; } = null!;

        public bool IsExclusion { get; set; } = false;
    }
}
