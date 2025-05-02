using AutoFusionPro.Application.DTOs.Vehicle;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Models;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class VehicleService : IVehicleService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VehicleService> _logger;
        private readonly IValidator<CreateVehicleDto> _createValidator;
        private readonly IValidator<UpdateVehicleDto> _updateValidator;

        #endregion

        public VehicleService(
                    IUnitOfWork unitOfWork,
                    ILogger<VehicleService> logger,
                    IValidator<CreateVehicleDto> createValidator,
                    IValidator<UpdateVehicleDto> updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));

            // Attempt to get context from UoW - Adjust based on your UoW implementation
            if (unitOfWork is IUnitOfWork concreteUoW) // Example cast
            {
                // _dbContext = concreteUoW.Context; // If UoW exposes it
                // For now, let's assume a way exists. If not, this dependency check needs adjustment.
                // If UoW exposes DbSets or a generic method, use that instead.
                // Fallback: Directly inject ApplicationDbContext if absolutely necessary (less ideal)
            }
            else
            {
                _logger.LogWarning("Could not resolve DbContext from IUnitOfWork for dependency checks. Delete operation might be unsafe.");
                // Handle this case - maybe disable delete or use alternative check?
            }

        }

        // --- Read Operations ---

        public async Task<VehicleDetailDto?> GetVehicleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get vehicle with invalid ID: {VehicleId}", id);
                return null;
            }
            try
            {
                var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
                if (vehicle == null)
                {
                    _logger.LogInformation("Vehicle with ID {VehicleId} not found.", id);
                    return null;
                }
                _logger.LogInformation("Retrieved vehicle with ID {VehicleId}.", id);
                return MapVehicleToDetailDto(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle with ID {VehicleId}", id);
                throw new ServiceException($"An error occurred while retrieving vehicle {id}.", ex);
            }
        }

        public async Task<VehicleDetailDto?> GetVehicleByVinAsync(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                _logger.LogWarning("Attempted to get vehicle with empty VIN.");
                return null;
            }
            // This method might be removed based on previous discussion,
            // but implemented here for completeness if kept.
            try
            {
                // Assuming GetByVinAsync exists on IVehicleRepository
                var vehicle = await _unitOfWork.Vehicles.GetByVinAsync(vin); // Add this to Repo interface if needed
                if (vehicle == null)
                {
                    _logger.LogInformation("Vehicle with VIN {VIN} not found.", vin);
                    return null;
                }
                _logger.LogInformation("Retrieved vehicle with VIN {VIN}.", vin);
                return MapVehicleToDetailDto(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle with VIN {VIN}", vin);
                throw new ServiceException($"An error occurred while retrieving vehicle with VIN {vin}.", ex);
            }
        }

        public async Task<PagedResult<VehicleSummaryDto>> GetFilteredVehiclesAsync(VehicleFilterCriteriaDto filterCriteria, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10; // Or a default value

            try
            {
                // Build filter expression
                var predicate = BuildVehicleFilterPredicate(filterCriteria);

                // Fetch paged data and total count
                var vehicles = await _unitOfWork.Vehicles.GetVehiclesPagedAsync(pageNumber, pageSize, predicate);
                var totalCount = await _unitOfWork.Vehicles.GetTotalVehiclesCountAsync(predicate);

                var vehicleDtos = vehicles.Select(MapVehicleToSummaryDto);

                _logger.LogInformation("Retrieved page {PageNumber} of vehicles with page size {PageSize}. Total matching: {TotalCount}", pageNumber, pageSize, totalCount);

                return new PagedResult<VehicleSummaryDto>
                {
                    Items = vehicleDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered vehicles.");
                throw new ServiceException("An error occurred while retrieving vehicles.", ex);
            }
        }

        // --- Write Operations ---

        public async Task<VehicleDetailDto> CreateVehicleAsync(CreateVehicleDto createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            _logger.LogInformation("Attempting to create vehicle: Make={Make}, Model={Model}, Year={Year}", createDto.Make, createDto.Model, createDto.Year);

            // 1. Validate DTO
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateVehicleDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Additional Business Logic Checks (Uniqueness)
            if (await _unitOfWork.Vehicles.MakeModelYearExistsAsync(createDto.Make, createDto.Model, createDto.Year))
            {
                string duplicateMessage = $"A vehicle with Make '{createDto.Make}', Model '{createDto.Model}', and Year '{createDto.Year}' already exists.";
                _logger.LogWarning(duplicateMessage);
                throw new ApplicationException(duplicateMessage); // Or custom DuplicateRecordException
            }
            // Optional: Check VIN uniqueness if VIN is provided
            if (!string.IsNullOrWhiteSpace(createDto.VIN) && await VinExistsAsync(createDto.VIN)) // Use service method
            {
                string vinMessage = $"A vehicle with VIN '{createDto.VIN}' already exists.";
                _logger.LogWarning(vinMessage);
                throw new ApplicationException(vinMessage);
            }


            try
            {
                // 3. Map DTO to Domain Entity
                var newVehicle = MapCreateDtoToVehicle(createDto);

                // 4. Add to Repository
                await _unitOfWork.Vehicles.AddAsync(newVehicle);

                // 5. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Vehicle created successfully with ID: {VehicleId}", newVehicle.Id);

                // 6. Map back to Detail DTO for return (includes the new ID)
                return MapVehicleToDetailDto(newVehicle);
            }
            catch (DbUpdateException dbEx)
            {
                // Catch specific DB errors, potentially unique constraints not caught earlier
                _logger.LogError(dbEx, "Database error occurred while creating vehicle Make={Make}, Model={Model}, Year={Year}.", createDto.Make, createDto.Model, createDto.Year);
                throw new ApplicationException($"A database error occurred while creating the vehicle.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating vehicle Make={Make}, Model={Model}, Year={Year}", createDto.Make, createDto.Model, createDto.Year);
                throw new ApplicationException("An unexpected error occurred while creating the vehicle.", ex);
            }
        }

        public async Task UpdateVehicleAsync(UpdateVehicleDto updateDto)
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            if (updateDto.Id <= 0) throw new ArgumentException("Invalid vehicle ID.", nameof(updateDto.Id));

            _logger.LogInformation("Attempting to update vehicle with ID: {VehicleId}", updateDto.Id);

            // 1. Validate DTO
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateVehicleDto (ID: {VehicleId}): {Errors}", updateDto.Id, validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch Existing Entity
            var existingVehicle = await _unitOfWork.Vehicles.GetByIdAsync(updateDto.Id);
            if (existingVehicle == null)
            {
                string notFoundMsg = $"Vehicle with ID {updateDto.Id} not found for update.";
                _logger.LogError(notFoundMsg);
                throw new ApplicationException(notFoundMsg); // Or custom NotFoundException
            }

            // 3. Additional Business Logic Checks (Uniqueness if key fields changed)
            if (existingVehicle.Make != updateDto.Make || existingVehicle.Model != updateDto.Model || existingVehicle.Year != updateDto.Year)
            {
                if (await _unitOfWork.Vehicles.MakeModelYearExistsAsync(updateDto.Make, updateDto.Model, updateDto.Year, updateDto.Id))
                {
                    string duplicateMessage = $"A vehicle with Make '{updateDto.Make}', Model '{updateDto.Model}', and Year '{updateDto.Year}' already exists.";
                    _logger.LogWarning(duplicateMessage);
                    throw new ApplicationException(duplicateMessage);
                }
            }
            // Optional: Check VIN uniqueness if VIN changed and is provided
            if (!string.IsNullOrWhiteSpace(updateDto.VIN) && updateDto.VIN != existingVehicle.VIN && await VinExistsAsync(updateDto.VIN, updateDto.Id))
            {
                string vinMessage = $"A vehicle with VIN '{updateDto.VIN}' already exists.";
                _logger.LogWarning(vinMessage);
                throw new ApplicationException(vinMessage);
            }

            try
            {
                // 4. Map changes from DTO to Existing Domain Entity
                MapUpdateDtoToVehicle(updateDto, existingVehicle);

                // 5. Save Changes (EF Core tracks changes on existingVehicle)
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Vehicle with ID {VehicleId} updated successfully.", updateDto.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while updating vehicle ID {VehicleId}.", updateDto.Id);
                throw new ApplicationException($"A database error occurred while updating the vehicle.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating vehicle ID {VehicleId}", updateDto.Id);
                throw new ApplicationException("An unexpected error occurred while updating the vehicle.", ex);
            }
        }

        public async Task DeleteVehicleAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid vehicle ID.", nameof(id));

            _logger.LogInformation("Attempting to delete vehicle with ID: {VehicleId}", id);

            // 1. Fetch Existing Entity
            var vehicleToDelete = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (vehicleToDelete == null)
            {
                _logger.LogWarning("Vehicle with ID {VehicleId} not found for deletion.", id);
                // Decide: Throw or return silently? Throwing is often better for explicit operations.
                throw new ServiceException($"Vehicle with ID {id} not found.");
            }

            // 2. Dependency Check (PartCompatibility)
            // --- ADJUSTED Dependency Check ---
            try
            {
                // Use the specific repository method via the Unit of Work
                bool hasCompatibilityLinks = await _unitOfWork.Vehicles.HasCompatibilityLinksAsync(id);

                if (hasCompatibilityLinks)
                {
                    string dependencyError = $"Cannot delete Vehicle ID {id} because it has associated Part Compatibility links.";
                    _logger.LogError(dependencyError);
                    throw new ServiceException(dependencyError);
                }
            }
            catch (Exception ex) // Catch potential exceptions from the check itself
            {
                _logger.LogError(ex, "Failed to check compatibility dependencies for Vehicle ID {VehicleId}. Deletion aborted.", id);
                // Throw a specific error indicating the check failed
                throw new ServiceException($"Could not verify dependencies before deleting Vehicle ID {id}. Deletion aborted.", ex);
            }


            try
            {
                // 3. Delete Entity
                _unitOfWork.Vehicles.Delete(vehicleToDelete); // Mark for deletion

                // 4. Save Changes
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Vehicle with ID {VehicleId} deleted successfully.", id);
            }
            catch (DbUpdateException dbEx) // Catch potential foreign key issues if dependency check fails
            {
                _logger.LogError(dbEx, "Database error occurred while deleting vehicle ID {VehicleId}.", id);
                throw new ServiceException($"A database error occurred while deleting the vehicle.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting vehicle ID {VehicleId}", id);
                throw new ServiceException("An unexpected error occurred while deleting the vehicle.", ex);
            }
        }


        // --- Helper / Validation Methods ---

        public async Task<bool> MakeModelYearExistsAsync(string make, string model, int year, int? excludeVehicleId = null)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(make) || string.IsNullOrWhiteSpace(model) || year <= 1900)
            {
                return false; // Or throw, depending on usage context
            }
            try
            {
                return await _unitOfWork.Vehicles.MakeModelYearExistsAsync(make, model, year, excludeVehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Make/Model/Year existence for Make={Make}, Model={Model}, Year={Year}", make, model, year);
                // Decide: return false or rethrow wrapped exception? Rethrow might be better.
                throw new ApplicationException("Could not verify vehicle uniqueness.", ex);
            }
        }

        // Public method to check VIN existence (if needed elsewhere, even if optional)
        public async Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(vin)) return false; // Empty VIN doesn't exist uniquely

            try
            {
                // Assuming VinExistsAsync exists on IVehicleRepository
                return await _unitOfWork.Vehicles.VinExistsAsync(vin, excludeVehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking VIN existence for VIN={VIN}", vin);
                throw new ApplicationException("Could not verify VIN uniqueness.", ex);
            }
        }

        // --- Methods for Populating UI Controls ---

        public async Task<IEnumerable<string>> GetDistinctMakesAsync()
        {
            try
            {
                return await _unitOfWork.Vehicles.GetDistinctMakesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distinct vehicle makes.");
                return Enumerable.Empty<string>(); // Return empty on error
            }
        }

        public async Task<IEnumerable<string>> GetDistinctModelsAsync(string make)
        {
            if (string.IsNullOrWhiteSpace(make)) return Enumerable.Empty<string>();
            try
            {
                return await _unitOfWork.Vehicles.GetDistinctModelsAsync(make);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distinct vehicle models for Make={Make}.", make);
                return Enumerable.Empty<string>();
            }
        }

        public async Task<IEnumerable<int>> GetDistinctYearsAsync(string? make = null, string? model = null)
        {
            try
            {
                return await _unitOfWork.Vehicles.GetDistinctYearsAsync(make, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distinct vehicle years for Make={Make}, Model={Model}.", make, model);
                return Enumerable.Empty<int>();
            }
        }

        // --- Private Helper Methods ---

        private Expression<Func<Vehicle, bool>> BuildVehicleFilterPredicate(VehicleFilterCriteriaDto criteria)
        {
            // Use a predicate builder library or construct manually
            // Example using PredicateBuilder (NuGet package LinqKit recommended)
            var predicate = LinqKit.PredicateBuilder.New<Vehicle>(true); // Start with true (AND)

            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                string term = criteria.SearchTerm.ToLower().Trim();
                predicate = predicate.And(v =>
                    (v.Make != null && v.Make.ToLower().Contains(term)) ||
                    (v.Model != null && v.Model.ToLower().Contains(term)) ||
                    (v.VIN != null && v.VIN.ToLower().Contains(term)) || // Still search VIN if provided
                    (v.Year.ToString().Contains(term)) // Basic year search
                );
            }

            if (!string.IsNullOrWhiteSpace(criteria.Make))
            {
                predicate = predicate.And(v => v.Make != null && v.Make == criteria.Make);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Model))
            {
                predicate = predicate.And(v => v.Model != null && v.Model == criteria.Model);
            }

            if (criteria.MinYear.HasValue)
            {
                predicate = predicate.And(v => v.Year >= criteria.MinYear.Value);
            }

            if (criteria.MaxYear.HasValue)
            {
                predicate = predicate.And(v => v.Year <= criteria.MaxYear.Value);
            }

            return predicate;
        }

        private Vehicle MapCreateDtoToVehicle(CreateVehicleDto dto)
        {
            // Assumes validation already passed
            return new Vehicle
            {
                Make = dto.Make.Trim(),
                Model = dto.Model.Trim(),
                Year = dto.Year,
                VIN = string.IsNullOrWhiteSpace(dto.VIN) ? null : dto.VIN.Trim().ToUpperInvariant(), // Store VIN uppercase?
                Engine = dto.Engine?.Trim() ?? string.Empty,
                Transmission = dto.Transmission?.Trim() ?? string.Empty,
                TrimLevel = dto.TrimLevel?.Trim() ?? string.Empty,
                BodyType = dto.BodyType?.Trim() ?? string.Empty
                // CompatibleParts is managed separately
                // BaseEntity properties (CreatedAt/ModifiedAt) handled by DbContext
            };
        }

        private void MapUpdateDtoToVehicle(UpdateVehicleDto dto, Vehicle existingVehicle)
        {
            // Assumes validation already passed and existingVehicle is not null
            existingVehicle.Make = dto.Make.Trim();
            existingVehicle.Model = dto.Model.Trim();
            existingVehicle.Year = dto.Year;
            existingVehicle.VIN = string.IsNullOrWhiteSpace(dto.VIN) ? null : dto.VIN.Trim().ToUpperInvariant();
            existingVehicle.Engine = dto.Engine?.Trim() ?? string.Empty;
            existingVehicle.Transmission = dto.Transmission?.Trim() ?? string.Empty;
            existingVehicle.TrimLevel = dto.TrimLevel?.Trim() ?? string.Empty;
            existingVehicle.BodyType = dto.BodyType?.Trim() ?? string.Empty;
            // ModifiedAt handled by DbContext
        }

        private VehicleDetailDto MapVehicleToDetailDto(Vehicle vehicle)
        {
            if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
            return new VehicleDetailDto( // Using constructor if available
                vehicle.Id,
                vehicle.Make,
                vehicle.Model,
                vehicle.Year,
                vehicle.VIN,
                vehicle.Engine,
                vehicle.Transmission ?? "Unknown",
                vehicle.TrimLevel ?? "Unknown",
                vehicle.BodyType ?? "Unknown"
            );
        }

        private VehicleSummaryDto MapVehicleToSummaryDto(Vehicle vehicle)
        {
            if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
            return new VehicleSummaryDto(
                vehicle.Id,
                vehicle.Make,
                vehicle.Model,
                vehicle.Year,
                vehicle.Engine,
                vehicle.Transmission ?? "Unknown",
                vehicle.TrimLevel ?? "Unknown",
                vehicle.BodyType ?? "Unknown"
            );
        }
    }
}
