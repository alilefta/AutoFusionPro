using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Storage;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class CategoryService : ICategoryService
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;
        private readonly IImageFileService _imageFileService;

        private readonly IValidator<CreateCategoryDto> _createCategoryValidator;
        private readonly IValidator<UpdateCategoryDto> _updateCategoryValidator;
        #endregion

        public CategoryService(
            IUnitOfWork unitOfWork, 
            IImageFileService imageFileService, 
            ILogger<CategoryService> logger,
            IValidator<CreateCategoryDto> createCategoryValidator, 
            IValidator<UpdateCategoryDto> updateCategoryValidator
            )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _imageFileService = imageFileService ?? throw new ArgumentNullException(nameof(imageFileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _createCategoryValidator = createCategoryValidator ?? throw new ArgumentNullException(nameof(createCategoryValidator));
            _updateCategoryValidator = updateCategoryValidator ?? throw new ArgumentNullException(nameof(updateCategoryValidator));
        }




        #region Get Methods

        /// <summary>
        /// Gets a single category by its ID, including details suitable for display or editing.
        /// </summary>
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get category with invalid ID: {CategoryId}", id);
                return null;
            }
            _logger.LogInformation("Attempting to retrieve category with ID: {CategoryId}", id);
            try
            {
                // Use the repository method that fetches necessary details
                var categoryEntity = await _unitOfWork.Categories.GetByIdWithChildrenAndParentAsync(id);

                if (categoryEntity == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved category with ID {CategoryId}: {CategoryName}", id, categoryEntity.Name);
                return MapCategoryToDto(categoryEntity, includeChildren: true); // Map with its immediate children
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with ID {CategoryId}.", id);
                throw new ServiceException($"Could not retrieve category with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Gets the immediate subcategories for a given parent category ID.
        /// </summary>
        public async Task<IEnumerable<CategoryDto>> GetSubcategoriesAsync(int parentCategoryId, bool onlyActive = true)
        {
            // Note: parentCategoryId can be 0 or invalid if the UI sends that,
            // but GetSubcategoriesAsync in repo handles <= 0.
            // If parentCategoryId represents a "top-level" pseudo-ID (like 0),
            // this method should probably call GetTopLevelCategoriesAsync instead.
            // For now, assuming parentCategoryId is a valid ID of an existing parent.
            if (parentCategoryId <= 0)
            {
                _logger.LogWarning("Attempted to get subcategories with invalid ParentCategoryID: {ParentCategoryId}", parentCategoryId);
                return Enumerable.Empty<CategoryDto>();
            }

            _logger.LogInformation("Attempting to retrieve subcategories for ParentCategoryID: {ParentCategoryId}, OnlyActive: {OnlyActive}",
                parentCategoryId, onlyActive);
            try
            {
                IEnumerable<Category> subcategoryEntities;
                if (onlyActive)
                {
                    // If repository's GetSubcategoriesAsync doesn't filter by IsActive, do it here
                    // Or add a version to the repo: GetActiveSubcategoriesAsync
                    var allSubcategories = await _unitOfWork.Categories.GetSubcategoriesAsync(parentCategoryId);
                    subcategoryEntities = allSubcategories.Where(sc => sc.IsActive);
                }
                else
                {
                    subcategoryEntities = await _unitOfWork.Categories.GetSubcategoriesAsync(parentCategoryId);
                }

                var dtos = subcategoryEntities.Select(sc => MapCategoryToDto(sc, includeChildren: false)).ToList(); // Don't load grandchildren by default here
                _logger.LogInformation("Successfully retrieved {Count} subcategories for ParentCategoryID {ParentCategoryId}.", dtos.Count, parentCategoryId);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving subcategories for ParentCategoryID {ParentCategoryId}.", parentCategoryId);
                throw new ServiceException($"Could not retrieve subcategories for parent ID {parentCategoryId}.", ex);
            }
        }

        /// <summary>
        /// Gets all top-level categories (those without a parent).
        /// </summary>
        public async Task<IEnumerable<CategoryDto>> GetTopLevelCategoriesAsync(bool onlyActive = true)
        {
            _logger.LogInformation("Attempting to retrieve top-level categories. OnlyActive: {OnlyActive}", onlyActive);
            try
            {
                // The repository method should handle fetching entities where ParentCategoryId is null.
                // We pass 'includeChildren: true' to repository if we want MapCategoryToDto to know about HasChildren
                // or even map the first level of children.
                var topLevelEntities = await _unitOfWork.Categories.GetTopLevelCategoriesAsync(includeChildren: true);

                IEnumerable<Category> filteredEntities = topLevelEntities;
                if (onlyActive)
                {
                    filteredEntities = topLevelEntities.Where(c => c.IsActive);
                }

                var dtos = filteredEntities
                    .Select(c => MapCategoryToDto(c, includeChildren: true, onlyActiveFilter: onlyActive, maxDepth: 1)) // Load only immediate children for a top-level list
                    .OrderBy(d => d.Name)
                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} top-level categories.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving top-level categories.");
                throw new ServiceException("Could not retrieve top-level categories.", ex);
            }
        }

        /// <summary>
        /// Gets a flat list of all categories suitable for selection (e.g., in a ComboBox).
        /// Names might be formatted to indicate hierarchy.
        /// </summary>
        public async Task<IEnumerable<CategorySelectionDto>> GetAllCategoriesForSelectionAsync(bool onlyActive = true)
        {
            _logger.LogInformation("Attempting to retrieve all categories for selection. OnlyActive: {OnlyActive}", onlyActive);
            try
            {
                // Fetch all categories. We need parent information to build hierarchical names.
                // GetAllWithFullHierarchyAsync from the repository is suitable here as it loads ParentCategory.
                var allCategoryEntities = await _unitOfWork.Categories.GetAllWithFullHierarchyAsync();

                IEnumerable<Category> categoriesToProcess = allCategoryEntities;
                if (onlyActive)
                {
                    // Filter out inactive categories. If a parent is inactive, its children
                    // (even if active) might not be reachable in a strict hierarchical display,
                    // but for a flat selection list, we might still want to show active children.
                    // This simple filter includes all active categories.
                    categoriesToProcess = allCategoryEntities.Where(c => c.IsActive);
                }

                var selectionDtos = new List<CategorySelectionDto>();
                var sortedCategories = categoriesToProcess.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.Name);

                // To build hierarchical names, we can iterate and prepend parent names.
                // This is a simple approach. For very deep hierarchies, it could be optimized.
                // We'll use a helper to get the full hierarchical name.
                foreach (var categoryEntity in sortedCategories)
                {
                    selectionDtos.Add(new CategorySelectionDto(
                        categoryEntity.Id,
                        GetHierarchicalCategoryName(categoryEntity, allCategoryEntities), // Helper to build name
                        categoryEntity.ParentCategoryId
                    ));
                }

                _logger.LogInformation("Successfully retrieved {Count} categories for selection.", selectionDtos.Count);
                // The list is already somewhat ordered by hierarchy due to the initial sort,
                // but for a flat list, a final sort by the full hierarchical name is best for display.
                return selectionDtos.OrderBy(dto => dto.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories for selection.");
                throw new ServiceException("Could not retrieve categories for selection.", ex);
            }
        }

        /// <summary>
        /// Gets all categories, structured hierarchically.
        /// Primarily for administrative UIs or full tree displays.
        /// </summary>
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesHierarchicalAsync(bool onlyActive = true)
        {
            _logger.LogInformation("Attempting to retrieve all categories hierarchically. OnlyActive: {OnlyActive}", onlyActive);
            try
            {
                // 1. Fetch all categories with their immediate parent/child relationships pre-loaded.
                // The repository's GetAllWithFullHierarchyAsync should handle this.
                var allCategoryEntities = await _unitOfWork.Categories.GetAllWithFullHierarchyAsync();

                IEnumerable<Category> filteredEntities = allCategoryEntities;
                if (onlyActive)
                {
                    // We need a more sophisticated filter here if we only want branches
                    // where the parents are also active.
                    // For now, let's filter individual categories, and the mapping will handle children.
                    // A more robust solution might involve filtering top-level active categories
                    // and then recursively building the tree only with active children.
                    // This simple filter ensures individual DTOs reflect the active status.
                    // The MapCategoryToDto will then only include active children if onlyActive is true.
                }

                // 2. Identify top-level categories (those without a parent or whose parent is not in the filtered list)
                var topLevelEntities = filteredEntities.Where(c => c.ParentCategoryId == null).ToList();

                // If onlyActive, we might also need to ensure that even if a category's ParentCategoryId is null,
                // it itself is active to be considered a top-level entry point.
                if (onlyActive)
                {
                    topLevelEntities = topLevelEntities.Where(c => c.IsActive).ToList();
                }


                // 3. Map to DTOs, letting the MapCategoryToDto helper handle recursion for children.
                var resultDtoTree = new List<CategoryDto>();
                foreach (var topLevelEntity in topLevelEntities.OrderBy(c => c.Name))
                {
                    // The 'onlyActive' flag will be passed down through the mapping
                    resultDtoTree.Add(MapCategoryToDto(topLevelEntity, includeChildren: true, onlyActiveFilter: onlyActive));
                }

                _logger.LogInformation("Successfully retrieved and structured {Count} top-level hierarchical categories.", resultDtoTree.Count);
                return resultDtoTree;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all categories hierarchically.");
                throw new ServiceException("Could not retrieve hierarchical categories.", ex);
            }
        }

        #endregion

        #region Check

        /// <summary>
        /// Checks if a category name is unique, optionally within a specific parent category.
        /// </summary>
        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? parentCategoryId, int? excludeCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("IsCategoryNameUniqueAsync called with empty name.");
                // Depending on business rule, an empty name might be considered "not unique" if it's invalid,
                // or "unique" if the validation for emptiness is handled elsewhere.
                // Let's assume it's not unique if it's invalid.
                return false;
            }

            _logger.LogInformation("Checking category name uniqueness: Name='{Name}', ParentId='{ParentId}', ExcludeId='{ExcludeId}'",
                name, parentCategoryId, excludeCategoryId);
            try
            {
                // Directly use the repository method
                bool exists = await _unitOfWork.Categories.NameExistsAsync(name, parentCategoryId, excludeCategoryId);
                return !exists; // Return true if unique (i.e., does not exist)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking category name uniqueness for Name '{Name}', ParentId '{ParentId}'.", name, parentCategoryId);
                // In case of error, conservatively assume it's not unique to prevent duplicates, or re-throw.
                throw new ServiceException($"Could not verify uniqueness for category name '{name}'.", ex);
            }
        }

        #endregion

        #region Create / Update

        /// <summary>
        /// Updates an existing category's details.
        /// </summary>
        public async Task UpdateCategoryAsync(UpdateCategoryDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto, nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Category ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update category: ID={CategoryId}, Name='{CategoryName}', ParentId='{ParentId}'",
                updateDto.Id, updateDto.Name, updateDto.ParentCategoryId);

            // 1. Validate DTO using FluentValidation
            // (This validator should check ParentCategoryId existence, circular refs, and name uniqueness excluding self)
            var validationResult = await _updateCategoryValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateCategoryDto (ID: {CategoryId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(updateDto.Id);
            if (existingCategory == null)
            {
                string notFoundMsg = $"Category with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
            }

            // Uniqueness and circular reference checks for ParentCategoryId are now primarily
            // handled by the UpdateCategoryDtoValidator using repository methods.

            string? oldPersistentImagePath = existingCategory.ImagePath;
            string? newPersistentImagePath = existingCategory.ImagePath; // Assume no change initially
            bool imageFileSavedForThisOperation = false;

            try
            {
                // 3. Handle Image Update/Removal
                // updateDto.ImagePath is the new source path from UI, or existing path if unchanged, or null/empty to clear.
                if (updateDto.ImagePath != oldPersistentImagePath) // An intent to change image state
                {
                    if (!string.IsNullOrWhiteSpace(updateDto.ImagePath))
                    {
                        // User provided a new source image path
                        _logger.LogInformation("New image source path provided: '{SourcePath}'. Saving new image for Category ID {CategoryId}.", updateDto.ImagePath, updateDto.Id);
                        newPersistentImagePath = await _imageFileService.SaveImageAsync(updateDto.ImagePath, "Categories");
                        imageFileSavedForThisOperation = true; // Mark that a new file was physically saved
                        _logger.LogInformation("New image saved. Persistent path: '{PersistentPath}' for Category ID {CategoryId}.", newPersistentImagePath, updateDto.Id);
                    }
                    else // updateDto.ImagePath is null or whitespace, meaning user wants to remove the image
                    {
                        _logger.LogInformation("Request to remove image for Category ID {CategoryId}. Old path was: '{OldPath}'", updateDto.Id, oldPersistentImagePath ?? "<none>");
                        newPersistentImagePath = null;
                    }
                }

                // 4. Map DTO changes to existing Domain Entity
                existingCategory.Name = updateDto.Name.Trim();
                existingCategory.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
                existingCategory.ImagePath = newPersistentImagePath; // Assign the new persistent path or null
                existingCategory.ParentCategoryId = updateDto.ParentCategoryId == 0 ? null : updateDto.ParentCategoryId;
                existingCategory.IsActive = updateDto.IsActive;
                // ModifiedAt will be handled by DbContext SaveChanges override or BaseEntity logic

                // 5. Save Changes to Database
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Category with ID {CategoryId} database record updated successfully. ImagePath is now: '{FinalImagePath}'",
                    updateDto.Id, existingCategory.ImagePath ?? "<null>");

                // 6. Clean Up Old Image File (if applicable and different from new)
                if (!string.IsNullOrWhiteSpace(oldPersistentImagePath) && oldPersistentImagePath != newPersistentImagePath)
                {
                    _logger.LogInformation("Attempting to delete old image '{OldImagePath}' for Category ID {CategoryId}.", oldPersistentImagePath, updateDto.Id);
                    await _imageFileService.DeleteImageAsync(oldPersistentImagePath);
                    _logger.LogInformation("Old image '{OldImagePath}' for Category ID {CategoryId} deleted successfully.", oldPersistentImagePath, updateDto.Id);
                }

                _logger.LogInformation("Category with ID {CategoryId} update process completed successfully.", updateDto.Id);
            }
            catch (ValidationException valEx) // Re-throw from validator
            {
                _logger.LogWarning(valEx, "Validation exception during category update for ID: {CategoryId}.", updateDto.Id);
                // If a new image was saved before validation failed (less likely if validator is comprehensive), cleanup.
                if (imageFileSavedForThisOperation && newPersistentImagePath != oldPersistentImagePath)
                {
                    await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"category update (validation error, ID: {updateDto.Id})");
                }
                throw;
            }
            catch (DuplicationException dupEx) // If specific checks are still in service
            {
                _logger.LogWarning(dupEx, "Duplication exception during category update for ID: {CategoryId}.", updateDto.Id);
                if (imageFileSavedForThisOperation && newPersistentImagePath != oldPersistentImagePath)
                {
                    await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"category update (duplication, ID: {updateDto.Id})");
                }
                throw;
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error during image management for Category ID {CategoryId}.", updateDto.Id);
                // If DB saved but file ops failed (like deleting old image), the primary update is done.
                // If saving new image failed, imageFileSavedForThisOperation is false.
                throw new ServiceException("An error occurred managing the image file. Category details might have been saved, but the image could not be fully processed.", ioEx);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while updating category ID {CategoryId}.", updateDto.Id);
                // If a new image was physically saved and DB update failed, attempt to roll back image save.
                if (imageFileSavedForThisOperation && newPersistentImagePath != oldPersistentImagePath) // Check if a new file was actually involved
                {
                    await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"category update (DB error, ID: {updateDto.Id})");
                }
                throw new ServiceException("A database error occurred while updating the category.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating category ID {CategoryId}", updateDto.Id);
                if (imageFileSavedForThisOperation && newPersistentImagePath != oldPersistentImagePath)
                {
                    await AttemptImageCleanupOnErrorAsync(newPersistentImagePath, $"category update (unexpected error, ID: {updateDto.Id})");
                }
                throw new ServiceException($"An error occurred while updating category with ID {updateDto.Id}.", ex);
            }
        }


        /// <summary>
        /// Creates a new category.
        /// </summary>
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto, nameof(createDto));

            _logger.LogInformation("Attempting to create category: Name='{CategoryName}', ParentId='{ParentId}'",
                createDto.Name, createDto.ParentCategoryId);

            // 1. Validate DTO using FluentValidation
            // (This validator should check ParentCategoryId existence and name format)
            var validationResult = await _createCategoryValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateCategoryDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Additional Business Rule: Check for name uniqueness under the parent (if not fully covered by validator)
            // The validator for CreateCategoryDto should ideally already handle this using NameExistsAsync.
            // If NameExistsAsync is not called within the validator, it must be called here.
            // Assuming the validator now handles the DB check for uniqueness:
            // if (await _unitOfWork.Categories.NameExistsAsync(createDto.Name, createDto.ParentCategoryId, null))
            // {
            //     string duplicateMessage = $"A category named '{createDto.Name}' already exists " +
            //                             (createDto.ParentCategoryId.HasValue ? "under the selected parent." : "at the top level.");
            //     _logger.LogWarning(duplicateMessage);
            //     throw new DuplicationException(duplicateMessage, nameof(Category), "NameAndParent", $"{createDto.Name}_{createDto.ParentCategoryId}");
            // }


            string? persistentImagePath = null;
            try
            {
                // 3. Handle Image Saving (if an image path is provided in the DTO)
                if (!string.IsNullOrWhiteSpace(createDto.ImagePath))
                {
                    _logger.LogInformation("Image source path provided: '{SourcePath}'. Saving image for new category.", createDto.ImagePath);
                    // createDto.ImagePath is the SOURCE path (e.g., from file dialog)
                    persistentImagePath = await _imageFileService.SaveImageAsync(createDto.ImagePath, "Categories");
                    // ^ SaveImageAsync should handle copying, unique naming, and return the PERSISTENT path
                    _logger.LogInformation("Image saved. Persistent path: '{PersistentPath}' for new category.", persistentImagePath);
                }

                // 4. Map DTO to Domain Entity
                var newCategory = new Category
                {
                    Name = createDto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(createDto.Description) ? null : createDto.Description.Trim(),
                    ImagePath = persistentImagePath, // Store the final persistent path (or null)
                    ParentCategoryId = createDto.ParentCategoryId == 0 ? null : createDto.ParentCategoryId, // Treat 0 as null for top-level
                    IsActive = createDto.IsActive,
                    CreatedAt = DateTime.UtcNow // Set by BaseEntity or explicitly here
                };

                // 5. Add to Repository
                await _unitOfWork.Categories.AddAsync(newCategory);

                // 6. Save Changes to Database
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Category created successfully: ID={CategoryId}, Name='{CategoryName}', ParentId='{ParentId}'",
                    newCategory.Id, newCategory.Name, newCategory.ParentCategoryId);

                // 7. Map back to DTO for return (fetch with details if needed for ParentCategoryName)
                var createdCategoryEntity = await _unitOfWork.Categories.GetByIdWithChildrenAndParentAsync(newCategory.Id);
                if (createdCategoryEntity == null) // Should not happen
                {
                    _logger.LogError("Critical: Failed to re-fetch category with ID {CategoryId} immediately after creation.", newCategory.Id);
                    throw new ServiceException("Failed to retrieve details for the newly created category. The category was created, but its full details could not be immediately fetched.");
                }
                return MapCategoryToDto(createdCategoryEntity, includeChildren: false); // Don't need children for a create response usually
            }
            catch (ValidationException valEx) // Re-throw validation exceptions from validator
            {
                _logger.LogWarning(valEx, "Validation exception during category creation for Name: {CategoryName}.", createDto.Name);
                throw;
            }
            catch (DuplicationException dupEx) // If you have a specific one
            {
                _logger.LogWarning(dupEx, "Duplication exception during category creation for Name: {CategoryName}.", createDto.Name);
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, "category creation (duplication)");
                throw;
            }
            catch (IOException ioEx) // Specific to file operations
            {
                _logger.LogError(ioEx, "File IO error occurred while saving image for new category '{CategoryName}'. Category not created.", createDto.Name);
                // No need to delete persistentImagePath as it wasn't saved to DB.
                throw new ServiceException("An error occurred saving the image file. The category was not created.", ioEx);
            }
            catch (DbUpdateException dbEx) // Handles potential database constraint violations
            {
                _logger.LogError(dbEx, "Database error occurred while creating category: '{CategoryName}'.", createDto.Name);
                // Attempt to clean up the image if it was saved before the DB error
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, "category creation (DB error)");
                throw new ServiceException("A database error occurred while creating the category.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating category: '{CategoryName}'.", createDto.Name);
                await AttemptImageCleanupOnErrorAsync(persistentImagePath, "category creation (unexpected error)");
                throw new ServiceException($"An error occurred while creating category '{createDto.Name}'.", ex);
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes a category. Checks for dependencies (subcategories, parts) before deletion.
        /// This is a hard delete from the database.
        /// </summary>
        public async Task DeleteCategoryAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to delete category with invalid ID: {CategoryId}", id);
                throw new ArgumentException("Invalid Category ID for deletion.", nameof(id));
            }

            _logger.LogInformation("Attempting to delete category with ID: {CategoryId}", id);

            // 1. Fetch Existing Entity
            var categoryToDelete = await _unitOfWork.Categories.GetByIdAsync(id);
            if (categoryToDelete == null)
            {
                string notFoundMsg = $"Category with ID {id} not found for deletion.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
            }

            // 2. Dependency Checks
            var blockingDependents = new List<string>();

            bool hasSubcategories = await _unitOfWork.Categories.HasSubcategoriesAsync(id);
            if (hasSubcategories)
            {
                _logger.LogWarning("Attempt to delete category ID {CategoryId} which has subcategories.", id);
                blockingDependents.Add(nameof(Category.Subcategories)); // Or a user-friendly "Subcategories"
            }

            bool hasParts = await _unitOfWork.Categories.HasAssociatedPartsAsync(id);
            if (hasParts)
            {
                _logger.LogWarning("Attempt to delete category ID {CategoryId} which has associated parts.", id);
                blockingDependents.Add(nameof(Part)); // Or a user-friendly "Associated Parts"
            }

            if (blockingDependents.Any())
            {
                string depError = $"Cannot delete Category '{categoryToDelete.Name}' (ID: {id}) due to existing dependencies: {string.Join(", ", blockingDependents)}. Please remove or reassign these items first.";
                _logger.LogError(depError);
                throw new DeletionBlockedException(depError, nameof(Category), id.ToString(), blockingDependents);
            }

            string? imagePathToDelete = categoryToDelete.ImagePath; // Store before entity is deleted

            try
            {
                // 3. Delete Entity from Database
                _unitOfWork.Categories.Delete(categoryToDelete); // Mark for deletion
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Category with ID {CategoryId} and Name '{CategoryName}' deleted from database successfully.", id, categoryToDelete.Name);

                // 4. Delete Associated Image File (if it exists) AFTER successful DB deletion
                if (!string.IsNullOrWhiteSpace(imagePathToDelete))
                {
                    _logger.LogInformation("Attempting to delete image file '{ImagePath}' for deleted category ID {CategoryId}.", imagePathToDelete, id);
                    await _imageFileService.DeleteImageAsync(imagePathToDelete);
                    _logger.LogInformation("Image file '{ImagePath}' for category ID {CategoryId} deleted successfully.", imagePathToDelete, id);
                }

                _logger.LogInformation("Category with ID {CategoryId} full delete process completed.", id);
            }
            catch (DbUpdateException dbEx) // Should be rare if dependency checks are thorough, but good for unexpected DB constraints
            {
                _logger.LogError(dbEx, "Database error occurred while deleting category ID {CategoryId}. The category might be in use by an unexpected entity.", id);
                // At this point, the image file (if any) has not been deleted yet.
                throw new ServiceException("A database error occurred while deleting the category. It might still be in use.", dbEx);
            }
            catch (IOException ioEx) // If image deletion fails after DB deletion
            {
                _logger.LogError(ioEx, "File IO error occurred while deleting image file '{ImagePath}' for category ID {CategoryId}. The category was deleted from the database, but its image file might remain.", imagePathToDelete, id);
                // The category is deleted from DB, but image file might be orphaned. This is an inconsistency.
                // Log it thoroughly. Depending on requirements, could try to mark the category for "image cleanup pending".
                throw new ServiceException($"The category was deleted, but an error occurred deleting its image file '{imagePathToDelete}'. Please check file system permissions or manually remove the file.", ioEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting category ID {CategoryId}", id);
                throw new ServiceException($"An error occurred while deleting category with ID {id}.", ex);
            }
        }

        #endregion

        #region Helpers

        // --- Mapping Helper ---
        private CategoryDto MapCategoryToDto(Category category, bool includeChildren = true, int currentDepth = 0, int maxDepth = 5, bool onlyActiveFilter = true) // Max depth & onlyActiveFilter added
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            var dto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImagePath = category.ImagePath,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                ModifiedAt = category.ModifiedAt,
                // For PartCount, it's better to query if not already loaded.
                // For now, assuming Parts collection might not be loaded for performance in lists.
                // PartCount = category.Parts?.Count ?? await _unitOfWork.Categories.GetPartCountAsync(category.Id), // Example if repo method exists
                PartCount = category.Parts?.Count ?? 0, // Simple version, relies on Parts being loaded
                HasChildren = category.Subcategories?.Any(sc => !onlyActiveFilter || sc.IsActive) ?? false // Check active children if filtering
            };

            if (includeChildren && category.Subcategories != null && currentDepth < maxDepth)
            {
                var childrenToConsider = onlyActiveFilter
                                        ? category.Subcategories.Where(sc => sc.IsActive)
                                        : category.Subcategories;

                foreach (var subcategory in childrenToConsider.OrderBy(s => s.Name))
                {
                    dto.Children.Add(MapCategoryToDto(subcategory, true, currentDepth + 1, maxDepth, onlyActiveFilter));
                }
                // Recalculate HasChildren based on what was actually added to dto.Children
                dto.HasChildren = dto.Children.Any();
            }
            return dto;
        }

        // Helper for active filtering in recursion (can be refined)
        private bool ShouldFilterByActive(bool includeChildrenFlagFromCaller) => includeChildrenFlagFromCaller;

        /// <summary>
        /// Helper method to generate a hierarchical name for a category.
        /// e.g., "Parent > Child > Grandchild"
        /// </summary>
        private string GetHierarchicalCategoryName(Category category, IEnumerable<Category> allCategories)
        {
            if (category == null) return string.Empty;

            var nameParts = new Stack<string>();
            nameParts.Push(category.Name);

            var currentParentId = category.ParentCategoryId;
            int safetyBreak = 0; // To prevent infinite loops in case of data issues

            while (currentParentId.HasValue && safetyBreak < 20) // Max 20 levels deep
            {
                var parent = allCategories.FirstOrDefault(c => c.Id == currentParentId.Value);
                if (parent != null)
                {
                    nameParts.Push(parent.Name);
                    currentParentId = parent.ParentCategoryId;
                }
                else
                {
                    break; // Parent not found in the provided list (should not happen if allCategories is complete)
                }
                safetyBreak++;
            }
            return string.Join(" > ", nameParts);
        }

        private async Task AttemptImageCleanupOnErrorAsync(string? imagePath, string operationContext)
        {
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    _logger.LogWarning("Attempting to clean up orphaned image file due to error in {Context}: {ImagePath}", operationContext, imagePath);
                    await _imageFileService.DeleteImageAsync(imagePath);
                    _logger.LogInformation("Orphaned image file cleaned up successfully: {ImagePath}", imagePath);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to clean up orphaned image file '{ImagePath}' after error in {Context}.", imagePath, operationContext);
                    // Log and continue, as the primary error is more important to report.
                }
            }
        }

        #endregion
    }
}
