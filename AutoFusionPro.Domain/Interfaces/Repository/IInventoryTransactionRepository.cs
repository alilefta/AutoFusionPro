using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IInventoryTransactionRepository : IBaseRepository<InventoryTransaction>
    {
        /// <summary>
        /// Gets a single inventory transaction by its ID, including related Part and User details.
        /// </summary>
        /// <param name="transactionId">The ID of the inventory transaction.</param>
        /// <returns>The InventoryTransaction entity with details, or null if not found.</returns>
        Task<InventoryTransaction?> GetByIdWithDetailsAsync(int transactionId);

        /// <summary>
        /// Gets all inventory transactions for a specific part, ordered by TransactionDate descending.
        /// Includes related Part and User details.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <returns>A collection of InventoryTransaction entities for the specified part.</returns>
        Task<IEnumerable<InventoryTransaction>> GetByPartIdAsync(int partId);

        /// <summary>
        /// Gets a paginated list of InventoryTransaction entities based on filter criteria.
        /// Includes related Part and User details.
        /// </summary>
        /// <param name="pageNumber">The current page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="filterPredicate">Optional direct filter on InventoryTransaction entity.</param>
        /// <param name="partIdFilter">Optional filter by Part ID.</param>
        /// <param name="userIdFilter">Optional filter by User ID.</param>
        /// <param name="transactionTypeFilter">Optional filter by TransactionType.</param>
        /// <param name="dateFromFilter">Optional filter for transactions from this date (inclusive).</param>
        /// <param name="dateToFilter">Optional filter for transactions up to this date (inclusive).</param>
        /// <param name="referenceNumberSearch">Optional search term for ReferenceNumber.</param>
        /// <returns>A collection of matching InventoryTransaction entities for the current page.</returns>
        Task<IEnumerable<InventoryTransaction>> GetFilteredTransactionsPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<InventoryTransaction, bool>>? filterPredicate = null,
            int? partIdFilter = null,
            int? userIdFilter = null,
            Core.Enums.ModelEnum.InventoryTransactionType? transactionTypeFilter = null, // Use fully qualified enum
            DateTime? dateFromFilter = null,
            DateTime? dateToFilter = null, 
            string? referenceNumberSearch = null,
            string? partNumberSearch = null,
            string? partNameSearch = null
        );

        /// <summary>
        /// Gets the total count of InventoryTransaction entities matching the specified filter criteria.
        /// Parameters should mirror GetFilteredTransactionsPagedAsync for accurate counts.
        /// </summary>
        Task<int> GetTotalFilteredTransactionsCountAsync(
            Expression<Func<InventoryTransaction, bool>>? filterPredicate = null,
            int? partIdFilter = null,
            int? userIdFilter = null,
            Core.Enums.ModelEnum.InventoryTransactionType? transactionTypeFilter = null,
            DateTime? dateFromFilter = null,
            DateTime? dateToFilter = null, 
            string? referenceNumberSearch = null,
            string? partNumberSearch = null,
            string? partNameSearch = null
        );

        /// <summary>
        /// Gets the most recent transaction for a specific part.
        /// Useful for determining the last known stock level if Part.CurrentStock wasn't directly available.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <returns>The most recent InventoryTransaction for the part, or null if none exist.</returns>
        Task<InventoryTransaction?> GetLatestTransactionForPartAsync(int partId);
    }
}
