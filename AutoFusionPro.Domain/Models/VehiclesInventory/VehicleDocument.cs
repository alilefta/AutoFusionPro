using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using AutoFusionPro.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Domain.Models.VehiclesInventory
{
    public class VehicleDocument : BaseEntity // For Registration, Insurance, etc.
    {
        [Required]
        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DocumentName { get; set; } // e.g., "Registration Q1 2024", "Insurance Policy 2024"

        [StringLength(500)] // Add if you want notes for the document record
        public string? Notes { get; set; }

        public DocumentType DocumentType { get; set; } // Enum: Registration, Insurance, ServiceRecordScan, Other

        [Required]
        [StringLength(500)] // Path or URL to the scanned document
        public string FilePath { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
    }
}
