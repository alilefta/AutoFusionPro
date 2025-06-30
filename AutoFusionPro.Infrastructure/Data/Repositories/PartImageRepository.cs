using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class PartImageRepository : Repository<PartImage, PartImageRepository>, IPartImageRepository
    {
        public PartImageRepository(ApplicationDbContext context, ILogger<PartImageRepository> logger) : base(context, logger)
        {
        }


        public async Task ClearPrimaryFlagsForPartAsync(int partId, int? excludeImageId = null)
        {
            // Implementation as discussed before:
            // Fetches images where VehicleId (should be PartId) == partId && IsPrimary == true
            // (and optionally excludes excludeImageId), then sets IsPrimary = false.
            // Does NOT call SaveChangesAsync.
            if (partId <= 0) { /* log & return */ return; }
            _logger.LogDebug("Clearing primary image flags for PartID: {PartId}, excluding ImageID: {ExcludeImageId}", partId, excludeImageId ?? 0);
            try
            {
                var imagesToUpdateQuery = _dbSet
                    .Where(img => img.PartId == partId && img.IsPrimary);

                if (excludeImageId.HasValue)
                {
                    imagesToUpdateQuery = imagesToUpdateQuery.Where(img => img.Id != excludeImageId.Value);
                }
                var imagesToUpdate = await imagesToUpdateQuery.ToListAsync();
                if (imagesToUpdate.Any())
                {
                    foreach (var img in imagesToUpdate) { img.IsPrimary = false; }
                    _logger.LogInformation("Primary flags cleared for {Count} images for PartID {PartId}.", imagesToUpdate.Count, partId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing primary image flags for PartID {PartId}.", partId);
                throw new RepositoryException($"Could not clear primary image flags for PartID {partId}.", ex);
            }
        }

        public async Task<IEnumerable<PartImage>> GetByPartIdAsync(int partId)
        {
            if (partId <= 0) return Enumerable.Empty<PartImage>();
            return await _dbSet.Where(img => img.PartId == partId)
                               .OrderBy(img => img.DisplayOrder).ThenBy(img => img.Id)
                               .ToListAsync();
        }
        public async Task<PartImage?> GetPrimaryImageForPartAsync(int partId)
        {
            if (partId <= 0) return null;
            return await _dbSet
                .FirstOrDefaultAsync(img => img.PartId == partId && img.IsPrimary);
        }
    }
}
