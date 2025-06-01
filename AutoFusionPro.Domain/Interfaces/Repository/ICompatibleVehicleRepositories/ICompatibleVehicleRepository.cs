using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories
{
    public interface ICompatibleVehicleRepository : IBaseRepository<CompatibleVehicle>
    {
        /// <summary>
        /// Gets a single CompatibleVehicle configuration by its ID, including its related
        /// Make, Model, TrimLevel, TransmissionType, EngineType, and BodyType details.
        /// </summary>
        /// <param name="id">The ID of the CompatibleVehicle.</param>
        /// <returns>The CompatibleVehicle entity with details, or null if not found.</returns>
        Task<CompatibleVehicle?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Checks if a specific CompatibleVehicle configuration (combination of Model, Years, Trim, etc.)
        /// already exists.
        /// </summary>
        /// <param name="modelId">The Model ID.</param>
        /// <param name="yearStart">The start year.</param>
        /// <param name="yearEnd">The end year.</param>
        /// <param name="trimLevelId">Optional TrimLevel ID.</param>
        /// <param name="transmissionTypeId">Optional TransmissionType ID.</param>
        /// <param name="engineTypeId">Optional EngineType ID.</param>
        /// <param name="bodyTypeId">Optional BodyType ID.</param>
        /// <param name="excludeCompatibleVehicleId">Optional ID of a CompatibleVehicle to exclude from the check (for updates).</param>
        /// <returns>True if the exact specification already exists, false otherwise.</returns>
        Task<bool> SpecificationExistsAsync(
            int modelId,
            int yearStart,
            int yearEnd,
            int? trimLevelId,
            int? transmissionTypeId,
            int? engineTypeId,
            int? bodyTypeId,
            int? excludeCompatibleVehicleId = null);

        /// <summary>
        /// Gets a paginated list of CompatibleVehicle configurations matching the filter,
        /// including details for display (Make Name, Model Name, etc.).
        /// </summary>
        /// <param name="pageNumber">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="filter">Optional filter predicate based on CompatibleVehicle properties.</param>
        /// <param name="makeIdFilter">Optional filter by Make ID (requires joining).</param>
        /// <param name="searchTerm">Optional search term for Make/Model names.</param>
        /// <returns>A collection of CompatibleVehicle entities with details.</returns>
        Task<IEnumerable<CompatibleVehicle>> GetFilteredWithDetailsPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<CompatibleVehicle, bool>>? filter = null, // Basic filter on CompatibleVehicle fields
            int? makeIdFilter = null, // Filter by Make
            string? searchTerm = null  // Generic text search on Make/Model
        );
        // Note: The filter predicate here applies directly to CompatibleVehicle.
        // For more complex filtering involving joins (like by Make name), the implementation
        // will need to build the query carefully.

        /// <summary>
        /// Gets the total count of CompatibleVehicle configurations matching the filter.
        /// </summary>
        /// <param name="filter">Optional filter predicate based on CompatibleVehicle properties.</param>
        /// <param name="makeIdFilter">Optional filter by Make ID (requires joining).</param>
        /// <param name="searchTerm">Optional search term for Make/Model names.</param>
        /// <returns>The total number of matching CompatibleVehicle configurations.</returns>
        Task<int> GetTotalCountAsync(
            Expression<Func<CompatibleVehicle, bool>>? filter = null,
            int? makeIdFilter = null,
            string? searchTerm = null
        );


        /// <summary>
        /// Gets ALL CompatibleVehicle configurations matching the filter,
        /// including details for display (Make Name, Model Name, etc.). NO PAGINATION.
        /// </summary>
        Task<IEnumerable<CompatibleVehicle>> GetAllFilteredWithDetailsAsync(
            Expression<Func<CompatibleVehicle, bool>>? filter = null,
            int? makeIdFilter = null,
            string? searchTerm = null
        );

        // Potentially a method to get CompatibleVehicles that can be linked to a specific PartId
        // (This would involve checking existing PartCompatibility links) - complex, might be service layer logic instead.
    }
}
