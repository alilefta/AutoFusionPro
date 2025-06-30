using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Core.Models;
using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    public interface IPartCompatibilityRuleService
    {
        /// <summary>
        /// Creates a new compatibility rule and links it to a specific part.
        /// </summary>
        /// <param name="partId">The ID of the part to associate the rule with.</param>
        /// <param name="createRuleDto">DTO containing the definition for the new rule.</param>
        /// <returns>The detailed DTO of the newly created PartCompatibilityRule.</returns>
        Task<PartCompatibilityRuleDetailDto> CreateRuleForPartAsync(int partId, CreatePartCompatibilityRuleDto createRuleDto);

        /// <summary>
        /// Updates an existing part compatibility rule.
        /// </summary>
        /// <param name="updateRuleDto">DTO containing the ID of the rule and updated information.</param>
        Task UpdateRuleAsync(UpdatePartCompatibilityRuleDto updateRuleDto);

        /// <summary>
        /// Deletes a part compatibility rule by its ID.
        /// </summary>
        /// <param name="ruleId">The ID of the rule to delete.</param>
        Task DeleteRuleAsync(int ruleId);

        /// <summary>
        /// Gets a specific part compatibility rule by its ID, with all details.
        /// </summary>
        /// <param name="ruleId">The ID of the rule.</param>
        /// <returns>The detailed DTO of the rule, or null if not found.</returns>
        Task<PartCompatibilityRuleDetailDto?> GetRuleByIdAsync(int ruleId);

        /// <summary>
        /// Gets all compatibility rules associated with a specific part.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <param name="onlyActiveRules">Flag to retrieve only active rules.</param>
        /// <returns>A collection of summary DTOs for the part's compatibility rules.</returns>
        Task<IEnumerable<PartCompatibilityRuleSummaryDto>> GetRulesForPartAsync(int partId, bool onlyActiveRules = true);

        // --- Template Management (Future) ---
        /// <summary>
        /// Gets all defined compatibility rule templates.
        /// </summary>
        //Task<IEnumerable<PartCompatibilityRuleTemplateDto>> GetRuleTemplatesAsync(bool onlyActive = true);

        /// <summary>
        /// Creates a new rule template from an existing rule or a new definition.
        /// </summary>
        // Task<PartCompatibilityRuleTemplateDto> CreateRuleTemplateAsync(CreatePartCompatibilityRuleDto templateData);
        // Or: Task<PartCompatibilityRuleTemplateDto> CreateTemplateFromRuleAsync(int existingRuleId, string templateName, string? templateDescription);


        /// <summary>
        /// Applies a rule template to a specific part, creating a new PartCompatibilityRule instance.
        /// </summary>
        // Task<PartCompatibilityRuleDetailDto> CreateRuleFromTemplateForPartAsync(int partId, int templateId, string? customRuleName, string? customNotes);

        // --- Copying Rules (Future) ---
        /// <summary>
        /// Copies an existing rule (or rules) from one part to another part.
        /// </summary>
        // Task<IEnumerable<PartCompatibilityRuleDetailDto>> CopyRulesToPartAsync(int sourcePartId, int targetPartId, IEnumerable<int>? specificRuleIdsToCopy = null);
        // Or copy a single rule:
        // Task<PartCompatibilityRuleDetailDto> CopyRuleToPartAsync(int ruleIdToCopy, int targetPartId, string newRuleName);



        // Removed, now it is the IPartService Responsibility to handle Parts. Instead FindPartIdsMatchingVehicleSpecAsync is now the solution
        // --- Querying for Parts based on Vehicle Specification (This is the "reverse" lookup) ---
        // This service is crucial for the "Find Parts that fit this Vehicle" use case.
        // It will take a specific vehicle's attributes and find parts whose rules match.
        // The 'VehicleSpecification' DTO here is the input from user (e.g., Make, Model, Year, Trim, etc.)
        // This is the replacement for the old system's direct query on PartCompatibility.
        /// <summary>
        /// Finds parts that are compatible with a given detailed vehicle specification.
        /// </summary>
        /// <param name="vehicleSpec">The detailed specification of the vehicle.</param>
        /// <param name="filterCriteria">Additional part filtering criteria (e.g., category, search term).</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A paged result of PartSummaryDto.</returns>
        //Task<PagedResult<Part>> FindCompatiblePartsEntitiesAsync( // Renamed and returns Part entities
        //    VehicleSpecificationDto vehicleSpec,
        //    PartCompatibilityRuleFilterDto? ruleAndPartFilters, // Use its own filter DTO
        //    int pageNumber,
        //    int pageSize
        //);


        /// <summary>
        /// Finds DISTINCT Part IDs that have at least one active compatibility rule matching the given vehicle specification.
        /// Can be further filtered by criteria applicable to the rules or the parts they belong to.
        /// </summary>
        /// <param name="vehicleSpec">The detailed specification of the vehicle to find parts for.</param>
        /// <param name="ruleAndPartFilters">Optional criteria to filter rules (e.g., ruleIsActive) or parts (e.g., partIsActive, partCategoryId) before matching.</param>
        /// <returns>A collection of distinct Part IDs that are compatible.</returns>
        Task<IEnumerable<int>> FindPartIdsMatchingVehicleSpecAsync(
            VehicleSpecificationDto vehicleSpec,
            PartCompatibilityRuleFilterDto? ruleAndPartFilters = null // New DTO for filtering rules/parts
        );

        /// <summary>
        /// Checks if a specific part is compatible with a given detailed vehicle specification.
        /// </summary>
        /// <param name="partId">The ID of the part to check.</param>
        /// <param name="vehicleSpec">The detailed specification of the vehicle.</param>
        /// <returns>True if compatible, false otherwise, along with matching details.</returns>
        Task<CompatibilityMatchResultDto> IsPartCompatibleAsync(int partId, VehicleSpecificationDto vehicleSpec);
    }
}
