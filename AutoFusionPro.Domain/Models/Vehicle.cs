using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Vehicle : BaseEntity
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Engine { get; set; } = string.Empty;
        public string? Transmission { get; set; } = string.Empty;
        public string? VIN { get; set; } = string.Empty;

        public string? TrimLevel { get; set; } = string.Empty; // SE, Limited, ...
        public string? BodyType { get; set; } // e.g., "Sedan", "SUV"

        public virtual ICollection<PartCompatibility> CompatibleParts { get; set; }
    }
}
