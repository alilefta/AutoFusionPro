using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.VehiclesInventory;

namespace AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory
{
    /// <summary>
    /// Repository interface for managing VehicleDamageLog entities.
    /// </summary>
    public interface IVehicleDamageLogRepository : IBaseRepository<VehicleDamageLog>
    {
        /// <summary>
        /// Gets all damage log entries associated with a specific vehicle,
        /// optionally including their related damage images.
        /// Ordered by DateNoted descending.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <param name="includeDamageImages">Whether to include the associated VehicleDamageImage collection for each log.</param>
        /// <returns>A collection of VehicleDamageLog entities for the specified vehicle.</returns>
        Task<IEnumerable<VehicleDamageLog>> GetByVehicleIdAsync(int vehicleId, bool includeDamageImages = false);

        /// <summary>
        /// Gets a single damage log entry by its ID, including its related damage images.
        /// </summary>
        /// <param name="damageLogId">The ID of the damage log entry.</param>
        /// <returns>The VehicleDamageLog entity with its images, or null if not found.</returns>
        Task<VehicleDamageLog?> GetByIdWithImagesAsync(int damageLogId);
    }
}
