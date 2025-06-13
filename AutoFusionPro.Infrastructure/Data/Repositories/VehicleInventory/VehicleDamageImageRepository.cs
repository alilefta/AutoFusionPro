using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory
{
    public class VehicleDamageImageRepository : Repository<VehicleDamageImage, VehicleDamageImageRepository>, IVehicleDamageImageRepository
    {
        // Constructor relies on base class
        public VehicleDamageImageRepository(ApplicationDbContext context, ILogger<VehicleDamageImageRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Gets all images associated with a specific vehicle damage log entry, ordered by their ID (creation order).
        /// </summary>
        /// <param name="vehicleDamageLogId">The ID of the vehicle damage log.</param>
        /// <returns>A collection of VehicleDamageImage entities for the specified damage log. Returns an empty list if vehicleDamageLogId is invalid or no images are found.</returns>
        /// <exception cref="RepositoryException">Thrown if an error occurs during database interaction.</exception>
        public async Task<IEnumerable<VehicleDamageImage>> GetByDamageLogIdAsync(int vehicleDamageLogId)
        {
            if (vehicleDamageLogId <= 0)
            {
                _logger.LogWarning("Attempted to get damage images with invalid VehicleDamageLogID: {DamageLogId}. Returning empty list.", vehicleDamageLogId);
                return Enumerable.Empty<VehicleDamageImage>();
            }

            _logger.LogDebug("Attempting to retrieve images for VehicleDamageLogID: {DamageLogId}", vehicleDamageLogId);
            try
            {
                return await _dbSet
                    .Where(img => img.VehicleDamageLogId == vehicleDamageLogId)
                    .OrderBy(img => img.Id) // Assuming ID represents creation order, or add a DisplayOrder property
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images for VehicleDamageLogID {DamageLogId}.", vehicleDamageLogId);
                throw new RepositoryException($"Could not retrieve images for VehicleDamageLogID {vehicleDamageLogId}.", ex);
            }
        }
    }
}
