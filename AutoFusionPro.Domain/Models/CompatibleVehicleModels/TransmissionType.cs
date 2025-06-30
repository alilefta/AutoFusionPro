using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    public class TransmissionType : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., Automatic 8-Speed, Manual 6-Speed, CVT
    }
}
