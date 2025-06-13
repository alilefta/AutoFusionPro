using AutoFusionPro.Application.DTOs.InventoryTransactions;
using AutoFusionPro.Core.Models;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    /// <summary>
    /// Defines application service operations for managing inventory transactions and stock levels.
    /// </summary>
    public interface IInventoryTransactionService
    {
        /// <summary>
        /// Records the receipt of stock for a part (e.g., from a purchase, initial stock).
        /// This operation increases the part's CurrentStock.
        /// </summary>
        /// <param name="receiptDto">Data for the stock receipt.</param>
        /// <returns>The DTO of the created inventory transaction.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid.</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown for other errors (e.g., part not found, database issues).</exception>
        Task<InventoryTransactionDto> ReceiveStockAsync(CreateStockReceiptDto receiptDto);

        /// <summary>
        /// Records the dispatch of stock for a part (e.g., for a sale, internal usage, wastage).
        /// This operation decreases the part's CurrentStock.
        /// </summary>
        /// <param name="dispatchDto">Data for the stock dispatch.</param>
        /// <returns>The DTO of the created inventory transaction.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid.</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown for other errors (e.g., part not found, insufficient stock, database issues).</exception>
        Task<InventoryTransactionDto> DispatchStockAsync(CreateStockDispatchDto dispatchDto);

        /// <summary>
        /// Records a manual adjustment to a part's stock level.
        /// The user provides the new actual quantity on hand.
        /// </summary>
        /// <param name="adjustmentDto">Data for the stock adjustment.</param>
        /// <returns>The DTO of the created inventory transaction.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid.</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown for other errors (e.g., part not found, database issues).</exception>
        Task<InventoryTransactionDto> AdjustStockAsync(CreateStockAdjustmentDto adjustmentDto);

        /// <summary>
        /// Records a stock return. This can be a customer return (increasing stock)
        /// or a return to a supplier (decreasing stock).
        /// </summary>
        /// <param name="returnDto">Data for the stock return.</param>
        /// <returns>The DTO of the created inventory transaction.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid.</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown for other errors.</exception>
        Task<InventoryTransactionDto> ProcessReturnAsync(CreateStockReturnDto returnDto);

        // Potentially other specific transaction types if needed:
        // Task<InventoryTransactionDto> TransferStockAsync(CreateStockTransferDto transferDto); // If transferring between locations

        /// <summary>
        /// Gets a paginated list of inventory transactions based on filter criteria.
        /// </summary>
        /// <param name="filterCriteria">DTO containing filter parameters.</param>
        /// <param name="pageNumber">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A PagedResult containing InventoryTransactionDto items and total count.</returns>
        Task<PagedResult<InventoryTransactionDto>> GetFilteredTransactionsAsync(
            InventoryTransactionFilterCriteriaDto filterCriteria,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Gets all inventory transactions for a specific part, ordered by date descending.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <returns>A collection of InventoryTransactionDto.</returns>
        Task<IEnumerable<InventoryTransactionDto>> GetTransactionsForPartAsync(int partId);

        /// <summary>
        /// Gets a specific inventory transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The ID of the inventory transaction.</param>
        /// <returns>The InventoryTransactionDto or null if not found.</returns>
        Task<InventoryTransactionDto?> GetTransactionByIdAsync(int transactionId);

        // Note: GetCurrentStockForPartAsync is on IPartService as it's a direct query on Part.
        // This service *modifies* stock and records *why*.
    }
}
