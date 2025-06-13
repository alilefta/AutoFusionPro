using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory
{
    /// <summary>
    /// Repository interface for managing comprehensive Vehicle asset entities.
    /// </summary>
    public interface IVehicleRepository : IBaseRepository<Vehicle>
    {
        /// <summary>
        /// Gets a single Vehicle by its ID, including all its detailed collections
        /// (Images, DamageLogs with their Images, ServiceRecords, Documents) and
        /// related lookup entity names (Make, Model, Trim, EngineType, etc.).
        /// </summary>
        /// <param name="vehicleId">The ID of the Vehicle.</param>
        /// <returns>The Vehicle entity with all details, or null if not found.</returns>
        Task<Vehicle?> GetByIdWithAllDetailsAsync(int vehicleId);

        /// <summary>
        /// Gets a single Vehicle by its VIN, optionally including all details.
        /// </summary>
        /// <param name="vin">The Vehicle Identification Number.</param>
        /// <param name="includeAllDetails">Whether to load all related collections and lookup names.</param>
        /// <returns>The Vehicle entity, or null if not found.</returns>
        Task<Vehicle?> GetByVinAsync(string vin, bool includeAllDetails = false);

        /// <summary>
        /// Checks if a VIN already exists, optionally excluding a specific Vehicle ID.
        /// Only considers non-null, non-empty VINs.
        /// </summary>
        /// <param name="vin">The VIN to check.</param>
        /// <param name="excludeVehicleId">Optional ID of a vehicle to exclude (for updates).</param>
        /// <returns>True if the VIN exists for another vehicle, false otherwise.</returns>
        Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null);

        /// <summary>
        /// Checks if a Registration Plate Number already exists, optionally excluding a specific Vehicle ID.
        /// Only considers non-null, non-empty plate numbers.
        /// </summary>
        /// <param name="plateNumber">The registration plate number to check.</param>
        /// <param name="countryOrState">Optional country/state for more specific check.</param>
        /// <param name="excludeVehicleId">Optional ID of a vehicle to exclude.</param>
        /// <returns>True if the plate number exists, false otherwise.</returns>
        Task<bool> RegistrationPlateExistsAsync(string plateNumber, string? countryOrState, int? excludeVehicleId = null);


        /// <summary>
        /// Gets a paginated list of Vehicle entities based on filter criteria.
        /// Includes summary-level details (MakeName, ModelName, PrimaryImage, etc.).
        /// </summary>
        /// <param name="pageNumber">Current page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="filterPredicate">Optional direct filter on Vehicle entity.</param>
        /// <param name="makeId">Optional filter by Make ID.</param>
        /// <param name="modelId">Optional filter by Model ID.</param>
        /// <param name="minYear">Optional filter by minimum manufacturing year.</param>
        /// <param name="maxYear">Optional filter by maximum manufacturing year.</param>
        /// <param name="status">Optional filter by VehicleStatus.</param>
        /// <param name="searchTerm">Optional search term for VIN, Make Name, Model Name, Plate Number.</param>
        /// <returns>A collection of Vehicle entities for the current page, prepared for summary display.</returns>
        Task<IEnumerable<Vehicle>> GetFilteredVehiclesPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Vehicle, bool>>? filterPredicate = null,
            int? makeId = null,
            int? modelId = null,
            int? minYear = null,
            int? maxYear = null,
            VehicleStatus? status = null, // Use fully qualified enum
            string? searchTerm = null
        // Add other common filter parameters as needed (e.g., color, fuel type)
        );

        /// <summary>
        /// Gets the total count of Vehicle entities matching the specified filter criteria.
        /// Parameters should mirror GetFilteredVehiclesPagedAsync.
        /// </summary>
        Task<int> GetTotalFilteredVehiclesCountAsync(
            Expression<Func<Vehicle, bool>>? filterPredicate = null,
            int? makeId = null,
            int? modelId = null,
            int? minYear = null,
            int? maxYear = null,
            VehicleStatus? status = null,
            string? searchTerm = null
        );
    }
}
