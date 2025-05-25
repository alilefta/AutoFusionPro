using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Storage;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Models;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq.Expressions;

namespace AutoFusionPro.Application.Services.DataServices
{
    public partial class CompatibleVehicleService : ICompatibleVehicleService
    {

        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompatibleVehicleService> _logger;
        private readonly IImageFileService _imageFileService;

        // Add Validators if you use them for Make DTOs
        private readonly IValidator<CreateMakeDto> _createMakeValidator;
        private readonly IValidator<UpdateMakeDto> _updateMakeValidator;

        private readonly IValidator<CreateModelDto> _createModelValidator;
        private readonly IValidator<UpdateModelDto> _updateModelValidator;

        private readonly IValidator<CreateLookupDto> _createLookupValidator;
        private readonly IValidator<UpdateLookupDto> _updateLookupValidator;

        private readonly IValidator<CreateEngineTypeDto> _createEngineTypeValidator;
        private readonly IValidator<UpdateEngineTypeDto> _updateEngineTypeValidator;

        private readonly IValidator<CreateTrimLevelDto> _createTrimLevelValidator;
        private readonly IValidator<UpdateTrimLevelDto> _updateTrimLevelValidator;

        private readonly IValidator<CreateCompatibleVehicleDto> _createCompatibleVehicleValidator;
        private readonly IValidator<UpdateCompatibleVehicleDto> _updateCompatibleVehicleValidator;

        #endregion

        public CompatibleVehicleService(
                    IUnitOfWork unitOfWork,
                    ILogger<CompatibleVehicleService> logger,
                    IImageFileService imageFileService,

                    IValidator<CreateMakeDto> createMakeValidator,
                    IValidator<UpdateMakeDto> updateMakeValidator,

                    IValidator<CreateModelDto> createModelValidator, 
                    IValidator<UpdateModelDto> updateModelValidator,

                    IValidator<CreateLookupDto> createLookupValidator,
                    IValidator<UpdateLookupDto> updateLookupValidator,

                    IValidator<CreateCompatibleVehicleDto> createCompatibleVehicleValidator,
                    IValidator<UpdateCompatibleVehicleDto> updateCompatibleVehicleValidator,

                    IValidator<CreateEngineTypeDto> createEngineTypeValidator,
                    IValidator<UpdateEngineTypeDto> updateEngineTypeValidator,
                    IValidator<CreateTrimLevelDto> createTrimLevelValidator,
                    IValidator<UpdateTrimLevelDto> updateTrimLevelValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageFileService = imageFileService ?? throw new ArgumentNullException(nameof(imageFileService));

            _createMakeValidator = createMakeValidator ?? throw new ArgumentNullException(nameof(createMakeValidator));
            _updateMakeValidator = updateMakeValidator ?? throw new ArgumentNullException(nameof(updateMakeValidator));

            _createCompatibleVehicleValidator = createCompatibleVehicleValidator ?? throw new ArgumentNullException(nameof(createCompatibleVehicleValidator));
            _updateCompatibleVehicleValidator = updateCompatibleVehicleValidator ?? throw new ArgumentNullException(nameof(updateCompatibleVehicleValidator));

            _createModelValidator = createModelValidator ?? throw new ArgumentNullException(nameof(createModelValidator));
            _updateModelValidator = updateModelValidator ?? throw new ArgumentNullException(nameof(updateModelValidator));

            _createLookupValidator = createLookupValidator ?? throw new ArgumentNullException(nameof(createLookupValidator));
            _updateLookupValidator = updateLookupValidator ?? throw new ArgumentNullException(nameof(updateLookupValidator));

            _createEngineTypeValidator = createEngineTypeValidator ?? throw new ArgumentNullException(nameof(createEngineTypeValidator));
            _updateEngineTypeValidator = updateEngineTypeValidator ?? throw new ArgumentNullException(nameof(updateEngineTypeValidator));
            _createTrimLevelValidator = createTrimLevelValidator ?? throw new ArgumentNullException(nameof(createTrimLevelValidator));
            _updateTrimLevelValidator = updateTrimLevelValidator ?? throw new ArgumentNullException(nameof(updateTrimLevelValidator));
        }



        #region CompatibleVehicle Management

