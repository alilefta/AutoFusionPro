using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class PartCompatibility : BaseEntity
    {
        public int PartId { get; set; }
        public virtual Part Part { get; set; }

        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public string Notes { get; set; }
    }
}
