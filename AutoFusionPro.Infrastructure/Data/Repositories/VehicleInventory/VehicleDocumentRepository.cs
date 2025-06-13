using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory
{
    public class VehicleDocumentRepository : Repository<VehicleDocument, VehicleDocumentRepository>, IVehicleDocumentRepository
    {
        // Constructor relies on base class to initialize _context, _logger, _dbSet
        public VehicleDocumentRepository(ApplicationDbContext context, ILogger<VehicleDocumentRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Gets all documents associated with a specific vehicle, ordered by DocumentType and then DocumentName.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>A collection of VehicleDocument entities for the specified vehicle. Returns an empty list if vehicleId is invalid or no documents are found.</returns>
        /// <exception cref="RepositoryException">Thrown if an error occurs during database interaction.</exception>
        public async Task<IEnumerable<VehicleDocument>> GetByVehicleIdAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to get vehicle documents with invalid VehicleID: {VehicleId}. Returning empty list.", vehicleId);
                return Enumerable.Empty<VehicleDocument>();
            }

            _logger.LogDebug("Attempting to retrieve documents for VehicleID: {VehicleId}", vehicleId);
            try
            {
                return await _dbSet
                    .Where(doc => doc.VehicleId == vehicleId)
                    .OrderBy(doc => doc.DocumentType)  // Primary sort by type
                    .ThenBy(doc => doc.DocumentName) // Secondary sort by name
                    .ThenBy(doc => doc.Id)           // Tie-breaker for stability
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for VehicleID {VehicleId}.", vehicleId);
                throw new RepositoryException($"Could not retrieve documents for VehicleID {vehicleId}.", ex);
            }
        }
    }
}
