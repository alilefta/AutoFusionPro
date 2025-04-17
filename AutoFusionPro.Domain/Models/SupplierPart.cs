using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class SupplierPart : BaseEntity
    {
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public int PartId { get; set; }
        public virtual Part Part { get; set; }

        public string SupplierPartNumber { get; set; }
        public decimal Cost { get; set; }
        public int LeadTimeInDays { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public bool IsPreferredSupplier { get; set; }
    }
}
