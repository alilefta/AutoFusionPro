using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory
{
    public class VehicleServiceHistoryRepository : Repository<VehicleServiceHistory, VehicleServiceHistoryRepository>, IVehicleServiceHistoryRepository
    {
        public VehicleServiceHistoryRepository(ApplicationDbContext context, ILogger<VehicleServiceHistoryRepository> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Gets all service history records associated with a specific vehicle,
        /// typically ordered by ServiceDate descending.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>A collection of VehicleServiceHistory entities for the specified vehicle.</returns>
        public async Task<IEnumerable<VehicleServiceHistory>> GetByVehicleIdAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to get service history with invalid VehicleID: {VehicleId}. Returning empty list.", vehicleId);
                return Enumerable.Empty<VehicleServiceHistory>();
            }

            _logger.LogDebug("Attempting to retrieve service history for VehicleID: {VehicleId}", vehicleId);
            try
            {
                return await _dbSet
                    .Where(sr => sr.VehicleId == vehicleId)
                    .OrderByDescending(sr => sr.ServiceDate) // Show most recent service first
                    .ThenByDescending(sr => sr.Id)          // Tie-breaker
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving service history for VehicleID {VehicleId}.", vehicleId);
                throw new RepositoryException($"Could not retrieve service history for VehicleID {vehicleId}.", ex);
            }
        }
    }
}
