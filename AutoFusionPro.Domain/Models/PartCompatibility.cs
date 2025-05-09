using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Models
{
    public class PartCompatibility : BaseEntity
    {
        public int PartId { get; set; }
        public virtual Part Part { get; set; } = null!;

        public int CompatibleVehicleId { get; set; } // Changed from VehicleId
        public virtual CompatibleVehicle CompatibleVehicle { get; set; } = null!; // Changed from Vehicle

        public string? Notes { get; set; } // Can store additional specific notes for this exact Part-VehicleConfig link
    }
}
