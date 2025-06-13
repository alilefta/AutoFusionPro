using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public int PartId { get; set; }
        public virtual Part Part { get; set; } = null!;

        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal LineTotal { get; set; }

        public int QuantitySold { get; set; } // Renamed from Quantity for clarity
        public int UnitOfMeasureId { get; set; } // FK to UnitOfMeasure in which it was sold
        public virtual UnitOfMeasure UnitOfMeasure { get; set; } = null!;
    }
}
