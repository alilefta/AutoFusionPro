using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    public class Make : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., Toyota, Ford, Nissan
        public string? ImagePath { get; set; } = string.Empty;

        // Navigation Property: A Make can have many Models
        public virtual ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
