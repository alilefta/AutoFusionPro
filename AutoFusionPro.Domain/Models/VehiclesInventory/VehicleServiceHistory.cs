using AutoFusionPro.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoFusionPro.Domain.Models.VehiclesInventory
{
    public class VehicleServiceHistory : BaseEntity
    {
        [Required]
        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        public DateTime ServiceDate { get; set; }
        public int? MileageAtService { get; set; } // Mileage when service was done

        [Required]
        [StringLength(500)]
        public string ServiceDescription { get; set; } = string.Empty; // e.g., "Oil Change", "Brake Pad Replacement"

        [StringLength(255)]
        public string? ServiceProviderName { get; set; } // Name of the garage/mechanic

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cost { get; set; }
        public string? Notes { get; set; }
    }

}
