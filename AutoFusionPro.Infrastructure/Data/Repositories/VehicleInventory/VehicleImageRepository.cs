using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory
{
    public class VehicleImageRepository : Repository<VehicleImage, VehicleImageRepository>, IVehicleImageRepository
    {
        // Constructor relies on base class to initialize _context, _logger, _dbSet
        public VehicleImageRepository(ApplicationDbContext context, ILogger<VehicleImageRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Gets all images associated with a specific vehicle, ordered by DisplayOrder then by Id.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>A collection of VehicleImage entities. Returns an empty list if vehicleId is invalid or no images are found.</returns>
        /// <exception cref="RepositoryException">Thrown if a database error occurs.</exception>
        public async Task<IEnumerable<VehicleImage>> GetByVehicleIdAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to get vehicle images with invalid VehicleID: {VehicleId}. Returning empty list.", vehicleId);
                return Enumerable.Empty<VehicleImage>();
            }

            _logger.LogDebug("Attempting to retrieve images for VehicleID: {VehicleId}", vehicleId);
            try
            {
                return await _dbSet
                    .Where(img => img.VehicleId == vehicleId)
                    .OrderBy(img => img.DisplayOrder) // Primary sort by user-defined order
                    .ThenBy(img => img.Id)           // Secondary sort for stability
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images for VehicleID {VehicleId}.", vehicleId);
                throw new RepositoryException($"Could not retrieve images for VehicleID {vehicleId}.", ex);
            }
        }

        /// <summary>
        /// Gets the primary image for a specific vehicle.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>The primary VehicleImage entity, or null if no primary image is set or vehicle not found.</returns>
        /// <exception cref="RepositoryException">Thrown if a database error occurs.</exception>
        public async Task<VehicleImage?> GetPrimaryImageForVehicleAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to get primary vehicle image with invalid VehicleID: {VehicleId}. Returning null.", vehicleId);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve primary image for VehicleID: {VehicleId}", vehicleId);
            try
            {
                return await _dbSet
                    .FirstOrDefaultAsync(img => img.VehicleId == vehicleId && img.IsPrimary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving primary image for VehicleID {VehicleId}.", vehicleId);
                throw new RepositoryException($"Could not retrieve primary image for VehicleID {vehicleId}.", ex);
            }
        }

        /// <summary>
        /// Clears the IsPrimary flag for all images associated with a specific vehicle,
        /// optionally excluding a specific image ID (used when setting a new primary).
        /// This method stages changes; SaveChangesAsync must be called by the service layer via UnitOfWork.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <param name="excludeImageId">Optional ID of an image to exclude from this operation (e.g., the image being set as primary).</param>
        /// <exception cref="RepositoryException">Thrown if a database error occurs during fetching or staging updates.</exception>
        public async Task ClearPrimaryFlagsForVehicleAsync(int vehicleId, int? excludeImageId = null)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to clear primary flags with invalid VehicleID: {VehicleId}. Operation aborted.", vehicleId);
                // Optionally throw ArgumentOutOfRangeException if this is considered a critical programming error
                // throw new ArgumentOutOfRangeException(nameof(vehicleId), "Vehicle ID must be greater than zero.");
                return; // Or simply do nothing if invalid ID is not exceptional for this method's contract
            }

            _logger.LogDebug("Attempting to clear primary image flags for VehicleID: {VehicleId}, excluding ImageID: {ExcludeImageId}",
                vehicleId, excludeImageId ?? 0); // Log 0 or similar if excludeImageId is null for clarity

            try
            {
                // Construct the query to find images that are currently primary for this vehicle
                IQueryable<VehicleImage> imagesToUpdateQuery = _dbSet
                    .Where(img => img.VehicleId == vehicleId && img.IsPrimary);

                // If an image ID is to be excluded (because it's the one being set as primary),
                // don't clear its flag if it's already primary.
                if (excludeImageId.HasValue && excludeImageId.Value > 0)
                {
                    imagesToUpdateQuery = imagesToUpdateQuery.Where(img => img.Id != excludeImageId.Value);
                }

                // Fetch the entities that need updating
                var imagesToUpdate = await imagesToUpdateQuery.ToListAsync();

                if (imagesToUpdate.Any())
                {
                    _logger.LogInformation("Found {Count} images to clear primary flag for VehicleID {VehicleId}.", imagesToUpdate.Count, vehicleId);
                    foreach (var img in imagesToUpdate)
                    {
                        img.IsPrimary = false;
                        // EF Core tracks this change on the fetched entity.
                        // No explicit _dbSet.Update(img) call is needed here.
                    }
                    // Note: SaveChangesAsync is NOT called here. It's the responsibility of the UnitOfWork
                    // managed by the service layer to commit these staged changes.
                }
                else
                {
                    _logger.LogDebug("No primary images found to clear for VehicleID {VehicleId} (or the one to exclude was the only primary).", vehicleId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing primary image flags for VehicleID {VehicleId}.", vehicleId);
                throw new RepositoryException($"Could not clear primary image flags for VehicleID {vehicleId}.", ex);
            }
        }
    }
}
