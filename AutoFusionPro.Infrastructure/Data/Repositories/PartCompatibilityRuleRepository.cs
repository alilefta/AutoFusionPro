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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class PartCompatibilityRuleRepository : Repository<PartCompatibilityRule, PartCompatibilityRuleRepository>, IPartCompatibilityRuleRepository
    {
        public PartCompatibilityRuleRepository(ApplicationDbContext context, ILogger<PartCompatibilityRuleRepository> logger) : base(context, logger)
        {
        }



        /// <summary>
        /// Gets a single PartCompatibilityRule by its ID, optionally including all its details
        /// like the Part it belongs to, its defined Make/Model, and its applicable attribute collections
        /// (TrimLevels, EngineTypes, etc.) with their respective taxonomy entity names.
        /// </summary>
        /// <param name="ruleId">The ID of the PartCompatibilityRule.</param>
        /// <returns>The PartCompatibilityRule entity with details, or null if not found.</returns>
        /// <exception cref="RepositoryException">Thrown if a database error occurs.</exception>
        public async Task<PartCompatibilityRule?> GetRuleByIdWithDetailsAsync(int ruleId)
        {
            if (ruleId <= 0)
            {
                _logger.LogWarning("Attempted to get part compatibility rule with invalid ID: {RuleId}. Returning null.", ruleId);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve part compatibility rule with details for ID: {RuleId}", ruleId);
            try
            {
                return await _dbSet
                    .Include(r => r.Part)       // Include the Part this rule belongs to
                        .ThenInclude(p => p.Category) // Optionally include part's category
                    .Include(r => r.Make)       // Include the Make defined in the rule (if any)
                    .Include(r => r.Model)      // Include the Model defined in the rule (if any)
                        .ThenInclude(m => m.Make) // Ensure Model's Make is loaded if Model is not null

                    // Include applicable attributes and their taxonomy details
                    .Include(r => r.ApplicableTrimLevels)
                        .ThenInclude(atl => atl.TrimLevel)
                    .Include(r => r.ApplicableEngineTypes)
                        .ThenInclude(aet => aet.EngineType)
                    .Include(r => r.ApplicableTransmissionTypes)
                        .ThenInclude(att => att.TransmissionType)
                    .Include(r => r.ApplicableBodyTypes)
                        .ThenInclude(abt => abt.BodyType)

                    .AsNoTracking() // Good for read-only complex queries
                    .FirstOrDefaultAsync(r => r.Id == ruleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving part compatibility rule with details for ID {RuleId}.", ruleId);
                throw new RepositoryException($"Could not retrieve detailed part compatibility rule with ID {ruleId}.", ex);
            }
        }

        /// <summary>
        /// Gets all compatibility rules associated with a specific part, optionally including full details for each rule.
        /// Rules are ordered by their Name.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <param name="includeDetails">Whether to include Make, Model, and applicable attribute collections for each rule.</param>
        /// <returns>A collection of PartCompatibilityRule entities. Returns an empty list if partId is invalid or no rules are found.</returns>
        /// <exception cref="RepositoryException">Thrown if a database error occurs.</exception>
        public async Task<IEnumerable<PartCompatibilityRule>> GetRulesByPartIdAsync(int partId, bool includeDetails = true)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get part compatibility rules with invalid PartID: {PartId}. Returning empty list.", partId);
                return Enumerable.Empty<PartCompatibilityRule>();
            }

            _logger.LogDebug("Attempting to retrieve compatibility rules for PartID: {PartId}, IncludeDetails: {IncludeDetails}", partId, includeDetails);
            try
            {
                IQueryable<PartCompatibilityRule> query = _dbSet.Where(r => r.PartId == partId);

                if (includeDetails)
                {
                    query = query
                        .Include(r => r.Make)
                        .Include(r => r.Model)
                            .ThenInclude(m => m.Make) // Ensure Model's Make is loaded
                        .Include(r => r.ApplicableTrimLevels)
                            .ThenInclude(atl => atl.TrimLevel)
                        .Include(r => r.ApplicableEngineTypes)
                            .ThenInclude(aet => aet.EngineType)
                        .Include(r => r.ApplicableTransmissionTypes)
                            .ThenInclude(att => att.TransmissionType)
                        .Include(r => r.ApplicableBodyTypes)
                            .ThenInclude(abt => abt.BodyType);
                }

                return await query
                    .OrderBy(r => r.Name) // Order rules by name for consistency
                    .ThenBy(r => r.Id)
                    .AsNoTracking() // Good for read-only lists
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving compatibility rules for PartID {PartId}.", partId);
                throw new RepositoryException($"Could not retrieve compatibility rules for PartID {partId}.", ex);
            }
        }

        /// <summary>
        /// Gets all PartCompatibilityRule entities, including all their detailed child collections
        /// and related Make, Model, and Part.
        /// Optionally allows a simple filter to be applied at the database level.
        /// </summary>
        public async Task<IEnumerable<PartCompatibilityRule>> GetAllWithDetailsAsync(Expression<Func<PartCompatibilityRule, bool>>? filter = null)
        {
            _logger.LogDebug("Attempting to retrieve all part compatibility rules with full details. Applying filter: {HasFilter}", filter != null);
            try
            {
                IQueryable<PartCompatibilityRule> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query
                    .Include(r => r.Part)
                        .ThenInclude(p => p.Category) // Optionally include part's category for context if needed by service filters
                    .Include(r => r.Make)
                    .Include(r => r.Model)
                        .ThenInclude(m => m.Make) // Ensure Model's Make is loaded

                    .Include(r => r.ApplicableTrimLevels)
                        .ThenInclude(atl => atl.TrimLevel)
                    .Include(r => r.ApplicableEngineTypes)
                        .ThenInclude(aet => aet.EngineType)
                    .Include(r => r.ApplicableTransmissionTypes)
                        .ThenInclude(att => att.TransmissionType)
                    .Include(r => r.ApplicableBodyTypes)
                        .ThenInclude(abt => abt.BodyType)

                    .OrderBy(r => r.PartId) // Default ordering
                    .ThenBy(r => r.Name)
                    .AsNoTracking() // Suitable for read-only comprehensive fetching
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all part compatibility rules with details.");
                throw new RepositoryException("Could not retrieve all part compatibility rules with details.", ex);
            }
        }

        // Implement other IPartCompatibilityRuleRepository methods (from IBaseRepository or specific ones) here...
        // For example, the NameExistsAsync for rules, if needed for validation within a part or globally for templates.
        // public async Task<bool> RuleNameExistsForPartAsync(int partId, string ruleName, int? excludeRuleId = null) { ... }
    }
}
