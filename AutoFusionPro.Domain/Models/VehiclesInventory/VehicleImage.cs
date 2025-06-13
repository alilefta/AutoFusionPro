using AutoFusionPro.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Domain.Models.VehiclesInventory
{
    public class VehicleImage : BaseEntity
    {
        [Required]
        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        [Required]
        [StringLength(500)] // Path or URL
        public string ImagePath { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Caption { get; set; }
        public bool IsPrimary { get; set; } // To mark one image as the main display image
        public int DisplayOrder { get; set; }
    }

}
