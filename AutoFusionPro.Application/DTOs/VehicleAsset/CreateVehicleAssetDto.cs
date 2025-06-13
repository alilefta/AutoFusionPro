using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for creating a new comprehensive Vehicle asset.
    /// </summary>
    public record CreateVehicleAssetDto(
    // --- Core Specification ---
        [Range(1, int.MaxValue, ErrorMessage = "A valid Make must be selected.")]
    int MakeId,
    [Range(1, int.MaxValue, ErrorMessage = "A valid Model must be selected.")]
        int ModelId,

        [Range(1900, 2100, ErrorMessage = "Year must be a valid manufacturing year (e.g., 1900-2100).")] // Adjust range as needed
        int Year,

        int? EngineTypeId,    // Optional
        int? TransmissionTypeId, // Optional
        int? TrimLevelId,      // Optional
        int? BodyTypeId,       // Optional

        // --- Identification ---
        [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN must be 17 characters if provided.")]
        string? VIN, // Optional, but should be unique if present

        [StringLength(100, ErrorMessage = "Registration plate number cannot exceed 100 characters.")]
        string? RegistrationPlateNumber,

        DateTime? RegistrationExpiryDate,

        [StringLength(50, ErrorMessage = "Registration country/state cannot exceed 50 characters.")]
        string? RegistrationCountryOrState,

        // --- Physical & Condition Details ---
        [StringLength(50, ErrorMessage = "Exterior color cannot exceed 50 characters.")]
        string? ExteriorColor,

        [StringLength(50, ErrorMessage = "Interior color cannot exceed 50 characters.")]
        string? InteriorColor,

        [Range(0, 2000000, ErrorMessage = "Mileage must be a non-negative number.")] // Example range
        int? Mileage,

        MileageUnit? MileageUnit, // Defaults to Kilometers in domain model if null

        FuelType? FuelType,
    DriveType? DriveType,

        [Range(0, 10, ErrorMessage = "Number of doors must be between 0 and 10.")]
        int? NumberOfDoors,

        [Range(0, 10, ErrorMessage = "Number of seats must be between 0 and 10.")]
        int? NumberOfSeats,

    // --- Status & Sales Information ---
        [Required(ErrorMessage = "Vehicle status is required.")]
        VehicleStatus InitialStatus, // e.g., InStock, ForSale

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Purchase price must be a non-negative value.")]
        decimal? PurchasePrice, // Price at which this vehicle was acquired

        DateTime? PurchaseDate,

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Asking price must be a non-negative value.")]
        decimal? AskingPrice,   // Current listed price if ForSale

        // --- Notes & Features ---
        [StringLength(2000, ErrorMessage = "General notes cannot exceed 2000 characters.")]
        string? GeneralNotes,

        [StringLength(1000, ErrorMessage = "Features list cannot exceed 1000 characters.")]
        string? FeaturesList
    );
    // FluentValidation will handle:
    // - Ensuring MakeId, ModelId, etc., refer to existing entities.
    // - Ensuring ModelId is valid for the given MakeId.
    // - Ensuring TrimLevelId (if provided) is valid for the given ModelId.
    // - Uniqueness of VIN (if provided).
    // - Uniqueness of RegistrationPlateNumber (if provided and scoped by country/state).
}
