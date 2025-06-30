using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class BodyTypeRepository : Repository<BodyType, BodyTypeRepository>, IBodyTypeRepository
    {
        public BodyTypeRepository(ApplicationDbContext context, ILogger<BodyTypeRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeBodyTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string nameLower = name.Trim().ToLowerInvariant();
            var query = _dbSet.Where(bt => bt.Name.ToLower() == nameLower);
            if (excludeBodyTypeId.HasValue)
            {
                query = query.Where(bt => bt.Id != excludeBodyTypeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> IsUsedInCompatibilityRuleAttributesAsync(int bodyTypeId)
        {
            if (bodyTypeId <= 0) return false;
            _logger.LogDebug("Checking if bodyTypeId {BodyTypeId} is used in any PartCompatibilityRuleBodyTypes.", bodyTypeId);
            try
            {
                // Check if any PartCompatibilityRuleBodyType uses this BodyTypeId
                return await _context.Set<PartCompatibilityRuleBodyType>() // Assuming DbSet is PartCompatibilityRuleBodyTypes
                                     .AnyAsync(ruleBody => ruleBody.BodyTypeId == bodyTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if bodyTypeId {BodyTypeId} is used in compatibility rule attributes.", bodyTypeId);
                throw new RepositoryException($"Could not check if BodyTypeId {bodyTypeId} is in use by compatibility rule attributes.", ex);
            }
        }
    }
}
