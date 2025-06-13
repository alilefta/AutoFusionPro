using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.VehiclesInventory;

namespace AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory
{
    /// <summary>
    /// Repository interface for managing VehicleServiceHistory entities.
    /// </summary>
    public interface IVehicleServiceHistoryRepository : IBaseRepository<VehicleServiceHistory>
    {
        /// <summary>
        /// Gets all service history records associated with a specific vehicle,
        /// typically ordered by ServiceDate descending.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>A collection of VehicleServiceHistory entities for the specified vehicle.</returns>
        Task<IEnumerable<VehicleServiceHistory>> GetByVehicleIdAsync(int vehicleId);

        // Potentially other specific methods if needed, e.g.:
        // Task<IEnumerable<VehicleServiceHistory>> GetByVehicleIdAndDateRangeAsync(int vehicleId, DateTime startDate, DateTime endDate);
    }
}
