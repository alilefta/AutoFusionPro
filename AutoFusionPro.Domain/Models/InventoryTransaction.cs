using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class InventoryTransaction : BaseEntity
    {
        public DateTime TransactionDate { get; set; }
        public int PartId { get; set; }
        public virtual Part Part { get; set; }

        public int PreviousQuantity { get; set; }
        public int Quantity { get; set; }
        public int NewQuantity { get; set; }
        public InventoryTransactionType TransactionType { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }

        // Optional: For tracking who performed the transaction
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Optional: References to related documents
        public int? OrderId { get; set; }
        public int? InvoiceId { get; set; }
        public int? PurchaseId { get; set; }
    }
}
