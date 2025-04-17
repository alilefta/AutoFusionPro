using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsWholesale { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
