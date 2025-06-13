using AutoFusionPro.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Domain.Models.VehiclesInventory
{
    public class VehicleDamageImage : BaseEntity // Similar to VehicleImage but links to VehicleDamageLog
    {
        [Required]
        public int VehicleDamageLogId { get; set; }
        public virtual VehicleDamageLog VehicleDamageLog { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string ImagePath { get; set; } = string.Empty;
        [StringLength(255)]
        public string? Caption { get; set; }
    }
}
