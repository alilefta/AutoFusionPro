using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    public class EngineType : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., 2.5L I4 Gas, 3.5L V6 EcoBoost, Electric 75kWh
        public string? Code { get; set; } // Optional engine code

        // Navigation Property: An EngineType can be used in many CompatibleVehicle specs
        public virtual ICollection<CompatibleVehicle> CompatibleVehicleSpecs { get; set; } = new List<CompatibleVehicle>();
    }
}
