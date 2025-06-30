using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    public class BodyType : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., Sedan, SUV, Coupe, Hatchback, Truck

        // You can add navigation properties for these junction tables if you need to navigate from TrimLevel
        // to all PartCompatibilityRuleBodyType records it's part of, but it's often not required.
        // public virtual ICollection<PartCompatibilityRuleBodyType> PartCompatibilityRuleLinks { get; set; } = new List<PartCompatibilityRuleBodyType>();

    }
}
