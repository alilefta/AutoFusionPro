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
        string CategoryName,
        string Manufacturer,
        decimal SellingPrice,
        int CurrentStock, // Display current stock level
        bool IsActive
    );

    /// <summary>
    /// DTO representing a vehicle compatibility link for display.
    /// </summary>
    public record PartCompatibilityDto(
        int Id, // The ID of the PartCompatibility record itself
        int VehicleId,
        string VehicleMake,
        string VehicleModel,
        int VehicleYear,
        string? Notes
    );

    /// <summary>
    /// DTO representing a supplier link for a part for display.
    /// </summary>
    public record PartSupplierDto(
        int Id, // The ID of the SupplierPart record itself
        int SupplierId,
        string SupplierName,
        string? SupplierPartNumber, // The part number used by this specific supplier
        decimal Cost, // The cost from this specific supplier
        bool IsPreferredSupplier
    );

    /// <summary>
    /// DTO holding detailed information for a single part, suitable for view/edit forms.
    /// </summary>
    public class PartDetailDto // Using class for potential future complexity/observability needs
    {
        public int Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int ReorderLevel { get; set; }
        public int MinimumStock { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsOriginal { get; set; }
        public string? ImagePath { get; set; }
        public string? Notes { get; set; }

        // Category Info
        // TODO: Change to Category Model
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Read-only display info
        public int CurrentStock { get; set; }
        public DateTime LastRestockDate { get; set; }

        // Related Collections
        public IEnumerable<PartCompatibilityDto> CompatibleVehicles { get; set; } = Enumerable.Empty<PartCompatibilityDto>();
        public IEnumerable<PartSupplierDto> Suppliers { get; set; } = Enumerable.Empty<PartSupplierDto>();
    }

    /// <summary>
    /// DTO for adding a new vehicle compatibility link.
    /// </summary>
    public record PartCompatibilityCreateDto(
        int VehicleId,
        int PartId,
        string? Notes
    );

    /// <summary>
    /// DTO for adding a new supplier link to a part.
    /// </summary>
    public record PartSupplierCreateDto(
        int SupplierId,
        string? SupplierPartNumber,
        decimal Cost,
        int LeadTimeInDays,
        int MinimumOrderQuantity,
        bool IsPreferredSupplier
    );

    /// <summary>
    /// DTO containing data required to create a new part.
    /// </summary>
    public record CreatePartDto(
        // Use Validation attributes if desired (FluentValidation is often preferred in Application layer)
        [Required(AllowEmptyStrings = false)] string PartNumber,
        [Required(AllowEmptyStrings = false)] string Name,
        string Description,
        string Manufacturer,
        [Range(0, double.MaxValue)] decimal CostPrice,
        [Range(0, double.MaxValue)] decimal SellingPrice,
        [Range(0, int.MaxValue)] int ReorderLevel,
        [Range(0, int.MaxValue)] int MinimumStock,
        string Location,
        bool IsActive,
        bool IsOriginal,
        string? ImagePath,
        string? Notes,
        [Range(1, int.MaxValue)] int CategoryId // Category is required
                                                 // Initial relationships (optional, could be added via separate calls)
        //IEnumerable<PartCompatibilityCreateDto>? InitialCompatibleVehicles,
        //IEnumerable<PartSupplierCreateDto>? InitialSuppliers
    );

    /// <summary>
    /// DTO containing data required to update an existing part.
    /// Compatibility and Suppliers might be updated via separate service calls.
    /// </summary>
    public record UpdatePartDto(
        [Range(1, int.MaxValue)] int Id, // ID of the part to update
        [Required(AllowEmptyStrings = false)] string PartNumber,
        [Required(AllowEmptyStrings = false)] string Name,
        string Description,
        string Manufacturer,
        [Range(0, double.MaxValue)] decimal CostPrice,
        [Range(0, double.MaxValue)] decimal SellingPrice,
        [Range(0, int.MaxValue)] int ReorderLevel,
        [Range(0, int.MaxValue)] int MinimumStock,
        string Location,
        bool IsActive,
        bool IsOriginal,
        string? ImagePath, // Allow updating image path
        string? Notes,
        [Range(1, int.MaxValue)] int CategoryId
    );

    /// <summary>
    /// DTO encapsulating criteria for filtering parts lists.
    /// Use null-able types to indicate a filter is not applied.
    /// </summary>
    public record PartFilterCriteriaDto(
        string? SearchTerm = null,
        int? CategoryId = null,
        string? Manufacturer = null,
        int? SupplierId = null,
        int? VehicleId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        StockStatusFilter? StockStatus = null, // Example using an enum
        bool? IsActive = null, // True for active, False for inactive, Null for both
        bool? IsOriginal = null // True for original, False for after-market, Null for both
    );
}
