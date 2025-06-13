using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Core.Models;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    public interface IPartService
    {
        #region Part Core CRUD & Read Operations

        /// <summary>
        /// Gets detailed information for a specific part by its ID.
        /// Includes Category, selected Suppliers, and selected CompatibleVehicles.
        /// </summary>
        Task<PartDetailDto?> GetPartDetailsByIdAsync(int id);


        // FOR DEV TESTING ONLY! 
        // TODO : To be removed
        Task<IEnumerable<PartSummaryDto>> GetAllPartsSummariesAsync();

        /// <summary>
        /// Gets detailed information for a specific part by its unique PartNumber.
        /// Includes Category.
        /// </summary>
        Task<PartDetailDto?> GetPartDetailsByPartNumberAsync(string partNumber);

        /// <summary>
        /// Gets a paginated list of part summaries based on filter criteria.
        /// </summary>
        Task<PagedResult<PartSummaryDto>> GetFilteredPartSummariesAsync(PartFilterCriteriaDto filterCriteria, int pageNumber, int pageSize);

        /// <summary>
        /// Gets a list of part summaries for parts that are low on stock.
        /// </summary>
        Task<IEnumerable<PartSummaryDto>> GetLowStockPartSummariesAsync(int? topN = null); // Optional: take top N most critical

        /// <summary>
        /// Creates a new part.
        /// </summary>
        Task<PartDetailDto> CreatePartAsync(CreatePartDto createDto);

        /// <summary>
        /// Updates an existing part's core details.
        /// Does NOT update suppliers or compatibilities directly through this method; use dedicated relationship methods.
        /// </summary>
        Task UpdatePartAsync(UpdatePartDto updateDto);

        /// <summary>
        /// Soft deletes a part (sets IsActive = false).
        /// </summary>
        Task DeactivatePartAsync(int id); // Renamed from DeletePartAsync for clarity of soft delete

        /// <summary>
        /// Activates a previously deactivated part (sets IsActive = true).
        /// </summary>
        Task ActivatePartAsync(int id);

        #endregion

        #region Part Image Management

        /// <summary>
        /// Updates the image path for a specific part. Handles saving new image and deleting old one.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <param name="sourceImagePath">The source path of the new image (from client), or null/empty to clear the image.</param>
        Task UpdatePartImageAsync(int partId, string? sourceImagePath); // Renamed for clarity

        #endregion

        #region Part Compatibility Management

        /// <summary>
        /// Adds a CompatibleVehicle specification to a Part.
        /// </summary>
        Task AddCompatibilityAsync(int partId, PartCompatibilityCreateDto compatibilityDto);

        /// <summary>
        /// Removes a specific PartCompatibility link by its own ID.
        /// </summary>
        Task RemoveCompatibilityAsync(int partCompatibilityId);

        /// <summary>
        /// Gets all compatibility links (as DTOs) for a specific part.
        /// </summary>
        Task<IEnumerable<PartCompatibilityDto>> GetCompatibilitiesForPartAsync(int partId);

        #endregion

        #region Part Supplier Management

        /// <summary>
        /// Adds a Supplier link to a Part with specific details (cost, supplier part number, etc.).
        /// </summary>
        Task AddSupplierToPartAsync(int partId, PartSupplierCreateDto supplierLinkDto);

        /// <summary>
        /// Updates the details of an existing link between a Part and a Supplier.
        /// </summary>
        /// <param name="supplierPartId">The ID of the SupplierPart join entity record.</param>
        /// <param name="updateDto">DTO containing updated cost, supplier part #, etc.</param>
        Task UpdateSupplierLinkForPartAsync(int supplierPartId, PartSupplierDto updateDto); // PartSupplierDto can be reused if it has relevant fields

        /// <summary>
        /// Removes a specific SupplierPart link by its own ID.
        /// </summary>
        Task RemoveSupplierFromPartAsync(int supplierPartId); // Use specific SupplierPart join table ID

        /// <summary>
        /// Gets all supplier links (as DTOs) for a specific part.
        /// </summary>
        Task<IEnumerable<PartSupplierDto>> GetSuppliersForPartAsync(int partId);

        #endregion

        #region Validation Helpers

        /// <summary>
        /// Checks if a PartNumber already exists.
        /// </summary>
        Task<bool> PartNumberExistsAsync(string partNumber, int? excludePartId = null);

        /// <summary>
        /// Checks if a Barcode already exists (if provided and non-empty).
        /// </summary>
        Task<bool> BarcodeExistsAsync(string barcode, int? excludePartId = null);

        #endregion

        #region Inventory Related (Interactions with a future InventoryService)
        // These methods might actually live on an InventoryService in the future,
        // but IPartService might need to query some basic stock info for display.

        /// <summary>
        /// Gets the current stock quantity for a specific part.
        /// (This might call an InventoryService or query a denormalized field).
        /// </summary>
        Task<int> GetCurrentStockForPartAsync(int partId);

        #endregion
    }
}
