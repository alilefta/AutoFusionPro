using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Part : BaseEntity
    {
        public string PartNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public bool IsOriginal { get; set; }
        public string ImagePath { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<PartCompatibility> CompatibleVehicles { get; set; }
        public virtual ICollection<SupplierPart> Suppliers { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
    }
}
