using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    public class BodyType : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., Sedan, SUV, Coupe, Hatchback, Truck

        // Navigation Property: A BodyType can be used in many CompatibleVehicle specs
        public virtual ICollection<CompatibleVehicle> CompatibleVehicleSpecs { get; set; } = new List<CompatibleVehicle>();
    }
}
