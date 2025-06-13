using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.DTOs.Part;
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
        private readonly ICompatibleVehicleService _compatibleVehicleService; // Will be used later

        // Validators - will be injected and used in Create/Update methods
        private readonly IValidator<CreatePartDto> _createPartValidator;
        private readonly IValidator<UpdatePartDto> _updatePartValidator;
        #endregion

        #region Constructor
        public PartService(
                   IUnitOfWork unitOfWork,
                   ILogger<PartService> logger,
                   IImageFileService imageFileService, // Will be used later
                   IValidator<CreatePartDto> createPartValidator,
                   IValidator<UpdatePartDto> updatePartValidator,
                   ICompatibleVehicleService compatibleVehicleService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageFileService = imageFileService ?? throw new ArgumentNullException(nameof(imageFileService));
            _createPartValidator = createPartValidator ?? throw new ArgumentNullException(nameof(createPartValidator));
            _updatePartValidator = updatePartValidator ?? throw new ArgumentNullException(nameof(updatePartValidator));
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
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
                    includeSuppliers: true,
                    includeCompatibility: true
                );

                if (partEntity == null)
                {
                    _logger.LogWarning("Part with ID {PartId} not found.", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved part details for ID {PartId}: {PartName}", id, partEntity.Name);
                return MapPartToDetailDto(partEntity);
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
                // The repository method GetFilteredPartsPagedAsync needs individual filter parameters.
                // The service layer is responsible for translating the PartFilterCriteriaDto.
                // The repository's GetFilteredPartsPagedAsync should have includeCategory=true by default or as a param.

                // Note: The PartFilterCriteriaDto has MakeId, ModelId, TrimId, SpecificYear.
                // The IPartRepository.GetFilteredPartsPagedAsync currently has `compatibleVehicleId`.
                // This implies a potential mismatch or that the service needs to first find CompatibleVehicle IDs
                // based on Make/Model/Trim/Year and then pass those IDs to the repository,
                // or the repository method needs to be enhanced to take these individual criteria.

                // For now, let's assume the repository's GetFilteredPartsPagedAsync can handle the basic criteria directly
                // and we might enhance it or the service logic later for complex vehicle filtering.
                // We will pass compatibleVehicleId directly if present in filterCriteria.
                // If MakeId, ModelId etc. are present, the service would typically find the corresponding
                // CompatibleVehicle IDs first and then filter parts. This is a more advanced step.
                // Let's proceed with what the current repository signature supports.


                IEnumerable<int>? vehicleSpecIdsToFilterBy = null;

                // Check if vehicle-specific filters are present in PartFilterCriteriaDto
                if (filterCriteria.MakeId.HasValue ||
                    filterCriteria.ModelId.HasValue ||
                    filterCriteria.TrimId.HasValue || // Assuming TrimId is in your DTO
                    filterCriteria.SpecificYear.HasValue /* || other vehicle criteria */)
                {
                    // 1. Construct a CompatibleVehicleFilterCriteriaDto from PartFilterCriteriaDto
                    var vehicleFilter = new CompatibleVehicleFilterCriteriaDto(
                        MakeId: filterCriteria.MakeId,
                        ModelId: filterCriteria.ModelId,
                        TrimLevelId: filterCriteria.TrimId,
                        ExactYear: filterCriteria.SpecificYear
                    // Map other relevant vehicle criteria if present in PartFilterCriteriaDto
                    );

                    _logger.LogInformation("Part filter requires vehicle spec filtering. Criteria: {@VehicleFilter}", vehicleFilter);

                    // 2. Call ICompatibleVehicleService to get matching CompatibleVehicle IDs
                    // You might want a method on ICompatibleVehicleService that just returns IDs for efficiency:
                    // Task<IEnumerable<int>> GetMatchingCompatibleVehicleIdsAsync(CompatibleVehicleFilterCriteriaDto criteria);
                    // Or use the existing method and extract IDs:
                    var matchingVehicleSpecs = await _compatibleVehicleService.GetAllFilteredCompatibleVehiclesAsync(vehicleFilter);
                    vehicleSpecIdsToFilterBy = matchingVehicleSpecs?.Select(vs => vs.Id).ToList();

                    if (vehicleSpecIdsToFilterBy == null || !vehicleSpecIdsToFilterBy.Any())
                    {
                        _logger.LogInformation("No compatible vehicle specs found matching the part filter's vehicle criteria. Returning empty result for parts.");
                        return new PagedResult<PartSummaryDto> { Items = Enumerable.Empty<PartSummaryDto>(), TotalCount = 0, PageNumber = pageNumber, PageSize = pageSize };
                    }
                    _logger.LogInformation("Found {Count} vehicle spec IDs to filter parts by.", vehicleSpecIdsToFilterBy.Count());
                }


                var partEntities = await _unitOfWork.Parts.GetFilteredPartsPagedAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    filterPredicate: null, // Direct predicate not used if using DTO criteria below
                    categoryId: filterCriteria.CategoryId,
                    manufacturer: filterCriteria.Manufacturer,
                    supplierId: filterCriteria.SupplierId,
                     // compatibleVehicleId: filterCriteria.CompatibleVehicleId, // This might be redundant if using the list below
                     restrictToCompatibleVehicleIds: vehicleSpecIdsToFilterBy, // NEW PARAMETER
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
                    restrictToCompatibleVehicleIds: vehicleSpecIdsToFilterBy,
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

            string? persistentImagePath = null;
            try
            {
                // 2. Handle Image Saving (if an image path is provided)
                if (!string.IsNullOrWhiteSpace(createDto.ImagePath))
                {
                    _logger.LogInformation("Image source path provided: '{SourcePath}'. Saving image for new part.", createDto.ImagePath);
                    persistentImagePath = await _imageFileService.SaveImageAsync(createDto.ImagePath, "Parts"); // "Parts" subfolder
                    _logger.LogInformation("Image saved. Persistent path: '{PersistentPath}' for new part.", persistentImagePath);
                }

                // 3. Map DTO to Domain Entity
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
                    ImagePath = persistentImagePath, // Store the final persistent path (or null)
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

                // 6. Handle Initial Suppliers and Compatibilities (if provided and if part creation was successful)
                // These are often better handled as separate calls after the main part is created,
                // but if included in CreatePartDto, process them here.
                if (createDto.InitialSuppliers != null && createDto.InitialSuppliers.Any())
                {
                    foreach (var supDto in createDto.InitialSuppliers)
                    {
                        // Assuming AddSupplierToPartAsync handles validation of SupplierId
                        await AddSupplierToPartAsync(newPart.Id, supDto);
                    }
                }
                if (createDto.InitialCompatibleVehicles != null && createDto.InitialCompatibleVehicles.Any())
                {
                    foreach (var compDto in createDto.InitialCompatibleVehicles)
                    {
                        // Assuming AddCompatibilityAsync handles validation of CompatibleVehicleId
                        await AddCompatibilityAsync(newPart.Id, compDto);
                    }
                }
                // Note: If adding suppliers/compatibility fails after part creation, the part is still created.
                // Consider if these should be part of the same transaction, or if failures are acceptable.

                await _unitOfWork.CommitTransactionAsync();

                // 7. Re-fetch with full details for return DTO
                var createdPartWithDetails = await _unitOfWork.Parts.GetByIdWithDetailsAsync(
                    newPart.Id, includeCategory: true, includeSuppliers: true, includeCompatibility: true);

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
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, "part creation (duplication)");
                throw;
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error saving image for new part '{PartName}'. Part not created.", createDto.Name);
                throw new ServiceException("Error saving part image. Part creation failed.", ioEx);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating part '{PartName}'.", createDto.Name);
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, "part creation (DB error)");
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new DuplicationException($"A part with similar unique information (e.g., Part Number '{createDto.PartNumber}') already exists.", nameof(Part), "UniqueConstraint", createDto.PartNumber, dbEx);
                }
                throw new ServiceException("Database error during part creation.", dbEx);
            }
            catch (Exception ex)
            {

                await _unitOfWork.RollbackTransactionAsync();


                _logger.LogError(ex, "Unexpected error creating part '{PartName}'.", createDto.Name);
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, "part creation (unexpected error)");
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

            string? oldPersistentImagePath = existingPart.ImagePath;
            string? newPersistentImagePath = existingPart.ImagePath; // Assume no change initially
            bool imageFileSavedForThisOperation = false;

            try
            {
                // 3. Handle Image Update/Removal
                if (updateDto.ImagePath != oldPersistentImagePath) // Intent to change image state
                {
                    if (!string.IsNullOrWhiteSpace(updateDto.ImagePath))
                    {
                        _logger.LogInformation("New image source path provided: '{SourcePath}'. Saving new image for Part ID {PartId}.", updateDto.ImagePath, updateDto.Id);
                        newPersistentImagePath = await _imageFileService.SaveImageAsync(updateDto.ImagePath, "Parts");
                        imageFileSavedForThisOperation = true;
                        _logger.LogInformation("New image saved. Persistent path: '{PersistentPath}' for Part ID {PartId}.", newPersistentImagePath, updateDto.Id);
                    }
                    else // User wants to remove image
                    {
                        _logger.LogInformation("Request to remove image for Part ID {PartId}.", updateDto.Id);
                        newPersistentImagePath = null;
                    }
                }

                // 4. Map DTO changes to existing Domain Entity
                existingPart.PartNumber = updateDto.PartNumber.Trim().ToUpperInvariant();
                existingPart.Name = updateDto.Name.Trim();
                existingPart.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
                existingPart.Manufacturer = string.IsNullOrWhiteSpace(updateDto.Manufacturer) ? null : updateDto.Manufacturer.Trim();
                existingPart.CostPrice = updateDto.CostPrice;
                existingPart.SellingPrice = updateDto.SellingPrice;
                existingPart.StockQuantity = updateDto.StockQuantity; // Base/defined stock
                // CurrentStock is NOT updated here. It's managed by Inventory transactions.
                existingPart.ReorderLevel = updateDto.ReorderLevel;
                existingPart.MinimumStock = updateDto.MinimumStock;
                existingPart.Location = string.IsNullOrWhiteSpace(updateDto.Location) ? string.Empty : updateDto.Location.Trim();
                existingPart.IsActive = updateDto.IsActive;
                existingPart.IsOriginal = updateDto.IsOriginal;
                existingPart.ImagePath = newPersistentImagePath;
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
                _logger.LogInformation("Part with ID {PartId} database record updated successfully. ImagePath is now: '{FinalImagePath}'",
                    updateDto.Id, existingPart.ImagePath ?? "<null>");

                // 6. Clean Up Old Image File
                if (!string.IsNullOrWhiteSpace(oldPersistentImagePath) && oldPersistentImagePath != newPersistentImagePath)
                {
                    await AttemptImageCleanupOnErrorAsync(oldPersistentImagePath, $"part update (old image cleanup, ID: {updateDto.Id})", isCleanupForOld: true);
                }
            }
            catch (ValidationException valEx) { _logger.LogWarning(valEx, "Validation failed during part update for ID: {PartId}.", updateDto.Id); throw; }
            catch (DuplicationException dupEx)
            {
                _logger.LogWarning(dupEx, "Duplication error during part update for ID: {PartId}.", updateDto.Id);
                if (imageFileSavedForThisOperation) await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"part update (duplication, ID: {updateDto.Id})");
                throw;
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error during image management for Part ID {PartId}.", updateDto.Id);
                throw new ServiceException("Error managing part image file. Part details may or may not have been saved.", ioEx);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error updating part ID {PartId}.", updateDto.Id);
                if (imageFileSavedForThisOperation) await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"part update (DB error, ID: {updateDto.Id})");

                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new DuplicationException($"A part with similar unique information (e.g., Part Number '{updateDto.PartNumber}') already exists.", nameof(Part), "UniqueConstraint", updateDto.PartNumber, dbEx);
                }
                throw new ServiceException("Database error during part update.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating part ID {PartId}.", updateDto.Id);
                if (imageFileSavedForThisOperation) await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"part update (unexpected error, ID: {updateDto.Id})");
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

        #region Part Image Handling

        /// <summary>
        /// Updates the image path for a specific part. Handles saving new image and deleting old one if necessary.
        /// </summary>
        public async Task UpdatePartImageAsync(int partId, string? sourceImagePath)
        {
            if (partId <= 0) throw new ArgumentException("Invalid Part ID.", nameof(partId));

            _logger.LogInformation("Attempting to update image for Part ID: {PartId}. New source image path: '{SourceImagePath}'",
                partId, sourceImagePath ?? "<null_or_empty_to_clear>");

            // 1. Fetch Existing Part
            var existingPart = await _unitOfWork.Parts.GetByIdAsync(partId);
            if (existingPart == null)
            {
                string notFoundMsg = $"Part with ID {partId} not found. Cannot update image.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
            }

            string? oldPersistentImagePath = existingPart.ImagePath;
            string? newPersistentImagePath = existingPart.ImagePath; // Assume no change initially
            bool imageFileSavedForThisOperation = false;

            try
            {
                // 2. Handle Image Update/Removal logic (same as in UpdatePartAsync)
                if (sourceImagePath != oldPersistentImagePath) // An intent to change image state
                {
                    if (!string.IsNullOrWhiteSpace(sourceImagePath))
                    {
                        // User provided a new source image path
                        _logger.LogInformation("New image source path provided: '{SourcePath}'. Saving new image for Part ID {PartId}.", sourceImagePath, partId);
                        newPersistentImagePath = await _imageFileService.SaveImageAsync(sourceImagePath, "Parts");
                        imageFileSavedForThisOperation = true;
                        _logger.LogInformation("New image saved. Persistent path: '{PersistentPath}' for Part ID {PartId}.", newPersistentImagePath, partId);
                    }
                    else // sourceImagePath is null or whitespace, meaning user wants to remove the image
                    {
                        _logger.LogInformation("Request to remove image for Part ID {PartId}. Old path was: '{OldPath}'", partId, oldPersistentImagePath ?? "<none>");
                        newPersistentImagePath = null;
                    }
                }

                // 3. Update only the ImagePath on the entity if it has changed
                if (existingPart.ImagePath != newPersistentImagePath)
                {
                    existingPart.ImagePath = newPersistentImagePath;
                    // existingPart.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext

                    // 4. Save Changes to Database
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Part ID {PartId} database record updated successfully with ImagePath: '{FinalImagePath}'",
                        partId, existingPart.ImagePath ?? "<null>");
                }
                else
                {
                    _logger.LogInformation("Image path for Part ID {PartId} is unchanged. No database update needed for image path.", partId);
                }


                // 5. Clean Up Old Image File (if applicable and different from new)
                if (!string.IsNullOrWhiteSpace(oldPersistentImagePath) && oldPersistentImagePath != newPersistentImagePath)
                {
                    _logger.LogInformation("Attempting to delete old image '{OldImagePath}' for Part ID {PartId}.", oldPersistentImagePath, partId);
                    await _imageFileService.DeleteImageAsync(oldPersistentImagePath);
                    _logger.LogInformation("Old image '{OldImagePath}' for Part ID {PartId} deleted successfully.", oldPersistentImagePath, partId);
                }

                _logger.LogInformation("Part image update process for ID {PartId} completed.", partId);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error during image management for Part ID {PartId}.", partId);
                throw new ServiceException("An error occurred managing the image file. The part image could not be updated.", ioEx);
            }
            catch (DbUpdateException dbEx) // Should be rare for just updating an image path unless concurrency issues
            {
                _logger.LogError(dbEx, "Database error occurred while updating image path for Part ID {PartId}.", partId);
                if (imageFileSavedForThisOperation && newPersistentImagePath != oldPersistentImagePath)
                {
                    await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"part image update (DB error, ID: {partId})");
                }
                throw new ServiceException("A database error occurred while updating the part's image information.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating image for Part ID {PartId}", partId);
                if (imageFileSavedForThisOperation && newPersistentImagePath != oldPersistentImagePath)
                {
                    await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"part image update (unexpected error, ID: {partId})");
                }
                throw new ServiceException($"An error occurred while updating image for part with ID {partId}.", ex);
            }
        }

        #endregion

        #region Compatible Vehicle 

        /// <summary>
        /// Adds a CompatibleVehicle specification to a Part.
        /// </summary>
        public async Task AddCompatibilityAsync(int partId, PartCompatibilityCreateDto compatibilityDto)
        {
            ArgumentNullException.ThrowIfNull(compatibilityDto, nameof(compatibilityDto));
            if (partId <= 0) throw new ArgumentException("Invalid Part ID.", nameof(partId));
            if (compatibilityDto.CompatibleVehicleId <= 0) throw new ArgumentException("Invalid CompatibleVehicle ID.", nameof(compatibilityDto.CompatibleVehicleId));

            _logger.LogInformation("Attempting to add compatibility link: PartId={PartId}, CompatibleVehicleId={CompatibleVehicleId}",
                partId, compatibilityDto.CompatibleVehicleId);

            // 1. Validate existence of Part and CompatibleVehicle
            // (Could also be done via FluentValidator for PartCompatibilityCreateDto if it took partId)
            if (!await _unitOfWork.Parts.ExistsAsync(p => p.Id == partId))
            {
                string msg = $"Part with ID {partId} not found. Cannot add compatibility.";
                _logger.LogWarning(msg);
                throw new ServiceException(msg); // Or NotFoundException
            }

            if (!await _unitOfWork.CompatibleVehicles.ExistsAsync(cv => cv.Id == compatibilityDto.CompatibleVehicleId))
            {
                string msg = $"CompatibleVehicle specification with ID {compatibilityDto.CompatibleVehicleId} not found. Cannot add compatibility.";
                _logger.LogWarning(msg);
                throw new ServiceException(msg); // Or NotFoundException
            }

            // 2. Check if this specific compatibility link already exists
            // Assumes IPartCompatibilityRepository is exposed via _unitOfWork.PartCompatibilities
            // And it has a method like: Task<bool> ExistsAsync(int partId, int compatibleVehicleId);
            // Or use the generic ExistsAsync:
            bool linkExists = await _unitOfWork.ExistsAsync<PartCompatibility>(
                pc => pc.PartId == partId && pc.CompatibleVehicleId == compatibilityDto.CompatibleVehicleId
            );

            if (linkExists)
            {
                string duplicateMsg = $"Compatibility link between Part ID {partId} and CompatibleVehicle ID {compatibilityDto.CompatibleVehicleId} already exists.";
                _logger.LogWarning(duplicateMsg);
                throw new DuplicationException(duplicateMsg, "PartCompatibility", "PartId_CompatibleVehicleId", $"{partId}_{compatibilityDto.CompatibleVehicleId}");
            }

            try
            {
                // 3. Create new PartCompatibility entity
                var newCompatibilityLink = new PartCompatibility
                {
                    PartId = partId,
                    CompatibleVehicleId = compatibilityDto.CompatibleVehicleId,
                    Notes = string.IsNullOrWhiteSpace(compatibilityDto.Notes) ? null : compatibilityDto.Notes.Trim(),
                    CreatedAt = DateTime.UtcNow // Or handled by BaseEntity/DbContext
                };

                // 4. Add to Repository (assuming a repository for PartCompatibility)
                // If no dedicated repo, add directly to context:
                // await _unitOfWork.Context.Set<PartCompatibility>().AddAsync(newCompatibilityLink);
                // OR if you have a generic Add method on UoW:
                await _unitOfWork.AddAsync(newCompatibilityLink); // Using a hypothetical generic Add on UoW
                // OR if Part.CompatibleVehicles is a collection you can add to:
                // var part = await _unitOfWork.Parts.GetByIdAsync(partId); // Fetch part if not already loaded
                // part.CompatibleVehicles.Add(newCompatibilityLink); // EF Core tracks this relationship

                // For clarity and consistency, let's assume a generic way to add the join entity.
                // If you have DbSet<PartCompatibility> on your DbContext, the generic add is simplest.
                // If you have a dedicated IPartCompatibilityRepository, use that.


                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully added compatibility link: PartId={PartId}, CompatibleVehicleId={CompatibleVehicleId}, LinkId={LinkId}",
                    partId, compatibilityDto.CompatibleVehicleId, newCompatibilityLink.Id);

                // Note: No DTO is typically returned for a simple "add link" operation.
                // The caller usually refreshes its list of compatibilities for the part.
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error adding compatibility: PartId={PartId}, CompatibleVehicleId={CompatibleVehicleId}.",
                    partId, compatibilityDto.CompatibleVehicleId);
                // Check for specific constraint violations if needed
                throw new ServiceException("A database error occurred while adding the compatibility link.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding compatibility: PartId={PartId}, CompatibleVehicleId={CompatibleVehicleId}.",
                    partId, compatibilityDto.CompatibleVehicleId);
                throw new ServiceException("An unexpected error occurred while adding the compatibility link.", ex);
            }
        }


        /// <summary>
        /// Removes a specific PartCompatibility link by its own ID.
        /// </summary>
        public async Task RemoveCompatibilityAsync(int partCompatibilityId)
        {
            if (partCompatibilityId <= 0)
            {
                _logger.LogWarning("Attempted to remove compatibility link with invalid ID: {PartCompatibilityId}", partCompatibilityId);
                throw new ArgumentException("Invalid PartCompatibility ID.", nameof(partCompatibilityId));
            }

            _logger.LogInformation("Attempting to remove PartCompatibility link with ID: {PartCompatibilityId}", partCompatibilityId);

            try
            {
                // 1. Fetch the PartCompatibility link entity
                var linkToDelete = await _unitOfWork.PartCompatibilities.GetByIdAsync(partCompatibilityId);

                if (linkToDelete == null)
                {
                    string notFoundMsg = $"PartCompatibility link with ID {partCompatibilityId} not found.";
                    _logger.LogWarning(notFoundMsg);
                    throw new ServiceException(notFoundMsg); // Or custom NotFoundException
                }

                // 2. Remove the entity
                _unitOfWork.PartCompatibilities.Delete(linkToDelete);

                // 3. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully removed PartCompatibility link with ID: {PartCompatibilityId} (PartId: {PartId}, CompatibleVehicleId: {CVId})",
                    partCompatibilityId, linkToDelete.PartId, linkToDelete.CompatibleVehicleId);
            }
            catch (DbUpdateException dbEx) // For potential concurrency issues or unexpected DB errors
            {
                _logger.LogError(dbEx, "Database error while removing PartCompatibility link ID {PartCompatibilityId}.", partCompatibilityId);
                throw new ServiceException("A database error occurred while removing the compatibility link.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while removing PartCompatibility link ID {PartCompatibilityId}.", partCompatibilityId);
                throw new ServiceException($"An unexpected error occurred while removing compatibility link ID {partCompatibilityId}.", ex);
            }
        }

        /// <summary>
        /// Gets all compatibility links (as DTOs) for a specific part.
        /// </summary>
        public async Task<IEnumerable<PartCompatibilityDto>> GetCompatibilitiesForPartAsync(int partId)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get compatibilities for invalid Part ID: {PartId}", partId);
                return Enumerable.Empty<PartCompatibilityDto>();
            }

            _logger.LogInformation("Attempting to retrieve compatibility links for Part ID: {PartId}", partId);
            try
            {
                // We need to fetch PartCompatibility entities and include their related CompatibleVehicle details
                var compatibilityEntities = await _unitOfWork.PartCompatibilities.GetByPartIdWithCompatibleVehicleDetailsAsync(partId);

                var dtos = compatibilityEntities.Select(MapPartCompatibilityToDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} compatibility links for Part ID {PartId}.", dtos.Count, partId);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving compatibility links for Part ID {PartId}.", partId);
                throw new ServiceException($"Could not retrieve compatibility links for Part ID {partId}.", ex);
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
                ImagePath = part.ImagePath,
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

                CompatibleVehicles = part.CompatibleVehicles?.Select(pc => new PartCompatibilityDto(
                    pc.Id,
                    pc.CompatibleVehicleId,
                    pc.CompatibleVehicle?.Model?.Make?.Name ?? "N/A",
                    pc.CompatibleVehicle?.Model?.Name ?? "N/A",
                    pc.CompatibleVehicle?.YearStart ?? 0,
                    pc.CompatibleVehicle?.YearEnd ?? 0,
                    pc.CompatibleVehicle?.TrimLevel?.Name,
                    pc.CompatibleVehicle?.EngineType?.Name,
                    pc.CompatibleVehicle?.TransmissionType?.Name,
                    pc.Notes
                )).ToList() ?? new List<PartCompatibilityDto>(),

                Suppliers = part.Suppliers?.Select(sp => new PartSupplierDto(
                    sp.Id,
                    sp.SupplierId,
                    sp.Supplier?.Name ?? "N/A",
                    sp.SupplierPartNumber,
                    sp.Cost,
                    sp.IsPreferredSupplier,
                    sp.LeadTimeInDays,
                    sp.MinimumOrderQuantity
                )).ToList() ?? new List<PartSupplierDto>()
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
                part.ImagePath,
                part.Location


            );
        }

        private PartCompatibilityDto MapPartCompatibilityToDto(PartCompatibility pc)
        {
            if (pc == null) throw new ArgumentNullException(nameof(pc));
            if (pc.CompatibleVehicle == null)
            {
                // This case should ideally not happen if includes are correct and data is consistent
                _logger.LogWarning("PartCompatibility ID {PartCompatibilityId} has a null CompatibleVehicle navigation property during mapping.", pc.Id);
                // Return a DTO with minimal info or throw, depending on how strict you want to be.
                return new PartCompatibilityDto(pc.Id, pc.CompatibleVehicleId, "Error", "Error", 0, 0, null, null, null, pc.Notes);
            }

            return new PartCompatibilityDto(
                pc.Id,
                pc.CompatibleVehicleId,
                pc.CompatibleVehicle.Model?.Make?.Name ?? "N/A",
                pc.CompatibleVehicle.Model?.Name ?? "N/A",
                pc.CompatibleVehicle.YearStart,
                pc.CompatibleVehicle.YearEnd,
                pc.CompatibleVehicle.TrimLevel?.Name,
                pc.CompatibleVehicle.EngineType?.Name,
                pc.CompatibleVehicle.TransmissionType?.Name,
                // BodyType was missing in your DTO, but your CompatibleVehicle model has it.
                // Assuming PartCompatibilityDto doesn't need it, or you'll add it.
                pc.Notes
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

        #endregion

    }
}
