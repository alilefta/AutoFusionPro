using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Models;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class PartService : IPartService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreatePartDto> _createPartValidator;
        private readonly ILogger<PartService> _logger;

        #endregion

        public PartService(IUnitOfWork unitOfWork,
            IValidator<CreatePartDto> createPartValidator,
            ILogger<PartService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _createPartValidator = createPartValidator ?? throw new ArgumentNullException(nameof(createPartValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));    
        }


        #region Create Operations

        public async Task<PartDetailDto> CreatePartAsync(CreatePartDto createDto)
        {
            if(createDto == null) throw new ArgumentNullException(nameof(createDto));


            var validationResult = await _createPartValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                // Log validation errors
                _logger.LogWarning("Validation failed for CreatePartDto: {Errors}", validationResult.ToString("~"));
                // Throw ValidationException - FluentValidation provides this exception type
                throw new ValidationException(validationResult.Errors);
            }

            try
            {

                var newPartModel = MapCreatePartDTOToPartModel(createDto);
                // Ensure defaults are set correctly (e.g., CurrentStock=0, etc.)

                await _unitOfWork.Parts.AddAsync(newPartModel);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Part created successfully with ID {PartId} and PartNumber {PartNumber}", newPartModel.Id, newPartModel.PartNumber);

                var createdPartWithDetails = await _unitOfWork.Parts.GetByIdWithDetailsAsync(newPartModel.Id);
                if (createdPartWithDetails == null)
                {
                    // This should ideally not happen, but handle it defensively
                    _logger.LogError("Failed to re-fetch part with ID {PartId} after creation.", newPartModel.Id);
                    throw new ServiceException($"Failed to retrieve details for newly created part {newPartModel.PartNumber}.");
                }
                return MapPartToPartDetailDTO(createdPartWithDetails); // Map the fully loaded entity
            }
            catch (ValidationException) // Catch the validation exception explicitly if needed
            {
                throw; // Re-throw validation exceptions to be handled by caller (e.g., ViewModel)
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while creating part with PartNumber {PartNumber}. Check InnerException.", createDto.PartNumber);
                // You might check dbEx.InnerException for specific SQL errors (like unique constraint)
                throw new ServiceException($"A database error occurred while creating the part. Please check logs.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating part with PartNumber {PartNumber}", createDto.PartNumber);
                throw new ServiceException($"An error occurred while creating part '{createDto.Name}'.", ex);
            }
        }

        public async Task AddPartCompatibilityAsync(int partId, int compatibileVehicle, string? notes)
        {
            if (partId <= 0 || compatibileVehicle <= 0)
            {
                _logger.LogError("Part ID or Vehicle ID is not correct");
                throw new ServiceException("Part ID or Vehicle ID is not correct");
            }

            // Check Part Existence
            var isPartExists = await _unitOfWork.Parts.ExistsAsync(p => p.Id == partId);

            if (!isPartExists)
            {
                _logger.LogError($"Part with ID: {partId} doesn't exists!");
                throw new ServiceException($"Part with ID: {partId} doesn't exists!");
            }

            // Check Vehicle Existence

            var isVehicleExists = await _unitOfWork.Vehicles.ExistsAsync(v => v.Id == compatibileVehicle);

            if (!isVehicleExists)
            {
                _logger.LogError($"Vehicle with ID: {compatibileVehicle} doesn't exists!");
                throw new ServiceException($"Vehicle with ID: {compatibileVehicle} doesn't exists!");
            }


            try
            {
                var newPartVehicleCompatibility = new PartCompatibility
                {
                    PartId = partId,
                    CompatibleVehicleId = compatibileVehicle,
                    Notes = notes ?? string.Empty,

                };

                await _unitOfWork.AddAsync(newPartVehicleCompatibility);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"An exception happened while saving a part compatibility to Db, {dbEx.Message}");
                throw new ServiceException("An exception happened while saving a part compatibility to Db", dbEx);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while saving a part compatibility, {ex.Message}");
                throw new ServiceException("An exception happened while saving a part compatibility", ex);
            }
        }

        #endregion

        #region Read Part

        public async Task<IEnumerable<PartSummaryDto>> GetPartSummariesAsync()
        {
            try
            {
               var partsData =  await _unitOfWork.Parts.GetAllAsync();

                var partSummaries = new List<PartSummaryDto>();

                foreach (var part in partsData)
                {
                    partSummaries.Add(MapPartToPartSummary(part));
                }

                return partSummaries;
            }
            catch(Exception ex)
            {
                throw;
            }
        }


        public Task<PagedResult<PartSummaryDto>> GetFilteredPartsAsync(PartFilterCriteriaDto filterCriteria, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PartSummaryDto>> GetLowStockPartsAsync(int thresholdQuantity)
        {
            throw new NotImplementedException();
        }

        public Task<PartDetailDto?> GetPartByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PartDetailDto?> GetPartByPartNumberAsync(string partNumber)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Check Operations

        public Task<bool> PartNumberExistsAsync(string partNumber, int? excludePartId = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Delete Operations

        public Task RemovePartCompatibilityAsync(int partCompatibilityId)
        {
            throw new NotImplementedException();
        }


        public Task DeletePartAsync(int id)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Update Operations

        public Task ActivatePartAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePartAsync(UpdatePartDto updateDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePartImagePathAsync(int partId, string? imagePath)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePartSuppliersAsync(int partId, IEnumerable<PartSupplierDto> suppliers)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Helpers

        public Part MapCreatePartDTOToPartModel(CreatePartDto createDto)
        {
            var newPart = new Part
            {
                PartNumber = createDto.PartNumber.Trim(), // Trim input
                Name = createDto.Name.Trim(),
                Description = createDto.Description?.Trim() ?? string.Empty,
                Manufacturer = createDto.Manufacturer?.Trim() ?? string.Empty,
                CostPrice = createDto.CostPrice,
                SellingPrice = createDto.SellingPrice,
                CategoryId = createDto.CategoryId,
                ReorderLevel = createDto.ReorderLevel,
                MinimumStock = createDto.MinimumStock, // Corrected
                Location = createDto.Location?.Trim() ?? string.Empty,
                IsActive = createDto.IsActive,
                IsOriginal = createDto.IsOriginal,
                ImagePath = createDto.ImagePath, // Path validation might be needed elsewhere
                Notes = createDto.Notes?.Trim() ?? string.Empty,
                // Set defaults for non-nullable value types if not in DTO
                CurrentStock = 0, // Initial stock is 0 until an inventory transaction
                // CreatedAt/ModifiedAt handled by DbContext
                // LastRestockDate handled by inventory transactions
                // Initialize collections
                CompatibleVehicles = new List<PartCompatibility>(),
                Suppliers = new List<SupplierPart>(),
                OrderItems = new List<OrderItem>(),
                InvoiceItems = new List<InvoiceItem>(),
                InventoryTransactions = new List<InventoryTransaction>()
            };

            return newPart;
        }

        public PartDetailDto MapPartToPartDetailDTO(Part part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            return new PartDetailDto
            {
                Id = part.Id,
                Name = part.Name,
                PartNumber = part.PartNumber,
                Description = part.Description,
                Manufacturer = part.Manufacturer,
                CostPrice = part.CostPrice,
                SellingPrice = part.SellingPrice,
                CategoryId = part.CategoryId,
                CategoryName = part.Category?.Name ?? "N/A", // Correct mapping
                ReorderLevel = part.ReorderLevel,
                MinimumStock = part.MinimumStock,
                Location = part.Location,
                IsActive = part.IsActive,
                IsOriginal = part.IsOriginal,
                ImagePath = part.ImagePath,
                Notes = part.Notes,
                CurrentStock = part.CurrentStock, // Assuming this is loaded correctly
                LastRestockDate = part.LastRestockDate, // Assuming this is loaded correctly

                // Correct relationship mapping
                //CompatibleVehicles = part.CompatibleVehicles?.Select(pc => new PartCompatibilityDto(
                //    pc.Id,
                //    pc.CompatibleVehicleId,
                //    pc.CompatibleVehicle?.Make ?? "Unknown",
                //    pc.Vehicle?.Model ?? "Unknown",
                //    pc.Vehicle?.Year ?? 0,
                //    pc.Notes
                //)).ToList() ?? Enumerable.Empty<PartCompatibilityDto>(),

                Suppliers = part.Suppliers?.Select(sp => new PartSupplierDto(
                    sp.Id,
                    sp.SupplierId,
                    sp.Supplier?.Name ?? "Unknown",
                    sp.SupplierPartNumber,
                    sp.Cost,
                    sp.IsPreferredSupplier
                )).ToList() ?? Enumerable.Empty<PartSupplierDto>(),
            };
        }

        public PartSummaryDto MapPartToPartSummary(Part part)
        {
            return new PartSummaryDto
           (
                part.Id,
                part.PartNumber,
                part.Name,
                part.Category?.Name ?? "Unknown",
                part.Manufacturer,
                part.SellingPrice,
                part.CurrentStock,
                part.IsActive
            );
        }

        #endregion
    }
}
