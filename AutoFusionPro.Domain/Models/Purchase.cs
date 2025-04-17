using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Purchase : BaseEntity
    {
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public PurchaseStatus Status { get; set; }
        public string Notes { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }

        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }
    }
}
