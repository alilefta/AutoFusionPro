using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory
{
    public class VehicleDamageLogRepository : Repository<VehicleDamageLog, VehicleDamageLogRepository>, IVehicleDamageLogRepository
    {
        public VehicleDamageLogRepository(ApplicationDbContext context, ILogger<VehicleDamageLogRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<VehicleDamageLog>> GetByVehicleIdAsync(int vehicleId, bool includeDamageImages = false)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to get damage logs with invalid VehicleID: {VehicleId}. Returning empty list.", vehicleId);
                return Enumerable.Empty<VehicleDamageLog>();
            }

            _logger.LogDebug("Attempting to retrieve damage logs for VehicleID: {VehicleId}, IncludeImages: {IncludeImages}", vehicleId, includeDamageImages);
            try
            {
                IQueryable<VehicleDamageLog> query = _dbSet.Where(dl => dl.VehicleId == vehicleId);

                if (includeDamageImages)
                {
                    query = query.Include(dl => dl.DamageImages);
                }

                return await query
                    .OrderByDescending(dl => dl.DateNoted) // Show most recent damage first
                    .ThenByDescending(dl => dl.Id)       // Tie-breaker
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving damage logs for VehicleID {VehicleId}.", vehicleId);
                throw new RepositoryException($"Could not retrieve damage logs for VehicleID {vehicleId}.", ex);
            }
        }

        public async Task<VehicleDamageLog?> GetByIdWithImagesAsync(int damageLogId)
        {
            if (damageLogId <= 0)
            {
                _logger.LogWarning("Attempted to get damage log with invalid ID: {DamageLogId}. Returning null.", damageLogId);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve damage log with images for ID: {DamageLogId}", damageLogId);
            try
            {
                return await _dbSet
                    .Include(dl => dl.DamageImages)
                    .FirstOrDefaultAsync(dl => dl.Id == damageLogId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving damage log with images for ID {DamageLogId}.", damageLogId);
                throw new RepositoryException($"Could not retrieve damage log with images for ID {damageLogId}.", ex);
            }
        }
    }
}
