using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class PurchaseItem : BaseEntity
    {
        public int PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; } = null!; // Added null-forgiving

        public int PartId { get; set; }
        public virtual Part Part { get; set; } = null!; // Added null-forgiving

        public int QuantityOrdered { get; set; } // Renamed from Quantity
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }

        public int UnitOfMeasureId { get; set; } // FK to UnitOfMeasure in which it was purchased
        public virtual UnitOfMeasure UnitOfMeasure { get; set; } = null!;
    }
}
