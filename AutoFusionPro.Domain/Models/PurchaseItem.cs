using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class PurchaseItem : BaseEntity
    {
        public int PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; }

        public int PartId { get; set; }
        public virtual Part Part { get; set; }

        public int Quantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }
}
