using AutoFusionPro.Application.DTOs.UnitOfMeasure;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class UnitOfMeasureService : IUnitOfMeasureService
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnitOfMeasureService> _logger;
        private readonly IValidator<CreateUnitOfMeasureDto> _createValidator;
        private readonly IValidator<UpdateUnitOfMeasureDto> _updateValidator;
        #endregion

        public UnitOfMeasureService(
            IUnitOfWork unitOfWork,
            ILogger<UnitOfMeasureService> logger,
            IValidator<CreateUnitOfMeasureDto> createValidator,
            IValidator<UpdateUnitOfMeasureDto> updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }


        #region CRUD Operations

        /// <summary>
        /// Gets a single Unit of Measure by its ID.
        /// </summary>
        public async Task<UnitOfMeasureDto?> GetUnitOfMeasureByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get Unit of Measure with invalid ID: {UnitOfMeasureId}", id);
                // Depending on desired behavior, you could throw ArgumentOutOfRangeException
                // or return null as the interface suggests.
                return null;
            }

            _logger.LogInformation("Attempting to retrieve Unit of Measure with ID: {UnitOfMeasureId}", id);
            try
            {
                var unitOfMeasureEntity = await _unitOfWork.UnitOfMeasures.GetByIdAsync(id);

                if (unitOfMeasureEntity == null)
                {
                    _logger.LogWarning("Unit of Measure with ID {UnitOfMeasureId} not found.", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved Unit of Measure with ID {UnitOfMeasureId}: {UnitOfMeasureName}",
                    id, unitOfMeasureEntity.Name);

                return MapUnitOfMeasureToDto(unitOfMeasureEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Unit of Measure with ID {UnitOfMeasureId}.", id);
                throw new ServiceException($"Could not retrieve Unit of Measure with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Gets all Units of Measure, typically for populating selection controls.
        /// Results are ordered by Name.
        /// </summary>
        public async Task<IEnumerable<UnitOfMeasureDto>> GetAllUnitOfMeasuresAsync()
        {
            _logger.LogInformation("Attempting to retrieve all Units of Measure.");
            try
            {
                var unitOfMeasureEntities = await _unitOfWork.UnitOfMeasures.GetAllAsync();

                // Order in memory after fetching, or ensure GetAllAsync from repo can take an OrderBy expression
                var dtos = unitOfMeasureEntities
                    .Select(MapUnitOfMeasureToDto)
                    .OrderBy(dto => dto.Name) // Order by name for consistent UI display
                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} Units of Measure.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all Units of Measure.");
                throw new ServiceException("Could not retrieve all Units of Measure.", ex);
            }
        }

        /// <summary>
        /// Creates a new Unit of Measure.
        /// </summary>
        public async Task<UnitOfMeasureDto> CreateUnitOfMeasureAsync(CreateUnitOfMeasureDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto, nameof(createDto));

            _logger.LogInformation("Attempting to create Unit of Measure: Name='{UomName}', Symbol='{UomSymbol}'",
                createDto.Name, createDto.Symbol);

            // 1. Validate DTO using FluentValidation
            // The validator (CreateUnitOfMeasureDtoValidator) should check for Name/Symbol uniqueness.
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateUnitOfMeasureDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }
            // Note: If your validator doesn't handle uniqueness via DB check, you'd add explicit checks here:
            // if (await _unitOfWork.UnitOfMeasures.NameExistsAsync(createDto.Name)) { /* throw DuplicationException */ }
            // if (await _unitOfWork.UnitOfMeasures.SymbolExistsAsync(createDto.Symbol)) { /* throw DuplicationException */ }

            try
            {
                // 2. Map DTO to Domain Entity
                var newUnitOfMeasure = new UnitOfMeasure
                {
                    Name = createDto.Name.Trim(),
                    Symbol = createDto.Symbol.Trim(), // Consider ToUpperInvariant() if symbols should be case-insensitive
                    Description = string.IsNullOrWhiteSpace(createDto.Description) ? null : createDto.Description.Trim(),
                    CreatedAt = DateTime.UtcNow // Or handled by BaseEntity/DbContext
                };

                // 3. Add to Repository
                await _unitOfWork.UnitOfMeasures.AddAsync(newUnitOfMeasure);

                // 4. Save Changes to Database
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Unit of Measure created successfully: ID={UomId}, Name='{UomName}', Symbol='{UomSymbol}'",
                    newUnitOfMeasure.Id, newUnitOfMeasure.Name, newUnitOfMeasure.Symbol);

                // 5. Map back to DTO for return
                return MapUnitOfMeasureToDto(newUnitOfMeasure);
            }
            catch (ValidationException) { throw; } // Re-throw if validator somehow runs again or for consistency
            catch (DbUpdateException dbEx) // Handles potential database constraint violations not caught by validator (e.g., race condition for unique index)
            {
                _logger.LogError(dbEx, "Database error occurred while creating Unit of Measure: {UomName}.", createDto.Name);
                // Check for specific unique constraint violations if your DB throws them in a identifiable way
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                {
                    // Determine if it was Name or Symbol based on exception details or re-query
                    throw new DuplicationException($"A Unit of Measure with similar Name or Symbol already exists.", nameof(UnitOfMeasure), "Name/Symbol", $"{createDto.Name}/{createDto.Symbol}", dbEx);
                }
                throw new ServiceException("A database error occurred while creating the Unit of Measure.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating Unit of Measure: {UomName}", createDto.Name);
                throw new ServiceException($"An error occurred while creating Unit of Measure '{createDto.Name}'.", ex);
            }
        }

        /// <summary>
        /// Updates an existing Unit of Measure's details.
        /// </summary>
        public async Task UpdateUnitOfMeasureAsync(UpdateUnitOfMeasureDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto, nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid Unit of Measure ID for update.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update Unit of Measure: ID={UomId}, Name='{UomName}', Symbol='{UomSymbol}'",
                updateDto.Id, updateDto.Name, updateDto.Symbol);

            // 1. Validate DTO using FluentValidation
            // The validator should check for Name/Symbol uniqueness, excluding the current entity's ID.
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateUnitOfMeasureDto (ID: {UomId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingUnitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(updateDto.Id);
            if (existingUnitOfMeasure == null)
            {
                string notFoundMsg = $"Unit of Measure with ID {updateDto.Id} not found for update.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or a custom NotFoundException
            }

            // Uniqueness checks (Name, Symbol) are now primarily handled by the validator.
            // If not, add explicit checks here similar to CreateMakeAsync.

            try
            {
                // 3. Map DTO changes to existing Domain Entity
                existingUnitOfMeasure.Name = updateDto.Name.Trim();
                existingUnitOfMeasure.Symbol = updateDto.Symbol.Trim(); // Consider ToUpperInvariant()
                existingUnitOfMeasure.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
                // existingUnitOfMeasure.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext/BaseEntity

                // 4. Save Changes (EF Core tracks changes to existingUnitOfMeasure)
                // No explicit _unitOfWork.UnitOfMeasures.Update() call needed if entity is tracked.
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Unit of Measure with ID {UomId} updated successfully.", updateDto.Id);
            }
            catch (ValidationException) { throw; } // Re-throw
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while updating Unit of Measure ID {UomId}.", updateDto.Id);
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                    dbEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new DuplicationException($"A Unit of Measure with similar Name or Symbol already exists.", nameof(UnitOfMeasure), "Name/Symbol", $"{updateDto.Name}/{updateDto.Symbol}", dbEx);
                }
                throw new ServiceException("A database error occurred while updating the Unit of Measure.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating Unit of Measure ID {UomId}", updateDto.Id);
                throw new ServiceException($"An error occurred while updating Unit of Measure with ID {updateDto.Id}.", ex);
            }
        }

        /// <summary>
        /// Deletes a Unit of Measure.
        /// Checks for dependencies (e.g., if used by any Part) before deletion.
        /// </summary>
        public async Task DeleteUnitOfMeasureAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to delete Unit of Measure with invalid ID: {UnitOfMeasureId}", id);
                throw new ArgumentException("Invalid Unit of Measure ID for deletion.", nameof(id));
            }

            _logger.LogInformation("Attempting to delete Unit of Measure with ID: {UnitOfMeasureId}", id);

            // 1. Fetch Existing Entity
            var uomToDelete = await _unitOfWork.UnitOfMeasures.GetByIdAsync(id);
            if (uomToDelete == null)
            {
                string notFoundMsg = $"Unit of Measure with ID {id} not found for deletion.";
                _logger.LogWarning(notFoundMsg);
                throw new ServiceException(notFoundMsg); // Or custom NotFoundException
            }

            // 2. Dependency Checks
            var blockingDependents = new List<string>();

            // Check Parts (Stocking UoM)
            if (await _unitOfWork.Parts.ExistsAsync(p => p.StockingUnitOfMeasureId == id))
            {
                blockingDependents.Add("Parts (as Stocking Unit)");
            }
            // Check Parts (Sales UoM)
            if (await _unitOfWork.Parts.ExistsAsync(p => p.SalesUnitOfMeasureId == id))
            {
                blockingDependents.Add("Parts (as Sales Unit)");
            }
            // Check Parts (Purchase UoM)
            if (await _unitOfWork.Parts.ExistsAsync(p => p.PurchaseUnitOfMeasureId == id))
            {
                blockingDependents.Add("Parts (as Purchase Unit)");
            }
            // Check OrderItems
            if (await _unitOfWork.ExistsAsync<OrderItem>(oi => oi.UnitOfMeasureId == id)) // Assuming generic ExistsAsync on UoW
            {
                blockingDependents.Add("Order Items");
            }
            // Check PurchaseItems
            if (await _unitOfWork.ExistsAsync<PurchaseItem>(pi => pi.UnitOfMeasureId == id)) // Assuming generic ExistsAsync on UoW
            {
                blockingDependents.Add("Purchase Items");
            }

            if (blockingDependents.Any())
            {
                string depError = $"Cannot delete Unit of Measure '{uomToDelete.Name}' (ID: {id}) as it is currently in use by: {string.Join(", ", blockingDependents)}. Please update these items first.";
                _logger.LogError(depError);
                throw new DeletionBlockedException(depError, nameof(UnitOfMeasure), id.ToString(), blockingDependents);
            }

            try
            {
                // 3. Delete Entity from Database
                _unitOfWork.UnitOfMeasures.Delete(uomToDelete); // Mark for deletion
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Unit of Measure with ID {UnitOfMeasureId} and Name '{UnitOfMeasureName}' deleted successfully.",
                    id, uomToDelete.Name);
            }
            catch (DbUpdateException dbEx) // Should be rare if dependency checks are thorough
            {
                _logger.LogError(dbEx, "Database error occurred while deleting Unit of Measure ID {UnitOfMeasureId}. It might still be referenced by an unexpected entity.", id);
                throw new ServiceException("A database error occurred while deleting the Unit of Measure. It might still be in use.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting Unit of Measure ID {UnitOfMeasureId}", id);
                throw new ServiceException($"An error occurred while deleting Unit of Measure with ID {id}.", ex);
            }
        }

        #endregion

        #region Helpers

        private UnitOfMeasureDto MapUnitOfMeasureToDto(UnitOfMeasure uom)
        {
            // ArgumentNullException.ThrowIfNull(uom); // C# 10+ shortcut
            if (uom == null) throw new ArgumentNullException(nameof(uom));

            return new UnitOfMeasureDto(
                uom.Id,
                uom.Name,
                uom.Symbol,
                uom.Description
            );
        }

        #endregion
    }
}
