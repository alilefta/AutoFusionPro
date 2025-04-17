using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string PaymentTerms { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Notes { get; set; }

        public virtual ICollection<SupplierPart> Parts { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
