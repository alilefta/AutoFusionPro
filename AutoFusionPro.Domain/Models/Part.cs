using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Part : BaseEntity
    {
        public string PartNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }

        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }

        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsOriginal { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public DateTime LastRestockDate { get; set; }

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
