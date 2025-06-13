using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for displaying a summary of a Vehicle asset, typically in lists or grids.
    /// </summary>
    public record VehicleAssetSummaryDto(
        int Id,
        string? PrimaryImagePath,   // Path or URL to the main display image
        string MakeName,
        string ModelName,
        int Year,
        string? TrimLevelName,      // Optional display
        string? ExteriorColor,
        string? VIN,                // Important for identification
        int? Mileage,
        string? MileageUnitDisplay, // e.g., "km", "mi"
        VehicleStatus Status,
        string StatusDisplay,      // User-friendly status text
        decimal? AskingPrice
    // bool IsFeatured (Optional, if you have such a flag for UI highlighting)
    );
}