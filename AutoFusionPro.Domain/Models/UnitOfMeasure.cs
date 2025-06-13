using AutoFusionPro.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Domain.Models
{
    public class UnitOfMeasure : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } // e.g., "Piece", "Liter", "Meter", "Box", "Carton", "Pair"

        [Required]
        [StringLength(10)]
        public string Symbol { get; set; } // e.g., "pcs", "L", "m", "box", "ctn", "pr"

        public string? Description { get; set; }

        // Navigation property (optional, if you categorize UoMs e.g., by type like Weight, Volume, Count)
        // public int? UnitOfMeasureTypeId { get; set; }
        // public virtual UnitOfMeasureType UnitOfMeasureType { get; set; }

        // Parts that use this as their base stocking unit
        public virtual ICollection<Part> PartsStockingUnit { get; set; } = new List<Part>();
        // Parts that use this as their sales unit
        public virtual ICollection<Part> PartsSalesUnit { get; set; } = new List<Part>();
        // Parts that use this as their purchase unit
        public virtual ICollection<Part> PartsPurchaseUnit { get; set; } = new List<Part>();
    }
}
