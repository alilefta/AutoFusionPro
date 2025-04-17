using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Payment : BaseEntity
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }

        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
