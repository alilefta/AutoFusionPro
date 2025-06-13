using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for filtering lists of comprehensive Vehicle assets.
    /// </summary>
    public record VehicleAssetFilterCriteriaDto(
        string? SearchTerm = null,     // Searches VIN, MakeName, ModelName, RegistrationPlateNumber, Color
        int? MakeId = null,
        int? ModelId = null,
        int? TrimLevelId = null,
        int? MinYear = null,
        int? MaxYear = null,
        int? EngineTypeId = null,
        int? TransmissionTypeId = null,
        int? BodyTypeId = null,
        string? ExteriorColorSearch = null, // Specific search for color
        int? MinMileage = null,
        int? MaxMileage = null,
        VehicleStatus? Status = null,
        decimal? MinAskingPrice = null,
        decimal? MaxAskingPrice = null,
        bool? HasDamageLogs = null,     // Example of an advanced filter
        string? SortByField = null,     // e.g., "AskingPrice", "Mileage", "Year", "MakeName"
        bool IsSortAscending = true
    );
    // Note: The service/repository will need to translate SortByField string into an actual OrderBy expression.
}
