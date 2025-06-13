using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class InventoryTransactionRepository : Repository<InventoryTransaction, InventoryTransactionRepository>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(ApplicationDbContext context, ILogger<InventoryTransactionRepository> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Gets a single inventory transaction by its ID, including related Part and User details.
        /// </summary>
        public async Task<InventoryTransaction?> GetByIdWithDetailsAsync(int transactionId)
        {
            if (transactionId <= 0)
            {
                _logger.LogWarning("Attempted to get inventory transaction with invalid ID: {TransactionId}", transactionId);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve inventory transaction with details for ID: {TransactionId}", transactionId);
            try
            {
                return await _dbSet
                    .Include(t => t.Part)    // Include Part details
                        .ThenInclude(p => p.Category) // Optionally include Part's Category
                    .Include(t => t.User)    // Include User details
                    .FirstOrDefaultAsync(t => t.Id == transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory transaction with details for ID {TransactionId}.", transactionId);
                throw new RepositoryException($"Could not retrieve detailed inventory transaction with ID {transactionId}.", ex);
            }
        }

        /// <summary>
        /// Gets all inventory transactions for a specific part, ordered by TransactionDate descending.
        /// Includes related User details. Part details are known by partId.
        /// </summary>
        public async Task<IEnumerable<InventoryTransaction>> GetByPartIdAsync(int partId)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get inventory transactions with invalid PartID: {PartId}", partId);
                return Enumerable.Empty<InventoryTransaction>();
            }

            _logger.LogDebug("Attempting to retrieve inventory transactions for PartID: {PartId}", partId);
            try
            {
                return await _dbSet
                    .Include(t => t.User) // Include User details
                    .Where(t => t.PartId == partId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ThenByDescending(t => t.Id) // For stable sort if multiple transactions on same date/time
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory transactions for PartID {PartId}.", partId);
                throw new RepositoryException($"Could not retrieve inventory transactions for PartID {partId}.", ex);
            }
        }

        /// <summary>
        /// Gets a paginated list of InventoryTransaction entities based on filter criteria.
        /// Includes related Part and User details.
        /// </summary>
        public async Task<IEnumerable<InventoryTransaction>> GetFilteredTransactionsPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<InventoryTransaction, bool>>? filterPredicate = null,
            int? partIdFilter = null,
            int? userIdFilter = null,
            InventoryTransactionType? transactionTypeFilter = null,
            DateTime? dateFromFilter = null,
            DateTime? dateToFilter = null,
            string? referenceNumberSearch = null,           
            string? partNumberSearch = null,
            string? partNameSearch = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 200) pageSize = 200; // Max page size safeguard

            _logger.LogDebug("Getting filtered paged inventory transactions. Page: {Page}, Size: {Size}, PartId: {PartId}, UserId: {UserId}, Type: {Type}, DateFrom: {DateFrom}, DateTo: {DateTo}, Ref: '{Ref}'",
                pageNumber, pageSize, partIdFilter, userIdFilter, transactionTypeFilter, dateFromFilter, dateToFilter, referenceNumberSearch);

            try
            {
                IQueryable<InventoryTransaction> query = _dbSet
                    .Include(t => t.Part) // Always include Part for display (PartNumber, Name)
                    .Include(t => t.User); // Always include User for display (UserName)

                if (filterPredicate != null)
                {
                    query = query.Where(filterPredicate);
                }
                if (partIdFilter.HasValue && partIdFilter.Value > 0)
                {
                    query = query.Where(t => t.PartId == partIdFilter.Value);
                }
                if (userIdFilter.HasValue && userIdFilter.Value > 0)
                {
                    query = query.Where(t => t.UserId == userIdFilter.Value);
                }
                if (transactionTypeFilter.HasValue)
                {
                    query = query.Where(t => t.TransactionType == transactionTypeFilter.Value);
                }
                if (dateFromFilter.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= dateFromFilter.Value);
                }
                if (dateToFilter.HasValue)
                {
                    // Add 1 day and check for less than, to include the whole "dateToFilter" day
                    query = query.Where(t => t.TransactionDate < dateToFilter.Value.AddDays(1));
                }

                if (!string.IsNullOrWhiteSpace(referenceNumberSearch))
                {
                    string refSearchLower = referenceNumberSearch.Trim().ToLowerInvariant();
                    query = query.Where(t => t.ReferenceNumber != null && t.ReferenceNumber.ToLower().Contains(refSearchLower));
                }

                if (!string.IsNullOrWhiteSpace(partNumberSearch))
                {
                    query = query.Where(t => t.Part.PartNumber.Contains(partNumberSearch)); // Case sensitivity depends on DB
                }
                if (!string.IsNullOrWhiteSpace(partNameSearch))
                {
                    query = query.Where(t => t.Part.Name.Contains(partNameSearch));
                }

                // Apply stable ordering
                query = query.OrderByDescending(t => t.TransactionDate)
                             .ThenByDescending(t => t.Id);

                return await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged and filtered inventory transactions. Page: {Page}, Size: {Size}", pageNumber, pageSize);
                throw new RepositoryException("Could not retrieve paged inventory transactions.", ex);
            }
        }

        /// <summary>
        /// Gets the total count of InventoryTransaction entities matching the specified filter criteria.
        /// </summary>
        public async Task<int> GetTotalFilteredTransactionsCountAsync(
            Expression<Func<InventoryTransaction, bool>>? filterPredicate = null,
            int? partIdFilter = null,
            int? userIdFilter = null,
            InventoryTransactionType? transactionTypeFilter = null,
            DateTime? dateFromFilter = null,
            DateTime? dateToFilter = null, 
            string? referenceNumberSearch = null,
            string? partNumberSearch = null,
            string? partNameSearch = null)
        {
            _logger.LogDebug("Getting total filtered inventory transactions count. PartId: {PartId}, UserId: {UserId}, Type: {Type}, DateFrom: {DateFrom}, DateTo: {DateTo}, Ref: '{Ref}'",
                 partIdFilter, userIdFilter, transactionTypeFilter, dateFromFilter, dateToFilter, referenceNumberSearch);
            try
            {
                IQueryable<InventoryTransaction> query = _dbSet; // No includes needed for CountAsync

                if (filterPredicate != null)
                {
                    query = query.Where(filterPredicate);
                }
                if (partIdFilter.HasValue && partIdFilter.Value > 0)
                {
                    query = query.Where(t => t.PartId == partIdFilter.Value);
                }
                if (userIdFilter.HasValue && userIdFilter.Value > 0)
                {
                    query = query.Where(t => t.UserId == userIdFilter.Value);
                }
                if (transactionTypeFilter.HasValue)
                {
                    query = query.Where(t => t.TransactionType == transactionTypeFilter.Value);
                }
                if (dateFromFilter.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= dateFromFilter.Value);
                }
                if (dateToFilter.HasValue)
                {
                    query = query.Where(t => t.TransactionDate < dateToFilter.Value.AddDays(1));
                }

                if (!string.IsNullOrWhiteSpace(referenceNumberSearch))
                {
                    string refSearchLower = referenceNumberSearch.Trim().ToLowerInvariant();
                    query = query.Where(t => t.ReferenceNumber != null && t.ReferenceNumber.ToLower().Contains(refSearchLower));
                }

                if (!string.IsNullOrWhiteSpace(partNumberSearch))
                {
                    query = query.Where(t => t.Part.PartNumber.Contains(partNumberSearch)); // Case sensitivity depends on DB
                }
                if (!string.IsNullOrWhiteSpace(partNameSearch))
                {
                    query = query.Where(t => t.Part.Name.Contains(partNameSearch));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total filtered inventory transactions count.");
                throw new RepositoryException("Could not retrieve total inventory transactions count.", ex);
            }
        }

        /// <summary>
        /// Gets the most recent transaction for a specific part.
        /// </summary>
        public async Task<InventoryTransaction?> GetLatestTransactionForPartAsync(int partId)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get latest transaction with invalid PartID: {PartId}", partId);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve the latest transaction for PartID: {PartId}", partId);
            try
            {
                return await _dbSet
                    .Where(t => t.PartId == partId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ThenByDescending(t => t.Id) // Crucial for tie-breaking if multiple transactions at exact same time
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving the latest transaction for PartID {PartId}.", partId);
                throw new RepositoryException($"Could not retrieve the latest transaction for PartID {partId}.", ex);
            }
        }
    }
}
