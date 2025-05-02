using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Core.Models;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    public interface IPartService
    {
        // --- Read Operations ---
        Task<PartDetailDto?> GetPartByIdAsync(int id);
        Task<PartDetailDto?> GetPartByPartNumberAsync(string partNumber);
        Task<PagedResult<PartSummaryDto>> GetFilteredPartsAsync(PartFilterCriteriaDto filterCriteria, int pageNumber, int pageSize);
        Task<IEnumerable<PartSummaryDto>> GetLowStockPartsAsync(int thresholdQuantity); // Returns summary

        Task<IEnumerable<PartSummaryDto>> GetPartSummariesAsync();

        // --- Write Operations ---
        Task<PartDetailDto> CreatePartAsync(CreatePartDto createDto); // Returns detail of created part
        Task UpdatePartAsync(UpdatePartDto updateDto);
        Task DeletePartAsync(int id); // Soft delete (sets IsActive = false)
        Task ActivatePartAsync(int id); // Sets IsActive = true

        // --- Relationship Management ---
        Task AddPartCompatibilityAsync(int partId, int vehicleId, string? notes);
        Task RemovePartCompatibilityAsync(int partCompatibilityId); // Use specific ID
        Task UpdatePartSuppliersAsync(int partId, IEnumerable<PartSupplierDto> suppliers); // Replaces/updates suppliers for the part

        // --- Validation / Helpers ---
        Task<bool> PartNumberExistsAsync(string partNumber, int? excludePartId = null);

        // --- Image Management ---
        Task UpdatePartImagePathAsync(int partId, string? imagePath);

        // REMOVED: Direct stock update method. InventoryService should handle this.
    }
}

// Helper class definitions needed by the interface:
//namespace AutoFusionPro.Application.Dtos.Part { public record PartFilterCriteriaDto { /* fields */ } }