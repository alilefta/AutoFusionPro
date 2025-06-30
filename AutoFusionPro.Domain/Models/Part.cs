using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Part : BaseEntity
    {
        public string PartNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Manufacturer { get; set; } = string.Empty;

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }

        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }

        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsOriginal { get; set; } = false;
        //public string? ImagePath { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;

        public string? Barcode { get; set; } = string.Empty;

        public DateTime? LastRestockDate { get; set; }

        // Navigation properties
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;




        // --- Unit of Measure Fields ---
        public int StockingUnitOfMeasureId { get; set; } // The base unit in which stock is counted and valued
        public virtual UnitOfMeasure StockingUnitOfMeasure { get; set; } = null!;

        public int? SalesUnitOfMeasureId { get; set; }    // Unit in which it's typically sold (can be same as stocking)
        public virtual UnitOfMeasure? SalesUnitOfMeasure { get; set; }
        public decimal? SalesConversionFactor { get; set; } // e.g., If StockingUoM is Liter and SalesUoM is Milliliter, factor is 1000.
                                                            // Factor to convert SalesUoM back to StockingUoM (StockingUoM = SalesUoM_Qty * SalesConversionFactor)
                                                            // Or, more commonly: How many StockingUoM are in one SalesUoM.
                                                            // Let's redefine: QuantityInBaseStockingUnitPerSalesUnit (e.g. 0.5 if SalesUnit is 500ml and StockingUnit is Liter)
                                                            // OR: How many Sales Units are in one Stocking Unit.
                                                            // For simplicity: SalesUnitsPerStockingUnit (e.g., if Stocking=Drum(20L), Sales=Bottle(1L), Factor=20)

        public int? PurchaseUnitOfMeasureId { get; set; } // Unit in which it's typically purchased
        public virtual UnitOfMeasure? PurchaseUnitOfMeasure { get; set; }
        public decimal? PurchaseConversionFactor { get; set; } // e.g., If StockingUoM is Piece and PurchaseUoM is Box of 10, factor is 10.
                                                               // (How many StockingUoM are in one PurchaseUoM)


        // public virtual ICollection<PartCompatibility> CompatibleVehicles { get; set; } = new List<PartCompatibility>();
        public virtual ICollection<PartCompatibilityRule> CompatibilityRules { get; set; } = new List<PartCompatibilityRule>();

        public virtual ICollection<SupplierPart> Suppliers { get; set; } = new List<SupplierPart>(); // Links Part to specific Suppliers with details
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Sales Order Items
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        public virtual ICollection<PurchaseItem> PartPurchaseItems { get; set; } = new List<PurchaseItem>();

        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

        public virtual ICollection<PartImage> Images { get; set; } = new List<PartImage>();

    }
}
