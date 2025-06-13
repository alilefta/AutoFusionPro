using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for updating the core scalar properties of an existing Vehicle asset.
    /// Does not include collections like images, damage logs, etc.
    /// </summary>
    public record UpdateVehicleAssetCoreDto(
        [Range(1, int.MaxValue)]
    int Id, // ID of the Vehicle to update

    // Specification (IDs are used, names are for context if needed by validator)
        [Range(1, int.MaxValue, ErrorMessage = "Make must be selected.")]
    int MakeId,
        [Range(1, int.MaxValue, ErrorMessage = "Model must be selected.")]
        int ModelId,
        [Range(1900, 2100, ErrorMessage = "Year must be valid.")]
        int Year,
        int? EngineTypeId,
        int? TransmissionTypeId,
        int? TrimLevelId,
        int? BodyTypeId,

        // Identification
        [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN must be 17 characters if provided.")]
        string? VIN,
        [StringLength(100)]
        string? RegistrationPlateNumber,
        DateTime? RegistrationExpiryDate,
        [StringLength(50)]
        string? RegistrationCountryOrState,

        // Condition & Appearance
        [StringLength(50)]
        string? ExteriorColor,
        [StringLength(50)]
        string? InteriorColor,
        [Range(0, 2000000)]
        int? Mileage,
        MileageUnit? MileageUnit,

        // Mechanicals
        FuelType? FuelType,
        DriveType? DriveType,
        [Range(0, 10)]
        int? NumberOfDoors,
        [Range(0, 10)]
        int? NumberOfSeats,

        // Sales & Acquisition
        VehicleStatus Status,
        [Range(0, double.MaxValue)]
        decimal? PurchasePrice,
        DateTime? PurchaseDate,
        [Range(0, double.MaxValue)]
        decimal? AskingPrice,
        // SoldPrice and SoldDate are typically set via a separate "Mark as Sold" operation

        // Notes & Features
        [StringLength(2000)]
        string? GeneralNotes,
        [StringLength(1000)]
        string? FeaturesList
    );
    // FluentValidation for this DTO will:
    // - Ensure MakeId, ModelId, etc., exist.
    // - Validate VIN uniqueness (excluding self).
    // - Validate RegistrationPlateNumber uniqueness (scoped, excluding self).
    // - Ensure ModelId is valid for MakeId, TrimLevelId for ModelId.
}
