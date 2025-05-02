using AutoFusionPro.Application.DTOs.Vehicle;
using AutoFusionPro.Core.Models;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    public interface IVehicleService
    {

        /// <summary>
        /// Gets detailed information for a specific vehicle by its ID.
        /// </summary>
        Task<VehicleDetailDto?> GetVehicleByIdAsync(int id);

        // GetVehicleByVinAsync can be removed
        // Task<VehicleDetailDto?> GetVehicleByVinAsync(string vin);

        /// <summary>
        /// Gets a paginated list of vehicles, potentially filtered.
        /// </summary>
        Task<PagedResult<VehicleSummaryDto>> GetFilteredVehiclesAsync(VehicleFilterCriteriaDto filterCriteria, int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new vehicle record. Checks for Make/Model/Year uniqueness.
        /// </summary>
        Task<VehicleDetailDto> CreateVehicleAsync(CreateVehicleDto createDto);

        /// <summary>
        /// Updates an existing vehicle's details. Checks for Make/Model/Year uniqueness if changed.
        /// </summary>
        Task UpdateVehicleAsync(UpdateVehicleDto updateDto);

        /// <summary>
        /// Deletes a vehicle record. Checks for PartCompatibility dependencies.
        /// </summary>
        Task DeleteVehicleAsync(int id);

        // --- Helper / Validation Methods (Uniqueness check exposed via service) ---

        /// <summary>
        /// Checks if a Make/Model/Year combination already exists, optionally excluding a specific vehicle ID.
        /// </summary>
        Task<bool> MakeModelYearExistsAsync(string make, string model, int year, int? excludeVehicleId = null);

        // VinExistsAsync can be removed
        Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null);

        // --- Methods for Populating UI Controls ---
        Task<IEnumerable<string>> GetDistinctMakesAsync();
        Task<IEnumerable<string>> GetDistinctModelsAsync(string make);
        Task<IEnumerable<int>> GetDistinctYearsAsync(string? make = null, string? model = null);
    }
}