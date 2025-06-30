using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Models;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class PartCompatibilityRuleService : IPartCompatibilityRuleService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PartCompatibilityRuleService> _logger;
        private readonly IValidator<CreatePartCompatibilityRuleDto> _createRuleValidator;
        private readonly IValidator<UpdatePartCompatibilityRuleDto> _updateRuleValidator;

        public PartCompatibilityRuleService(
            IValidator<CreatePartCompatibilityRuleDto> createRuleValidator, 
            IValidator<UpdatePartCompatibilityRuleDto> updateRuleValidator,
            IUnitOfWork unitOfWork,
            ILogger<PartCompatibilityRuleService> logger)
        {
            _createRuleValidator = createRuleValidator ?? throw new ArgumentNullException(nameof(createRuleValidator)); 
            _updateRuleValidator = updateRuleValidator ?? throw new ArgumentNullException(nameof(updateRuleValidator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Create & Update

        /// <summary>
        /// Creates a new compatibility rule and links it to a specific part.
        /// </summary>
        public async Task<PartCompatibilityRuleDetailDto> CreateRuleForPartAsync(int partId, CreatePartCompatibilityRuleDto createRuleDto)
        {
            ArgumentNullException.ThrowIfNull(createRuleDto, nameof(createRuleDto));
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to create compatibility rule with invalid PartID: {PartId}", partId);
                throw new ArgumentException("Part ID must be greater than zero.", nameof(partId));
            }

            _logger.LogInformation("Attempting to create compatibility rule '{RuleName}' for PartID: {PartId}", createRuleDto.Name, partId);

            // 1. Validate DTO
            // The validator should check existence of MakeId, ModelId, and all AttributeIds in the collections.
            // It should also check hierarchical consistency (e.g., Model belongs to Make, Trims belong to Model).
            var validationResult = await _createRuleValidator.ValidateAsync(createRuleDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreatePartCompatibilityRuleDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Validate Part Existence
            if (!await _unitOfWork.Parts.ExistsAsync(p => p.Id == partId))
            {
                string msg = $"Part with ID {partId} not found. Cannot create compatibility rule.";
                _logger.LogWarning(msg);
                throw new ServiceException(msg); // Or NotFoundException
            }

            // Optional: Check for an absolutely identical existing rule for THIS part.
            // This is complex because it involves comparing all collections.
            // The UI/user is expected to manage creating distinct rules.
            // However, a unique constraint on PartCompatibilityRule.Name PER PartId could be useful.
            // For now, we assume rule names don't have to be unique per part, or it's handled by UI/user.

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Map DTO to Domain Entity (PartCompatibilityRule)
                var newRule = new PartCompatibilityRule
                {
                    PartId = partId,
                    Name = createRuleDto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(createRuleDto.Description) ? null : createRuleDto.Description.Trim(),
                    MakeId = createRuleDto.MakeId == 0 ? null : createRuleDto.MakeId, // Assume 0 means "Any" or null from DTO
                    ModelId = createRuleDto.ModelId == 0 ? null : createRuleDto.ModelId,
                    YearStart = createRuleDto.YearStart,
                    YearEnd = createRuleDto.YearEnd,
                    Notes = string.IsNullOrWhiteSpace(createRuleDto.Notes) ? null : createRuleDto.Notes.Trim(),
                    IsActive = createRuleDto.IsActive,
                    IsTemplate = createRuleDto.CreateAsTemplate, // From DTO
                    CopiedFromRuleId = createRuleDto.CopiedFromRuleId, // From DTO
                    CreatedAt = DateTime.UtcNow // Or handled by BaseEntity
                };

                // 4. Add Applicable Attributes (Junction Table Records)
                // ApplicableTrimLevels
                if (createRuleDto.ApplicableTrimLevels != null)
                {
                    foreach (var trimAttr in createRuleDto.ApplicableTrimLevels.Where(ta => ta.AttributeId > 0)) // Ensure valid ID
                    {
                        newRule.ApplicableTrimLevels.Add(new PartCompatibilityRuleTrimLevel
                        {
                            // PartCompatibilityRule will be set by EF Core relationship fixup
                            TrimLevelId = trimAttr.AttributeId,
                            IsExclusion = trimAttr.IsExclusion
                            // CreatedAt/ModifiedAt handled by BaseEntity if junction inherits it
                        });
                    }
                }

                // ApplicableEngineTypes
                if (createRuleDto.ApplicableEngineTypes != null)
                {
                    foreach (var engineAttr in createRuleDto.ApplicableEngineTypes.Where(ea => ea.AttributeId > 0))
                    {
                        newRule.ApplicableEngineTypes.Add(new PartCompatibilityRuleEngineType
                        {
                            EngineTypeId = engineAttr.AttributeId,
                            IsExclusion = engineAttr.IsExclusion
                        });
                    }
                }

                // ApplicableTransmissionTypes
                if (createRuleDto.ApplicableTransmissionTypes != null)
                {
                    foreach (var transAttr in createRuleDto.ApplicableTransmissionTypes.Where(ta => ta.AttributeId > 0))
                    {
                        newRule.ApplicableTransmissionTypes.Add(new PartCompatibilityRuleTransmissionType
                        {
                            TransmissionTypeId = transAttr.AttributeId,
                            IsExclusion = transAttr.IsExclusion
                        });
                    }
                }

                // ApplicableBodyTypes
                if (createRuleDto.ApplicableBodyTypes != null)
                {
                    foreach (var bodyAttr in createRuleDto.ApplicableBodyTypes.Where(ba => ba.AttributeId > 0))
                    {
                        newRule.ApplicableBodyTypes.Add(new PartCompatibilityRuleBodyType
                        {
                            BodyTypeId = bodyAttr.AttributeId,
                            IsExclusion = bodyAttr.IsExclusion
                        });
                    }
                }

                // 5. Add Rule to Repository
                await _unitOfWork.PartCompatibilityRules.AddAsync(newRule);

                // 6. Save All Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("PartCompatibilityRule created successfully: ID={RuleId}, Name='{RuleName}' for PartID={PartId}",
                    newRule.Id, newRule.Name, partId);

                // 7. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // 8. Re-fetch with details for return DTO
                var createdRuleWithDetails = await _unitOfWork.PartCompatibilityRules.GetRuleByIdWithDetailsAsync(newRule.Id);
                if (createdRuleWithDetails == null) // Should not happen
                {
                    _logger.LogError("Critical: Failed to re-fetch PartCompatibilityRule with ID {RuleId} immediately after creation.", newRule.Id);
                    throw new ServiceException("Compatibility rule was created, but its full details could not be immediately retrieved.");
                }
                return MapRuleToDetailDto(createdRuleWithDetails);
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (DbUpdateException dbEx) // Handles potential database constraint violations
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error occurred while creating compatibility rule '{RuleName}' for PartID {PartId}.",
                    createRuleDto.Name, partId);
                // Check for specific unique constraint violations if needed
                throw new ServiceException("A database error occurred while creating the compatibility rule.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An unexpected error occurred while creating compatibility rule '{RuleName}' for PartID {PartId}.",
                    createRuleDto.Name, partId);
                throw new ServiceException($"An error occurred while creating compatibility rule '{createRuleDto.Name}'.", ex);
            }
        }


        /// <summary>
        /// Updates an existing part compatibility rule.
        /// </summary>
        public async Task UpdateRuleAsync(UpdatePartCompatibilityRuleDto updateRuleDto)
        {
            ArgumentNullException.ThrowIfNull(updateRuleDto, nameof(updateRuleDto));
            if (updateRuleDto.Id <= 0)
            {
                _logger.LogWarning("Attempted to update compatibility rule with invalid ID: {RuleId}", updateRuleDto.Id);
                throw new ArgumentException("Rule ID must be greater than zero for an update.", nameof(updateRuleDto.Id));
            }

            _logger.LogInformation("Attempting to update compatibility rule ID: {RuleId}, Name: '{RuleName}'",
                updateRuleDto.Id, updateRuleDto.Name);

            // 1. Validate DTO
            // Validator ensures rule exists, taxonomy IDs exist, hierarchical consistency.
            var validationResult = await _updateRuleValidator.ValidateAsync(updateRuleDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdatePartCompatibilityRuleDto (ID: {RuleId}): {Errors}",
                    updateRuleDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity WITH its current collections for comparison
            var existingRule = await _unitOfWork.PartCompatibilityRules.GetRuleByIdWithDetailsAsync(updateRuleDto.Id);
            if (existingRule == null) // Validator should catch this, but defensive check
            {
                string notFoundMsg = $"PartCompatibilityRule with ID {updateRuleDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or custom NotFoundException
            }

            // Optional: Rule Name uniqueness check per Part (if not handled by validator comprehensively)
            // if (existingRule.Name != updateRuleDto.Name.Trim() &&
            //     await _unitOfWork.PartCompatibilityRules.NameExistsForPartAsync(existingRule.PartId, updateRuleDto.Name.Trim(), existingRule.Id))
            // {
            //     throw new DuplicationException(...);
            // }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Update Scalar Properties
                existingRule.Name = updateRuleDto.Name.Trim();
                existingRule.Description = string.IsNullOrWhiteSpace(updateRuleDto.Description) ? null : updateRuleDto.Description.Trim();
                existingRule.MakeId = updateRuleDto.MakeId == 0 ? null : updateRuleDto.MakeId;
                existingRule.ModelId = updateRuleDto.ModelId == 0 ? null : updateRuleDto.ModelId;
                existingRule.YearStart = updateRuleDto.YearStart;
                existingRule.YearEnd = updateRuleDto.YearEnd;
                existingRule.Notes = string.IsNullOrWhiteSpace(updateRuleDto.Notes) ? null : updateRuleDto.Notes.Trim();
                existingRule.IsActive = updateRuleDto.IsActive;
                existingRule.IsTemplate = updateRuleDto.IsTemplate;
                // existingRule.ModifiedAt = DateTime.UtcNow; // Handled by BaseEntity/DbContext

                // 4. Update Applicable Attribute Collections (Delete old, Add new/updated)
                // This approach (clear and re-add) is often simpler than trying to diff collections.
                // Ensure junction tables have CascadeOnDelete set up from PartCompatibilityRule.

                // Trim Levels
                UpdateApplicableAttributes<PartCompatibilityRuleTrimLevel>( // Explicitly specify TJunctionEntity
                    existingRule.ApplicableTrimLevels,
                    updateRuleDto.ApplicableTrimLevels,
                    (attrDto) => new PartCompatibilityRuleTrimLevel
                    {
                        PartCompatibilityRuleId = existingRule.Id, // Set FK to the rule
                        TrimLevelId = attrDto.AttributeId,
                        IsExclusion = attrDto.IsExclusion
                        // CreatedAt/ModifiedAt from BaseEntity if inherited
                    },
                    _unitOfWork.PartCompatibilityRuleTrimLevels); // Pass the specific repository

                // Engine Types
                UpdateApplicableAttributes<PartCompatibilityRuleEngineType>(
                    existingRule.ApplicableEngineTypes,
                    updateRuleDto.ApplicableEngineTypes,
                    (attrDto) => new PartCompatibilityRuleEngineType
                    {
                        PartCompatibilityRuleId = existingRule.Id,
                        EngineTypeId = attrDto.AttributeId,
                        IsExclusion = attrDto.IsExclusion
                    },
                    _unitOfWork.PartCompatibilityRuleEngineTypes);

                // Transmission Types
                UpdateApplicableAttributes<PartCompatibilityRuleTransmissionType>(
                    existingRule.ApplicableTransmissionTypes,
                    updateRuleDto.ApplicableTransmissionTypes,
                    (attrDto) => new PartCompatibilityRuleTransmissionType
                    {
                        PartCompatibilityRuleId = existingRule.Id,
                        TransmissionTypeId = attrDto.AttributeId,
                        IsExclusion = attrDto.IsExclusion
                    },
                    _unitOfWork.PartCompatibilityRuleTransmissionTypes);

                // Body Types
                UpdateApplicableAttributes<PartCompatibilityRuleBodyType>(
                    existingRule.ApplicableBodyTypes,
                    updateRuleDto.ApplicableBodyTypes,
                    (attrDto) => new PartCompatibilityRuleBodyType
                    {
                        PartCompatibilityRuleId = existingRule.Id,
                        BodyTypeId = attrDto.AttributeId,
                        IsExclusion = attrDto.IsExclusion
                    },
                    _unitOfWork.PartCompatibilityRuleBodyTypes);


                // 5. Save Changes
                // EF Core will track changes to existingRule and its collections
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("PartCompatibilityRule ID {RuleId} updated successfully.", updateRuleDto.Id);

                // 6. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error updating PartCompatibilityRule ID {RuleId}.", updateRuleDto.Id);
                throw new ServiceException("A database error occurred while updating the compatibility rule.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Unexpected error updating PartCompatibilityRule ID {RuleId}.", updateRuleDto.Id);
                throw new ServiceException($"An unexpected error occurred while updating compatibility rule ID {updateRuleDto.Id}.", ex);
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Gets a specific part compatibility rule by its ID, with all details.
        /// </summary>
        /// <param name="ruleId">The ID of the rule.</param>
        /// <returns>The detailed DTO of the rule, or null if not found.</returns>
        /// <exception cref="ServiceException">Thrown if an error occurs during data retrieval.</exception>
        public async Task<PartCompatibilityRuleDetailDto?> GetRuleByIdAsync(int ruleId)
        {
            if (ruleId <= 0)
            {
                _logger.LogWarning("Attempted to get part compatibility rule with invalid ID: {RuleId}", ruleId);
                return null; // Or throw new ArgumentOutOfRangeException(nameof(ruleId), "Rule ID must be positive.");
            }

            _logger.LogInformation("Attempting to retrieve part compatibility rule with details for ID: {RuleId}", ruleId);
            try
            {
                var ruleEntity = await _unitOfWork.PartCompatibilityRules.GetRuleByIdWithDetailsAsync(ruleId);

                if (ruleEntity == null)
                {
                    _logger.LogWarning("Part compatibility rule with ID {RuleId} not found.", ruleId);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved part compatibility rule with ID {RuleId} named '{RuleName}'.",
                    ruleId, ruleEntity.Name);

                return MapRuleToDetailDto(ruleEntity);
            }
            catch (Exception ex) // Catching generic Exception after specific RepositoryException might be caught by repo
            {
                _logger.LogError(ex, "An error occurred while retrieving part compatibility rule with ID {RuleId}.", ruleId);
                // Avoid exposing raw EF Core or repository exceptions directly.
                throw new ServiceException($"Could not retrieve part compatibility rule with ID {ruleId} due to an internal error.", ex);
            }
        }


        /// <summary>
        /// Gets all compatibility rules associated with a specific part.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <param name="onlyActiveRules">Flag to retrieve only active rules.</param>
        /// <returns>A collection of summary DTOs for the part's compatibility rules.</returns>
        /// <exception cref="ServiceException">Thrown if an error occurs during data retrieval.</exception>
        public async Task<IEnumerable<PartCompatibilityRuleSummaryDto>> GetRulesForPartAsync(int partId, bool onlyActiveRules = true)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get part compatibility rules with invalid PartID: {PartId}", partId);
                return Enumerable.Empty<PartCompatibilityRuleSummaryDto>();
            }

            _logger.LogInformation("Attempting to retrieve compatibility rules for PartID: {PartId}, OnlyActive: {OnlyActive}", partId, onlyActiveRules);
            try
            {
                // Fetch rules with all necessary details for summary DTO mapping
                var ruleEntities = await _unitOfWork.PartCompatibilityRules.GetRulesByPartIdAsync(partId, includeDetails: true);

                IEnumerable<PartCompatibilityRule> filteredRules = ruleEntities;
                if (onlyActiveRules)
                {
                    filteredRules = ruleEntities.Where(r => r.IsActive);
                }

                var summaryDtos = filteredRules
                                    .Select(rule => MapRuleToSummaryDto(rule)) // Pass the original Part entity
                                    .OrderBy(dto => dto.Name) // Order by rule name
                                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} compatibility rules for PartID {PartId}.", summaryDtos.Count, partId);
                return summaryDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving compatibility rules for PartID {PartId}.", partId);
                throw new ServiceException($"Could not retrieve compatibility rules for PartID {partId}.", ex);
            }
        }



        #endregion

        #region Delete Method

        /// <summary>
        /// Deletes a part compatibility rule by its ID.
        /// Associated junction table entries (applicable trims, engines, etc.) should be cascade deleted by the database.
        /// </summary>
        /// <param name="ruleId">The ID of the rule to delete.</param>
        /// <exception cref="ServiceException">Thrown if the rule is not found or if a database error occurs.</exception>
        /// <exception cref="ArgumentException">Thrown if ruleId is invalid.</exception>
        public async Task DeleteRuleAsync(int ruleId)
        {
            if (ruleId <= 0)
            {
                _logger.LogWarning("Attempted to delete compatibility rule with invalid ID: {RuleId}", ruleId);
                throw new ArgumentException("Rule ID must be greater than zero for deletion.", nameof(ruleId));
            }

            _logger.LogInformation("Attempting to delete compatibility rule with ID: {RuleId}", ruleId);

            // 1. Fetch Existing Entity to ensure it exists and for logging its name
            var ruleToDelete = await _unitOfWork.PartCompatibilityRules.GetByIdAsync(ruleId);
            if (ruleToDelete == null)
            {
                string notFoundMsg = $"PartCompatibilityRule with ID {ruleId} not found for deletion.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
            }

            // Note: Dependency checks on whether this rule is actively used by a Part
            // is already handled by the relationship Part -> PartCompatibilityRule.
            // If a Part is deleted, its rules are cascade deleted (as per PartCompatibilityRuleConfiguration).
            // If we are deleting a rule, it implies it's being explicitly removed from a part or from a template list.

            // If this rule IS a template (`IsTemplate = true`) and other rules were `CopiedFromRuleId == ruleId`,
            // you might have business logic here to either:
            //  a) Prevent deletion of the template.
            //  b) Nullify `CopiedFromRuleId` on those child rules.
            //  c) Delete the child rules as well (cascade, if that's the desired behavior for templates).
            // For now, we'll assume a simple delete. Add such logic if needed for your template system.

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 2. Delete the Entity
                // The repository's Delete method marks the entity for deletion.
                // EF Core, when SaveChangesAsync is called, will generate SQL to delete the rule.
                // If CascadeOnDelete is configured correctly from PartCompatibilityRule to its
                // junction tables (e.g., PartCompatibilityRuleTrimLevels), the database
                // will automatically delete those related junction records.
                _unitOfWork.PartCompatibilityRules.Delete(ruleToDelete);

                // 3. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("PartCompatibilityRule ID {RuleId} and Name '{RuleName}' deleted successfully from database.",
                    ruleId, ruleToDelete.Name);

                // 4. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Compatibility rule delete process completed for ID: {RuleId}", ruleId);
            }
            catch (DbUpdateException dbEx) // Catches issues during SaveChangesAsync
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error occurred while deleting compatibility rule ID {RuleId}. It might be unexpectedly in use or a constraint violated.", ruleId);
                // Check for specific FK constraint issues if cascade delete isn't working as expected
                // or if other unexpected constraints are hit.
                throw new ServiceException("A database error occurred while deleting the compatibility rule.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An unexpected error occurred while deleting compatibility rule ID {RuleId}", ruleId);
                throw new ServiceException($"An error occurred while deleting compatibility rule with ID {ruleId}.", ex);
            }
        }

        #endregion


        #region Find (Compatibility Querying)

        /// <summary>
        /// Finds DISTINCT Part IDs that have at least one active compatibility rule matching the given vehicle specification.
        /// Can be further filtered by criteria applicable to the rules or the parts they belong to.
        /// </summary>
        public async Task<IEnumerable<int>> FindPartIdsMatchingVehicleSpecAsync(
            VehicleSpecificationDto vehicleSpec,
            PartCompatibilityRuleFilterDto? ruleAndPartFilters = null)
        {
            ArgumentNullException.ThrowIfNull(vehicleSpec, nameof(vehicleSpec));
            ruleAndPartFilters ??= new PartCompatibilityRuleFilterDto();

            _logger.LogInformation("Finding part IDs matching VehicleSpec: {@VehicleSpec} with RuleFilters: {@RuleFilters}",
                vehicleSpec, ruleAndPartFilters);

            try
            {
                // 1. Build a base filter for the repository call to narrow down rules initially.
                Expression<Func<PartCompatibilityRule, bool>>? repoFilter = BuildRuleRepoFilter(vehicleSpec, ruleAndPartFilters);

                // 2. Fetch candidate rules with ALL necessary details for in-memory evaluation.
                // GetAllWithDetailsAsync should load Part, Make, Model, and all Applicable... collections with their taxonomy entities.
                IEnumerable<PartCompatibilityRule> candidateRules =
                    await _unitOfWork.PartCompatibilityRules.GetAllWithDetailsAsync(repoFilter);

                // 3. Apply additional in-memory filters from ruleAndPartFilters that couldn't be translated easily to repoFilter
                if (ruleAndPartFilters.OnlyActiveParts)
                {
                    candidateRules = candidateRules.Where(rule => rule.Part != null && rule.Part.IsActive);
                }
                if (ruleAndPartFilters.PartCategoryId.HasValue)
                {
                    candidateRules = candidateRules.Where(rule => rule.Part != null && rule.Part.CategoryId == ruleAndPartFilters.PartCategoryId.Value);
                }
                // Note: OnlyActiveRules is handled by repoFilter if possible, or here.

                // 4. Evaluate each candidate rule against the vehicleSpec (in-memory)
                var matchingPartIds = new HashSet<int>(); // Use HashSet for distinct IDs

                foreach (var rule in candidateRules) // Already filtered by IsActive if OnlyActiveRules = true in repoFilter
                {
                    // Year Match (already part of repoFilter if possible, or re-evaluate for precision)
                    bool yearMatch = (!rule.YearStart.HasValue && !rule.YearEnd.HasValue) ||
                                     (rule.YearStart.HasValue && !rule.YearEnd.HasValue && vehicleSpec.Year >= rule.YearStart.Value) ||
                                     (!rule.YearStart.HasValue && rule.YearEnd.HasValue && vehicleSpec.Year <= rule.YearEnd.Value) ||
                                     (rule.YearStart.HasValue && rule.YearEnd.HasValue && vehicleSpec.Year >= rule.YearStart.Value && vehicleSpec.Year <= rule.YearEnd.Value);
                    if (!yearMatch) continue;

                    // Attribute Matching using the in-memory helper
                    bool trimMatch = CheckAttributeMatchInternal(
                        rule.ApplicableTrimLevels?.Select(atl => new AttributeMatchItem(atl.TrimLevelId, atl.IsExclusion)),
                        vehicleSpec.TrimLevelId);
                    if (!trimMatch) continue;

                    bool engineMatch = CheckAttributeMatchInternal(
                        rule.ApplicableEngineTypes?.Select(aet => new AttributeMatchItem(aet.EngineTypeId, aet.IsExclusion)),
                        vehicleSpec.EngineTypeId);
                    if (!engineMatch) continue;

                    bool transmissionMatch = CheckAttributeMatchInternal(
                        rule.ApplicableTransmissionTypes?.Select(att => new AttributeMatchItem(att.TransmissionTypeId, att.IsExclusion)),
                        vehicleSpec.TransmissionTypeId);
                    if (!transmissionMatch) continue;

                    bool bodyMatch = CheckAttributeMatchInternal(
                        rule.ApplicableBodyTypes?.Select(abt => new AttributeMatchItem(abt.BodyTypeId, abt.IsExclusion)),
                        vehicleSpec.BodyTypeId);
                    if (!bodyMatch) continue;

                    // If all specific attribute checks pass for this rule
                    matchingPartIds.Add(rule.PartId);
                }

                _logger.LogInformation("Found {Count} distinct part IDs matching VehicleSpec: {@VehicleSpec} with RuleFilters: {@RuleFilters}",
                    matchingPartIds.Count, vehicleSpec, ruleAndPartFilters);

                return matchingPartIds.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding part IDs for VehicleSpec: {@VehicleSpec} with RuleFilters: {@RuleFilters}", vehicleSpec, ruleAndPartFilters);
                throw new ServiceException("Could not find compatible part IDs due to an internal error.", ex);
            }
        }

        /// <summary>
        /// Builds a base predicate for repository filtering of PartCompatibilityRules.
        /// </summary>
        private Expression<Func<PartCompatibilityRule, bool>>? BuildRuleRepoFilter(
            VehicleSpecificationDto vehicleSpec,
            PartCompatibilityRuleFilterDto ruleAndPartFilters)
        {
            var predicate = LinqKit.PredicateBuilder.New<PartCompatibilityRule>(true); // Start with a true predicate

            if (ruleAndPartFilters.OnlyActiveRules)
            {
                predicate = predicate.And(r => r.IsActive);
            }

            // Filter by MakeId on the rule, if vehicleSpec.MakeId is provided
            if (vehicleSpec.MakeId.HasValue)
            {
                predicate = predicate.And(r => !r.MakeId.HasValue || r.MakeId == vehicleSpec.MakeId.Value);
            }
            else // If vehicleSpec has no MakeId, rule must also have no MakeId (or be "Any Make")
            {
                predicate = predicate.And(r => !r.MakeId.HasValue);
            }

            // Filter by ModelId on the rule, if vehicleSpec.ModelId is provided
            if (vehicleSpec.ModelId.HasValue)
            {
                predicate = predicate.And(r => !r.ModelId.HasValue || r.ModelId == vehicleSpec.ModelId.Value);
            }
            else // If vehicleSpec has no ModelId, rule must also have no ModelId (or be "Any Model")
            {
                predicate = predicate.And(r => !r.ModelId.HasValue);
            }

            // Add more pre-filters here if they can be efficiently translated to SQL
            // For example, a broad year overlap.
            // (r => (r.YearStart == null || r.YearStart <= vehicleSpec.Year) &&
            //       (r.YearEnd == null || r.YearEnd >= vehicleSpec.Year))
            // This simple year overlap is a good candidate for DB-side filtering.

            // If predicate remains just "true" (no conditions added), return null so repo fetches all.
            return predicate.IsStarted ? predicate : null;
        }


        /// <summary>
        /// Checks if a specific part is compatible with a given detailed vehicle specification.
        /// (Implementation using in-memory CheckAttributeMatchInternal - looks good)
        /// </summary>
        public async Task<CompatibilityMatchResultDto> IsPartCompatibleAsync(int partId, VehicleSpecificationDto vehicleSpec)
        {
            // ... (your existing robust implementation is good here) ...
            // It should fetch rules for `partId` using `_unitOfWork.PartCompatibilityRules.GetRulesByPartIdAsync(partId, true)`
            // and then iterate and use `CheckAttributeMatchInternal`.
            ArgumentNullException.ThrowIfNull(vehicleSpec, nameof(vehicleSpec));
            if (partId <= 0)
            {
                _logger.LogWarning("IsPartCompatibleAsync called with invalid PartID: {PartId}", partId);
                return new CompatibilityMatchResultDto(false, "Invalid Part ID provided.", new List<string>());
            }

            _logger.LogInformation("Checking compatibility for PartID {PartId} with VehicleSpec: {@VehicleSpec}", partId, vehicleSpec);

            try
            {
                var rulesForPart = await _unitOfWork.PartCompatibilityRules.GetRulesByPartIdAsync(partId, includeDetails: true);

                if (rulesForPart == null || !rulesForPart.Any())
                {
                    _logger.LogInformation("PartID {PartId} has no compatibility rules defined.", partId);
                    return new CompatibilityMatchResultDto(false, "Part has no compatibility rules.", new List<string>());
                }

                var part = await _unitOfWork.Parts.GetByIdAsync(partId); // Needed for Part.IsActive check
                if (part == null || !part.IsActive)
                {
                    _logger.LogWarning("PartID {PartId} not found or is inactive for compatibility check.", partId);
                    return new CompatibilityMatchResultDto(false, "Part not found or is inactive.", new List<string>());
                }


                var matchingRuleNames = new List<string>();
                foreach (var rule in rulesForPart.Where(r => r.IsActive))
                {
                    bool makeMatch = !rule.MakeId.HasValue || rule.MakeId == vehicleSpec.MakeId;
                    if (!makeMatch) continue;

                    bool modelMatch = !rule.ModelId.HasValue || rule.ModelId == vehicleSpec.ModelId;
                    if (!modelMatch) continue;

                    bool yearMatch = (!rule.YearStart.HasValue && !rule.YearEnd.HasValue) ||
                                     (rule.YearStart.HasValue && !rule.YearEnd.HasValue && vehicleSpec.Year >= rule.YearStart.Value) ||
                                     (!rule.YearStart.HasValue && rule.YearEnd.HasValue && vehicleSpec.Year <= rule.YearEnd.Value) ||
                                     (rule.YearStart.HasValue && rule.YearEnd.HasValue && vehicleSpec.Year >= rule.YearStart.Value && vehicleSpec.Year <= rule.YearEnd.Value);
                    if (!yearMatch) continue;

                    bool trimMatch = CheckAttributeMatchInternal(
                        rule.ApplicableTrimLevels?.Select(atl => new AttributeMatchItem(atl.TrimLevelId, atl.IsExclusion)),
                        vehicleSpec.TrimLevelId);
                    if (!trimMatch) continue;

                    bool engineMatch = CheckAttributeMatchInternal(
                        rule.ApplicableEngineTypes?.Select(aet => new AttributeMatchItem(aet.EngineTypeId, aet.IsExclusion)),
                        vehicleSpec.EngineTypeId);
                    if (!engineMatch) continue;

                    bool transmissionMatch = CheckAttributeMatchInternal(
                        rule.ApplicableTransmissionTypes?.Select(att => new AttributeMatchItem(att.TransmissionTypeId, att.IsExclusion)),
                        vehicleSpec.TransmissionTypeId);
                    if (!transmissionMatch) continue;

                    bool bodyMatch = CheckAttributeMatchInternal(
                        rule.ApplicableBodyTypes?.Select(abt => new AttributeMatchItem(abt.BodyTypeId, abt.IsExclusion)),
                        vehicleSpec.BodyTypeId);
                    if (!bodyMatch) continue;

                    matchingRuleNames.Add(rule.Name);
                }

                if (matchingRuleNames.Any())
                {
                    _logger.LogInformation("PartID {PartId} IS compatible with VehicleSpec. Matching rules: {Rules}", partId, string.Join(", ", matchingRuleNames));
                    return new CompatibilityMatchResultDto(true, $"Matches rule(s): {string.Join(", ", matchingRuleNames)}", matchingRuleNames);
                }

                _logger.LogInformation("PartID {PartId} is NOT compatible with VehicleSpec.", partId);
                return new CompatibilityMatchResultDto(false, "No active compatibility rules matched the vehicle specification.", new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking part compatibility for PartID {PartId} and VehicleSpec: {@VehicleSpec}", partId, vehicleSpec);
                throw new ServiceException("Could not determine part compatibility due to an internal error.", ex);
            }
        }


        // Renamed CheckAttributeMatch to CheckAttributeMatchInternal to avoid conflict if you had it elsewhere.
        // And using the private record AttributeMatchItem.
        private record AttributeMatchItem(int Id, bool IsExclusion);
        private bool CheckAttributeMatchInternal(IEnumerable<AttributeMatchItem>? ruleAttributes, int? vehicleAttributeId)
        {
            if (ruleAttributes == null || !ruleAttributes.Any()) // Rule applies to ANY for this attribute type
            {
                return true;
            }

            if (vehicleAttributeId.HasValue) // Vehicle has a specific attribute
            {
                if (ruleAttributes.Any(attr => attr.Id == vehicleAttributeId.Value && attr.IsExclusion))
                {
                    return false; // Excluded
                }
                if (ruleAttributes.Any(attr => !attr.IsExclusion)) // If there are any inclusion rules
                {
                    // It must be one of the included attributes
                    return ruleAttributes.Any(attr => attr.Id == vehicleAttributeId.Value && !attr.IsExclusion);
                }
                return true; // No specific includes defined for the rule, and not excluded, so it passes
            }
            else // Vehicle does NOT have this specific attribute type specified (e.g., vehicleSpec.TrimLevelId is null)
            {
                // If the rule *requires* specific includes for this attribute type, then it's a mismatch.
                if (ruleAttributes.Any(attr => !attr.IsExclusion))
                {
                    return false;
                }
                // Otherwise (rule is "Any" or only has exclusions), and vehicle has no spec for this, so it passes.
                return true;
            }
        }

        #endregion


        #region Helpers

        private PartCompatibilityRuleDetailDto MapRuleToDetailDto(PartCompatibilityRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            return new PartCompatibilityRuleDetailDto
            {
                Id = rule.Id,
                PartId = rule.PartId,
                PartName = rule.Part?.Name ?? "N/A", // Assumes Part is loaded
                PartNumber = rule.Part?.PartNumber ?? "N/A", // Assumes Part is loaded
                Name = rule.Name,
                Description = rule.Description,
                MakeId = rule.MakeId,
                MakeName = rule.Make?.Name, // Assumes Make is loaded
                ModelId = rule.ModelId,
                ModelName = rule.Model?.Name, // Assumes Model is loaded
                YearStart = rule.YearStart,
                YearEnd = rule.YearEnd,
                Notes = rule.Notes,
                IsActive = rule.IsActive,
                IsTemplate = rule.IsTemplate,
                CopiedFromRuleId = rule.CopiedFromRuleId,
                CreatedAt = rule.CreatedAt,
                ModifiedAt = rule.ModifiedAt,

                ApplicableTrimLevels = rule.ApplicableTrimLevels?.Select(atl =>
                    new PartCompatibilityRuleApplicableAttributeDto(
                        atl.TrimLevelId,
                        atl.TrimLevel?.Name ?? "N/A", // Assumes TrimLevel within atl is loaded
                        atl.IsExclusion))
                    .OrderBy(a => a.AttributeName).ToList() ?? new List<PartCompatibilityRuleApplicableAttributeDto>(),

                ApplicableEngineTypes = rule.ApplicableEngineTypes?.Select(aet =>
                    new PartCompatibilityRuleApplicableAttributeDto(
                        aet.EngineTypeId,
                        aet.EngineType?.Name ?? "N/A", // Assumes EngineType within aet is loaded
                        aet.IsExclusion))
                    .OrderBy(a => a.AttributeName).ToList() ?? new List<PartCompatibilityRuleApplicableAttributeDto>(),

                ApplicableTransmissionTypes = rule.ApplicableTransmissionTypes?.Select(att =>
                    new PartCompatibilityRuleApplicableAttributeDto(
                        att.TransmissionTypeId,
                        att.TransmissionType?.Name ?? "N/A", // Assumes TransmissionType within att is loaded
                        att.IsExclusion))
                    .OrderBy(a => a.AttributeName).ToList() ?? new List<PartCompatibilityRuleApplicableAttributeDto>(),

                ApplicableBodyTypes = rule.ApplicableBodyTypes?.Select(abt =>
                    new PartCompatibilityRuleApplicableAttributeDto(
                        abt.BodyTypeId,
                        abt.BodyType?.Name ?? "N/A", // Assumes BodyType within abt is loaded
                        abt.IsExclusion))
                    .OrderBy(a => a.AttributeName).ToList() ?? new List<PartCompatibilityRuleApplicableAttributeDto>()
            };
        }

        private PartCompatibilityRuleSummaryDto MapRuleToSummaryDto(PartCompatibilityRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            // These display strings will require some logic
            string makeDisplay = rule.Make?.Name ?? "Any Make";
            string modelDisplay = rule.Model?.Name ?? (rule.MakeId.HasValue ? "Any Model for " + rule.Make!.Name : "Any Model");

            string yearDisplay = "Any Year";
            if (rule.YearStart.HasValue && rule.YearEnd.HasValue)
            {
                yearDisplay = rule.YearStart == rule.YearEnd
                    ? rule.YearStart.Value.ToString()
                    : $"{rule.YearStart.Value} - {rule.YearEnd.Value}";
            }
            else if (rule.YearStart.HasValue)
            {
                yearDisplay = $"From {rule.YearStart.Value}";
            }
            else if (rule.YearEnd.HasValue)
            {
                yearDisplay = $"Up to {rule.YearEnd.Value}";
            }

            return new PartCompatibilityRuleSummaryDto(
                rule.Id,
                rule.Name,
                rule.Description,
                makeDisplay,
                modelDisplay,
                yearDisplay,
                FormatApplicableAttributesForSummary(rule.ApplicableTrimLevels?.Select(atl => new { atl.TrimLevel?.Name, atl.IsExclusion }), "Trim"),
                FormatApplicableAttributesForSummary(rule.ApplicableEngineTypes?.Select(aet => new { aet.EngineType?.Name, aet.IsExclusion }), "Engine"),
                FormatApplicableAttributesForSummary(rule.ApplicableTransmissionTypes?.Select(att => new { att.TransmissionType?.Name, att.IsExclusion }), "Transmission"),
                FormatApplicableAttributesForSummary(rule.ApplicableBodyTypes?.Select(abt => new { abt.BodyType?.Name, abt.IsExclusion }), "Body Type"),
                rule.IsActive,
                rule.IsTemplate,
                rule.CreatedAt,
                rule.ModifiedAt,
                rule.PartId,
                rule.Part?.PartNumber ?? "N/A" // Assumes Part is loaded with the rule
            );
        }

        /// <summary>
        /// Helper to format a list of applicable attributes (like trims, engines) for summary display.
        /// </summary>
        private string FormatApplicableAttributesForSummary(
            IEnumerable<dynamic>? attributes, // Expecting collection of { Name (string), IsExclusion (bool) }
            string attributeTypeSingular)
        {
            if (attributes == null || !attributes.Any())
            {
                return $"Any {attributeTypeSingular}";
            }

            var included = new List<string>();
            var excluded = new List<string>();

            foreach (var attr in attributes)
            {
                string? name = attr.Name; // Access dynamic property
                bool isExclusion = attr.IsExclusion; // Access dynamic property

                if (string.IsNullOrWhiteSpace(name)) continue;

                if (isExclusion)
                {
                    excluded.Add(name);
                }
                else
                {
                    included.Add(name);
                }
            }

            if (!included.Any() && !excluded.Any()) return $"Any {attributeTypeSingular}"; // Should not happen if attributes list was not empty initially

            string result = "";
            if (included.Any())
            {
                result = string.Join(", ", included.OrderBy(n => n));
            }
            else // Only exclusions, or no includes means "All except these exclusions"
            {
                result = $"All {attributeTypeSingular}s";
            }

            if (excluded.Any())
            {
                result += (string.IsNullOrEmpty(result) || result.StartsWith("All ") ? "" : " ") + $"(Excludes: {string.Join(", ", excluded.OrderBy(n => n))})";
            }

            return result.Trim();
        }


        /// <summary>
        /// Generic helper to update a collection of applicable attributes (junction entities) for a rule.
        /// It clears existing attributes linked to the rule and adds the new ones.
        /// </summary>
        private void UpdateApplicableAttributes<TJunctionEntity>(
            ICollection<TJunctionEntity> existingJunctionEntities, // e.g., existingRule.ApplicableTrimLevels
            List<PartCompatibilityRuleApplicableAttributeDto>? newAttributeDtos, // e.g., updateRuleDto.ApplicableTrimLevels
                                                                                 // ruleId is implicitly known if existingJunctionEntities is part of the existingRule object.
                                                                                 // However, when creating NEW junction entities, they need the ruleId.
                                                                                 // The createJunctionEntity func will now be responsible for setting the FK to the rule.
            Func<PartCompatibilityRuleApplicableAttributeDto, TJunctionEntity> createJunctionEntity,
            Domain.Interfaces.Repository.Base.IBaseRepository<TJunctionEntity> junctionRepository)
            where TJunctionEntity : class // Or BaseEntity if your junction tables inherit it AND have their own Id
        {
            // 1. Remove all existing junction entities for this rule and attribute type
            if (existingJunctionEntities != null)
            {
                var itemsToDelete = existingJunctionEntities.ToList(); // Create a copy to modify original
                foreach (var item in itemsToDelete)
                {
                    junctionRepository.Delete(item); // Mark for deletion
                }
                // After marking for deletion, EF Core will handle removing them from the collection
                // or you can clear it if you prefer the collection to be empty immediately in memory.
                // For safety, we'll let EF Core manage the removal from the collection during SaveChanges
                // if the relationship is correctly configured with CascadeDelete or if items are detached.
                // If not using CascadeDelete for the junction from the rule, then clearing here is important.
                // Let's assume CascadeDelete is set on the rule's collections of junction entities.
                // So, when new items are added, EF Core's change tracking will figure out the diff.
                // A safer "clear and re-add" if CascadeDelete is not perfectly set up for all cases:
                // First, delete them explicitly via repository.
                // Then clear the navigation collection on the parent.
                existingJunctionEntities.Clear(); // Clear the navigation collection on the parent entity
            }
            else if (existingJunctionEntities == null && newAttributeDtos != null && newAttributeDtos.Any(a => a.AttributeId > 0))
            {
                // This case should ideally not happen if GetRuleByIdWithDetailsAsync always initializes collections.
                // If it can, then the parent entity (existingRule) needs to have its collection initialized,
                // e.g., existingRule.ApplicableTrimLevels = new List<PartCompatibilityRuleTrimLevel>();
                // This situation indicates an issue with how existingRule was loaded or initialized.
                _logger.LogWarning("UpdateApplicableAttributes called with a null existingJunctionEntities collection but new attributes to add. This might indicate incomplete loading of the parent rule entity.");
                // Depending on your design, you might throw or attempt to initialize the collection on the parent.
                // For now, we'll proceed assuming the collection on the parent will be managed by EF Core if we add to it.
            }


            // 2. Add new junction entities from DTO
            if (newAttributeDtos != null)
            {
                foreach (var attrDto in newAttributeDtos.Where(a => a.AttributeId > 0)) // Process only valid IDs
                {
                    var newJunctionEntity = createJunctionEntity(attrDto);
                    // The createJunctionEntity Func is responsible for setting PartCompatibilityRuleId on newJunctionEntity

                    // Add to the parent's collection (EF Core tracks this if parent is tracked)
                    // If existingJunctionEntities was initially null and not part of a tracked parent, this would fail.
                    // But since it's existingRule.ApplicableTrimLevels, existingRule is tracked.
                    existingJunctionEntities?.Add(newJunctionEntity);
                }
            }
        }


        #endregion
    }
}
