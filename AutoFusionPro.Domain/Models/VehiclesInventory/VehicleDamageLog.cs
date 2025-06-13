using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using AutoFusionPro.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoFusionPro.Domain.Models.VehiclesInventory
{
    public class VehicleDamageLog : BaseEntity
    {
        [Required]
        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        public DateTime DateNoted { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty; // e.g., "Dent on driver side door", "Scratch on hood"

        public DamageSeverity Severity { get; set; } // Enum: Minor, Moderate, Major
        public bool IsRepaired { get; set; } = false;
        public DateTime? RepairedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedRepairCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualRepairCost { get; set; }

        public string? RepairNotes { get; set; }
        public virtual ICollection<VehicleDamageImage> DamageImages { get; set; } = new List<VehicleDamageImage>(); // Images specific to this damage
    }
}
