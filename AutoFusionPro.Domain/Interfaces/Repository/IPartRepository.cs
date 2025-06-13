using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IPartRepository : IBaseRepository<Part>
    {
        // --- SINGLE ENTITY FETCH METHODS ---

        /// <summary>
        /// Gets a single Part by its ID, optionally including Unit of Measures
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeUoMs"></param>
        /// <returns></returns>
        Task<Part?> GetByIdAsync(int id, bool includeUoMs = false);


        /// <summary>
        /// Gets a single Part by its ID, optionally including detailed related entities
        /// like Category, Suppliers (with Supplier details), and CompatibleVehicles (with Vehicle details).
        /// </summary>
        /// <param name="id">The ID of the Part.</param>
        /// <param name="includeCategory">Whether to include the Category.</param>
        /// <param name="includeSuppliers">Whether to include Suppliers and their details.</param>
        /// <param name="includeCompatibility">Whether to include CompatibleVehicles and their details.</param>
        /// <returns>The Part entity with specified details, or null if not found.</returns>
        Task<Part?> GetByIdWithDetailsAsync(int id,
            bool includeCategory = true,
            bool includeSuppliers = false,
            bool includeCompatibility = false,
            bool includeUnitOfMeasures = true);

        /// <summary>
        /// Gets a Part by its unique PartNumber, optionally including its Category.
        /// </summary>
        /// <param name="partNumber">The unique part number.</param>
        /// <param name="includeCategory">Whether to include the Category.</param>
        /// <returns>The Part entity, or null if not found.</returns>
        Task<Part?> GetByPartNumberAsync(string partNumber, bool includeCategory = true);

        /// <summary>
        /// Get Parts by Specific Category ID
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of Parts</returns>
        Task<IEnumerable<Part>> GetPartsByCategory(int categoryId);


        // --- MULTIPLE ENTITY FETCH METHODS (FILTERING & SEARCHING) ---

        /// <summary>
        /// Gets a collection of Parts based on a flexible filter predicate.
        /// Allows specifying which related entities to include for performance.
        /// </summary>
        /// <param name="predicate">The filter condition.</param>
        /// <param name="includeCategory">Whether to include the Category for each part.</param>
        /// <param name="includeSuppliers">Whether to include Suppliers for each part.</param>
        /// <param name="orderBy">Optional ordering expression.</param>
        /// <param name="take">Optional limit on the number of results.</param>
        /// <returns>A collection of matching Part entities.</returns>
        Task<IEnumerable<Part>> FindPartsAsync(Expression<Func<Part, bool>> predicate,
            bool includeCategory = false,
            bool includeSuppliers = false,
            Func<IQueryable<Part>, IOrderedQueryable<Part>>? orderBy = null,
            int? take = null);

        /// <summary>
        /// Gets a paginated list of Parts based on filter criteria and search term.
        /// This method is designed to be flexible for complex filtering UIs.
        /// </summary>
        /// <param name="pageNumber">The current page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="filterPredicate">Optional direct filter on Part entity.</param>
        /// <param name="categoryId">Optional filter by Category ID.</param>
        /// <param name="manufacturer">Optional filter by Manufacturer name (case-insensitive).</param>
        /// <param name="supplierId">Optional filter by Supplier ID.</param>
        /// <param name="restrictToCompatibleVehicleIds">Optional filter by CompatibleVehicle ID.</param>
        /// <param name="searchTerm">Optional search term for PartNumber, Name, Description, Barcode.</param>
        /// <param name="isActive">Optional filter by IsActive status.</param>
        /// <param name="isLowStock">Optional filter for low stock parts (CurrentStock <= ReorderLevel).</param>
        /// <param name="includeCategory">Whether to include the Category.</param>
        /// <param name="includeSuppliers">Whether to include Suppliers.</param>
        /// <returns>A collection of matching Part entities for the current page.</returns>
        Task<IEnumerable<Part>> GetFilteredPartsPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Part, bool>>? filterPredicate = null,
            int? categoryId = null,
            string? manufacturer = null,
            int? supplierId = null,
            IEnumerable<int>? restrictToCompatibleVehicleIds = null,
            string? searchTerm = null,
            bool? isActive = true, // Default to active parts
            bool? isLowStock = null,
            bool includeCategory = true,
            bool includeSuppliers = false,
            bool includeStockingUnitOfMeasure = true);
        /// <summary>
        /// Gets the total count of Parts matching the specified filter criteria.
        /// Parameters should mirror GetFilteredPartsPagedAsync for accurate counts.
        /// </summary>
        Task<int> GetTotalFilteredPartsCountAsync(
            Expression<Func<Part, bool>>? filterPredicate = null,
            int? categoryId = null,
            string? manufacturer = null,
            int? supplierId = null,
            IEnumerable<int>? restrictToCompatibleVehicleIds = null,
            string? searchTerm = null,
            bool? isActive = true,
            bool? isLowStock = null);


        // --- UTILITY / VALIDATION METHODS ---

        /// <summary>
        /// Checks if a PartNumber already exists, optionally excluding a specific Part ID.
        /// </summary>
        /// <param name="partNumber">The part number to check.</param>
        /// <param name="excludePartId">Optional ID of a part to exclude from the check (for updates).</param>
        /// <returns>True if the part number exists, false otherwise.</returns>
        Task<bool> PartNumberExistsAsync(string partNumber, int? excludePartId = null);

        /// <summary>
        /// Checks if a Barcode already exists, optionally excluding a specific Part ID.
        /// (Assumes Barcode should be unique if not null/empty)
        /// </summary>
        /// <param name="barcode">The barcode to check.</param>
        /// <param name="excludePartId">Optional ID of a part to exclude from the check.</param>
        /// <returns>True if the barcode exists and is not null/empty, false otherwise.</returns>
        Task<bool> BarcodeExistsAsync(string barcode, int? excludePartId = null);

        /// <summary>
        /// Gets a list of Parts that are low on stock (CurrentStock <= ReorderLevel).
        /// </summary>
        /// <param name="includeCategory">Whether to include Category information.</param>
        /// <param name="take">Optional limit on the number of results.</param>
        /// <returns>A collection of low stock Part entities.</returns>
        Task<IEnumerable<Part>> GetLowStockPartsAsync(bool includeCategory = true, int? take = null);
    }
}
