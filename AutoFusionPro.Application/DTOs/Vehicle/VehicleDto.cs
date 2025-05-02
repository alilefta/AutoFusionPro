using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.Vehicle
{
    /// <summary>
    /// DTO containing data required to create a new vehicle.
    /// </summary>
    public record CreateVehicleDto(
        [Required(AllowEmptyStrings = false)] string Make,
        [Required(AllowEmptyStrings = false)] string Model,
        [Range(1900, 2100)] int Year, // Example range validation
        string? VIN, // Optional
        string? Engine,
        string? Transmission,
        string? TrimLevel,
        string? BodyType
    );
    // Note: Uniqueness validation (Make/Model/Year) should be handled
    // by the service/FluentValidator calling the repository check.

    /// <summary>
    /// DTO containing data required to update an existing vehicle.
    /// </summary>
    public record UpdateVehicleDto(
        [Range(1, int.MaxValue)] int Id, // ID of the vehicle to update
        [Required(AllowEmptyStrings = false)] string Make,
        [Required(AllowEmptyStrings = false)] string Model,
        [Range(1900, 2100)] int Year,
        string? VIN, // Optional
        string? Engine,
        string? Transmission,
        string? TrimLevel,
        string? BodyType
    );

    public record VehicleSummaryDto 
        (
            int Id,
            string Make,
            string Model,
            int Year,
            string Engine,
            string Transmission,
            string TrimLevel,
            string BodyType
        );


    public class VehicleDetailDto // Using class, but record is also fine
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? VIN { get; set; } // Optional
        public string Engine { get; set; } = string.Empty; // Assuming Engine can be empty/optional
        public string Transmission { get; set; } = string.Empty; // Assuming Transmission can be empty/optional
        public string TrimLevel { get; set; } = string.Empty; // Assuming Trim can be empty/optional
        public string BodyType { get; set; } = string.Empty; // Assuming BodyType can be empty/optional

        // Constructor can be helpful for mapping
        public VehicleDetailDto() { } // Parameterless if needed

        public VehicleDetailDto(int id, string make, string model, int year, string? vin, string engine, string transmission, string trimLevel, string bodyType)
        {
            Id = id;
            Make = make;
            Model = model;
            Year = year;
            VIN = vin;
            Engine = engine;
            Transmission = transmission;
            TrimLevel = trimLevel;
            BodyType = bodyType;
        }
    }

    /// <summary>
    /// DTO encapsulating criteria for filtering vehicle lists.
    /// </summary>
    public record VehicleFilterCriteriaDto(
        string? SearchTerm = null, // Can search Make, Model, VIN, etc.
        string? Make = null,
        string? Model = null,
        int? MinYear = null,
        int? MaxYear = null,
        string? BodyType = null,
        string? TrimLevel = null

    // Add other filterable fields if needed (e.g., BodyType)
    );

    // --- Assumed DTO Adjustments ---
    // - CreateVehicleDto: Make, Model, Year are required; VIN is string? (nullable)
    // - UpdateVehicleDto: Make, Model, Year are required; VIN is string?
    // - VehicleDetailDto: VIN is string?
    // - VehicleSummaryDto: VIN is string?
    // - VehicleFilterCriteriaDto: Search might still look at VIN, but Make/Model/Year are primary filters.
}