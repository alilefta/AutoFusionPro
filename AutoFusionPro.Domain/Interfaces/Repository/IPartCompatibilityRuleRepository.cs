using AutoFusionPro.Core.SharedDTOs.PartCompatibilityRule;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IPartCompatibilityRuleRepository : IBaseRepository<PartCompatibilityRule>
    {
        Task<PartCompatibilityRule?> GetRuleByIdWithDetailsAsync(int ruleId); // (To load the rule with its Part, Make, Model, and collections like ApplicableTrimLevels with their actual TrimLevel names, etc.)
        Task<IEnumerable<PartCompatibilityRule>> GetRulesByPartIdAsync(int partId, bool includeDetails = true);

        /// <summary>
        /// Gets all PartCompatibilityRule entities, including all their detailed child collections
        /// (ApplicableTrimLevels with TrimLevel, ApplicableEngineTypes with EngineType, etc.)
        /// and related Make, Model, and Part.
        /// Optionally allows a simple filter to be applied at the database level.
        /// </summary>
        /// <param name="filter">Optional predicate to filter rules at the database level.</param>
        /// <returns>A collection of PartCompatibilityRule entities with their details.</returns>
        Task<IEnumerable<PartCompatibilityRule>> GetAllWithDetailsAsync(Expression<Func<PartCompatibilityRule, bool>>? filter = null);


        /// <summary>
        /// Gets a paginated list of PartCompatibilityRule entities based on filter criteria.
        /// Includes details required for the PartCompatibilityRuleSummaryDto.
        /// </summary>
        /// <param name="criteria">The DTO containing all filter parameters.</param>
        /// <param name="pageNumber">The current page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A collection of matching PartCompatibilityRule entities for the current page.</returns>
        Task<IEnumerable<PartCompatibilityRule>> GetFilteredRulesPagedAsync(
            RuleFilterCriteriaDto criteria,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Gets the total count of PartCompatibilityRule entities matching the specified filter criteria.
        /// Parameters should mirror GetFilteredRulesPagedAsync for accurate counts.
        /// </summary>
        /// <param name="criteria">The DTO containing all filter parameters.</param>
        /// <returns>The total number of matching rules.</returns>
        Task<int> GetTotalFilteredRulesCountAsync(RuleFilterCriteriaDto criteria);

        // should be at service level
        //    Task<bool> RuleDefinitionExistsAsync(PartCompatibilityRule ruleToCompare, int? excludeRuleId = null); //(A method to check if an identical rule definition already exists – this will be complex as it needs to compare MakeId, ModelId, YearRange, AND the content of all applicable attribute collections). This logic might be better suited for the service layer. Methods for fetching rules based on template status, active status, etc.
    }
}
