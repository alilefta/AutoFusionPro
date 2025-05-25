using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    public class Model : BaseEntity // Represents a vehicle model line
    {
        public string Name { get; set; } = string.Empty; // e.g., Camry, F-150, Altima

        public int MakeId { get; set; } // Foreign Key
        public virtual Make Make { get; set; } = null!; // Navigation to parent Make
        public string? ImagePath { get; set; } = string.Empty;

        // Navigation Property: A Model can have many TrimLevels
        public virtual ICollection<TrimLevel> TrimLevels { get; set; } = new List<TrimLevel>();

        // Navigation Property: A Model (across years/trims) can be part of many CompatibleVehicle specs
        public virtual ICollection<CompatibleVehicle> CompatibleVehicleSpecs { get; set; } = new List<CompatibleVehicle>();
    }
}
