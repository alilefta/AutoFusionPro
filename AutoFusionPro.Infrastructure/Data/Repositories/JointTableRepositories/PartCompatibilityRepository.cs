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
    public class PartCompatibilityRepository : Repository<PartCompatibility, PartCompatibilityRepository>, IPartCompatibilityRepository
    {
        public PartCompatibilityRepository(ApplicationDbContext context, ILogger<PartCompatibilityRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<PartCompatibility>> GetByPartIdWithCompatibleVehicleDetailsAsync(int partId)
        {
            if (partId <= 0) return Enumerable.Empty<PartCompatibility>();
            try
            {
                return await _dbSet // _dbSet is Set<PartCompatibility>()
                    .Where(pc => pc.PartId == partId)
                    .Include(pc => pc.CompatibleVehicle)
                        .ThenInclude(cv => cv.Model)
                            .ThenInclude(m => m.Make)
                    .Include(pc => pc.CompatibleVehicle)
                        .ThenInclude(cv => cv.TrimLevel)
                    .Include(pc => pc.CompatibleVehicle)
                        .ThenInclude(cv => cv.EngineType)
                    .Include(pc => pc.CompatibleVehicle)
                        .ThenInclude(cv => cv.TransmissionType)
                    .Include(pc => pc.CompatibleVehicle)
                        .ThenInclude(cv => cv.BodyType)
                    .OrderBy(pc => pc.CompatibleVehicle.Model.Make.Name)
                    .ThenBy(pc => pc.CompatibleVehicle.Model.Name)
                    .ThenByDescending(pc => pc.CompatibleVehicle.YearEnd)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PartCompatibilities by PartId {PartId} with details.", partId);
                throw new RepositoryException($"Could not retrieve compatibility links for PartId {partId}.", ex);
            }
        }
    }
}
