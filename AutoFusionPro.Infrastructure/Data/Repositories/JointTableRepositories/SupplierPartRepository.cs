using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Repositories.JointTableRepositories
{
    public class SupplierPartRepository : Repository<SupplierPart, SupplierPartRepository>, ISupplierPartRepository
    {
        public SupplierPartRepository(ApplicationDbContext context, ILogger<SupplierPartRepository> logger) : base(context, logger)
        {
        }



        /// <summary>
        /// Get Supplier Details by Part ID
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        /// <exception cref="RepositoryException"></exception>
        public async Task<IEnumerable<SupplierPart>> GetByPartIdWithSupplierDetailsAsync(int partId)
        {
            if (partId <= 0) return Enumerable.Empty<SupplierPart>();
            try
            {
                return await _dbSet // _dbSet is Set<SupplierPart>()
                    .Where(sp => sp.PartId == partId)
                    .Include(sp => sp.Supplier) // Eager load Supplier details
                    .OrderBy(sp => sp.Supplier.Name)
                    .ThenByDescending(sp => sp.IsPreferredSupplier) // True (preferred) first
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SupplierParts by PartId {PartId} with supplier details.", partId);
                throw new RepositoryException($"Could not retrieve supplier links for PartId {partId}.", ex);
            }
        }

        /// <summary>
        /// Gets all preferred SupplierPart links for a specific part,
        /// optionally excluding one specific SupplierPart link.
        /// </summary>
        public async Task<IEnumerable<SupplierPart>> GetPreferredSupplierLinksForPartAsync(int partId, int? excludeSupplierPartId = null)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get preferred supplier links with invalid PartID: {PartId}", partId);
                return Enumerable.Empty<SupplierPart>();
            }

            _logger.LogDebug("Retrieving preferred supplier links for PartId: {PartId}, excluding SupplierPartId: {ExcludeId}", partId, excludeSupplierPartId);
            try
            {
                var query = _dbSet
                    .Where(sp => sp.PartId == partId && sp.IsPreferredSupplier);

                if (excludeSupplierPartId.HasValue)
                {
                    query = query.Where(sp => sp.Id != excludeSupplierPartId.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving preferred supplier links for PartId {PartId}.", partId);
                throw new RepositoryException($"Could not retrieve preferred supplier links for PartId {partId}.", ex);
            }
        }
    }

}

