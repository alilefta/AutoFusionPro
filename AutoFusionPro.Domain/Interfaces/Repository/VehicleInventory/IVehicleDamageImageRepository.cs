using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.VehiclesInventory;

namespace AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory
{
    /// <summary>
    /// Repository interface for managing VehicleDamageImage entities.
    /// </summary>
    public interface IVehicleDamageImageRepository : IBaseRepository<VehicleDamageImage>
    {
        /// <summary>
        /// Gets all images associated with a specific vehicle damage log entry.
        /// </summary>
        /// <param name="vehicleDamageLogId">The ID of the vehicle damage log.</param>
        /// <returns>A collection of VehicleDamageImage entities for the specified damage log.</returns>
        Task<IEnumerable<VehicleDamageImage>> GetByDamageLogIdAsync(int vehicleDamageLogId);

        // Potentially other specific methods if needed later.
    }
}
