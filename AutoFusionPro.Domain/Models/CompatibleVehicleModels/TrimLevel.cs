using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    public class TrimLevel : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., LE, XLT, SL, Sport

        public int ModelId { get; set; } // Foreign Key
        public virtual Model Model { get; set; } = null!; // Navigation to parent Model

        // Navigation Property: A TrimLevel can be used in many CompatibleVehicle specs
        public virtual ICollection<CompatibleVehicle> CompatibleVehicleSpecs { get; set; } = new List<CompatibleVehicle>();
    }
}
