using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Core.Enums.DTOEnums;
using AutoFusionPro.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.Part
{
    /// <summary>
    /// DTO for displaying essential part information in lists or grids.
    /// </summary>
    public record PartSummaryDto(
        int Id,
        string PartNumber,
        string Name,
        string CategoryName, // Good
        string? Manufacturer, // Make Manufacturer nullable if it's optional in domain
        decimal SellingPrice,
        int CurrentStock,
        string? StockingUnitOfMeasureSymbol,
        bool IsActive,
        string? ImagePath,
        string? Location
    );

    /// <summary>
    /// DTO representing a vehicle compatibility link for display.
    /// </summary>
    public record PartCompatibilityDto(
        int Id, // The ID of the PartCompatibility record itself
        int CompatibleVehicleId, // Renamed from VehicleId for clarity
        string VehicleMake,
        string VehicleModel,
        int VehicleYearStart, // Changed from VehicleYear to reflect range
        int VehicleYearEnd,   // Added for range
        string? TrimLevelName, // Added
        string? EngineTypeName, // Added
        string? TransmissionTypeName, // Added
        string? Notes
    );

    /// <summary>
    /// DTO representing a supplier link for a part for display.
    /// </summary>
    public record PartSupplierDto(
        int Id, // The ID of the SupplierPart record itself
        int SupplierId,
        string SupplierName,
        string? SupplierPartNumber,
        decimal Cost,
        bool IsPreferredSupplier,
        int LeadTimeInDays, // Added
        int MinimumOrderQuantity // Added

    );

    /// <summary>
    /// DTO holding detailed information for a single part, suitable for view/edit forms.
    /// </summary>
    public class PartDetailDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Manufacturer { get; set; } = string.Empty; // Made nullable
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; } // This often means initial or manually set base, CurrentStock is live
        public int ReorderLevel { get; set; }
        public int MinimumStock { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsOriginal { get; set; }
        public string? ImagePath { get; set; }
        public string? Notes { get; set; }
        public string? Barcode { get; set; }

        public int StockingUnitOfMeasureId { get; set; }
        public string StockingUnitOfMeasureName { get; set; } = string.Empty;
        public string StockingUnitOfMeasureSymbol { get; set; } = string.Empty;

        public int? SalesUnitOfMeasureId { get; set; }
        public string? SalesUnitOfMeasureName { get; set; }
        public string? SalesUnitOfMeasureSymbol { get; set; }
        public decimal? SalesConversionFactor { get; set; }

        public int? PurchaseUnitOfMeasureId { get; set; }
        public string? PurchaseUnitOfMeasureName { get; set; }
        public string? PurchaseUnitOfMeasureSymbol { get; set; }
        public decimal? PurchaseConversionFactor { get; set; }

        // Category Info
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Keep simple CategoryName for display
                                                                 // public CategoryDto Category { get; set; } // REMOVED: Avoid nesting full DTOs unless specifically needed for complex UI.
                                                                 // CategoryName is usually sufficient for part details.
                                                                 // If you need to select/change category, use CategorySelectionDto for dropdowns.

        // Live/Calculated Stock Info
        public int CurrentStock { get; set; } // This is the actual live stock
        public DateTime? LastRestockDate { get; set; } // Made nullable, as a new part might not have been restocked

        // Related Collections for display
        public List<PartCompatibilityDto> CompatibleVehicles { get; set; } = new(); // Initialize
        public List<PartSupplierDto> Suppliers { get; set; } = new();          // Initialize
    }

    /// <summary>
    /// DTO for adding a new vehicle compatibility link.
    /// </summary>
    public record PartCompatibilityCreateDto(
        [Range(1, int.MaxValue)] 
        int CompatibleVehicleId, // The ID of the CompatibleVehicle spec

        string? Notes
    );

    /// <summary>
    /// DTO for adding a new supplier link to a part.
    /// </summary>
    public record PartSupplierCreateDto(
        [Range(1, int.MaxValue)] int SupplierId,
        string? SupplierPartNumber,
        [Range(0, double.MaxValue)] decimal Cost,
        [Range(0, int.MaxValue)] int LeadTimeInDays,
        [Range(0, int.MaxValue)] int MinimumOrderQuantity,
        bool IsPreferredSupplier
    );

    /// <summary>
    /// DTO containing data required to create a new part.
    /// </summary>
    public record CreatePartDto(
         [Required(AllowEmptyStrings = false)] string PartNumber,
         [Required(AllowEmptyStrings = false)] string Name,
         string? Description,
         string? Manufacturer,
         [Range(0, double.MaxValue)] decimal CostPrice,
         [Range(0, double.MaxValue)] decimal SellingPrice,
         [Range(0, int.MaxValue)] int StockQuantity,     // Initial stock quantity or base quantity
         [Range(0, int.MaxValue)] int ReorderLevel,
         [Range(0, int.MaxValue)] int MinimumStock,
         string? Location,
         bool IsActive,
         bool IsOriginal,
         string? ImagePath, // Source path from client for NEW image
         string? Notes,
         string? Barcode, // Added
         [Range(1, int.MaxValue)] int CategoryId,

         [Range(1, int.MaxValue)] int StockingUnitOfMeasureId,
         int? SalesUnitOfMeasureId,
         decimal? SalesConversionFactor,
         int? PurchaseUnitOfMeasureId,
         decimal? PurchaseConversionFactor,

         // For initial creation, it's often better to add suppliers/compatibility in separate steps/calls
         // after the part is created and has an ID.
         // If you MUST include them on creation:
         List<PartSupplierCreateDto>? InitialSuppliers = null,
         List<PartCompatibilityCreateDto>? InitialCompatibleVehicles = null
     );

    /// <summary>
    /// DTO containing data required to update an existing part.
    /// Compatibility and Suppliers might be updated via separate service calls.
    /// </summary>
    public record UpdatePartDto(
         [Range(1, int.MaxValue)] int Id,
         [Required(AllowEmptyStrings = false)] string PartNumber,
         [Required(AllowEmptyStrings = false)] string Name,
         string? Description,
         string? Manufacturer,
         [Range(0, double.MaxValue)] decimal CostPrice,
         [Range(0, double.MaxValue)] decimal SellingPrice,
         [Range(0, int.MaxValue)] int StockQuantity,
         [Range(0, int.MaxValue)] int ReorderLevel,
         [Range(0, int.MaxValue)] int MinimumStock,
         string? Location,
         bool IsActive,
         bool IsOriginal,
         string? ImagePath, // Source path from client if image changed, existing path if not, null to clear
         string? Notes,
         string? Barcode, // Added
         [Range(1, int.MaxValue)] int CategoryId,
         [Range(1, int.MaxValue)] int StockingUnitOfMeasureId,
         int? SalesUnitOfMeasureId,
         decimal? SalesConversionFactor,
         int? PurchaseUnitOfMeasureId,
         decimal? PurchaseConversionFactor
     // Supplier and Compatibility updates are typically handled by dedicated service methods
     // e.g., AddSupplierToPart, RemoveSupplierFromPart, UpdatePartSupplierLink, etc.
     // rather than trying to pass entire collections for update.
     );

    /// <summary>
    /// DTO encapsulating criteria for filtering parts lists.
    /// Use null-able types to indicate a filter is not applied.
    /// </summary>
    public record PartFilterCriteriaDto(
        string? SearchTerm = null,      // Searches PartNumber, Name, Description, Manufacturer, Barcode, CategoryName
        int? CategoryId = null,
        string? Manufacturer = null,    // Allow direct text filter for manufacturer
        int? SupplierId = null,         // To find parts supplied by a specific supplier
        int? CompatibleVehicleId = null, // To find parts compatible with a specific vehicle spec
        decimal? MinSellingPrice = null, // Renamed for clarity
        decimal? MaxSellingPrice = null, // Renamed for clarity
        StockStatusFilter? StockStatus = null,
        bool? IsActive = true,          // Default to true
        bool? IsOriginal = null,
        // For service layer, consider adding specific make/model/year etc. if direct joins are too complex for repo
        int? MakeId = null,
        int? ModelId = null,
        int? TrimId = null,
        int? SpecificYear = null // To find parts where compatiblevehicle.yearstart <= year <= compatiblevehicle.yearend
    );
}
