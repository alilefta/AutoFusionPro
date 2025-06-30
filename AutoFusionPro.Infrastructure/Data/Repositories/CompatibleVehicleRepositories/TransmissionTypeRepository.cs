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
    public class TransmissionTypeRepository : Repository<TransmissionType, TransmissionTypeRepository>, ITransmissionTypeRepository
    {
        public TransmissionTypeRepository(ApplicationDbContext context, ILogger<TransmissionTypeRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeTransmissionTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string nameLower = name.Trim().ToLowerInvariant();
            var query = _dbSet.Where(tt => tt.Name.ToLower() == nameLower);
            if (excludeTransmissionTypeId.HasValue)
            {
                query = query.Where(tt => tt.Id != excludeTransmissionTypeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> IsUsedInCompatibilityRuleAttributesAsync(int transmissionTypeId)
        {
            if (transmissionTypeId <= 0) return false;
            _logger.LogDebug("Checking if transmissionTypeId {TransmissionTypeId} is used in any PartCompatibilityRuleTransmissionTypes.", transmissionTypeId);
            try
            {
                // Check if any PartCompatibilityRuleEngineType uses this transmissionTypeId
                return await _context.Set<PartCompatibilityRuleTransmissionType>() // Assuming DbSet is PartCompatibilityRuleEngineTypes
                                     .AnyAsync(ruleTransmssion => ruleTransmssion.TransmissionTypeId == transmissionTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if transmissionTypeId {TransmissionTypeId} is used in compatibility rule attributes.", transmissionTypeId);
                throw new RepositoryException($"Could not check if transmissionTypeId {transmissionTypeId} is in use by compatibility rule attributes.", ex);
            }
        }
    }
}