        public async Task<CompatibleVehicleDetailDto?> GetCompatibleVehicleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get CompatibleVehicle with invalid ID: {CompatibleVehicleId}", id);
                return null; // Or throw ArgumentOutOfRangeException
            }
            _logger.LogInformation("Attempting to retrieve CompatibleVehicle with ID: {CompatibleVehicleId}", id);
            try
            {
                var compatibleVehicle = await _unitOfWork.CompatibleVehicles.GetByIdWithDetailsAsync(id);
                if (compatibleVehicle == null)
                {
                    _logger.LogWarning("CompatibleVehicle with ID {CompatibleVehicleId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved CompatibleVehicle with ID {CompatibleVehicleId}", id);
                return MapCompatibleVehicleToDetailDto(compatibleVehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving CompatibleVehicle with ID {CompatibleVehicleId}.", id);
                throw new ServiceException($"Could not retrieve CompatibleVehicle specification with ID {id}.", ex);
            }
        }

        public async Task<PagedResult<CompatibleVehicleSummaryDto>> GetFilteredCompatibleVehiclesAsync(
            CompatibleVehicleFilterCriteriaDto filterCriteria, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10; // Default page size
            if (pageSize > 100) pageSize = 100; // Max page size

            _logger.LogInformation("Retrieving filtered CompatibleVehicles. Page: {Page}, Size: {Size}, Criteria: {@Criteria}",
                pageNumber, pageSize, filterCriteria);

            try
            {
                var predicate = BuildCompatibleVehicleFilterPredicate(filterCriteria);

                var compatibleVehicles = await _unitOfWork.CompatibleVehicles
                    .GetFilteredWithDetailsPagedAsync(pageNumber, pageSize, predicate, filterCriteria.MakeId, filterCriteria.SearchTerm);

                var totalCount = await _unitOfWork.CompatibleVehicles
                    .GetTotalCountAsync(predicate, filterCriteria.MakeId, filterCriteria.SearchTerm);

                var summaryDtos = compatibleVehicles.Select(MapCompatibleVehicleToSummaryDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} CompatibleVehicles for page {Page}. Total matching: {Total}",
                    summaryDtos.Count, pageNumber, totalCount);

                return new PagedResult<CompatibleVehicleSummaryDto>
                {
                    Items = summaryDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving filtered CompatibleVehicles with criteria: {@Criteria}", filterCriteria);
                throw new ServiceException("Could not retrieve CompatibleVehicle specifications.", ex);
            }
        }


        public async Task<CompatibleVehicleDetailDto> CreateCompatibleVehicleAsync(CreateCompatibleVehicleDto createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            _logger.LogInformation("Attempting to create CompatibleVehicle: ModelId={ModelId}, YearStart={YearStart}, YearEnd={YearEnd}",
                createDto.ModelId, createDto.YearStart, createDto.YearEnd);

            // 1. Validate DTO
            var validationResult = await _createCompatibleVehicleValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateCompatibleVehicleDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }
            // Note: Uniqueness for the spec is now primarily handled by the validator's MustAsync rule.

            try
            {
                // 2. Map DTO to Domain Entity
                var newCompatibleVehicle = MapCreateDtoToCompatibleVehicle(createDto);

                // 3. Add to Repository
                await _unitOfWork.CompatibleVehicles.AddAsync(newCompatibleVehicle);

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("CompatibleVehicle created successfully with ID: {CompatibleVehicleId}", newCompatibleVehicle.Id);

                // 5. Re-fetch with details for return (to get all related names)
                var createdWithDetails = await _unitOfWork.CompatibleVehicles.GetByIdWithDetailsAsync(newCompatibleVehicle.Id);
                if (createdWithDetails == null)
                {
                    _logger.LogError("Critical: Failed to re-fetch CompatibleVehicle with ID {Id} immediately after creation.", newCompatibleVehicle.Id);
                    throw new ServiceException($"Failed to retrieve details for newly created CompatibleVehicle specification ID {newCompatibleVehicle.Id}.");
                }
                return MapCompatibleVehicleToDetailDto(createdWithDetails);
            }
            catch (DbUpdateException dbEx) // Handles potential database constraint violations not caught by validator
            {
                _logger.LogError(dbEx, "Database error while creating CompatibleVehicle. ModelId={ModelId}, YearStart={YearStart}.",
                    createDto.ModelId, createDto.YearStart);
                // Check for unique constraint violation if validator didn't catch all scenarios or if there's a race condition
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase)) // SQLite specific
                {
                    throw new ServiceException("This exact vehicle specification already exists.", dbEx);
                }
                throw new ServiceException("A database error occurred while creating the vehicle specification.", dbEx);
            }
            catch (ValidationException) { throw; } // Re-throw validation exceptions
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating CompatibleVehicle. ModelId={ModelId}, YearStart={YearStart}.",
                    createDto.ModelId, createDto.YearStart);
                throw new ServiceException("An unexpected error occurred while creating the vehicle specification.", ex);
            }
        }

        public async Task UpdateCompatibleVehicleAsync(UpdateCompatibleVehicleDto updateDto)
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid CompatibleVehicle ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update CompatibleVehicle with ID: {CompatibleVehicleId}", updateDto.Id);

            // 1. Validate DTO
            var validationResult = await _updateCompatibleVehicleValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateCompatibleVehicleDto (ID: {Id}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }
            // Uniqueness check excluding self is handled by the validator.

            // 2. Fetch Existing Entity
            var existingCompatibleVehicle = await _unitOfWork.CompatibleVehicles.GetByIdAsync(updateDto.Id);
            if (existingCompatibleVehicle == null)
            {
                string notFoundMsg = $"CompatibleVehicle specification with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
            }

            try
            {
                // 3. Map DTO changes to existing Domain Entity
                MapUpdateDtoToCompatibleVehicle(updateDto, existingCompatibleVehicle);

                // 4. Save Changes (EF Core tracks changes on the 'existingCompatibleVehicle' entity)
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("CompatibleVehicle with ID {CompatibleVehicleId} updated successfully.", updateDto.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while updating CompatibleVehicle ID {Id}.", updateDto.Id);
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ServiceException("This exact vehicle specification already exists for another record.", dbEx);
                }
                throw new ServiceException("A database error occurred while updating the vehicle specification.", dbEx);
            }
            catch (ValidationException) { throw; } // Re-throw validation exceptions
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating CompatibleVehicle ID {Id}", updateDto.Id);
                throw new ServiceException($"An unexpected error occurred while updating vehicle specification ID {updateDto.Id}.", ex);
            }
        }

        public async Task DeleteCompatibleVehicleAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid CompatibleVehicle ID for deletion.", nameof(id));
            _logger.LogInformation("Attempting to delete CompatibleVehicle with ID: {CompatibleVehicleId}", id);

            var compatibleVehicleToDelete = await _unitOfWork.CompatibleVehicles.GetByIdAsync(id);
            if (compatibleVehicleToDelete == null)
            {
                _logger.LogWarning("CompatibleVehicle specification with ID {Id} not found for deletion. No action taken.", id);
                // Decide: return gracefully or throw a not found exception
                throw new ServiceException($"CompatibleVehicle specification with ID {id} not found.");
            }

            // Dependency Check: Is this CompatibleVehicle linked in any PartCompatibility records?
            // Assuming IUnitOfWork has a generic ExistsAsync or IPartCompatibilityRepository has such a method.
            bool isLinkedToParts = await _unitOfWork.ExistsAsync<PartCompatibility>(pc => pc.CompatibleVehicleId == id);
            if (isLinkedToParts)
            {
                string dependencyError = $"Cannot delete CompatibleVehicle specification ID {id} as it is currently linked to one or more parts. Please remove those links first.";
                _logger.LogError(dependencyError);
                throw new ServiceException(dependencyError);
            }

            try
            {
                _unitOfWork.CompatibleVehicles.Delete(compatibleVehicleToDelete);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("CompatibleVehicle with ID {CompatibleVehicleId} deleted successfully.", id);
            }
            catch (DbUpdateException dbEx) // Should be rare if dependency check is thorough
            {
                _logger.LogError(dbEx, "Database error while deleting CompatibleVehicle ID {Id}.", id);
                throw new ServiceException("A database error occurred while deleting the vehicle specification. It might still be in use.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting CompatibleVehicle ID {Id}", id);
                throw new ServiceException($"An unexpected error occurred while deleting vehicle specification ID {id}.", ex);
            }
        }

        // This service method directly calls the repository.
        // It's useful for UI/ViewModels to proactively check before attempting creation.
        public async Task<bool> CompatibleVehicleSpecExistsAsync(
            int modelId, int yearStart, int yearEnd,
            int? trimLevelId, int? transmissionTypeId, int? engineTypeId, int? bodyTypeId,
            int? excludeCompatibleVehicleId = null)
        {
            _logger.LogDebug("Service check if CompatibleVehicle spec exists: ModelId={ModelId}, Years={YearStart}-{YearEnd}, ExcludeId={ExcludeId} ...",
                modelId, yearStart, yearEnd, excludeCompatibleVehicleId);
            try
            {
                return await _unitOfWork.CompatibleVehicles.SpecificationExistsAsync(
                    modelId, yearStart, yearEnd,
                    trimLevelId, transmissionTypeId, engineTypeId, bodyTypeId,
                    excludeCompatibleVehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during service check for CompatibleVehicle specification existence.");
                // Depending on desired behavior, either re-throw or return true to prevent operation
                throw new ServiceException("Could not verify if vehicle specification exists due to an internal error.", ex);
            }
        }
        #endregion

        #region Make Methods

        public async Task<IEnumerable<MakeDto>> GetAllMakesAsync()
        {
            _logger.LogInformation("Attempting to retrieve all makes.");
            try
            {
                var makes = await _unitOfWork.Makes.GetAllAsync();
                var makeDtos = makes.Select(m => new MakeDto(m.Id, m.Name, m.ImagePath))
                                    .OrderBy(m => m.Id)
                                    .ToList();
                _logger.LogInformation("Successfully retrieved {Count} makes.", makeDtos.Count);
                return makeDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all makes.");
                throw new ServiceException("Could not retrieve makes.", ex);
            }
        }

        public async Task<MakeDto?> GetMakeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get make with invalid ID: {MakeId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve make with ID: {MakeId}", id);
            try
            {
                var make = await _unitOfWork.Makes.GetByIdAsync(id);
                if (make == null)
                {
                    _logger.LogWarning("Make with ID {MakeId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved make with ID {MakeId}: {MakeName}", id, make.Name);
                return new MakeDto(make.Id, make.Name, make.ImagePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving make with ID {MakeId}.", id);
                throw new ServiceException($"Could not retrieve make with ID {id}.", ex);
            }
        }

        public async Task<MakeDto> CreateMakeAsync(CreateMakeDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            _logger.LogInformation("Attempting to create make with Name: {MakeName}", createDto.Name);

            // 1. Validate DTO using FluentValidation
            var validationResult = await _createMakeValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateMakeDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Check for uniqueness (could also be part of FluentValidator if NameExistsAsync is injected)
            if (await _unitOfWork.Makes.NameExistsAsync(createDto.Name))
            {
                string duplicateMessage = $"Make with name '{createDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new DuplicationException(duplicateMessage, nameof(Make), "Name", createDto.Name);
            }


            string? persistentImagePath = null;

            try
            {

                if (!string.IsNullOrWhiteSpace(createDto.ImagePath))
                {
                    _logger.LogInformation("Image source path provided: '{SourcePath}'. Saving image for new make.", createDto.ImagePath);
                    persistentImagePath = await _imageFileService.SaveImageAsync(createDto.ImagePath, "Makes");
                    _logger.LogInformation("Image saved. Persistent path: '{PersistentPath}' for new make.", persistentImagePath);
                }

                // 3. Map DTO to Domain Entity
                var newMake = new Make
                {
                    Name = createDto.Name.Trim(), // Trim whitespace,
                    ImagePath = persistentImagePath
                };

                // 4. Add to Repository
                await _unitOfWork.Makes.AddAsync(newMake);


                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Make created successfully with ID {MakeId}, Name {MakeName}, ImagePath: {ImagePath}",
                            newMake.Id, newMake.Name, newMake.ImagePath ?? "<null>");

                // 6. Map back to DTO for return
                return new MakeDto(newMake.Id, newMake.Name, newMake.ImagePath);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error occurred while saving image for new make {MakeName}.", createDto.Name);
                throw new ServiceException("An error occurred saving the image file. The make was not created.", ioEx);
            }
            catch (DbUpdateException dbEx) // Catch potential DB constraint issues
            {
                _logger.LogError(dbEx, "Database error occurred while creating make: {MakeName}. Check InnerException for details.", createDto.Name);

                // Rollback image save
                if (!string.IsNullOrWhiteSpace(persistentImagePath))
                {
                    _logger.LogWarning("Attempting to clean up orphaned image file: {ImagePath}", persistentImagePath);
                    await _imageFileService.DeleteImageAsync(persistentImagePath); // Attempt cleanup
                }

                throw new ServiceException("A database error occurred while creating the make.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating make: {MakeName}", createDto.Name);
                throw new ServiceException($"An error occurred while creating make '{createDto.Name}'.", ex);
            }
        }

        public async Task UpdateMakeAsync(UpdateMakeDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Make ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update make with ID: {MakeId}, New Name: {NewMakeName}, New ImagePath: {NewImagePath}",
                updateDto.Id, updateDto.Name, updateDto.ImagePath);

            var validationResult = await _updateMakeValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateMakeDto (ID: {MakeId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            var existingMake = await _unitOfWork.Makes.GetByIdAsync(updateDto.Id);
            if (existingMake == null)
            {
                string notFoundMsg = $"Make with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg);
            }

            if (!existingMake.Name.Equals(updateDto.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                await _unitOfWork.Makes.NameExistsAsync(updateDto.Name, updateDto.Id))
            {
                string duplicateMessage = $"Make with name '{updateDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new ServiceException(duplicateMessage);
            }

            string? oldPersistentImagePath = existingMake.ImagePath;
            string? newPersistentImagePath = existingMake.ImagePath; // Assume no change initially

            try
            {
                if (updateDto.ImagePath != oldPersistentImagePath) // Only proceed if the proposed path state is different
                {
                    if (!string.IsNullOrWhiteSpace(updateDto.ImagePath))
                    {
                        // Case 1: User provided a new source image path to set/change the image.
                        _logger.LogInformation("New image source path provided: '{SourcePath}'. Saving new image for Make ID {MakeId}.", updateDto.ImagePath, updateDto.Id);
                        newPersistentImagePath = await _imageFileService.SaveImageAsync(updateDto.ImagePath, "Makes");
                        _logger.LogInformation("New image saved. Persistent path: '{PersistentPath}' for Make ID {MakeId}.", newPersistentImagePath, updateDto.Id);
                    }
                    else // updateDto.ImagePath is null or whitespace
                    {
                        // Case 2: User wants to remove the existing image.
                        _logger.LogInformation("Request to remove image for Make ID {MakeId}. Old path was: '{OldPath}'", updateDto.Id, oldPersistentImagePath ?? "<none>");
                        newPersistentImagePath = null;
                    }
                }
                // If updateDto.ImagePath is the same as oldImagePath, newFinalImagePath remains oldImagePath, no file ops needed yet.


                existingMake.Name = updateDto.Name.Trim();
                existingMake.ImagePath = newPersistentImagePath;

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Make with ID {MakeId} database record updated successfully.", updateDto.Id);

                // --- Clean Up Old Image File (if applicable) ---
                // Delete the old physical image file IF:
                // 1. There was an old image (oldPersistentImagePath was not null/whitespace).
                // 2. AND The new persistent image path is different from the old one (this includes the case where the new path is null).
                if (!string.IsNullOrWhiteSpace(oldPersistentImagePath) && oldPersistentImagePath != newPersistentImagePath)
                {
                    _logger.LogInformation("Attempting to delete old image '{OldImagePath}' for Make ID {MakeId}.", oldPersistentImagePath, updateDto.Id);
                    await _imageFileService.DeleteImageAsync(oldPersistentImagePath);
                    _logger.LogInformation("Old image '{OldImagePath}' for Make ID {MakeId} deleted successfully.", oldPersistentImagePath, updateDto.Id);
                }


                _logger.LogInformation("Make with ID {MakeId} update process completed successfully.", updateDto.Id);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error during image management for Make ID {MakeId}.", updateDto.Id);
                throw new ServiceException("An error occurred managing the image file. The make details might have been saved, but the image could not be updated.", ioEx);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while updating make ID {MakeId}.", updateDto.Id);
                if (!string.IsNullOrEmpty(newPersistentImagePath))
                {
                  await _imageFileService.DeleteImageAsync(newPersistentImagePath);
                }
                throw new ServiceException("A database error occurred while updating the make.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating make ID {MakeId}", updateDto.Id);
                throw new ServiceException($"An error occurred while updating make with ID {updateDto.Id}.", ex);
            }
        }

        public async Task DeleteMakeAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Make ID for deletion.", nameof(id));

            _logger.LogInformation("Attempting to delete make with ID: {MakeId}", id);

            // 1. Fetch Existing Entity
            var makeToDelete = await _unitOfWork.Makes.GetByIdAsync(id);
            if (makeToDelete == null)
            {
                _logger.LogWarning("Make with ID {MakeId} not found for deletion. No action taken.", id);
                // Consider if throwing an exception is better if the caller expects it to exist
                return; // Or throw new ServiceException($"Make with ID {id} not found.");
            }

            // 2. Dependency Check: Check if any Models are associated with this Make
            // This requires IModelRepository to be accessible, e.g., via _unitOfWork.Models
            // Or a specific method on IMakeRepository: Task<bool> HasAssociatedModelsAsync(int makeId);
            bool hasModels = await _unitOfWork.Models.ExistsAsync(m => m.MakeId == id); // Example
            if (hasModels)
            {
                string dependencyError = $"Cannot delete Make";
                _logger.LogError(dependencyError);
                throw new DeletionBlockedException(dependencyError, nameof(Make), makeToDelete.Id.ToString(), new List<string> { nameof(Model)});
            }

            try
            {
                // 3. Delete Entity
                _unitOfWork.Makes.Delete(makeToDelete); // Mark for deletion

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();

                // Delete Image if exists
                if (!string.IsNullOrEmpty(makeToDelete.ImagePath))
                {
                    await _imageFileService.DeleteImageAsync(makeToDelete.ImagePath);
                }

                _logger.LogInformation("Make with ID {MakeId} and Name {MakeName} deleted successfully.", id, makeToDelete.Name);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting make's image with Path {modelToDelete.ImagePath}", makeToDelete.ImagePath);
                throw new ServiceException($"An error occurred while deleting make's image with Path {makeToDelete.ImagePath}.", ex);
            }
            catch (DbUpdateException dbEx) // Catch FK issues if dependency check was incomplete
            {
                _logger.LogError(dbEx, "Database error occurred while deleting make ID {MakeId}.", id);
                throw new ServiceException("A database error occurred while deleting the make. It might still be in use.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting make ID {MakeId}", id);
                throw new ServiceException($"An error occurred while deleting make with ID {id}.", ex);
            }
        }

        #endregion

        #region Model Management

        public async Task<IEnumerable<ModelDto>> GetModelsByMakeIdAsync(int makeId)
        {
            if (makeId <= 0)
            {
                _logger.LogWarning("Attempted to get models with invalid Make ID: {MakeId}", makeId);
                return Enumerable.Empty<ModelDto>();
            }
            _logger.LogInformation("Attempting to retrieve models for Make ID: {MakeId}", makeId);
            try
            {
                // Use the repository method that includes Make for DTO mapping
                var models = await _unitOfWork.Models.GetByMakeIdAsync(makeId);
                var modelDtos = models.Select(MapModelToDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} models for Make ID {MakeId}.", modelDtos.Count, makeId);
                return modelDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving models for Make ID {MakeId}.", makeId);
                throw new ServiceException($"Could not retrieve models for Make ID {makeId}.", ex);
            }
        }

        public async Task<ModelDto?> GetModelByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get model with invalid ID: {ModelId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve model with ID: {ModelId}", id);
            try
            {
                // Use repository method that includes Make for DTO mapping
                var model = await _unitOfWork.Models.GetByIdWithMakeAsync(id);
                if (model == null)
                {
                    _logger.LogWarning("Model with ID {ModelId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved model with ID {ModelId}: {ModelName}", id, model.Name);
                return MapModelToDto(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving model with ID {ModelId}.", id);
                throw new ServiceException($"Could not retrieve model with ID {id}.", ex);
            }
        }

        public async Task<ModelDto> CreateModelAsync(CreateModelDto createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            _logger.LogInformation("Attempting to create model: Name={ModelName}, MakeId={MakeId}", createDto.Name, createDto.MakeId);

            // 1. Validate DTO (this now includes MakeId existence and Name uniqueness for Make)
            var validationResult = await _createModelValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateModelDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Check for uniqueness (could also be part of FluentValidator if NameExistsAsync is injected)
            if (await _unitOfWork.Models.NameExistsForMakeAsync(createDto.Name, createDto.MakeId, null))
            {
                string duplicateMessage = $"Model with name '{createDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new DuplicationException(duplicateMessage, nameof(Model), "Name", createDto.Name);
            }

            string? persistentImagePath = null;


            try
            {

                if (!string.IsNullOrWhiteSpace(createDto.ImagePath))
                {
                    _logger.LogInformation("Image source path provided: '{SourcePath}'. Saving image for new model.", createDto.ImagePath);
                    persistentImagePath = await _imageFileService.SaveImageAsync(createDto.ImagePath, "Makes");
                    _logger.LogInformation("Image saved. Persistent path: '{PersistentPath}' for new model.", persistentImagePath);
                }

                // 2. Map DTO to Domain Entity
                var newModel = new Model
                {
                    Name = createDto.Name.Trim(),
                    MakeId = createDto.MakeId,
                    ImagePath = persistentImagePath
                };

                // 3. Add to Repository
                await _unitOfWork.Models.AddAsync(newModel);

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Model created successfully with ID {ModelId}, Name {ModelName}, ImagePath: {ImagePath}",
                            newModel.Id, newModel.Name, newModel.ImagePath ?? "<null>");

                // 5. Re-fetch with Make for complete DTO (or map carefully)
                var createdModelWithMake = await _unitOfWork.Models.GetByIdWithMakeAsync(newModel.Id);
                if (createdModelWithMake == null) // Should not happen
                {
                    _logger.LogError("Failed to re-fetch model with ID {ModelId} after creation.", newModel.Id);
                    throw new ServiceException($"Failed to retrieve details for newly created model '{newModel.Name}'.");
                }
                return MapModelToDto(createdModelWithMake);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error occurred while saving image for new make {MakeName}.", createDto.Name);
                throw new ServiceException("An error occurred saving the image file. The make was not created.", ioEx);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while creating model: Name={ModelName}, MakeId={MakeId}. Check InnerException.",
                    createDto.Name, createDto.MakeId);

                // Rollback image save
                if (!string.IsNullOrWhiteSpace(persistentImagePath))
                {
                    _logger.LogWarning("Attempting to clean up orphaned image file: {ImagePath}", persistentImagePath);
                    await _imageFileService.DeleteImageAsync(persistentImagePath); // Attempt cleanup
                }

                throw new ServiceException("A database error occurred while creating the model.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating model: Name={ModelName}, MakeId={MakeId}",
                    createDto.Name, createDto.MakeId);
                throw new ServiceException($"An error occurred while creating model '{createDto.Name}'.", ex);
            }
        }

        public async Task UpdateModelAsync(UpdateModelDto updateDto)
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Model ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update model with ID: {ModelId}, New Name: {NewModelName}, New MakeId: {NewMakeId}",
                updateDto.Id, updateDto.Name, updateDto.MakeId);

            // 1. Validate DTO (includes MakeId existence and Name uniqueness excluding self)
            var validationResult = await _updateModelValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateModelDto (ID: {ModelId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingModel = await _unitOfWork.Models.GetByIdAsync(updateDto.Id);
            if (existingModel == null)
            {
                string notFoundMsg = $"Model with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg);
            }

            // 3. Check for uniqueness (could also be part of FluentValidator if NameExistsAsync is injected)
            if (!existingModel.Name.Equals(updateDto.Name.Trim(), StringComparison.OrdinalIgnoreCase) && await _unitOfWork.Models.NameExistsForMakeAsync(updateDto.Name, updateDto.MakeId, null))
            {
                string duplicateMessage = $"Model with name '{updateDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new DuplicationException(duplicateMessage, nameof(Model), "Name", updateDto.Name);
            }


            string? oldPersistentImagePath = existingModel.ImagePath;
            string? newPersistentImagePath = existingModel.ImagePath; // Assume no change initially

            try
            {

                if (updateDto.ImagePath != oldPersistentImagePath) // Only proceed if the proposed path state is different
                {
                    if (!string.IsNullOrWhiteSpace(updateDto.ImagePath))
                    {
                        // Case 1: User provided a new source image path to set/change the image.
                        _logger.LogInformation("New image source path provided: '{SourcePath}'. Saving new image for Make ID {MakeId}.", updateDto.ImagePath, updateDto.Id);
                        newPersistentImagePath = await _imageFileService.SaveImageAsync(updateDto.ImagePath, "Makes");
                        _logger.LogInformation("New image saved. Persistent path: '{PersistentPath}' for Make ID {MakeId}.", newPersistentImagePath, updateDto.Id);
                    }
                    else // updateDto.ImagePath is null or whitespace
                    {
                        // Case 2: User wants to remove the existing image.
                        _logger.LogInformation("Request to remove image for Make ID {MakeId}. Old path was: '{OldPath}'", updateDto.Id, oldPersistentImagePath ?? "<none>");
                        newPersistentImagePath = null;
                    }
                }
                // If updateDto.ImagePath is the same as oldImagePath, newFinalImagePath remains oldImagePath, no file ops needed yet.


                // 3. Map changes from DTO to Existing Domain Entity
                existingModel.Name = updateDto.Name.Trim();
                existingModel.MakeId = updateDto.MakeId; // Update MakeId if changed
                existingModel.ImagePath = newPersistentImagePath; // Update MakeId if changed

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();

                // --- Clean Up Old Image File (if applicable) ---
                // Delete the old physical image file IF:
                // 1. There was an old image (oldPersistentImagePath was not null/whitespace).
                // 2. AND The new persistent image path is different from the old one (this includes the case where the new path is null).
                if (!string.IsNullOrWhiteSpace(oldPersistentImagePath) && oldPersistentImagePath != newPersistentImagePath)
                {
                    _logger.LogInformation("Attempting to delete old image '{OldImagePath}' for Model ID {ModelId}.", oldPersistentImagePath, updateDto.Id);
                    await _imageFileService.DeleteImageAsync(oldPersistentImagePath);
                    _logger.LogInformation("Old image '{OldImagePath}' for Model ID {ModelId} deleted successfully.", oldPersistentImagePath, updateDto.Id);
                }


                _logger.LogInformation("Model with ID {ModelId} updated successfully.", updateDto.Id);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error during image management for Model ID {ModelId}.", updateDto.Id);
                throw new ServiceException("An error occurred managing the image file. The model details might have been saved, but the image could not be updated.", ioEx);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while updating model ID {ModelId}.", updateDto.Id);
                if (!string.IsNullOrEmpty(newPersistentImagePath))
                {
                    await _imageFileService.DeleteImageAsync(newPersistentImagePath);
                }
                throw new ServiceException("A database error occurred while updating the model.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating model ID {ModelId}", updateDto.Id);
                throw new ServiceException($"An error occurred while updating model with ID {updateDto.Id}.", ex);
            }
        }

        public async Task DeleteModelAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Model ID for deletion.", nameof(id));
            _logger.LogInformation("Attempting to delete model with ID: {ModelId}", id);

            var modelToDelete = await _unitOfWork.Models.GetByIdAsync(id);
            if (modelToDelete == null)
            {
                _logger.LogWarning("Model with ID {ModelId} not found for deletion. No action taken.", id);
                return; // Or throw ServiceException
            }

            var dependentEntities = new List<string>();

            // Dependency Check 1: TrimLevels
            bool hasTrims = await _unitOfWork.Models.HasAssociatedTrimLevelsAsync(id); // Assumes this method exists on IModelRepository
            if (hasTrims)
            {
                dependentEntities.Add(nameof(TrimLevel));
            }

            // Dependency Check 2: CompatibleVehicles
            bool hasCompatibleVehicles = await _unitOfWork.Models.HasAssociatedCompatibleVehiclesAsync(id); // Assumes this method exists
            if (hasCompatibleVehicles)
            {
                dependentEntities.Add(nameof(CompatibleVehicle));
            }

            if (dependentEntities.Any())
            {
                string errorMessage = $"Cannot delete Model '{modelToDelete.Name}' (ID: {id}) as it has associations.";
                _logger.LogError(errorMessage + $" Dependent types: {string.Join(", ", dependentEntities)}");
                throw new DeletionBlockedException(
                    errorMessage,
                    nameof(Model),
                    modelToDelete.Id.ToString(),
                    dependentEntities
                );
            }


            try
            {
                _unitOfWork.Models.Delete(modelToDelete);
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(modelToDelete.ImagePath))
                {
                    await _imageFileService.DeleteImageAsync(modelToDelete.ImagePath);
                }
                _logger.LogInformation("Model with ID {ModelId} and Name {ModelName} deleted successfully.", id, modelToDelete.Name);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting model's image with Path {modelToDelete.ImagePath}", modelToDelete.ImagePath);
                throw new ServiceException($"An error occurred while deleting model's image with Path {modelToDelete.ImagePath}.", ex);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting model ID {ModelId}.", id);
                throw new ServiceException("A database error occurred while deleting the model. It might still be in use.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting model ID {ModelId}", id);
                throw new ServiceException($"An error occurred while deleting model with ID {id}.", ex);
            }
        }

        #endregion

        #region TransmissionType Management

        public async Task<IEnumerable<TransmissionTypeDto>> GetAllTransmissionTypesAsync()
        {
            _logger.LogInformation("Attempting to retrieve all transmission types.");
            try
            {
                var transmissionTypes = await _unitOfWork.TransmissionTypes.GetAllAsync();
                var dtos = transmissionTypes.Select(tt => new TransmissionTypeDto(tt.Id, tt.Name))
                                            .OrderBy(dto => dto.Name)
                                            .ToList();
                _logger.LogInformation("Successfully retrieved {Count} transmission types.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all transmission types.");
                throw new ServiceException("Could not retrieve transmission types.", ex);
            }
        }

        public async Task<TransmissionTypeDto?> GetTransmissionTypeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get transmission type with invalid ID: {TransmissionTypeId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve transmission type with ID: {TransmissionTypeId}", id);
            try
            {
                var transmissionType = await _unitOfWork.TransmissionTypes.GetByIdAsync(id);
                if (transmissionType == null)
                {
                    _logger.LogWarning("Transmission type with ID {TransmissionTypeId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved transmission type with ID {TransmissionTypeId}: {TransmissionTypeName}", id, transmissionType.Name);
                return new TransmissionTypeDto(transmissionType.Id, transmissionType.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving transmission type with ID {TransmissionTypeId}.", id);
                throw new ServiceException($"Could not retrieve transmission type with ID {id}.", ex);
            }
        }

        public async Task<TransmissionTypeDto> CreateTransmissionTypeAsync(CreateLookupDto createDto) // Using generic DTO
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            _logger.LogInformation("Attempting to create transmission type with Name: {TransmissionTypeName}", createDto.Name);

            // 1. Validate DTO
            var validationResult = await _createLookupValidator.ValidateAsync(createDto); // Use generic validator
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateLookupDto (TransmissionType): {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Check for uniqueness
            if (await _unitOfWork.TransmissionTypes.NameExistsAsync(createDto.Name))
            {
                string duplicateMessage = $"Transmission Type with name '{createDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new ServiceException(duplicateMessage);
            }

            try
            {
                // 3. Map DTO to Domain Entity
                var newTransmissionType = new TransmissionType
                {
                    Name = createDto.Name.Trim()
                };

                // 4. Add to Repository
                await _unitOfWork.TransmissionTypes.AddAsync(newTransmissionType);

                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Transmission Type created successfully with ID {TransmissionTypeId} and Name {TransmissionTypeName}", newTransmissionType.Id, newTransmissionType.Name);

                // 6. Map back to DTO for return
                return new TransmissionTypeDto(newTransmissionType.Id, newTransmissionType.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while creating transmission type: {TransmissionTypeName}.", createDto.Name);
                throw new ServiceException("A database error occurred while creating the transmission type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating transmission type: {TransmissionTypeName}", createDto.Name);
                throw new ServiceException($"An error occurred while creating transmission type '{createDto.Name}'.", ex);
            }
        }

        public async Task UpdateTransmissionTypeAsync(UpdateLookupDto updateDto) // Using generic DTO
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Transmission Type ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update transmission type with ID: {TransmissionTypeId}, New Name: {NewTransmissionTypeName}", updateDto.Id, updateDto.Name);

            // 1. Validate DTO
            var validationResult = await _updateLookupValidator.ValidateAsync(updateDto); // Use generic validator
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateLookupDto (TransmissionType ID: {TransmissionTypeId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingTransmissionType = await _unitOfWork.TransmissionTypes.GetByIdAsync(updateDto.Id);
            if (existingTransmissionType == null)
            {
                string notFoundMsg = $"Transmission Type with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg);
            }

            // 3. Check for uniqueness if name changed
            if (!existingTransmissionType.Name.Equals(updateDto.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                await _unitOfWork.TransmissionTypes.NameExistsAsync(updateDto.Name, updateDto.Id))
            {
                string duplicateMessage = $"Transmission Type with name '{updateDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new ServiceException(duplicateMessage);
            }

            try
            {
                // 4. Map changes
                existingTransmissionType.Name = updateDto.Name.Trim();

                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Transmission Type with ID {TransmissionTypeId} updated successfully to Name: {NewTransmissionTypeName}", updateDto.Id, existingTransmissionType.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while updating transmission type ID {TransmissionTypeId}.", updateDto.Id);
                throw new ServiceException("A database error occurred while updating the transmission type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating transmission type ID {TransmissionTypeId}", updateDto.Id);
                throw new ServiceException($"An error occurred while updating transmission type with ID {updateDto.Id}.", ex);
            }
        }

        public async Task DeleteTransmissionTypeAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Transmission Type ID for deletion.", nameof(id));
            _logger.LogInformation("Attempting to delete transmission type with ID: {TransmissionTypeId}", id);

            var transmissionTypeToDelete = await _unitOfWork.TransmissionTypes.GetByIdAsync(id);
            if (transmissionTypeToDelete == null)
            {
                _logger.LogWarning("Transmission Type with ID {TransmissionTypeId} not found for deletion. No action taken.", id);
                return;
            }

            // Dependency Check: CompatibleVehicles
            bool isUsed = await _unitOfWork.CompatibleVehicles.ExistsAsync(cv => cv.TransmissionTypeId == id);
            if (isUsed)
            {
                string depError = $"Cannot delete Transmission Type '{transmissionTypeToDelete.Name}' (ID: {id}) as it is used in Compatible Vehicle specifications.";
                _logger.LogError(depError);
                throw new DeletionBlockedException(depError, nameof(TransmissionType), transmissionTypeToDelete.Id.ToString(), new List<string> { nameof(CompatibleVehicle) });
            }

            try
            {
                _unitOfWork.TransmissionTypes.Delete(transmissionTypeToDelete);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Transmission Type with ID {TransmissionTypeId} and Name {TransmissionTypeName} deleted successfully.", id, transmissionTypeToDelete.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting transmission type ID {TransmissionTypeId}.", id);
                throw new ServiceException("A database error occurred while deleting the transmission type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting transmission type ID {TransmissionTypeId}", id);
                throw new ServiceException($"An error occurred while deleting transmission type with ID {id}.", ex);
            }
        }

        #endregion

        #region EngineType Management

        public async Task<IEnumerable<EngineTypeDto>> GetAllEngineTypesAsync()
        {
            _logger.LogInformation("Attempting to retrieve all engine types.");
            try
            {
                var engineTypes = await _unitOfWork.EngineTypes.GetAllAsync();
                var dtos = engineTypes.Select(et => new EngineTypeDto(et.Id, et.Name, et.Code))
                                      .OrderBy(dto => dto.Name)
                                      .ToList();
                _logger.LogInformation("Successfully retrieved {Count} engine types.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all engine types.");
                throw new ServiceException("Could not retrieve engine types.", ex);
            }
        }

        public async Task<EngineTypeDto?> GetEngineTypeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get engine type with invalid ID: {EngineTypeId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve engine type with ID: {EngineTypeId}", id);
            try
            {
                var engineType = await _unitOfWork.EngineTypes.GetByIdAsync(id);
                if (engineType == null)
                {
                    _logger.LogWarning("Engine type with ID {EngineTypeId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved engine type with ID {EngineTypeId}: {EngineTypeName}", id, engineType.Name);
                return new EngineTypeDto(engineType.Id, engineType.Name, engineType.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving engine type with ID {EngineTypeId}.", id);
                throw new ServiceException($"Could not retrieve engine type with ID {id}.", ex);
            }
        }

        public async Task<EngineTypeDto> CreateEngineTypeAsync(CreateEngineTypeDto createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            _logger.LogInformation("Attempting to create engine type: Name={EngineTypeName}, Code={EngineTypeCode}", createDto.Name, createDto.Code);

            // 1. Validate DTO (includes uniqueness checks for Name and Code)
            var validationResult = await _createEngineTypeValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateEngineTypeDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }
            // Uniqueness checks are now handled by the validator using repository methods.

            try
            {
                // 2. Map DTO to Domain Entity
                var newEngineType = new EngineType
                {
                    Name = createDto.Name.Trim(),
                    Code = string.IsNullOrWhiteSpace(createDto.Code) ? null : createDto.Code.Trim().ToUpperInvariant() // Normalize code
                };

                // 3. Add to Repository
                await _unitOfWork.EngineTypes.AddAsync(newEngineType);

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Engine Type created successfully with ID {EngineTypeId}, Name {EngineTypeName}, Code {EngineTypeCode}",
                    newEngineType.Id, newEngineType.Name, newEngineType.Code);

                // 5. Map back to DTO for return
                return new EngineTypeDto(newEngineType.Id, newEngineType.Name, newEngineType.Code);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while creating engine type: {EngineTypeName}.", createDto.Name);
                throw new ServiceException("A database error occurred while creating the engine type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating engine type: {EngineTypeName}", createDto.Name);
                throw new ServiceException($"An error occurred while creating engine type '{createDto.Name}'.", ex);
            }
        }

        public async Task UpdateEngineTypeAsync(UpdateEngineTypeDto updateDto)
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Engine Type ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update engine type with ID: {EngineTypeId}, New Name: {NewName}, New Code: {NewCode}",
                updateDto.Id, updateDto.Name, updateDto.Code);

            // 1. Validate DTO (includes uniqueness checks excluding self)
            var validationResult = await _updateEngineTypeValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateEngineTypeDto (ID: {EngineTypeId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingEngineType = await _unitOfWork.EngineTypes.GetByIdAsync(updateDto.Id);
            if (existingEngineType == null)
            {
                string notFoundMsg = $"Engine Type with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg);
            }
            // Uniqueness checks are now handled by the validator.

            try
            {
                // 3. Map changes
                existingEngineType.Name = updateDto.Name.Trim();
                existingEngineType.Code = string.IsNullOrWhiteSpace(updateDto.Code) ? null : updateDto.Code.Trim().ToUpperInvariant();

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Engine Type with ID {EngineTypeId} updated successfully.", updateDto.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while updating engine type ID {EngineTypeId}.", updateDto.Id);
                throw new ServiceException("A database error occurred while updating the engine type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating engine type ID {EngineTypeId}", updateDto.Id);
                throw new ServiceException($"An error occurred while updating engine type with ID {updateDto.Id}.", ex);
            }
        }

        public async Task DeleteEngineTypeAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Engine Type ID for deletion.", nameof(id));
            _logger.LogInformation("Attempting to delete engine type with ID: {EngineTypeId}", id);

            var engineTypeToDelete = await _unitOfWork.EngineTypes.GetByIdAsync(id);
            if (engineTypeToDelete == null)
            {
                _logger.LogWarning("Engine Type with ID {EngineTypeId} not found for deletion. No action taken.", id);
                return;
            }

            // Dependency Check: CompatibleVehicles
            bool isUsed = await _unitOfWork.CompatibleVehicles.ExistsAsync(cv => cv.EngineTypeId == id);
            if (isUsed)
            {
                string depError = $"Cannot delete Engine Type '{engineTypeToDelete.Name}' (ID: {id}) as it is used in Compatible Vehicle specifications.";
                _logger.LogError(depError);
                throw new DeletionBlockedException(depError, nameof(EngineType), engineTypeToDelete.Id.ToString(), new List<string> { nameof(CompatibleVehicle) });
            }

            try
            {
                _unitOfWork.EngineTypes.Delete(engineTypeToDelete);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Engine Type with ID {EngineTypeId} and Name {EngineTypeName} deleted successfully.", id, engineTypeToDelete.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting engine type ID {EngineTypeId}.", id);
                throw new ServiceException("A database error occurred while deleting the engine type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting engine type ID {EngineTypeId}", id);
                throw new ServiceException($"An error occurred while deleting engine type with ID {id}.", ex);
            }
        }

        #endregion

        #region TrimLevel Management

        public async Task<IEnumerable<TrimLevelDto>> GetTrimLevelsByModelIdAsync(int modelId)
        {
            if (modelId <= 0)
            {
                _logger.LogWarning("Attempted to get trim levels with invalid Model ID: {ModelId}", modelId);
                return Enumerable.Empty<TrimLevelDto>();
            }
            _logger.LogInformation("Attempting to retrieve trim levels for Model ID: {ModelId}", modelId);
            try
            {
                var trimLevels = await _unitOfWork.TrimLevels.GetByModelIdAsync(modelId);
                var dtos = trimLevels.Select(MapTrimLevelToDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} trim levels for Model ID {ModelId}.", dtos.Count, modelId);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving trim levels for Model ID {ModelId}.", modelId);
                throw new ServiceException($"Could not retrieve trim levels for Model ID {modelId}.", ex);
            }
        }

        public async Task<TrimLevelDto?> GetTrimLevelByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get trim level with invalid ID: {TrimLevelId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve trim level with ID: {TrimLevelId}", id);
            try
            {
                var trimLevel = await _unitOfWork.TrimLevels.GetByIdWithModelAsync(id); // Get with Model for context
                if (trimLevel == null)
                {
                    _logger.LogWarning("Trim level with ID {TrimLevelId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved trim level with ID {TrimLevelId}: {TrimLevelName}", id, trimLevel.Name);
                return MapTrimLevelToDto(trimLevel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving trim level with ID {TrimLevelId}.", id);
                throw new ServiceException($"Could not retrieve trim level with ID {id}.", ex);
            }
        }

        public async Task<TrimLevelDto> CreateTrimLevelAsync(CreateTrimLevelDto createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            _logger.LogInformation("Attempting to create trim level: Name={TrimLevelName}, ModelId={ModelId}", createDto.Name, createDto.ModelId);

            // 1. Validate DTO (includes ModelId existence and Name uniqueness for Model)
            var validationResult = await _createTrimLevelValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateTrimLevelDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Check for uniqueness (could also be part of FluentValidator if NameExistsAsync is injected)
            if (await _unitOfWork.TrimLevels.NameExistsForModelAsync(createDto.Name, createDto.ModelId, null))
            {
                string duplicateMessage = $"Trim with name '{createDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new DuplicationException(duplicateMessage, nameof(TrimLevel), "Name", createDto.Name);
            }

            try
            {
                // 2. Map DTO to Domain Entity
                var newTrimLevel = new TrimLevel
                {
                    Name = createDto.Name.Trim(),
                    ModelId = createDto.ModelId
                };

                // 3. Add to Repository
                await _unitOfWork.TrimLevels.AddAsync(newTrimLevel);

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Trim Level created successfully with ID {TrimLevelId}, Name {TrimLevelName}, for ModelId {ModelId}",
                    newTrimLevel.Id, newTrimLevel.Name, newTrimLevel.ModelId);

                // 5. Re-fetch with Model for complete DTO
                var createdTrimLevelWithModel = await _unitOfWork.TrimLevels.GetByIdWithModelAsync(newTrimLevel.Id);
                if (createdTrimLevelWithModel == null)
                {
                    _logger.LogError("Failed to re-fetch trim level with ID {TrimLevelId} after creation.", newTrimLevel.Id);
                    throw new ServiceException($"Failed to retrieve details for newly created trim level '{newTrimLevel.Name}'.");
                }
                return MapTrimLevelToDto(createdTrimLevelWithModel);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while creating trim level: Name={TrimLevelName}, ModelId={ModelId}.",
                    createDto.Name, createDto.ModelId);
                throw new ServiceException("A database error occurred while creating the trim level.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating trim level: Name={TrimLevelName}, ModelId={ModelId}",
                    createDto.Name, createDto.ModelId);
                throw new ServiceException($"An error occurred while creating trim level '{createDto.Name}'.", ex);
            }
        }

        public async Task UpdateTrimLevelAsync(UpdateTrimLevelDto updateDto)
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Trim Level ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update trim level with ID: {TrimLevelId}, New Name: {NewName}, New ModelId: {NewModelId}",
                updateDto.Id, updateDto.Name, updateDto.ModelId);

            // 1. Validate DTO (includes ModelId existence and Name uniqueness for Model excluding self)
            var validationResult = await _updateTrimLevelValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateTrimLevelDto (ID: {TrimLevelId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingTrimLevel = await _unitOfWork.TrimLevels.GetByIdAsync(updateDto.Id);
            if (existingTrimLevel == null)
            {
                string notFoundMsg = $"Trim Level with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg);
            }

            // 2. Check for uniqueness (could also be part of FluentValidator if NameExistsAsync is injected)
            if (await _unitOfWork.TrimLevels.NameExistsForModelAsync(updateDto.Name, updateDto.ModelId, null))
            {
                string duplicateMessage = $"Trim with name '{updateDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new DuplicationException(duplicateMessage, nameof(TrimLevel), "Name", updateDto.Name);
            }

            try
            {
                // 3. Map changes
                existingTrimLevel.Name = updateDto.Name.Trim();
                existingTrimLevel.ModelId = updateDto.ModelId;

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Trim Level with ID {TrimLevelId} updated successfully.", updateDto.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while updating trim level ID {TrimLevelId}.", updateDto.Id);
                throw new ServiceException("A database error occurred while updating the trim level.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating trim level ID {TrimLevelId}", updateDto.Id);
                throw new ServiceException($"An error occurred while updating trim level with ID {updateDto.Id}.", ex);
            }
        }

        public async Task DeleteTrimLevelAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Trim Level ID for deletion.", nameof(id));
            _logger.LogInformation("Attempting to delete trim level with ID: {TrimLevelId}", id);

            var trimLevelToDelete = await _unitOfWork.TrimLevels.GetByIdAsync(id);
            if (trimLevelToDelete == null)
            {
                _logger.LogWarning("Trim Level with ID {TrimLevelId} not found for deletion. No action taken.", id);
                return;
            }

            // Dependency Check: CompatibleVehicles
            bool isUsed = await _unitOfWork.TrimLevels.HasAssociatedCompatibleVehiclesAsync(id);
            if (isUsed)
            {
                string depError = $"Cannot delete Trim Level '{trimLevelToDelete.Name}' (ID: {id}) as it is used in Compatible Vehicle specifications.";
                _logger.LogError(depError);
                throw new DeletionBlockedException(depError, nameof(TrimLevel), trimLevelToDelete.Id.ToString(), new List<string> { nameof(CompatibleVehicle) });
            }

            try
            {
                _unitOfWork.TrimLevels.Delete(trimLevelToDelete);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Trim Level with ID {TrimLevelId} and Name {TrimLevelName} deleted successfully.", id, trimLevelToDelete.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting trim level ID {TrimLevelId}.", id);
                throw new ServiceException("A database error occurred while deleting the trim level.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting trim level ID {TrimLevelId}", id);
                throw new ServiceException($"An error occurred while deleting trim level with ID {id}.", ex);
            }
        }

        #endregion


        #region BodyType Management

        public async Task<IEnumerable<BodyTypeDto>> GetAllBodyTypesAsync()
        {
            _logger.LogInformation("Attempting to retrieve all body types.");
            try
            {
                var bodyTypes = await _unitOfWork.BodyTypes.GetAllAsync();
                var dtos = bodyTypes.Select(bt => new BodyTypeDto(bt.Id, bt.Name))
                                    .OrderBy(dto => dto.Name)
                                    .ToList();
                _logger.LogInformation("Successfully retrieved {Count} body types.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all body types.");
                throw new ServiceException("Could not retrieve body types.", ex);
            }
        }

        public async Task<BodyTypeDto?> GetBodyTypeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get body type with invalid ID: {BodyTypeId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve body type with ID: {BodyTypeId}", id);
            try
            {
                var bodyType = await _unitOfWork.BodyTypes.GetByIdAsync(id);
                if (bodyType == null)
                {
                    _logger.LogWarning("Body type with ID {BodyTypeId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Successfully retrieved body type with ID {BodyTypeId}: {BodyTypeName}", id, bodyType.Name);
                return new BodyTypeDto(bodyType.Id, bodyType.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving body type with ID {BodyTypeId}.", id);
                throw new ServiceException($"Could not retrieve body type with ID {id}.", ex);
            }
        }

        public async Task<BodyTypeDto> CreateBodyTypeAsync(CreateLookupDto createDto) // Using generic DTO
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            _logger.LogInformation("Attempting to create body type with Name: {BodyTypeName}", createDto.Name);

            // 1. Validate DTO
            var validationResult = await _createLookupValidator.ValidateAsync(createDto); // Use generic validator
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateLookupDto (BodyType): {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Check for uniqueness
            if (await _unitOfWork.BodyTypes.NameExistsAsync(createDto.Name))
            {
                string duplicateMessage = $"Body Type with name '{createDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new ServiceException(duplicateMessage);
            }

            try
            {
                // 3. Map DTO to Domain Entity
                var newBodyType = new BodyType
                {
                    Name = createDto.Name.Trim()
                };

                // 4. Add to Repository
                await _unitOfWork.BodyTypes.AddAsync(newBodyType);

                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Body Type created successfully with ID {BodyTypeId} and Name {BodyTypeName}", newBodyType.Id, newBodyType.Name);

                // 6. Map back to DTO for return
                return new BodyTypeDto(newBodyType.Id, newBodyType.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while creating body type: {BodyTypeName}.", createDto.Name);
                throw new ServiceException("A database error occurred while creating the body type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating body type: {BodyTypeName}", createDto.Name);
                throw new ServiceException($"An error occurred while creating body type '{createDto.Name}'.", ex);
            }
        }

        public async Task UpdateBodyTypeAsync(UpdateLookupDto updateDto) // Using generic DTO
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Body Type ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update body type with ID: {BodyTypeId}, New Name: {NewBodyTypeName}", updateDto.Id, updateDto.Name);

            // 1. Validate DTO
            var validationResult = await _updateLookupValidator.ValidateAsync(updateDto); // Use generic validator
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateLookupDto (BodyType ID: {BodyTypeId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingBodyType = await _unitOfWork.BodyTypes.GetByIdAsync(updateDto.Id);
            if (existingBodyType == null)
            {
                string notFoundMsg = $"Body Type with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg);
            }

            // 3. Check for uniqueness if name changed
            if (!existingBodyType.Name.Equals(updateDto.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                await _unitOfWork.BodyTypes.NameExistsAsync(updateDto.Name, updateDto.Id))
            {
                string duplicateMessage = $"Body Type with name '{updateDto.Name}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new ServiceException(duplicateMessage);
            }

            try
            {
                // 4. Map changes
                existingBodyType.Name = updateDto.Name.Trim();

                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Body Type with ID {BodyTypeId} updated successfully to Name: {NewBodyTypeName}", updateDto.Id, existingBodyType.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while updating body type ID {BodyTypeId}.", updateDto.Id);
                throw new ServiceException("A database error occurred while updating the body type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating body type ID {BodyTypeId}", updateDto.Id);
                throw new ServiceException($"An error occurred while updating body type with ID {updateDto.Id}.", ex);
            }
        }

        public async Task DeleteBodyTypeAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Body Type ID for deletion.", nameof(id));
            _logger.LogInformation("Attempting to delete body type with ID: {BodyTypeId}", id);

            var bodyTypeToDelete = await _unitOfWork.BodyTypes.GetByIdAsync(id);
            if (bodyTypeToDelete == null)
            {
                _logger.LogWarning("Body Type with ID {BodyTypeId} not found for deletion. No action taken.", id);
                return;
            }

            // Dependency Check: CompatibleVehicles
            // Assumes ICompatibleVehicleRepository is on IUnitOfWork
            bool isUsed = await _unitOfWork.CompatibleVehicles.ExistsAsync(cv => cv.BodyTypeId == id);
            if (isUsed)
            {
                string depError = $"Cannot delete Body Type '{bodyTypeToDelete.Name}' (ID: {id}) as it is used in Compatible Vehicle specifications.";
                _logger.LogError(depError);
                throw new DeletionBlockedException(depError, nameof(BodyType), bodyTypeToDelete.Id.ToString(), new List<string> { nameof(CompatibleVehicle) });
            }

            try
            {
                _unitOfWork.BodyTypes.Delete(bodyTypeToDelete);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Body Type with ID {BodyTypeId} and Name {BodyTypeName} deleted successfully.", id, bodyTypeToDelete.Name);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting body type ID {BodyTypeId}.", id);
                throw new ServiceException("A database error occurred while deleting the body type.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting body type ID {BodyTypeId}", id);
                throw new ServiceException($"An error occurred while deleting body type with ID {id}.", ex);
            }
        }

        #endregion

        #region Helpers

        private Expression<Func<CompatibleVehicle, bool>> BuildCompatibleVehicleFilterPredicate(CompatibleVehicleFilterCriteriaDto criteria)
        {
            var predicate = LinqKit.PredicateBuilder.New<CompatibleVehicle>(true);

            if (criteria.MakeId.HasValue)
            {
                predicate = predicate.And(cv => cv.Model.MakeId == criteria.MakeId.Value);
            }
            if (criteria.ModelId.HasValue)
            {
                predicate = predicate.And(cv => cv.ModelId == criteria.ModelId.Value);
            }
            if (criteria.ExactYear.HasValue)
            {
                int year = criteria.ExactYear.Value;
                predicate = predicate.And(cv => cv.YearStart <= year && cv.YearEnd >= year);
            }
            if (criteria.TrimLevelId.HasValue)
            {
                predicate = predicate.And(cv => cv.TrimLevelId == criteria.TrimLevelId.Value);
            }
            if (criteria.TransmissionTypeId.HasValue)
            {
                predicate = predicate.And(cv => cv.TransmissionTypeId == criteria.TransmissionTypeId.Value);
            }
            if (criteria.EngineTypeId.HasValue)
            {
                predicate = predicate.And(cv => cv.EngineTypeId == criteria.EngineTypeId.Value);
            }
            if (criteria.BodyTypeId.HasValue)
            {
                predicate = predicate.And(cv => cv.BodyTypeId == criteria.BodyTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                string term = criteria.SearchTerm.ToLower().Trim();
                predicate = predicate.And(cv =>
                    (cv.Model.Make.Name.ToLower().Contains(term)) ||
                    (cv.Model.Name.ToLower().Contains(term)) ||
                    (cv.VIN != null && cv.VIN.ToLower().Contains(term)) ||
                    (cv.YearStart.ToString().Contains(term)) ||
                    (cv.YearEnd.ToString().Contains(term))
                );
            }
            return predicate;
        }

        private CompatibleVehicle MapCreateDtoToCompatibleVehicle(CreateCompatibleVehicleDto dto)
        {
            return new CompatibleVehicle
            {
                ModelId = dto.ModelId,
                YearStart = dto.YearStart,
                YearEnd = dto.YearEnd,
                TrimLevelId = dto.TrimLevelId,
                TransmissionTypeId = dto.TransmissionTypeId,
                EngineTypeId = dto.EngineTypeId,
                BodyTypeId = dto.BodyTypeId,
                VIN = string.IsNullOrWhiteSpace(dto.VIN) ? null : dto.VIN.Trim().ToUpperInvariant()
            };
        }

        private void MapUpdateDtoToCompatibleVehicle(UpdateCompatibleVehicleDto dto, CompatibleVehicle entity)
        {
            entity.ModelId = dto.ModelId;
            entity.YearStart = dto.YearStart;
            entity.YearEnd = dto.YearEnd;
            entity.TrimLevelId = dto.TrimLevelId;
            entity.TransmissionTypeId = dto.TransmissionTypeId;
            entity.EngineTypeId = dto.EngineTypeId;
            entity.BodyTypeId = dto.BodyTypeId;
            entity.VIN = string.IsNullOrWhiteSpace(dto.VIN) ? null : dto.VIN.Trim().ToUpperInvariant();
            // ModifiedAt will be handled by DbContext
        }

        private CompatibleVehicleDetailDto MapCompatibleVehicleToDetailDto(CompatibleVehicle entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new CompatibleVehicleDetailDto(
                entity.Id,
                entity.ModelId,
                entity.Model?.Make?.Name ?? "N/A", // Eager load Model.Make
                entity.Model?.Name ?? "N/A",       // Eager load Model
                entity.YearStart,
                entity.YearEnd,
                entity.TrimLevelId,
                entity.TrimLevel?.Name,       // Eager load TrimLevel
                entity.TransmissionTypeId,
                entity.TransmissionType?.Name, // Eager load TransmissionType
                entity.EngineTypeId,
                entity.EngineType?.Name,       // Eager load EngineType
                entity.BodyTypeId,
                entity.BodyType?.Name,         // Eager load BodyType
                entity.VIN
            );
        }

        private CompatibleVehicleSummaryDto MapCompatibleVehicleToSummaryDto(CompatibleVehicle entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new CompatibleVehicleSummaryDto(
                entity.Id,
                entity.Model?.Make?.Name ?? "N/A",
                entity.Model?.Name ?? "N/A",
                entity.YearStart,
                entity.YearEnd,
                entity.TrimLevel?.Name,
                entity.TransmissionType?.Name,
                entity.EngineType?.Name,
                entity.BodyType?.Name,
                entity.VIN
            );
        }


        private ModelDto MapModelToDto(Model model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            return new ModelDto(
                model.Id,
                model.Name,
                model.MakeId,
                model.Make?.Name ?? "N/A",
                model.ImagePath
         
            );
        }

        private TrimLevelDto MapTrimLevelToDto(TrimLevel trimLevel)
        {
            if (trimLevel == null) throw new ArgumentNullException(nameof(trimLevel));
            return new TrimLevelDto(
                trimLevel.Id,
                trimLevel.Name,
                trimLevel.ModelId,
                trimLevel.Model?.Name ?? "N/A" // Safely access Model.Name
            );
        }


        #endregion

    }
}
