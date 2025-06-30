using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Application.DTOs.PartImage;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Storage;
using AutoFusionPro.Core.Enums.DTOEnums;
using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Models;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class PartService : IPartService
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PartService> _logger;
        private readonly IImageFileService _imageFileService; // Will be used later
        private readonly IPartCompatibilityRuleService _partCompatibilityRuleService; // Will be used later

        private readonly IValidator<CreatePartDto> _createPartValidator;
        private readonly IValidator<UpdatePartDto> _updatePartValidator;

        private readonly IValidator<CreatePartImageDto> _createPartImageValidator;
        private readonly IValidator<UpdatePartImageDto> _updatePartImageValidator;


        #endregion

        #region Constructor
        public PartService(
                   IUnitOfWork unitOfWork,
                   ILogger<PartService> logger,
                   IImageFileService imageFileService, 
                   IValidator<CreatePartDto> createPartValidator,
                   IValidator<UpdatePartDto> updatePartValidator,
                   IValidator<CreatePartImageDto> createPartImageValidator,
                   IValidator<UpdatePartImageDto> updatePartImageValidator,
                   IPartCompatibilityRuleService partCompatibilityRuleService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageFileService = imageFileService ?? throw new ArgumentNullException(nameof(imageFileService));
            _createPartValidator = createPartValidator ?? throw new ArgumentNullException(nameof(createPartValidator));
            _updatePartValidator = updatePartValidator ?? throw new ArgumentNullException(nameof(updatePartValidator));
            _partCompatibilityRuleService = partCompatibilityRuleService ?? throw new ArgumentNullException(nameof(partCompatibilityRuleService));

            _createPartImageValidator = createPartImageValidator ?? throw new ArgumentNullException(nameof(createPartImageValidator));
            _updatePartImageValidator = updatePartImageValidator ?? throw new ArgumentNullException(nameof(updatePartImageValidator)); 

        }

        #endregion

        #region CRUD Operations


        /// <summary>
        /// Gets detailed information for a specific part by its ID.
        /// Includes Category, selected Suppliers, and selected CompatibleVehicles.
        /// </summary>
        public async Task<PartDetailDto?> GetPartDetailsByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get part details with invalid ID: {PartId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve part details for ID: {PartId}", id);

            try
            {
                // Fetch the Part entity with all necessary related data for the PartDetailDto
                var partEntity = await _unitOfWork.Parts.GetByIdWithDetailsAsync(
                    id,
                    includeCategory: true,
                    includeSuppliers: false,
                    includeImages: false,
                    includeCompatibilityRules: false
                );

                if (partEntity == null)
                {
                    _logger.LogWarning("Part with ID {PartId} not found.", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved part details for ID {PartId}: {PartName}", id, partEntity.Name);
                var partDetailDto = MapPartToDetailDto(partEntity);

                if (partDetailDto != null && partEntity != null)
                {
                    partDetailDto.Images = (await GetImagesForPartAsync(partEntity.Id))?.ToList() ?? new List<PartImageDto>();
                    partDetailDto.Suppliers = (await GetSuppliersForPartAsync(partEntity.Id))?.ToList() ?? new List<PartSupplierDto>();
                    partDetailDto.CompatibilityRules = (await _partCompatibilityRuleService.GetRulesForPartAsync(partEntity.Id))?.ToList() ?? new List<PartCompatibilityRuleSummaryDto>();
                }

                return partDetailDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving part details for ID {PartId}.", id);
                throw new ServiceException($"Could not retrieve part details for ID {id}.", ex);
            }
        }


        // TODO : TO BE REMOVED OR ADJUSTED 
        public async Task<IEnumerable<PartSummaryDto>> GetAllPartsSummariesAsync()
        {
            _logger.LogInformation("Attempting to retrieve Parts Summaries");
            try
            {
                // Use the specific repository method
                var allParts = await _unitOfWork.Parts.GetAllAsync();

                var summaryDtos = allParts.Select(MapPartToSummaryDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} part summaries.", summaryDtos.Count);
                return summaryDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all part summaries.");
                throw new ServiceException("Could not retrieve all part summaries.", ex);
            }
        }

        /// <summary>
        /// Gets detailed information for a specific part by its unique PartNumber.
        /// Includes Category. Other details like Suppliers/Compatibility can be loaded separately if needed.
        /// </summary>
        public async Task<PartDetailDto?> GetPartDetailsByPartNumberAsync(string partNumber)
        {
            if (string.IsNullOrWhiteSpace(partNumber))
            {
                _logger.LogWarning("Attempted to get part details with null or empty PartNumber.");
                return null; // Or throw ArgumentException
            }
            _logger.LogInformation("Attempting to retrieve part details for PartNumber: {PartNumber}", partNumber);

            try
            {
                // Fetch the Part entity including its category.
                // If full details (suppliers, compatibility) are needed, this repo method might need enhancement
                // or we make separate calls if GetByIdWithDetailsAsync is more suitable after getting ID.
                // For now, assume GetByPartNumberAsync from repo includes category.
                // If you need full details, you'd get the ID then call GetPartDetailsByIdAsync.
                var partEntity = await _unitOfWork.Parts.GetByPartNumberAsync(partNumber, includeCategory: true);

                if (partEntity == null)
                {
                    _logger.LogWarning("Part with PartNumber '{PartNumber}' not found.", partNumber);
                    return null;
                }

                // To return a full PartDetailDto, we need suppliers and compatibility.
                // It's often better to fetch the full entity once by ID if all details are needed.
                // Alternative: Call GetByIdWithDetailsAsync after finding the ID.
                // For now, let's assume we might only return partial details or this method is for quick lookup.
                // If the primary use case for GetByPartNumber is to get *full* details, then:
                // var fullPartEntity = await GetPartDetailsByIdAsync(partEntity.Id); return fullPartEntity;
                // Let's assume for now this method primarily focuses on the part and its category.
                // The PartDetailDto can be sparsely populated.

                _logger.LogInformation("Successfully retrieved part details for PartNumber {PartNumber}: {PartName}", partNumber, partEntity.Name);
                // If we only have category, suppliers and compatibility will be empty lists in the DTO.
                // This might be acceptable for some use cases of GetByPartNumber.
                // If not, fetch the full details via GetByIdWithDetailsAsync after getting the ID.
                // To provide a more complete DTO from this method, we'd need to load more here or chain calls.
                // For consistency and to fulfill PartDetailDto, let's fetch full details if partEntity is found.
                if (partEntity.Id > 0)
                {
                    return await GetPartDetailsByIdAsync(partEntity.Id); // Call the more detailed fetcher
                }
                return MapPartToDetailDto(partEntity); // Fallback if ID somehow wasn't set (should not happen)

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving part details for PartNumber '{PartNumber}'.", partNumber);
                throw new ServiceException($"Could not retrieve part details for PartNumber '{partNumber}'.", ex);
            }
        }

        /// <summary>
        /// Gets a paginated list of part summaries based on filter criteria.
        /// </summary>
        public async Task<PagedResult<PartSummaryDto>> GetFilteredPartSummariesAsync(
            PartFilterCriteriaDto filterCriteria, int pageNumber, int pageSize)
        {
            ArgumentNullException.ThrowIfNull(filterCriteria, nameof(filterCriteria));
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;    // Default page size
            if (pageSize > 100) pageSize = 100;  // Max page size safeguard

            _logger.LogInformation("Attempting to retrieve filtered part summaries. Page: {Page}, Size: {Size}, Criteria: {@Criteria}",
                pageNumber, pageSize, filterCriteria);

            try
            {
                IEnumerable<int>? compatiblePartIds = null;
                bool filterByCompatibility = false;

                // Check if vehicle-specific filters are present in PartFilterCriteriaDto
                if (filterCriteria.MakeId.HasValue ||
                    filterCriteria.ModelId.HasValue ||
                    filterCriteria.TrimId.HasValue || // Assuming TrimId is in your DTO
                    filterCriteria.SpecificYear.HasValue /* || other vehicle criteria */)
                {
                    filterByCompatibility = true;
                    var vehicleSpec = new VehicleSpecificationDto( // From PartCompatibilityDtos namespace
                        filterCriteria.MakeId, filterCriteria.ModelId, filterCriteria.TrimId,
                        null, null, null, // Assuming Transmission, Engine, Body not in PartFilterCriteriaDto for now
                        filterCriteria.SpecificYear ?? DateTime.Now.Year // Needs a year; default or handle if null
                    );

                    // Prepare rule filters (e.g., only consider active parts/rules)
                    var ruleFilters = new PartCompatibilityRuleFilterDto(
                        OnlyActiveRules: true, // Example
                        OnlyActiveParts: filterCriteria.IsActive ?? true, // Align with part filter
                        PartCategoryId: filterCriteria.CategoryId
                    );

                    compatiblePartIds = await _partCompatibilityRuleService.FindPartIdsMatchingVehicleSpecAsync(vehicleSpec, ruleFilters);

                    if (compatiblePartIds == null || !compatiblePartIds.Any())
                    {
                        _logger.LogInformation("No parts found matching vehicle specification via compatibility rules.");
                        return new PagedResult<PartSummaryDto> { Items = Enumerable.Empty<PartSummaryDto>(), TotalCount = 0, PageNumber = pageNumber, PageSize = pageSize };
                    }
                    _logger.LogInformation("Found {Count} vehicle spec IDs to filter parts by.", compatiblePartIds.Count());
                }

                // Now call the IPartRepository method, passing compatiblePartIds if applicable.
                // The IPartRepository methods need to be updated to accept 'IEnumerable<int>? restrictToPartIds = null'
                // and add 'query = query.Where(p => restrictToPartIds.Contains(p.Id))' if provided.

                var partEntities = await _unitOfWork.Parts.GetFilteredPartsPagedAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    filterPredicate: null, // Direct predicate not used if using DTO criteria below
                    categoryId: filterCriteria.CategoryId,
                    manufacturer: filterCriteria.Manufacturer,
                    supplierId: filterCriteria.SupplierId,
                    // compatibleVehicleId: filterCriteria.CompatibleVehicleId, // This might be redundant if using the list below
                    restrictToPartIds: filterByCompatibility ? compatiblePartIds : null, // Pass the list of IDs
                     searchTerm: filterCriteria.SearchTerm,
                    isActive: filterCriteria.IsActive,
                    isLowStock: filterCriteria.StockStatus == StockStatusFilter.LowStock ? true :
                                filterCriteria.StockStatus == StockStatusFilter.InStock ? false : // Assuming InStock means NOT low stock for this flag
                                (bool?)null, // For All or OutOfStock, isLowStock flag is not directly applicable
                    includeCategory: true, // Essential for PartSummaryDto's CategoryName
                    includeSuppliers: false // Typically not needed for summary

                );

                var totalCount = await _unitOfWork.Parts.GetTotalFilteredPartsCountAsync(
                    filterPredicate: null,
                    categoryId: filterCriteria.CategoryId,
                    manufacturer: filterCriteria.Manufacturer,
                    supplierId: filterCriteria.SupplierId,
                    restrictToPartIds: filterByCompatibility ? compatiblePartIds : null,
                    searchTerm: filterCriteria.SearchTerm,
                    isActive: filterCriteria.IsActive,
                    isLowStock: filterCriteria.StockStatus == StockStatusFilter.LowStock ? true :
                                filterCriteria.StockStatus == StockStatusFilter.InStock ? false :
                                (bool?)null
                    
                );

                var summaryDtos = partEntities.Select(MapPartToSummaryDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} part summaries for page {Page}. Total matching: {Total}",
                    summaryDtos.Count, pageNumber, totalCount);

                return new PagedResult<PartSummaryDto>
                {
                    Items = summaryDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                    // TotalPages will be calculated by PagedResult or UI
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving filtered part summaries with criteria: {@Criteria}", filterCriteria);
                throw new ServiceException("Could not retrieve part summaries.", ex);
            }
        }


        /// <summary>
        /// Gets a list of part summaries for parts that are low on stock.
        /// </summary>
        public async Task<IEnumerable<PartSummaryDto>> GetLowStockPartSummariesAsync(int? topN = null)
        {
            _logger.LogInformation("Attempting to retrieve low stock part summaries. TopN: {TopN}", topN ?? 0);
            try
            {
                // Use the specific repository method
                var lowStockPartEntities = await _unitOfWork.Parts.GetLowStockPartsAsync(
                    includeCategory: true, // Needed for CategoryName in PartSummaryDto
                    take: topN
                );

                var summaryDtos = lowStockPartEntities.Select(MapPartToSummaryDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} low stock part summaries.", summaryDtos.Count);
                return summaryDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving low stock part summaries.");
                throw new ServiceException("Could not retrieve low stock part summaries.", ex);
            }
        }


        /// <summary>
        /// Creates a new part.
        /// </summary>
        public async Task<PartDetailDto> CreatePartAsync(CreatePartDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto, nameof(createDto));

            _logger.LogInformation("Attempting to create part: PartNumber='{PartNumber}', Name='{PartName}'",
                createDto.PartNumber, createDto.Name);

            // 1. Validate DTO using FluentValidation
            var validationResult = await _createPartValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreatePartDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }
            // Note: The validator should already check for PartNumber uniqueness and Barcode uniqueness (if barcode provided).
            // It also checks if CategoryId exists.

            try
            {

                var newPart = new Part
                {
                    PartNumber = createDto.PartNumber.Trim().ToUpperInvariant(), // Normalize PartNumber
                    Name = createDto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(createDto.Description) ? null : createDto.Description.Trim(),
                    Manufacturer = string.IsNullOrWhiteSpace(createDto.Manufacturer) ? null : createDto.Manufacturer.Trim(),
                    CostPrice = createDto.CostPrice,
                    SellingPrice = createDto.SellingPrice,
                    StockQuantity = createDto.StockQuantity, // Initial/base stock
                    CurrentStock = createDto.StockQuantity,  // For a new part, CurrentStock often starts equal to initial StockQuantity
                    ReorderLevel = createDto.ReorderLevel,
                    MinimumStock = createDto.MinimumStock,
                    Location = string.IsNullOrWhiteSpace(createDto.Location) ? string.Empty : createDto.Location.Trim(),
                    IsActive = createDto.IsActive,
                    IsOriginal = createDto.IsOriginal,
                    Notes = string.IsNullOrWhiteSpace(createDto.Notes) ? null : createDto.Notes.Trim(),
                    Barcode = string.IsNullOrWhiteSpace(createDto.Barcode) ? null : createDto.Barcode.Trim(), // Normalize Barcode
                    StockingUnitOfMeasureId = createDto.StockingUnitOfMeasureId,
                    SalesUnitOfMeasureId = createDto.SalesUnitOfMeasureId,
                    SalesConversionFactor = (createDto.SalesUnitOfMeasureId.HasValue && createDto.SalesUnitOfMeasureId.Value != createDto.StockingUnitOfMeasureId)
                             ? createDto.SalesConversionFactor
                             : null, // Factor is only relevant if SalesUoM is different from StockingUoM
                    PurchaseUnitOfMeasureId = createDto.PurchaseUnitOfMeasureId,
                    PurchaseConversionFactor = (createDto.PurchaseUnitOfMeasureId.HasValue && createDto.PurchaseUnitOfMeasureId.Value != createDto.StockingUnitOfMeasureId)
                                ? createDto.PurchaseConversionFactor
                                : null,
                    CategoryId = createDto.CategoryId,
                    LastRestockDate = DateTime.UtcNow, // Or null if initial stock isn't a "restock"
                    CreatedAt = DateTime.UtcNow // Or handled by BaseEntity/DbContext
                };

                await _unitOfWork.BeginTransactionAsync();



                // 4. Add to Repository
                await _unitOfWork.Parts.AddAsync(newPart);

                // 5. Save Changes to Database (this will assign an ID to newPart)
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Part created successfully: ID={PartId}, PartNumber='{PartNumber}'",
                    newPart.Id, newPart.PartNumber);



                // Initial Images, Suppliers and PartCompatibilityRules are removed in respect to the two stage process, save part, then add other properties

                //// 6. Handle Initial Suppliers and Compatibilities (if provided and if part creation was successful)
                //// These are often better handled as separate calls after the main part is created,
                //// but if included in CreatePartDto, process them here.
                //if (createDto.InitialSuppliers != null && createDto.InitialSuppliers.Any())
                //{
                //    foreach (var supDto in createDto.InitialSuppliers)
                //    {
                //        // Assuming AddSupplierToPartAsync handles validation of SupplierId
                //        await AddSupplierToPartAsync(newPart.Id, supDto);
                //    }
                //}

                // In case we added support for adding initial Part Compatibility Rule, uncomment this!
                // if (createDto.InitialCompatibilityRules != null && createDto.InitialCompatibilityRules.Any())
                // {
                //     foreach (var ruleDto in createDto.InitialCompatibilityRules)
                //     {
                //         // The PartId is newPart.Id
                //         await _partCompatibilityRuleService.CreateRuleForPartAsync(newPart.Id, ruleDto);
                //     }
                // }

                //if (createDto.InitialImages != null && createDto.InitialImages.Any())
                //{
                //    _logger.LogInformation("Adding initial images for Part ID: {PartId}", newPart.Id);
                //    foreach (var imgDto in createDto.InitialImages)
                //    {
                //        // This is tricky because AddImageToPartAsync expects a Stream.
                //        // The UI ViewModel would need to open streams for each SourceClientPath.
                //        // Or, AddImageToPartAsync is refactored or a new helper is made.
                //        // For now, let's assume you have a way to get the stream for imgDto.SourceClientPath.
                //        // This is a simplified example of the call:
                //        // using (var stream = File.OpenRead(imgDto.SourceClientPath))
                //        // {
                //        //    await AddImageToPartAsync(newPart.Id,
                //        //        new CreatePartImageDto(imgDto.Caption, imgDto.IsPrimary, imgDto.DisplayOrder),
                //        //        stream,
                //        //        Path.GetFileName(imgDto.SourceClientPath));
                //        // }
                //    }
                //}


                // Note: If adding suppliers/compatibility fails after part creation, the part is still created.
                // Consider if these should be part of the same transaction, or if failures are acceptable.

                await _unitOfWork.CommitTransactionAsync();

                // 7. Re-fetch with full details for return DTO
                var createdPartWithDetails = await _unitOfWork.Parts.GetByIdWithDetailsAsync(
                    newPart.Id, 
                    includeCategory: true, 
                    includeSuppliers: true, 
                    includeCompatibilityRules: true,
                    includeImages: true);

                if (createdPartWithDetails == null) // Should ideally not happen
                {
                    _logger.LogError("Critical: Failed to re-fetch part with ID {PartId} immediately after creation.", newPart.Id);
                    throw new ServiceException($"Part was created (ID: {newPart.Id}), but its full details could not be immediately fetched.");
                }
                return MapPartToDetailDto(createdPartWithDetails);
            }
            catch (ValidationException valEx) { _logger.LogWarning(valEx, "Validation failed during part creation."); throw; }
            catch (DuplicationException dupEx) // If validator doesn't catch all or for race conditions
            {
                _logger.LogWarning(dupEx, "Duplication error during part creation.");
                throw;
            }

            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync(); // Ensure rollback on DB error
                _logger.LogError(dbEx, "Database error creating part '{PartName}'.", createDto.Name);
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new DuplicationException($"A part with similar unique information (e.g., Part Number '{createDto.PartNumber}') already exists.", nameof(Part), "UniqueConstraint", createDto.PartNumber, dbEx);
                }
                throw new ServiceException("Database error during part creation.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(); // Ensure rollback on general error
                _logger.LogError(ex, "Unexpected error creating part '{PartName}'.", createDto.Name);
                throw new ServiceException($"Unexpected error creating part '{createDto.Name}'.", ex);
            }
        }


        /// <summary>
        /// Updates an existing part's core details.
        /// Does NOT update suppliers or compatibilities directly; use dedicated relationship methods.
        /// </summary>
        public async Task UpdatePartAsync(UpdatePartDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto, nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Part ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update part: ID={PartId}, PartNumber='{PartNumber}'",
                updateDto.Id, updateDto.PartNumber);

            // 1. Validate DTO
            var validationResult = await _updatePartValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdatePartDto (ID: {PartId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }
            // Validator should check PartNumber/Barcode uniqueness (excluding self) and CategoryId existence.

            // 2. Fetch Existing Entity (include current image path for comparison and potential deletion)
            var existingPart = await _unitOfWork.Parts.GetByIdAsync(updateDto.Id); // Get basic entity first
            if (existingPart == null)
            {
                string notFoundMsg = $"Part with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or custom NotFoundException
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {

                existingPart.PartNumber = updateDto.PartNumber.Trim().ToUpperInvariant();
                existingPart.Name = updateDto.Name.Trim();
                existingPart.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
                existingPart.Manufacturer = string.IsNullOrWhiteSpace(updateDto.Manufacturer) ? null : updateDto.Manufacturer.Trim();
                existingPart.CostPrice = updateDto.CostPrice;
                existingPart.SellingPrice = updateDto.SellingPrice;
                existingPart.StockQuantity = updateDto.StockQuantity; // Base/defined stock
                existingPart.ReorderLevel = updateDto.ReorderLevel;
                existingPart.MinimumStock = updateDto.MinimumStock;
                existingPart.Location = string.IsNullOrWhiteSpace(updateDto.Location) ? string.Empty : updateDto.Location.Trim();
                existingPart.IsActive = updateDto.IsActive;
                existingPart.IsOriginal = updateDto.IsOriginal;
                existingPart.Notes = string.IsNullOrWhiteSpace(updateDto.Notes) ? null : updateDto.Notes.Trim();
                existingPart.Barcode = string.IsNullOrWhiteSpace(updateDto.Barcode) ? null : updateDto.Barcode.Trim();
                existingPart.StockingUnitOfMeasureId = updateDto.StockingUnitOfMeasureId;
                existingPart.SalesUnitOfMeasureId = updateDto.SalesUnitOfMeasureId;
                existingPart.SalesConversionFactor = (updateDto.SalesUnitOfMeasureId.HasValue && updateDto.SalesUnitOfMeasureId.Value != updateDto.StockingUnitOfMeasureId)
                                                     ? updateDto.SalesConversionFactor
                                                     : null;
                existingPart.PurchaseUnitOfMeasureId = updateDto.PurchaseUnitOfMeasureId;
                existingPart.PurchaseConversionFactor = (updateDto.PurchaseUnitOfMeasureId.HasValue && updateDto.PurchaseUnitOfMeasureId.Value != updateDto.StockingUnitOfMeasureId)
                                                        ? updateDto.PurchaseConversionFactor
                                                        : null;
                existingPart.CategoryId = updateDto.CategoryId;
                // existingPart.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext

                // 5. Save Changes to Database
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Part core details with ID {PartId} updated successfully in database.", updateDto.Id);

                // REMOVE Old Image File Cleanup logic from here

                // 6. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("Part with ID {PartId} core update process completed successfully.", updateDto.Id);
            }
            catch (ValidationException valEx) { _logger.LogWarning(valEx, "Validation failed during part update for ID: {PartId}.", updateDto.Id); throw; }
            catch (DuplicationException dupEx)
            {
                _logger.LogWarning(dupEx, "Duplication error during part update for ID: {PartId}.", updateDto.Id);
                throw;
            }

            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error updating part ID {PartId}.", updateDto.Id);
                // No new image to clean up here
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new DuplicationException($"A part with similar unique information (e.g., Part Number '{updateDto.PartNumber}') already exists.", nameof(Part), "UniqueConstraint", updateDto.PartNumber, dbEx);
                }
                throw new ServiceException("Database error during part update.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Unexpected error updating part ID {PartId}.", updateDto.Id);
                // No new image to clean up here
                throw new ServiceException($"Unexpected error updating part ID {updateDto.Id}.", ex);
            }
        }


        /// <summary>
        /// Soft deletes a part (sets IsActive = false).
        /// </summary>
        public async Task DeactivatePartAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to deactivate part with invalid ID: {PartId}", id);
                throw new ArgumentException("Invalid Part ID for deactivation.", nameof(id));
            }

            _logger.LogInformation("Attempting to deactivate part with ID: {PartId}", id);

            try
            {
                var partToDeactivate = await _unitOfWork.Parts.GetByIdAsync(id);

                if (partToDeactivate == null)
                {
                    string notFoundMsg = $"Part with ID {id} not found. Cannot deactivate.";
                    _logger.LogWarning(notFoundMsg);
                    throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
                }

                if (!partToDeactivate.IsActive)
                {
                    _logger.LogInformation("Part with ID {PartId} is already inactive. No action taken.", id);
                    return; // Already in the desired state
                }

                partToDeactivate.IsActive = false;
                // ModifiedAt will be handled by DbContext SaveChanges override or BaseEntity logic

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Part with ID {PartId} deactivated successfully.", id);
            }
            catch (DbUpdateException dbEx) // Catch potential issues during save, though less likely for a simple flag change
            {
                _logger.LogError(dbEx, "Database error occurred while deactivating part ID {PartId}.", id);
                throw new ServiceException("A database error occurred while deactivating the part.", dbEx);
            }
            catch (ServiceException) // Re-throw service exceptions (like NotFound from above)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deactivating part ID {PartId}", id);
                throw new ServiceException($"An error occurred while deactivating part with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Activates a previously deactivated part (sets IsActive = true).
        /// </summary>
        public async Task ActivatePartAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to activate part with invalid ID: {PartId}", id);
                throw new ArgumentException("Invalid Part ID for activation.", nameof(id));
            }

            _logger.LogInformation("Attempting to activate part with ID: {PartId}", id);

            try
            {
                var partToActivate = await _unitOfWork.Parts.GetByIdAsync(id);

                if (partToActivate == null)
                {
                    string notFoundMsg = $"Part with ID {id} not found. Cannot activate.";
                    _logger.LogWarning(notFoundMsg);
                    throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
                }

                if (partToActivate.IsActive)
                {
                    _logger.LogInformation("Part with ID {PartId} is already active. No action taken.", id);
                    return; // Already in the desired state
                }

                partToActivate.IsActive = true;
                // ModifiedAt will be handled by DbContext SaveChanges override or BaseEntity logic

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Part with ID {PartId} activated successfully.", id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while activating part ID {PartId}.", id);
                throw new ServiceException("A database error occurred while activating the part.", dbEx);
            }
            catch (ServiceException) // Re-throw service exceptions
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while activating part ID {PartId}", id);
                throw new ServiceException($"An error occurred while activating part with ID {id}.", ex);
            }
        }


        #endregion

        #region Part Image Management New 

        /// <summary>
        /// Adds an image to a specific Part. Handles file saving and database record creation.
        /// If IsPrimary is true, it will unset other primary images for the part.
        /// </summary>
        public async Task<PartImageDto> AddImageToPartAsync(int partId, CreatePartImageDto imageDto)
        {
            ArgumentNullException.ThrowIfNull(imageDto, nameof(imageDto));
            if (partId <= 0) throw new ArgumentException("Invalid Part ID.", nameof(partId));
            if (string.IsNullOrWhiteSpace(imageDto.ImagePath)) // Validate path from DTO
                throw new ArgumentException("Source image path cannot be empty.", nameof(imageDto.ImagePath));
            if (!File.Exists(imageDto.ImagePath)) // Ensure file exists
                throw new FileNotFoundException("Source image file not found.", imageDto.ImagePath);


            _logger.LogInformation("Attempting to add image to Part ID: {PartId}. SourcePath: '{SourcePath}', IsPrimary: {IsPrimary}",
                partId, imageDto.ImagePath, imageDto.IsPrimary);

            // 1. Validate DTO (CreatePartImageDtoValidator)
            var validationResult = await _createPartImageValidator.ValidateAsync(imageDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreatePartImageDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Validate Part Existence
            if (!await _unitOfWork.Parts.ExistsAsync(p => p.Id == partId))
            {
                string msg = $"Part with ID {partId} not found. Cannot add image.";
                _logger.LogWarning(msg);
                throw new ServiceException(msg);
            }

            string? persistentImagePath = null;
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Save the physical image file using the PATH-BASED method
                _logger.LogDebug("Saving physical image file for Part ID: {PartId}, source path: {SourcePath}", partId, imageDto.ImagePath);
                persistentImagePath = await _imageFileService.SaveImageAsync(imageDto.ImagePath, "Parts"); // Target subfolder "Parts"

                if (string.IsNullOrWhiteSpace(persistentImagePath))
                {
                    throw new ServiceException("Failed to save image file. Path returned was null or empty from image service.");
                }
                _logger.LogInformation("Image file saved for Part ID: {PartId}. Persistent path: '{PersistentPath}'", partId, persistentImagePath);

                // 4. Handle IsPrimary logic
                if (imageDto.IsPrimary)
                {
                    _logger.LogDebug("New image is primary. Clearing other primary flags for Part ID: {PartId}", partId);
                    await _unitOfWork.PartImages.ClearPrimaryFlagsForPartAsync(partId, null);
                }

                // 5. Create PartImage entity
                var newPartImage = new PartImage
                {
                    PartId = partId,
                    ImagePath = persistentImagePath,
                    Caption = string.IsNullOrWhiteSpace(imageDto.Caption) ? null : imageDto.Caption.Trim(),
                    IsPrimary = imageDto.IsPrimary,
                    DisplayOrder = imageDto.DisplayOrder,
                    CreatedAt = DateTime.UtcNow
                };

                // 6. Add to Repository and Save Changes
                await _unitOfWork.PartImages.AddAsync(newPartImage);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("PartImage record created successfully for Part ID: {PartId}. Image ID: {ImageId}", partId, newPartImage.Id);

                // 7. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // 8. Map to DTO and return
                return MapPartImageToDto(newPartImage);
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (FileNotFoundException fnfEx) // Specific catch for image source not found
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(fnfEx, "Source image file not found for Part ID {PartId}, Path '{SourcePath}'.", partId, imageDto.ImagePath);
                throw new ServiceException($"Source image file not found: {imageDto.ImagePath}", fnfEx);
            }
            catch (IOException ioEx) // From _imageFileService.SaveImageAsync
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ioEx, "File IO error saving image for Part ID {PartId}, SourcePath '{SourcePath}'.", partId, imageDto.ImagePath);
                throw new ServiceException("An error occurred saving the image file. The image was not added.", ioEx);
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error adding image record for Part ID {PartId}.", partId);
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, $"add image to part {partId} (DB error)");
                throw new ServiceException("A database error occurred while saving the image record.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Unexpected error adding image to Part ID {PartId}.", partId);
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, $"add image to part {partId} (unexpected error)");
                throw new ServiceException($"An unexpected error occurred while adding image to part {partId}.", ex);
            }
        }


        /// <summary>
        /// Updates metadata for an existing part image (e.g., caption, IsPrimary, DisplayOrder).
        /// If IsPrimary is set to true, other images for the same part will be unset as primary.
        /// </summary>
        public async Task UpdatePartImageDetailsAsync(UpdatePartImageDto imageDetailsDto)
        {
            ArgumentNullException.ThrowIfNull(imageDetailsDto, nameof(imageDetailsDto));

            _logger.LogInformation("Attempting to update details for PartImage ID: {PartImageId}. IsPrimary: {IsPrimary}",
                imageDetailsDto.Id, imageDetailsDto.IsPrimary);

            // 1. Validate DTO
            var validationResult = await _updatePartImageValidator.ValidateAsync(imageDetailsDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdatePartImageDto (ID: {PartImageId}): {Errors}",
                    imageDetailsDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing PartImage Entity
            // We need to include PartId to clear other primary flags if this one becomes primary.
            // A specific repository method might be slightly cleaner: GetByIdWithPartIdAsync(int imageId)
            // Or fetch it and then access PartId.
            var existingPartImage = await _unitOfWork.PartImages.GetByIdAsync(imageDetailsDto.Id);
            // The validator already ensures it exists, but a defensive check is good.
            if (existingPartImage == null)
            {
                string notFoundMsg = $"PartImage with ID {imageDetailsDto.Id} not found. Cannot update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Should have been caught by validator
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Handle IsPrimary logic
                // If this image is being set as primary AND it wasn't primary before
                if (imageDetailsDto.IsPrimary && !existingPartImage.IsPrimary)
                {
                    _logger.LogDebug("Setting PartImage ID {PartImageId} as primary. Clearing other primary flags for Part ID: {PartId}",
                        imageDetailsDto.Id, existingPartImage.PartId);
                    // Clear primary flag for other images of the same part, excluding this one
                    await _unitOfWork.PartImages.ClearPrimaryFlagsForPartAsync(existingPartImage.PartId, imageDetailsDto.Id);
                }
                // If this image is being unset as primary, and no other image is being set as primary in this call,
                // the part might end up with no primary image. This is usually acceptable.
                // If a rule "must have one primary" exists, that's more complex UI/service logic (e.g., when unsetting, prompt to pick another).

                // 4. Update properties on the entity
                existingPartImage.Caption = string.IsNullOrWhiteSpace(imageDetailsDto.Caption) ? null : imageDetailsDto.Caption.Trim();
                existingPartImage.IsPrimary = imageDetailsDto.IsPrimary;
                existingPartImage.DisplayOrder = imageDetailsDto.DisplayOrder;
                // existingPartImage.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext

                // 5. Save Changes (EF Core tracks changes to existingPartImage)
                await _unitOfWork.SaveChangesAsync();

                // 6. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("PartImage ID {PartImageId} details updated successfully.", imageDetailsDto.Id);
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error updating PartImage ID {PartImageId}.", imageDetailsDto.Id);
                throw new ServiceException("A database error occurred while updating the part image details.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Unexpected error updating PartImage ID {PartImageId}.", imageDetailsDto.Id);
                throw new ServiceException($"An unexpected error occurred while updating part image details for ID {imageDetailsDto.Id}.", ex);
            }
        }

        /// <summary>
        /// Removes an image from a Part asset by the PartImage record's ID.
        /// Also deletes the physical file via IImageFileService.
        /// </summary>
        public async Task RemoveImageFromPartAsync(int partImageId)
        {
            if (partImageId <= 0)
            {
                _logger.LogWarning("Attempted to remove part image with invalid ID: {PartImageId}", partImageId);
                throw new ArgumentException("Part Image ID must be greater than zero.", nameof(partImageId));
            }

            _logger.LogInformation("Attempting to remove PartImage with ID: {PartImageId}", partImageId);

            // 1. Fetch Existing PartImage Entity to get its path and ensure it exists
            // It's good to fetch it even if we just need the path, to confirm it's a valid record to delete.
            var imageToDelete = await _unitOfWork.PartImages.GetByIdAsync(partImageId);

            if (imageToDelete == null)
            {
                string notFoundMsg = $"PartImage with ID {partImageId} not found. Cannot remove.";
                _logger.LogWarning(notFoundMsg);
                // Depending on strictness, you might not throw if it's already gone, or throw a NotFoundException.
                // For this operation, if it's not found, our job is arguably done, but logging is good.
                // Let's throw to indicate the requested operation couldn't be performed on a non-existent entity.
                throw new ServiceException(notFoundMsg);
            }

            string? persistentImagePath = imageToDelete.ImagePath; // Get path before DB record is gone

            // 2. Begin Transaction (to ensure DB delete and file delete are somewhat coordinated,
            //    though true atomicity across DB and file system is hard)
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Delete the database record for PartImage
                _unitOfWork.PartImages.Delete(imageToDelete);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("PartImage record ID {PartImageId} deleted from database successfully.", partImageId);

                // 4. Delete the physical image file AFTER successful DB deletion
                // This order is generally preferred: if file deletion fails, the DB record is gone,
                // which is better than an orphaned DB record pointing to a non-existent file.
                // The main risk is an orphaned file if the app crashes between DB commit and file delete.
                if (!string.IsNullOrWhiteSpace(persistentImagePath))
                {
                    _logger.LogInformation("Attempting to delete physical image file '{ImagePath}' for PartImage ID {PartImageId}.",
                        persistentImagePath, partImageId);
                    await _imageFileService.DeleteImageAsync(persistentImagePath); // This should be resilient
                    _logger.LogInformation("Physical image file '{ImagePath}' for PartImage ID {PartImageId} deletion attempted.",
                        persistentImagePath, partImageId);
                }
                else
                {
                    _logger.LogWarning("No image path found for PartImage ID {PartImageId}. No physical file to delete.", partImageId);
                }

                // 5. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("PartImage ID {PartImageId} removal process completed.", partImageId);

                // Business Rule: If the deleted image was primary, a new primary might need to be selected.
                // This logic is usually handled by the UI/ViewModel layer prompting the user,
                // or by a background process, or by automatically selecting the next image by DisplayOrder.
                // For now, this service method focuses on the deletion itself.
                // If a rule exists that a part MUST have a primary image (if it has any images),
                // this service method might need to enforce it by trying to set another image as primary.
                if (imageToDelete.IsPrimary)
                {
                    _logger.LogInformation("Deleted image (ID: {PartImageId}) was primary for PartID: {PartId}. Consider prompting for a new primary.",
                        partImageId, imageToDelete.PartId);
                    // Optionally, add logic here to automatically set a new primary if desired:
                    // var remainingImages = await _unitOfWork.PartImages.GetByPartIdAsync(imageToDelete.PartId);
                    // if (remainingImages.Any() && !remainingImages.Any(img => img.IsPrimary))
                    // {
                    //     var newPrimary = remainingImages.OrderBy(img => img.DisplayOrder).ThenBy(img => img.Id).First();
                    //     newPrimary.IsPrimary = true;
                    //     await _unitOfWork.SaveChangesAsync(); // This would need to be part of the same transaction
                    //     _logger.LogInformation("Automatically set ImageID {NewPrimaryImageId} as primary for PartID {PartId}.", newPrimary.Id, imageToDelete.PartId);
                    // }
                }
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error while deleting PartImage record ID {PartImageId}.", partImageId);
                throw new ServiceException("A database error occurred while deleting the part image record.", dbEx);
            }
            catch (IOException ioEx) // If _imageFileService.DeleteImageAsync throws for critical reasons
            {
                // Transaction was likely committed for DB delete. The file system delete failed.
                // This results in an orphaned file. This is hard to make truly atomic.
                // Log thoroughly.
                _logger.LogError(ioEx, "File IO error occurred while deleting physical image file '{ImagePath}' for PartImage ID {PartImageId}. The database record was deleted.",
                    persistentImagePath, partImageId);
                throw new ServiceException($"The image record was deleted, but an error occurred deleting its physical file '{persistentImagePath}'. Manual cleanup may be required.", ioEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An unexpected error occurred while removing PartImage ID {PartImageId}.", partImageId);
                throw new ServiceException($"An error occurred while removing part image ID {partImageId}.", ex);
            }
        }

        /// <summary>
        /// Sets a specific image as the primary display image for a part.
        /// Ensures other images for the same part are unset as primary.
        /// </summary>
        public async Task SetPrimaryPartImageAsync(int partId, int partImageId)
        {
            if (partId <= 0)
                throw new ArgumentException("Invalid Part ID.", nameof(partId));
            if (partImageId <= 0)
                throw new ArgumentException("Invalid Part Image ID.", nameof(partImageId));

            _logger.LogInformation("Attempting to set PartImage ID {PartImageId} as primary for Part ID: {PartId}",
                partImageId, partId);

            // 1. Fetch the PartImage to be set as primary
            // It's good to ensure it belongs to the specified partId as well.
            var imageToSetAsPrimary = await _unitOfWork.PartImages.GetByIdAsync(partImageId);

            if (imageToSetAsPrimary == null)
            {
                string msg = $"PartImage with ID {partImageId} not found.";
                _logger.LogWarning(msg);
                throw new ServiceException(msg); // Or custom NotFoundException
            }

            // 2. Verify the image belongs to the correct part
            if (imageToSetAsPrimary.PartId != partId)
            {
                string mismatchMsg = $"PartImage ID {partImageId} does not belong to Part ID {partId}. Operation aborted.";
                _logger.LogError(mismatchMsg);
                throw new ServiceException(mismatchMsg); // Or a more specific InvalidOperationException
            }

            // 3. If it's already primary, no action needed (idempotency)
            if (imageToSetAsPrimary.IsPrimary)
            {
                _logger.LogInformation("PartImage ID {PartImageId} is already set as primary for Part ID {PartId}. No changes made.",
                    partImageId, partId);
                return;
            }

            // 4. Begin Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 5. Clear IsPrimary flag for all other images of this part
                // Pass `partImageId` to exclude the current image from being unset if it was somehow already primary
                // (though the check above handles that, this makes ClearPrimaryFlagsForPartAsync more robust).
                await _unitOfWork.PartImages.ClearPrimaryFlagsForPartAsync(partId, partImageId);

                // 6. Set the target image as primary
                imageToSetAsPrimary.IsPrimary = true;
                // imageToSetAsPrimary.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext

                // 7. Save Changes (EF Core tracks changes to imageToSetAsPrimary and those from ClearPrimaryFlags)
                await _unitOfWork.SaveChangesAsync();

                // 8. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("PartImage ID {PartImageId} successfully set as primary for Part ID {PartId}.",
                    partImageId, partId);
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error setting primary image for Part ID {PartId}, Image ID {PartImageId}.",
                    partId, partImageId);
                throw new ServiceException("A database error occurred while setting the primary image.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Unexpected error setting primary image for Part ID {PartId}, Image ID {PartImageId}.",
                    partId, partImageId);
                throw new ServiceException($"An error occurred while setting primary image for Part ID {partId}.", ex);
            }
        }


        /// <summary>
        /// Gets all images for a specific part, ordered by DisplayOrder then ID.
        /// </summary>
        public async Task<IEnumerable<PartImageDto>> GetImagesForPartAsync(int partId)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get images for invalid Part ID: {PartId}", partId);
                return Enumerable.Empty<PartImageDto>(); // Or throw ArgumentException
            }

            _logger.LogInformation("Attempting to retrieve all images for Part ID: {PartId}", partId);
            try
            {
                // Use the repository method that gets images by PartId and orders them
                var imageEntities = await _unitOfWork.PartImages.GetByPartIdAsync(partId);

                var dtos = imageEntities.Select(MapPartImageToDto).ToList();

                _logger.LogInformation("Successfully retrieved {Count} images for Part ID {PartId}.", dtos.Count, partId);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving images for Part ID {PartId}.", partId);
                throw new ServiceException($"Could not retrieve images for Part ID {partId}.", ex);
            }
        }

        #endregion


        #region Supplier-Part CRUD

        /// <summary>
        /// Adds a Supplier link to a Part with specific details (cost, supplier part number, etc.).
        /// </summary>
        public async Task AddSupplierToPartAsync(int partId, PartSupplierCreateDto supplierLinkDto)
        {
            ArgumentNullException.ThrowIfNull(supplierLinkDto, nameof(supplierLinkDto));
            if (partId <= 0) throw new ArgumentException("Invalid Part ID.", nameof(partId));
            if (supplierLinkDto.SupplierId <= 0) throw new ArgumentException("Invalid Supplier ID.", nameof(supplierLinkDto.SupplierId));

            _logger.LogInformation("Attempting to add supplier link: PartId={PartId}, SupplierId={SupplierId}, SupplierPartNumber='{SupPartNum}'",
                partId, supplierLinkDto.SupplierId, supplierLinkDto.SupplierPartNumber);

            // Optional: Validate supplierLinkDto using a specific FluentValidator if complex rules exist
            // var validationResult = await _createSupplierLinkValidator.ValidateAsync(supplierLinkDto);
            // if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            // 1. Validate existence of Part and Supplier
            if (!await _unitOfWork.Parts.ExistsAsync(p => p.Id == partId))
            {
                string msg = $"Part with ID {partId} not found. Cannot add supplier link.";
                _logger.LogWarning(msg);
                throw new ServiceException(msg);
            }
            if (!await _unitOfWork.Suppliers.ExistsAsync(s => s.Id == supplierLinkDto.SupplierId))
            {
                string msg = $"Supplier with ID {supplierLinkDto.SupplierId} not found. Cannot add supplier link.";
                _logger.LogWarning(msg);
                throw new ServiceException(msg);
            }

            // 2. Check if this specific Part-Supplier link already exists
            // (Prevents adding the same supplier multiple times to the same part, unless your business rules allow it
            // - if they do, this check might be based on more criteria or removed)
            bool linkExists = await _unitOfWork.ExistsAsync<SupplierPart>( // Assuming generic ExistsAsync
                sp => sp.PartId == partId && sp.SupplierId == supplierLinkDto.SupplierId
            );
            if (linkExists)
            {
                string duplicateMsg = $"Supplier ID {supplierLinkDto.SupplierId} is already linked to Part ID {partId}. Update existing link if needed.";
                _logger.LogWarning(duplicateMsg);
                throw new DuplicationException(duplicateMsg, "SupplierPart", "PartId_SupplierId", $"{partId}_{supplierLinkDto.SupplierId}");
            }

            try
            {
                // 3. Create new SupplierPart entity
                var newSupplierLink = new SupplierPart
                {
                    PartId = partId,
                    SupplierId = supplierLinkDto.SupplierId,
                    SupplierPartNumber = string.IsNullOrWhiteSpace(supplierLinkDto.SupplierPartNumber) ? null : supplierLinkDto.SupplierPartNumber.Trim(),
                    Cost = supplierLinkDto.Cost, // Validation for >= 0 should be on DTO or validator
                    LeadTimeInDays = supplierLinkDto.LeadTimeInDays,
                    MinimumOrderQuantity = supplierLinkDto.MinimumOrderQuantity,
                    IsPreferredSupplier = supplierLinkDto.IsPreferredSupplier,
                    CreatedAt = DateTime.UtcNow
                };

                // Handle IsPreferredSupplier logic: Ensure only one preferred supplier per part
                if (newSupplierLink.IsPreferredSupplier)
                {
                    await ClearPreferredSupplierForPartAsync(partId, null); // Clear any existing preferred for this part
                }

                // 4. Add to Repository/Context
                await _unitOfWork.AddAsync(newSupplierLink); // Assuming generic AddAsync for SupplierPart

                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully added supplier link: PartId={PartId}, SupplierId={SupplierId}, LinkId={LinkId}",
                    partId, supplierLinkDto.SupplierId, newSupplierLink.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error adding supplier link: PartId={PartId}, SupplierId={SupplierId}.",
                    partId, supplierLinkDto.SupplierId);
                throw new ServiceException("A database error occurred while adding the supplier link.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding supplier link: PartId={PartId}, SupplierId={SupplierId}.",
                    partId, supplierLinkDto.SupplierId);
                throw new ServiceException("An unexpected error occurred while adding the supplier link.", ex);
            }
        }

        /// <summary>
        /// Updates the details of an existing link between a Part and a Supplier.
        /// </summary>
        public async Task UpdateSupplierLinkForPartAsync(int supplierPartId, PartSupplierDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto, nameof(updateDto));
            if (supplierPartId <= 0) throw new ArgumentException("Invalid SupplierPart ID.", nameof(supplierPartId));
            if (updateDto.Id != supplierPartId) throw new ArgumentException("SupplierPart ID in DTO does not match the ID parameter.", nameof(updateDto.Id));
            if (updateDto.SupplierId <= 0) throw new ArgumentException("Invalid Supplier ID in DTO.", nameof(updateDto.SupplierId));


            _logger.LogInformation("Attempting to update supplier link with ID: {SupplierPartId}", supplierPartId);

            // Optional: Validate updateDto using a specific FluentValidator
            // var validationResult = await _updateSupplierLinkValidator.ValidateAsync(updateDto);
            // if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            try
            {
                // 1. Fetch the existing SupplierPart link entity
                // Include Part only if needed for validation (e.g., if preferred supplier logic needs PartId)
                var existingLink = await _unitOfWork.SupplierParts // Assuming generic repo
                                                    .GetByIdAsync(supplierPartId);
                // .Include(sp => sp.Part) // Optional, if PartId is needed for logic below

                if (existingLink == null)
                {
                    string notFoundMsg = $"SupplierPart link with ID {supplierPartId} not found.";
                    _logger.LogWarning(notFoundMsg);
                    throw new ServiceException(notFoundMsg);
                }

                // Security/Integrity check: Ensure the SupplierId isn't being changed to a different supplier for an existing link.
                // If changing supplier, it should be a remove old + add new operation.
                // However, PartSupplierDto contains SupplierId, so we allow updating it if necessary,
                // but must check for duplicates if the SupplierId changes for this Part.
                if (existingLink.SupplierId != updateDto.SupplierId)
                {
                    _logger.LogWarning("Attempt to change SupplierId on an existing SupplierPart link (ID: {SupplierPartId}) from {OldSupplierId} to {NewSupplierId}. This might indicate an incorrect update operation.",
                        supplierPartId, existingLink.SupplierId, updateDto.SupplierId);
                    // Check if the NEW SupplierId already exists for this PartId
                    bool newLinkCombinationExists = await _unitOfWork.ExistsAsync<SupplierPart>(
                        sp => sp.PartId == existingLink.PartId &&
                              sp.SupplierId == updateDto.SupplierId &&
                              sp.Id != supplierPartId // Exclude the current link itself
                    );
                    if (newLinkCombinationExists)
                    {
                        throw new DuplicationException($"Supplier ID {updateDto.SupplierId} is already linked to Part ID {existingLink.PartId} via a different record.", "SupplierPart", "PartId_SupplierId", $"{existingLink.PartId}_{updateDto.SupplierId}");
                    }
                    // Also validate the new SupplierId exists
                    if (!await _unitOfWork.Suppliers.ExistsAsync(s => s.Id == updateDto.SupplierId))
                    {
                        throw new ServiceException($"Target Supplier with ID {updateDto.SupplierId} does not exist.");
                    }
                }


                // 2. Update properties
                existingLink.SupplierId = updateDto.SupplierId; // Allow updating, but with checks
                existingLink.SupplierPartNumber = string.IsNullOrWhiteSpace(updateDto.SupplierPartNumber) ? null : updateDto.SupplierPartNumber.Trim();
                existingLink.Cost = updateDto.Cost;
                existingLink.LeadTimeInDays = updateDto.LeadTimeInDays;
                existingLink.MinimumOrderQuantity = updateDto.MinimumOrderQuantity;

                // Handle IsPreferredSupplier logic
                if (updateDto.IsPreferredSupplier && !existingLink.IsPreferredSupplier) // If becoming preferred
                {
                    await ClearPreferredSupplierForPartAsync(existingLink.PartId, existingLink.Id); // Clear others for this part
                    existingLink.IsPreferredSupplier = true;
                }
                else if (!updateDto.IsPreferredSupplier && existingLink.IsPreferredSupplier) // If unsetting preferred
                {
                    existingLink.IsPreferredSupplier = false;
                }
                // If updateDto.IsPreferredSupplier and existingLink.IsPreferredSupplier are both true, no change needed by this logic block.

                // existingLink.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext

                // 3. Save Changes (EF Core tracks changes to existingLink)
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully updated supplier link with ID: {SupplierPartId}", supplierPartId);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error updating supplier link ID {SupplierPartId}.", supplierPartId);
                throw new ServiceException("A database error occurred while updating the supplier link.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating supplier link ID {SupplierPartId}.", supplierPartId);
                throw new ServiceException($"An unexpected error occurred while updating supplier link ID {supplierPartId}.", ex);
            }
        }


        /// <summary>
        /// Removes a specific SupplierPart link by its own ID.
        /// </summary>
        public async Task RemoveSupplierFromPartAsync(int supplierPartId)
        {
            if (supplierPartId <= 0)
            {
                _logger.LogWarning("Attempted to remove supplier link with invalid SupplierPart ID: {SupplierPartId}", supplierPartId);
                throw new ArgumentException("Invalid SupplierPart ID.", nameof(supplierPartId));
            }

            _logger.LogInformation("Attempting to remove SupplierPart link with ID: {SupplierPartId}", supplierPartId);

            try
            {
                // 1. Fetch the SupplierPart link entity
                var linkToDelete = await _unitOfWork.SupplierParts // Assuming generic repo getter
                                                    .GetByIdAsync(supplierPartId);
                // OR using context directly:
                // var linkToDelete = await _unitOfWork.Context.Set<SupplierPart>().FindAsync(supplierPartId);

                if (linkToDelete == null)
                {
                    string notFoundMsg = $"SupplierPart link with ID {supplierPartId} not found. Cannot remove.";
                    _logger.LogWarning(notFoundMsg);
                    throw new ServiceException(notFoundMsg); // Or custom NotFoundException
                }

                // Optional: Business logic before deletion, e.g., cannot delete preferred supplier if it's the only one.
                // However, typically, a part might have no preferred supplier.

                // 2. Remove the entity
                _unitOfWork.SupplierParts.Delete(linkToDelete);
                // OR using context directly:
                // _unitOfWork.Context.Set<SupplierPart>().Remove(linkToDelete);

                // 3. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully removed SupplierPart link with ID: {SupplierPartId} (PartId: {PartId}, SupplierId: {SupplierId})",
                    supplierPartId, linkToDelete.PartId, linkToDelete.SupplierId);
            }
            catch (DbUpdateException dbEx) // For potential concurrency issues or unexpected DB errors
            {
                _logger.LogError(dbEx, "Database error while removing SupplierPart link ID {SupplierPartId}.", supplierPartId);
                throw new ServiceException("A database error occurred while removing the supplier link.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while removing SupplierPart link ID {SupplierPartId}.", supplierPartId);
                throw new ServiceException($"An unexpected error occurred while removing supplier link ID {supplierPartId}.", ex);
            }
        }


        /// <summary>
        /// Gets all supplier links (as DTOs) for a specific part.
        /// </summary>
        public async Task<IEnumerable<PartSupplierDto>> GetSuppliersForPartAsync(int partId)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get supplier links for invalid Part ID: {PartId}", partId);
                return Enumerable.Empty<PartSupplierDto>();
            }

            _logger.LogInformation("Attempting to retrieve supplier links for Part ID: {PartId}", partId);
            try
            {
                var supplierLinkEntities = await _unitOfWork.SupplierParts.GetByPartIdWithSupplierDetailsAsync(partId);

                var dtos = supplierLinkEntities.Select(MapSupplierPartToDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} supplier links for Part ID {PartId}.", dtos.Count, partId);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving supplier links for Part ID {PartId}.", partId);
                throw new ServiceException($"Could not retrieve supplier links for Part ID {partId}.", ex);
            }
        }

        #endregion


        #region Validation Helpers

        /// <summary>
        /// Checks if a PartNumber already exists.
        /// </summary>
        public async Task<bool> PartNumberExistsAsync(string partNumber, int? excludePartId = null)
        {
            if (string.IsNullOrWhiteSpace(partNumber))
            {
                _logger.LogWarning("PartNumberExistsAsync called with null or empty part number.");
                // An empty part number typically isn't considered "existing" in a way that causes a clash.
                // Validation for required part number should be on the DTO.
                return false;
            }

            _logger.LogInformation("Checking existence for PartNumber: '{PartNumber}', ExcludePartId: {ExcludePartId}",
                partNumber, excludePartId);
            try
            {
                return await _unitOfWork.Parts.PartNumberExistsAsync(partNumber, excludePartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking PartNumber existence for '{PartNumber}'.", partNumber);
                // It's often safer to assume existence or re-throw to prevent accidental duplicates if check fails.
                throw new ServiceException($"Could not verify existence for PartNumber '{partNumber}'.", ex);
            }
        }

        /// <summary>
        /// Checks if a Barcode already exists (if provided and non-empty).
        /// </summary>
        public async Task<bool> BarcodeExistsAsync(string barcode, int? excludePartId = null)
        {
            // If barcode is optional and can be empty/null, then an empty/null barcode doesn't "exist" as a duplicate.
            if (string.IsNullOrWhiteSpace(barcode))
            {
                _logger.LogDebug("BarcodeExistsAsync called with null or empty barcode. Returning false as it cannot clash if not provided.");
                return false;
            }

            _logger.LogInformation("Checking existence for Barcode: '{Barcode}', ExcludePartId: {ExcludePartId}",
                barcode, excludePartId);
            try
            {
                return await _unitOfWork.Parts.BarcodeExistsAsync(barcode, excludePartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Barcode existence for '{Barcode}'.", barcode);
                throw new ServiceException($"Could not verify existence for Barcode '{barcode}'.", ex);
            }
        }

        #endregion

        #region Inventory Related (Interactions with a future InventoryService)


        //Recommendation: Hybrid Approach(Denormalized Part.CurrentStock with Transactional Integrity)
        //Given the need for performance in displaying stock levels in lists and for filtering, and the fact that your InventoryTransaction model already stores NewQuantity, a hybrid approach is often best:
        //Keep CurrentStock on the Part Entity: This will be the primary source for quick reads.
        //public int CurrentStock { get; set; } in Part.cs.
        //Maintain InventoryTransaction Records: Every stock change must create an InventoryTransaction record.This is your audit trail and the ultimate source of truth.
        //Update Part.CurrentStock Atomically:
        //When an inventory operation occurs (e.g., in a future InventoryService):
        //Start a database transaction.
        //Create the InventoryTransaction record.The NewQuantity in this record will be the calculated new stock.
        //Update Part.CurrentStock to this same NewQuantity.
        //Commit the transaction.
        //This ensures Part.CurrentStock and the latest InventoryTransaction.NewQuantity are always in sync.
        //GetCurrentStockForPartAsync(int partId) Implementation in PartService:
        //This method can simply fetch the Part and return its CurrentStock property.
        //It acts as a convenient accessor.


        /// <summary>
        /// Gets the current stock quantity for a specific part.
        /// This retrieves the denormalized CurrentStock value from the Part entity.
        /// </summary>
        public async Task<int> GetCurrentStockForPartAsync(int partId)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get current stock for invalid Part ID: {PartId}", partId);
                // Depending on requirements, could throw ArgumentException or return a specific value like -1 or 0.
                throw new ArgumentException("Invalid Part ID.", nameof(partId));
            }

            _logger.LogInformation("Attempting to retrieve current stock for Part ID: {PartId}", partId);
            try
            {
                var part = await _unitOfWork.Parts.GetByIdAsync(partId); // Basic fetch is enough

                if (part == null)
                {
                    string notFoundMsg = $"Part with ID {partId} not found. Cannot retrieve stock.";
                    _logger.LogWarning(notFoundMsg);
                    throw new ServiceException(notFoundMsg); // Or custom NotFoundException
                }

                _logger.LogInformation("Current stock for Part ID {PartId} is {StockCount}.", partId, part.CurrentStock);
                return part.CurrentStock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving current stock for Part ID {PartId}.", partId);
                throw new ServiceException($"Could not retrieve current stock for Part ID {partId}.", ex);
            }
        }


        // --- If you needed to calculate from transactions (Option 2) ---
        // This would typically be in an InventoryService or a more specialized method.
        /*
        public async Task<int> CalculateCurrentStockFromTransactionsAsync(int partId)
        {
            if (partId <= 0) throw new ArgumentException("Invalid Part ID.", nameof(partId));
            _logger.LogInformation("Calculating current stock from transactions for Part ID: {PartId}", partId);

            try
            {
                // Fetch the latest transaction for the part
                var latestTransaction = await _unitOfWork.Context.Set<InventoryTransaction>()
                    .Where(t => t.PartId == partId)
                    .OrderByDescending(t => t.TransactionDate) // Order by date
                    .ThenByDescending(t => t.Id) // Then by ID for transactions on same date
                    .FirstOrDefaultAsync();

                if (latestTransaction == null)
                {
                    _logger.LogInformation("No inventory transactions found for Part ID {PartId}. Assuming stock is 0.", partId);
                    return 0; // No transactions means stock is 0 (or handle as initial stock differently)
                }
                return latestTransaction.NewQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating stock from transactions for Part ID {PartId}.", partId);
                throw new ServiceException($"Could not calculate stock from transactions for Part ID {partId}.", ex);
            }
        }
        */


        #endregion

        #region Helpers

        // Helper for image cleanup, slightly modified for clarity
        private async Task AttemptImageCleanupOnErrorAsync(string? imagePath, string operationContext, bool isCleanupForOld = false)
        {
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                var actionDescription = isCleanupForOld ? "old image file" : "newly saved (orphaned) image file";
                try
                {
                    _logger.LogWarning("Attempting to clean up {ActionDescription} due to error in {Context}: {ImagePath}", actionDescription, operationContext, imagePath);
                    await _imageFileService.DeleteImageAsync(imagePath); // DeleteImageAsync should be resilient to non-existent files
                    _logger.LogInformation("{ActionDescription} cleaned up successfully: {ImagePath}", char.ToUpper(actionDescription[0]) + actionDescription.Substring(1), imagePath);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to clean up {ActionDescription} '{ImagePath}' after error in {Context}.", actionDescription, imagePath, operationContext);
                }
            }
        }

        // --- Private Mapping Helper ---
        private PartDetailDto MapPartToDetailDto(Part part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            return new PartDetailDto
            {
                Id = part.Id,
                PartNumber = part.PartNumber,
                Name = part.Name,
                Description = part.Description,
                Manufacturer = part.Manufacturer,
                CostPrice = part.CostPrice,
                SellingPrice = part.SellingPrice,
                StockQuantity = part.StockQuantity, // This is the definition field
                ReorderLevel = part.ReorderLevel,
                MinimumStock = part.MinimumStock,
                Location = part.Location,
                IsActive = part.IsActive,
                IsOriginal = part.IsOriginal,
                Notes = part.Notes,
                Barcode = part.Barcode,

                CategoryId = part.CategoryId,
                CategoryName = part.Category?.Name ?? "N/A", // Assumes Category is loaded

                StockingUnitOfMeasureId = part.StockingUnitOfMeasureId,
                StockingUnitOfMeasureName = part.StockingUnitOfMeasure?.Name ?? "N/A",
                StockingUnitOfMeasureSymbol = part.StockingUnitOfMeasure?.Symbol ?? "N/A",

                SalesUnitOfMeasureId = part.SalesUnitOfMeasureId,
                SalesUnitOfMeasureName = part.SalesUnitOfMeasure?.Name,
                SalesUnitOfMeasureSymbol = part.SalesUnitOfMeasure?.Symbol,
                SalesConversionFactor = part.SalesConversionFactor,

                PurchaseUnitOfMeasureId = part.PurchaseUnitOfMeasureId,
                PurchaseUnitOfMeasureName = part.PurchaseUnitOfMeasure?.Name,
                PurchaseUnitOfMeasureSymbol = part.PurchaseUnitOfMeasure?.Symbol,
                PurchaseConversionFactor = part.PurchaseConversionFactor,

                CurrentStock = part.CurrentStock, // Actual live stock
                LastRestockDate = part.LastRestockDate == DateTime.MinValue ? null : part.LastRestockDate, // Handle default DateTime
            };
        }

        private PartSummaryDto MapPartToSummaryDto(Part part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            return new PartSummaryDto(
                part.Id,
                part.PartNumber,
                part.Name,
                part.Category?.Name ?? "N/A", // Assumes Category is loaded by repository
                part.Manufacturer,
                part.SellingPrice,
                part.CurrentStock,
                part.StockingUnitOfMeasure?.Symbol,
                part.IsActive,
                part.Images.FirstOrDefault(img => img.IsPrimary)?.ImagePath,
                part.Location


            );
        }

        /// <summary>
        /// Helper to ensure only one preferred supplier per part.
        /// Sets IsPreferredSupplier = false for all other SupplierPart records for the given partId.
        /// </summary>
        private async Task ClearPreferredSupplierForPartAsync(int partId, int? excludeSupplierPartId)
        {
            _logger.LogDebug("Clearing preferred supplier status for PartId: {PartId}, excluding SupplierPartId: {ExcludeId}", partId, excludeSupplierPartId);
            try
            {
                // Use the new repository method
                var otherPreferredSuppliers = await _unitOfWork.SupplierParts
                    .GetPreferredSupplierLinksForPartAsync(partId, excludeSupplierPartId);

                if (otherPreferredSuppliers.Any())
                {
                    foreach (var sp in otherPreferredSuppliers)
                    {
                        sp.IsPreferredSupplier = false;
                        // EF Core will track these changes on the fetched entities.
                        // The calling method (AddSupplierToPartAsync or UpdateSupplierLinkForPartAsync)
                        // will be responsible for calling _unitOfWork.SaveChangesAsync().
                    }
                    _logger.LogInformation("Cleared preferred status for {Count} other suppliers for PartId {PartId}.", otherPreferredSuppliers.Count(), partId);
                }
            }
            catch (RepositoryException ex) // Catch specific exception from repository
            {
                _logger.LogError(ex, "Repository error while clearing preferred suppliers for PartId {PartId}.", partId);
                // Re-throw as ServiceException or handle as appropriate for the calling context
                throw new ServiceException($"Failed to clear preferred supplier status for part ID {partId} due to a data access error.", ex);
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                _logger.LogError(ex, "Unexpected error while clearing preferred suppliers for PartId {PartId}.", partId);
                throw new ServiceException($"An unexpected error occurred while clearing preferred supplier status for part ID {partId}.", ex);
            }
        }

        /// <summary>
        /// Maps a SupplierPart domain entity to a PartSupplierDto.
        /// Assumes sp.Supplier navigation property is loaded if SupplierName is needed.
        /// </summary>
        private PartSupplierDto MapSupplierPartToDto(SupplierPart sp)
        {
            if (sp == null)
            {
                _logger.LogError("Attempted to map a null SupplierPart entity to PartSupplierDto.");
                // Depending on strictness, either throw or return a default/empty DTO
                // Throwing is generally safer to highlight issues.
                throw new ArgumentNullException(nameof(sp), "SupplierPart entity cannot be null for mapping.");
            }

            return new PartSupplierDto(
                sp.Id,                      // The ID of the SupplierPart join record itself
                sp.SupplierId,
                sp.Supplier?.Name ?? "N/A", // Safely access Supplier name, assumes sp.Supplier is loaded
                sp.SupplierPartNumber,
                sp.Cost,
                sp.IsPreferredSupplier,
                sp.LeadTimeInDays,
                sp.MinimumOrderQuantity
            );
        }


        // Helper for mapping PartImage to PartImageDto
        private PartImageDto MapPartImageToDto(PartImage image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            return new PartImageDto(image.Id, image.ImagePath, image.Caption, image.IsPrimary, image.DisplayOrder);
        }
        #endregion

    }
}
