using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.UI.ViewModels.Parts.DesignTimeViewModels
{
    /// <summary>
    /// Provides sample Part data for use in the XAML designer (d:DesignInstance).
    /// </summary>
    public class DesignTimePartsViewModel
    {
        /// <summary>
        /// A collection of sample Part objects for the designer.
        /// The runtime ItemsSource should bind to a similar property on the actual ViewModel.
        /// </summary>
        public List<Part> Parts { get; set; }

        /// <summary>
        /// Parameterless constructor required by d:DesignInstance.
        /// Populates the Parts collection with sample data.
        /// </summary>
        public DesignTimePartsViewModel()
        {
            // Create sample Category objects if your UI binds to Category.Name
            // If not, you can set Category = null or CategoryId directly.
            var sampleCategoryEngine = new Category { Id = 1, Name = "Engine Components" };
            var sampleCategoryBrakes = new Category { Id = 2, Name = "Braking System" };
            var sampleCategoryFilters = new Category { Id = 3, Name = "Filters" };

            Parts = new List<Part>
            {
                new Part
                {
                    Id = 1, // BaseEntity property
                    PartNumber = "ENG-SPK-001",
                    Name = "Spark Plug NGK BKR6E",
                    Description = "Standard copper core spark plug for various models.",
                    Manufacturer = "NGK",
                    CostPrice = 2.50m,
                    SellingPrice = 5.99m,
                    StockQuantity = 120, // Keeping this, although CurrentStock is also present
                    ReorderLevel = 20,
                    CurrentStock = 120, // This is likely what the UI should primarily show
                    MinimumStock = 10,
                    Location = "Shelf A1",
                    IsActive = true,
                    IsOriginal = true,
                    ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/Photos/Avatars/Avatar1.png", // Example Pack URI
                    Notes = "Commonly used in Toyota Corolla 2005-2010.",
                    LastRestockDate = DateTime.Now.AddDays(-10),
                    CreatedAt = DateTime.Now.AddMonths(-1), // BaseEntity property
                    CategoryId = sampleCategoryEngine.Id,
                    Category = sampleCategoryEngine, // Assign object if binding to Category.Name
                    // Initialize collections to avoid null reference issues in designer templates if accessed
                    CompatibleVehicles = new List<PartCompatibility>(),
                    Suppliers = new List<SupplierPart>(),
                    OrderItems = new List<OrderItem>(),
                    InvoiceItems = new List<InvoiceItem>(),
                    InventoryTransactions = new List<InventoryTransaction>()
                },
                new Part
                {
                    Id = 2,
                    PartNumber = "BRK-PAD-015",
                    Name = "Front Brake Pads - Brembo P24",
                    Description = "High-performance ceramic front brake pads.",
                    Manufacturer = "Brembo",
                    CostPrice = 45.00m,
                    SellingPrice = 89.95m,
                    StockQuantity = 35,
                    ReorderLevel = 10,
                    CurrentStock = 35,
                    MinimumStock = 5,
                    Location = "Shelf B3",
                    IsActive = true,
                    IsOriginal = false, // Aftermarket example
                    ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/Photos/Avatars/Avatar1.png", // Example Pack URI
                    Notes = "Fits Honda Civic 2016+.",
                    LastRestockDate = DateTime.Now.AddDays(-5),
                    CreatedAt = DateTime.Now.AddMonths(-2),
                    CategoryId = sampleCategoryBrakes.Id,
                    Category = sampleCategoryBrakes,
                    CompatibleVehicles = new List<PartCompatibility>(),
                    Suppliers = new List<SupplierPart>(),
                    OrderItems = new List<OrderItem>(),
                    InvoiceItems = new List<InvoiceItem>(),
                    InventoryTransactions = new List<InventoryTransaction>()
                },
                new Part
                {
                    Id = 3,
                    PartNumber = "FIL-OIL-101",
                    Name = "Oil Filter - Mann W712/82",
                    Description = "Standard spin-on oil filter.",
                    Manufacturer = "Mann-Filter",
                    CostPrice = 4.00m,
                    SellingPrice = 9.50m,
                    StockQuantity = 0, // Out of stock example
                    ReorderLevel = 15,
                    CurrentStock = 0,
                    MinimumStock = 5,
                    Location = "Shelf C2",
                    IsActive = true,
                    IsOriginal = true,
                    ImagePath = string.Empty, // Example with no image path
                    Notes = "",
                    LastRestockDate = DateTime.Now.AddDays(-45), // Older restock
                    CreatedAt = DateTime.Now.AddDays(-50),
                    CategoryId = sampleCategoryFilters.Id,
                    Category = sampleCategoryFilters,
                    CompatibleVehicles = new List<PartCompatibility>(),
                    Suppliers = new List<SupplierPart>(),
                    OrderItems = new List<OrderItem>(),
                    InvoiceItems = new List<InvoiceItem>(),
                    InventoryTransactions = new List<InventoryTransaction>()
                },
                 new Part
                {
                    Id = 4,
                    PartNumber = "FIL-AIR-225",
                    Name = "Air Filter - Fram CA101",
                    Description = "Panel air filter.",
                    Manufacturer = "Fram",
                    CostPrice = 8.00m,
                    SellingPrice = 15.00m,
                    StockQuantity = 8, // Low stock example
                    ReorderLevel = 10,
                    CurrentStock = 8,
                    MinimumStock = 5,
                    Location = "Shelf C3",
                    IsActive = false, // Inactive example
                    IsOriginal = false,
                    ImagePath = null, // Example with null image path
                    Notes = "Superseded by CA101X",
                    LastRestockDate = DateTime.Now.AddDays(-90),
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    CategoryId = sampleCategoryFilters.Id,
                    Category = sampleCategoryFilters,
                    CompatibleVehicles = new List<PartCompatibility>(),
                    Suppliers = new List<SupplierPart>(),
                    OrderItems = new List<OrderItem>(),
                    InvoiceItems = new List<InvoiceItem>(),
                    InventoryTransactions = new List<InventoryTransaction>()
                }
                // Add more sample parts as needed
            };
        }


        public class Part : AutoFusionPro.Domain.Models.Part
        {

        }
    }
}
