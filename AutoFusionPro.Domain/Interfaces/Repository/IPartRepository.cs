using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IPartRepository : IBaseRepository<Part>
    {
        Task<IEnumerable<Part>> GetPartsByCategory(int categoryId);
        Task<IEnumerable<Part>> GetLowStockParts(int thresholdQuantity);
        Task<IEnumerable<Part>> GetPartsByManufacturer(string manufacturer);
        Task<IEnumerable<Part>> SearchParts(string searchTerm);
        Task<IEnumerable<Part>> GetPartsByVehicle(int vehicleId);
       // Task<bool> UpdatePartStock(int partId, int newQuantity, string reason = null);
        Task<Part?> GetPartByPartNumber(string partNumber);
        Task<IEnumerable<Part>> GetActivePartsWithPagination(int pageNumber, int pageSize);
        Task<IEnumerable<Part>> GetPartsWithSuppliers();


        /// <summary>
        /// Gets a Part by its unique PartNumber, optionally including related entities.
        /// </summary>
        Task<Part?> GetByPartNumberAsync(string partNumber, bool includeDetails = false);

        /// <summary>
        /// Gets a single Part by ID, including detailed related entities like Category, Suppliers, Compatibility.
        /// </summary>
        Task<Part?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Finds parts based on a predicate, optionally including Category for summary display.
        /// </summary>
        Task<IEnumerable<Part>> FindWithCategoryAsync(Expression<Func<Part, bool>> predicate);

        /// <summary>
        /// Searches parts based on a search term across multiple fields (optimized query).
        /// </summary>
        Task<IEnumerable<Part>> SearchPartsAsync(string searchTerm);

        /// <summary>
        /// Gets parts with pagination, optionally including Category.
        /// </summary>
        Task<IEnumerable<Part>> GetPartsPagedAsync(int pageNumber, int pageSize, bool includeCategory = true, Expression<Func<Part, bool>>? filter = null);

        /// <summary>
        /// Gets the total count of parts, potentially applying a filter.
        /// </summary>
        Task<int> GetTotalPartsCountAsync(Expression<Func<Part, bool>>? filter = null);

        /// <summary>
        /// Checks if a PartNumber already exists, optionally excluding a specific Part ID.
        /// </summary>
        Task<bool> PartNumberExistsAsync(string partNumber, int? excludePartId = null);

        // Add other specific query methods if needed (e.g., GetPartsBySupplierIdAsync)

        // REMOVED: UpdatePartStock - This logic belongs in a service layer.
    }
}
