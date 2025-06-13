using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.VehiclesInventory;

namespace AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory
{
    /// <summary>
    /// Repository interface for managing VehicleImage entities.
    /// </summary>
    public interface IVehicleImageRepository : IBaseRepository<VehicleImage>
    {
        /// <summary>
        /// Gets all images associated with a specific vehicle, ordered by DisplayOrder then by Id.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>A collection of VehicleImage entities for the specified vehicle.</returns>
        Task<IEnumerable<VehicleImage>> GetByVehicleIdAsync(int vehicleId);

        /// <summary>
        /// Gets the primary image for a specific vehicle.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>The primary VehicleImage entity, or null if no primary image is set or vehicle not found.</returns>
        Task<VehicleImage?> GetPrimaryImageForVehicleAsync(int vehicleId);

        /// <summary>
        /// Clears the IsPrimary flag for all images associated with a specific vehicle,
        /// optionally excluding a specific image ID (used when setting a new primary).
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <param name="excludeImageId">Optional ID of an image to exclude from this operation (e.g., the image being set as primary).</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task ClearPrimaryFlagsForVehicleAsync(int vehicleId, int? excludeImageId = null);
    }
}
