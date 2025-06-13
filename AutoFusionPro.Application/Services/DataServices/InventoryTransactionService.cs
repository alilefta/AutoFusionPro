using AutoFusionPro.Application.DTOs.InventoryTransactions;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Enums.DTOEnums.Inventory;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Models;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class InventoryTransactionService : IInventoryTransactionService
    {

        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InventoryTransactionService> _logger;
        // Add Validators 
        private readonly IValidator<CreateStockReceiptDto> _createStockReceiptValidator; // Not implemented
        private readonly IValidator<CreateStockDispatchDto> _createStockDispatchValidator; // Not implemented

        private readonly IValidator<CreateStockAdjustmentDto> _createStockAdjustmentValidator;
        private readonly IValidator<CreateStockReturnDto> _createStockReturnValidator;

        #endregion

        public InventoryTransactionService(
           IUnitOfWork unitOfWork,
           ILogger<InventoryTransactionService> logger,
           IValidator<CreateStockReceiptDto> createStockReceiptValidator,
            IValidator<CreateStockDispatchDto> createStockDispatchValidator,
            IValidator<CreateStockAdjustmentDto> createStockAdjustmentValidator,
            IValidator<CreateStockReturnDto> createStockReturnValidator
        // ... inject other validators later ...
           )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createStockReceiptValidator = createStockReceiptValidator ?? throw new ArgumentNullException(nameof(createStockReceiptValidator));
            _createStockDispatchValidator = createStockDispatchValidator ?? throw new ArgumentNullException(nameof(createStockDispatchValidator));

            _createStockAdjustmentValidator = createStockAdjustmentValidator ?? throw new ArgumentNullException(nameof(createStockAdjustmentValidator));
            _createStockReturnValidator = createStockReturnValidator ?? throw new ArgumentNullException(nameof(createStockReturnValidator));
        }


        /// <summary>
        /// Records the receipt of stock for a part (e.g., from a purchase, initial stock).
        /// This operation increases the part's CurrentStock.
        /// </summary>
        public async Task<InventoryTransactionDto> ReceiveStockAsync(CreateStockReceiptDto receiptDto)
        {
            ArgumentNullException.ThrowIfNull(receiptDto, nameof(receiptDto));

            _logger.LogInformation("Attempting to receive stock: PartId={PartId}, Quantity={Quantity}, Ref='{Reference}'",
                receiptDto.PartId, receiptDto.QuantityReceived, receiptDto.Reference);

            // 1. Validate DTO
            var validationResult = await _createStockReceiptValidator.ValidateAsync(receiptDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateStockReceiptDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Begin Transaction (crucial for atomicity)
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Fetch the Part
                var part = await _unitOfWork.Parts.GetByIdAsync(receiptDto.PartId);
                if (part == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // Rollback before throwing
                    string msg = $"Part with ID {receiptDto.PartId} not found. Cannot receive stock.";
                    _logger.LogWarning(msg);
                    throw new ServiceException(msg); // Or custom NotFoundException
                }


                int quantityReceivedInStockingUoM = receiptDto.QuantityReceived; // Assume this is the default

                if (part.PurchaseUnitOfMeasureId.HasValue && part.PurchaseUnitOfMeasureId.Value != part.StockingUnitOfMeasureId)
                {
                    if (!part.PurchaseConversionFactor.HasValue || part.PurchaseConversionFactor.Value <= 0)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        string errMsg = $"Part ID {part.Id} has a different Purchase UoM but no valid Purchase Conversion Factor defined.";
                        _logger.LogError(errMsg);
                        throw new ServiceException(errMsg);
                    }
                    quantityReceivedInStockingUoM = (int)(receiptDto.QuantityReceived * part.PurchaseConversionFactor.Value);
                    _logger.LogInformation("Converted {QtyReceived} {PurchaseUoMSymbol} to {QtyStockingUoM} {StockingUoMSymbol} for PartId {PartId}.",
                       receiptDto.QuantityReceived, part.PurchaseUnitOfMeasure?.Symbol ?? "PurchaseUoM",
                       quantityReceivedInStockingUoM, part.StockingUnitOfMeasure.Symbol, part.Id);
                }
                else if (!part.PurchaseUnitOfMeasureId.HasValue) // No purchase UoM defined, assume receipt is in stocking UoM
                {
                    _logger.LogInformation("No Purchase UoM defined for PartId {PartId}. Assuming QuantityReceived is in Stocking UoM ({StockingUoMSymbol}).",
                       part.Id, part.StockingUnitOfMeasure.Symbol);
                }

                // 4. Prepare InventoryTransaction
                int previousQuantity = part.CurrentStock;
                int quantityChanged = quantityReceivedInStockingUoM; // This is now in stocking UoM
                int newQuantity = previousQuantity + quantityChanged;

                var transaction = new InventoryTransaction
                {
                    TransactionDate = DateTime.UtcNow,
                    PartId = receiptDto.PartId,
                    PreviousQuantity = previousQuantity,
                    Quantity = quantityChanged, // Stored in Stocking UoM
                    NewQuantity = newQuantity,       // In Stocking UoM
                    TransactionType = receiptDto.PurchaseOrderId.HasValue ? InventoryTransactionType.Purchase : InventoryTransactionType.InitialStock, // Or determine by another field if more types of receipt
                    ReferenceNumber = receiptDto.Reference.Trim(),
                    Notes = string.IsNullOrWhiteSpace(receiptDto.Notes) ? null : receiptDto.Notes.Trim(),
                    UserId = receiptDto.UserId,
                    PurchaseId = receiptDto.PurchaseOrderId // Corrected field name
                    // OrderId and InvoiceId would typically be null for a stock receipt unless it's a sales return
                };

                // 5. Update Part's Stock
                part.CurrentStock = newQuantity; // CurrentStock is always in Stocking UoM
                part.LastRestockDate = transaction.TransactionDate; // Update last restock date
                // part.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext override

                // 6. Add Transaction and Save Part (EF Core tracks 'part' changes)
                await _unitOfWork.InventoryTransactions.AddAsync(transaction);
                // No explicit _unitOfWork.Parts.Update(part) needed if 'part' is tracked

                // 7. Save All Changes within the Transaction
                await _unitOfWork.SaveChangesAsync();

                // 8. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Stock received successfully for PartId={PartId}. TransactionId={TransactionId}, NewStock={NewStock}",
                    receiptDto.PartId, transaction.Id, newQuantity);

                // 9. Map to DTO and return (fetch with details for DTO)
                var createdTransactionEntity = await _unitOfWork.InventoryTransactions.GetByIdWithDetailsAsync(transaction.Id);
                if (createdTransactionEntity == null) // Should not happen
                {
                    _logger.LogError("Critical: Failed to re-fetch InventoryTransaction with ID {TransactionId} after creation.", transaction.Id);
                    throw new ServiceException("Stock receipt was recorded, but its details could not be immediately retrieved.");
                }
                return MapTransactionToDto(createdTransactionEntity);
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; } // Re-throw validation exceptions
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error occurred while receiving stock for PartId {PartId}.", receiptDto.PartId);
                throw new ServiceException("A database error occurred while receiving stock.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An unexpected error occurred while receiving stock for PartId {PartId}.", receiptDto.PartId);
                throw new ServiceException($"An error occurred while receiving stock for PartId {receiptDto.PartId}.", ex);
            }
        }

        /// <summary>
        /// Records the dispatch of stock for a part (e.g., for a sale, internal usage, wastage).
        /// This operation decreases the part's CurrentStock.
        /// </summary>
        public async Task<InventoryTransactionDto> DispatchStockAsync(CreateStockDispatchDto dispatchDto)
        {
            ArgumentNullException.ThrowIfNull(dispatchDto, nameof(dispatchDto));

            _logger.LogInformation("Attempting to dispatch stock: PartId={PartId}, Quantity={Quantity}, Ref='{Reference}'",
                dispatchDto.PartId, dispatchDto.QuantityDispatched, dispatchDto.Reference);

            // 1. Validate DTO
            var validationResult = await _createStockDispatchValidator.ValidateAsync(dispatchDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateStockDispatchDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Begin Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Fetch the Part
                var part = await _unitOfWork.Parts.GetByIdAsync(dispatchDto.PartId);
                if (part == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    string msg = $"Part with ID {dispatchDto.PartId} not found. Cannot dispatch stock.";
                    _logger.LogWarning(msg);
                    throw new ServiceException(msg);
                }


                int quantityToDispatchInStockingUoM = dispatchDto.QuantityDispatched; // Assume this is the default

                if (part.SalesUnitOfMeasureId.HasValue && part.SalesUnitOfMeasureId.Value != part.StockingUnitOfMeasureId)
                {
                    if (!part.SalesConversionFactor.HasValue || part.SalesConversionFactor.Value <= 0)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        string errMsg = $"Part ID {part.Id} has a different Sales UoM but no valid Sales Conversion Factor defined.";
                        _logger.LogError(errMsg);
                        throw new ServiceException(errMsg);
                    }
                    quantityToDispatchInStockingUoM = (int)(dispatchDto.QuantityDispatched * part.SalesConversionFactor.Value);
                    _logger.LogInformation("Converted {QtyDispatched} {SalesUoMSymbol} to {QtyStockingUoM} {StockingUoMSymbol} for PartId {PartId}.",
                        dispatchDto.QuantityDispatched, part.SalesUnitOfMeasure?.Symbol ?? "SalesUoM",
                        quantityToDispatchInStockingUoM, part.StockingUnitOfMeasure.Symbol, part.Id);
                }
                else if (!part.SalesUnitOfMeasureId.HasValue)
                {
                    _logger.LogInformation("No Sales UoM defined for PartId {PartId}. Assuming QuantityDispatched is in Stocking UoM ({StockingUoMSymbol}).",
                        part.Id, part.StockingUnitOfMeasure.Symbol);
                }

                // 4. Business Rule: Check for sufficient stock
                if (part.CurrentStock < quantityToDispatchInStockingUoM)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    string insufficientMsg = $"Insufficient stock for PartId {dispatchDto.PartId}. Available: {part.CurrentStock}, Requested: {quantityToDispatchInStockingUoM}.";
                    _logger.LogWarning(insufficientMsg);
                    // Consider a more specific exception type for insufficient stock if UI needs to handle it differently
                    throw new ServiceException(insufficientMsg);
                }

                // 5. Prepare InventoryTransaction
                int previousQuantity = part.CurrentStock;
                int quantityChanged = -quantityToDispatchInStockingUoM; // Negative
                int newQuantity = previousQuantity + quantityChanged;

                var transaction = new InventoryTransaction
                {
                    TransactionDate = DateTime.UtcNow,
                    PartId = dispatchDto.PartId,
                    PreviousQuantity = previousQuantity,
                    Quantity = quantityChanged, // Stored in Stocking UoM
                    NewQuantity = newQuantity,       // In Stocking UoM
                    // Determine TransactionType based on context, e.g., if SalesOrderId is present
                    TransactionType = dispatchDto.SalesOrderId.HasValue ? InventoryTransactionType.Sale : InventoryTransactionType.Adjustment, // Or other type like "InternalUse", "Wastage"
                    ReferenceNumber = dispatchDto.Reference.Trim(),
                    Notes = string.IsNullOrWhiteSpace(dispatchDto.Notes) ? null : dispatchDto.Notes.Trim(),
                    UserId = dispatchDto.UserId,
                    OrderId = dispatchDto.SalesOrderId, // Using OrderId for SalesOrder
                    InvoiceId = dispatchDto.InvoiceId
                };

                // 6. Update Part's Stock
                part.CurrentStock = newQuantity;
                // part.ModifiedAt = DateTime.UtcNow; // Or handled by DbContext override
                // LastRestockDate is typically not updated on dispatch

                // 7. Add Transaction and Save Part
                await _unitOfWork.InventoryTransactions.AddAsync(transaction);

                // 8. Save All Changes
                await _unitOfWork.SaveChangesAsync();

                // 9. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Stock dispatched successfully for PartId={PartId}. TransactionId={TransactionId}, NewStock={NewStock}",
                    dispatchDto.PartId, transaction.Id, newQuantity);

                // 10. Map to DTO and return
                var createdTransactionEntity = await _unitOfWork.InventoryTransactions.GetByIdWithDetailsAsync(transaction.Id);
                if (createdTransactionEntity == null)
                {
                    _logger.LogError("Critical: Failed to re-fetch InventoryTransaction with ID {TransactionId} after dispatch.", transaction.Id);
                    throw new ServiceException("Stock dispatch was recorded, but its details could not be immediately retrieved.");
                }
                return MapTransactionToDto(createdTransactionEntity);
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (ServiceException) // Catch specific service exceptions like insufficient stock or part not found
            {
                await _unitOfWork.RollbackTransactionAsync(); // Ensure rollback for these known business errors too
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error occurred while dispatching stock for PartId {PartId}.", dispatchDto.PartId);
                throw new ServiceException("A database error occurred while dispatching stock.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An unexpected error occurred while dispatching stock for PartId {PartId}.", dispatchDto.PartId);
                throw new ServiceException($"An error occurred while dispatching stock for PartId {dispatchDto.PartId}.", ex);
            }

        }

        /// <summary>
        /// Records a manual adjustment to a part's stock level.
        /// The user provides the new actual quantity on hand.
        /// </summary>
        public async Task<InventoryTransactionDto> AdjustStockAsync(CreateStockAdjustmentDto adjustmentDto)
        {
            ArgumentNullException.ThrowIfNull(adjustmentDto, nameof(adjustmentDto));

            _logger.LogInformation("Attempting to adjust stock: PartId={PartId}, NewActualQty={NewQty}, Reason='{Reason}'",
                adjustmentDto.PartId, adjustmentDto.NewActualQuantityOnHand, adjustmentDto.Reason);

            // 1. Validate DTO
            var validationResult = await _createStockAdjustmentValidator.ValidateAsync(adjustmentDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateStockAdjustmentDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Begin Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Fetch the Part
                var part = await _unitOfWork.Parts.GetByIdAsync(adjustmentDto.PartId);
                if (part == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    string msg = $"Part with ID {adjustmentDto.PartId} not found. Cannot adjust stock.";
                    _logger.LogWarning(msg);
                    throw new ServiceException(msg);
                }

                // 4. Prepare InventoryTransaction
                int previousQuantity = part.CurrentStock;
                int newQuantity = adjustmentDto.NewActualQuantityOnHand;
                int quantityChanged = newQuantity - previousQuantity; // Can be positive or negative

                var transaction = new InventoryTransaction
                {
                    TransactionDate = DateTime.UtcNow,
                    PartId = adjustmentDto.PartId,
                    PreviousQuantity = previousQuantity,
                    Quantity = quantityChanged, // The amount that changed
                    NewQuantity = newQuantity,
                    TransactionType = InventoryTransactionType.Adjustment,
                    ReferenceNumber = adjustmentDto.Reason.Trim(), // Use Reason as reference for adjustment
                    Notes = string.IsNullOrWhiteSpace(adjustmentDto.Notes) ? null : adjustmentDto.Notes.Trim(),
                    UserId = adjustmentDto.UserId
                };

                // 5. Update Part's Stock
                part.CurrentStock = newQuantity;
                // LastRestockDate might not be relevant for all adjustments, or could be set if stock increases
                if (quantityChanged > 0) part.LastRestockDate = transaction.TransactionDate;

                // 6. Add Transaction and Save Part
                await _unitOfWork.InventoryTransactions.AddAsync(transaction);

                // 7. Save All Changes
                await _unitOfWork.SaveChangesAsync();

                // 8. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Stock adjusted successfully for PartId={PartId}. TransactionId={TransactionId}, NewStock={NewStock}",
                    adjustmentDto.PartId, transaction.Id, newQuantity);

                // 9. Map to DTO and return
                var createdTransactionEntity = await _unitOfWork.InventoryTransactions.GetByIdWithDetailsAsync(transaction.Id);
                if (createdTransactionEntity == null)
                {
                    _logger.LogError("Critical: Failed to re-fetch InventoryTransaction ID {TransactionId} after adjustment.", transaction.Id);
                    throw new ServiceException("Stock adjustment was recorded, but its details could not be immediately retrieved.");
                }
                return MapTransactionToDto(createdTransactionEntity);
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (ServiceException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error occurred while adjusting stock for PartId {PartId}.", adjustmentDto.PartId);
                throw new ServiceException("A database error occurred while adjusting stock.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An unexpected error occurred while adjusting stock for PartId {PartId}.", adjustmentDto.PartId);
                throw new ServiceException($"An error occurred while adjusting stock for PartId {adjustmentDto.PartId}.", ex);
            }
        }

        /// <summary>
        /// Records a stock return.
        /// </summary>
        public async Task<InventoryTransactionDto> ProcessReturnAsync(CreateStockReturnDto returnDto)
        {
            ArgumentNullException.ThrowIfNull(returnDto, nameof(returnDto));

            _logger.LogInformation("Processing stock return: PartId={PartId}, QuantityReturned={Quantity}, ReturnedUoMId={UoMId}, Type={ReturnType}, Ref='{Reference}'",
                returnDto.PartId, returnDto.QuantityReturned, returnDto.ReturnedUnitOfMeasureId, returnDto.ReturnType, returnDto.Reference);

            // 1. Validate DTO
            var validationResult = await _createStockReturnValidator.ValidateAsync(returnDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateStockReturnDto: {Errors}", validationResult.ToString("~"));
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Begin Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Fetch the Part with necessary UoM details
                //    The repository method needs to include StockingUnitOfMeasure, SalesUnitOfMeasure, PurchaseUnitOfMeasure
                //    Let's assume GetByIdAsync on PartRepository can be augmented or a specific method exists.
                //    For this example, I'll assume GetByIdAsync is modified or you have GetByIdWithUoMDetailsAsync.
                var part = await _unitOfWork.Parts.GetByIdAsync(returnDto.PartId, includeUoMs: true); // Assuming includeUoMs flag
                if (part == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    string msg = $"Part with ID {returnDto.PartId} not found. Cannot process return.";
                    _logger.LogWarning(msg);
                    throw new ServiceException(msg);
                }

                // Fetch the UoM in which the quantity was returned
                UnitOfMeasure? returnedUom = await _unitOfWork.UnitOfMeasures.GetByIdAsync(returnDto.ReturnedUnitOfMeasureId);
                if (returnedUom == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    string msg = $"Returned Unit of Measure with ID {returnDto.ReturnedUnitOfMeasureId} not found. Cannot process return.";
                    _logger.LogWarning(msg);
                    throw new ServiceException(msg);
                }

                // Ensure the part has a stocking UoM (should be enforced by Part creation)
                if (part.StockingUnitOfMeasure == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    string msg = $"Part ID {part.Id} does not have a primary Stocking Unit of Measure defined. Cannot process return.";
                    _logger.LogError(msg);
                    throw new ServiceException(msg);
                }

                // 4. Convert QuantityReturned to Stocking Unit of Measure
                int quantityReturnedInStockingUoM = returnDto.QuantityReturned; // Default if UoMs are the same

                if (returnDto.ReturnedUnitOfMeasureId != part.StockingUnitOfMeasureId)
                {
                    decimal conversionFactor = 1;
                    string sourceUomSymbolForLog = returnedUom.Symbol;

                    // Determine the correct conversion factor based on the return type and the part's defined UoMs
                    if (returnDto.ReturnType == StockReturnType.CustomerReturn &&
                        part.SalesUnitOfMeasureId == returnDto.ReturnedUnitOfMeasureId &&
                        part.SalesConversionFactor.HasValue && part.SalesConversionFactor.Value > 0)
                    {
                        conversionFactor = part.SalesConversionFactor.Value;
                        sourceUomSymbolForLog = part.SalesUnitOfMeasure?.Symbol ?? sourceUomSymbolForLog;
                    }
                    else if (returnDto.ReturnType == StockReturnType.ReturnToSupplier &&
                             part.PurchaseUnitOfMeasureId == returnDto.ReturnedUnitOfMeasureId &&
                             part.PurchaseConversionFactor.HasValue && part.PurchaseConversionFactor.Value > 0)
                    {
                        conversionFactor = part.PurchaseConversionFactor.Value;
                        sourceUomSymbolForLog = part.PurchaseUnitOfMeasure?.Symbol ?? sourceUomSymbolForLog;
                    }
                    else
                    {
                        // If not a standard Sales/Purchase UoM for this part, and not the Stocking UoM, it's an error
                        // unless you have a more advanced UoM conversion system.
                        await _unitOfWork.RollbackTransactionAsync();
                        string errMsg = $"Cannot convert returned quantity for Part ID {part.Id}. " +
                                        $"Returned Unit of Measure '{returnedUom.Name}' is not the part's Stocking UoM, " +
                                        $"nor does it match its default Sales or Purchase UoM with a defined conversion factor.";
                        _logger.LogError(errMsg);
                        throw new ServiceException(errMsg);
                    }

                    quantityReturnedInStockingUoM = (int)Math.Round(returnDto.QuantityReturned * conversionFactor, MidpointRounding.AwayFromZero); // Or other rounding
                    _logger.LogInformation("Converted {QtyReturned} {ReturnedUoMSymbol} to {QtyStockingUoM} {StockingUoMSymbol} for PartId {PartId} for return.",
                        returnDto.QuantityReturned, sourceUomSymbolForLog,
                        quantityReturnedInStockingUoM, part.StockingUnitOfMeasure.Symbol, part.Id);
                }


                // 5. Determine actual stock change and update part
                int previousQuantity = part.CurrentStock;
                int quantityChangedInStockingUoM; // This is the value that goes into InventoryTransaction.Quantity
                InventoryTransactionType transactionType = InventoryTransactionType.Return; // General Return type

                if (returnDto.ReturnType == StockReturnType.CustomerReturn)
                {
                    quantityChangedInStockingUoM = quantityReturnedInStockingUoM; // Positive, stock increases
                    part.LastRestockDate = DateTime.UtcNow; // Customer return can be considered a form of restock
                }
                else // StockReturnType.ReturnToSupplier
                {
                    quantityChangedInStockingUoM = -quantityReturnedInStockingUoM; // Negative, stock decreases
                                                                                   // Business rule: Check for sufficient stock if returning to supplier (in stocking UoM)
                    if (part.CurrentStock < quantityReturnedInStockingUoM) // Compare with positive value of what's being removed
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        string insufficientMsg = $"Insufficient stock for PartId {returnDto.PartId} to return to supplier. " +
                                                 $"Available (Stocking UoM): {part.CurrentStock}, Requested to Return (Stocking UoM): {quantityReturnedInStockingUoM}.";
                        _logger.LogWarning(insufficientMsg);
                        throw new ServiceException(insufficientMsg);
                    }
                }
                int newQuantity = previousQuantity + quantityChangedInStockingUoM;

                // 6. Prepare InventoryTransaction
                var transaction = new InventoryTransaction
                {
                    TransactionDate = DateTime.UtcNow,
                    PartId = returnDto.PartId,
                    PreviousQuantity = previousQuantity,         // In Stocking UoM
                    Quantity = quantityChangedInStockingUoM,   // The CHANGE in Stocking UoM
                    NewQuantity = newQuantity,               // In Stocking UoM
                    TransactionType = transactionType,
                    ReferenceNumber = returnDto.Reference.Trim(),
                    Notes = string.IsNullOrWhiteSpace(returnDto.Notes) ? null : returnDto.Notes.Trim(),
                    UserId = returnDto.UserId,
                    OrderId = returnDto.OriginalSalesOrderId,
                    PurchaseId = returnDto.OriginalPurchaseOrderId
                };

                // 7. Update Part's Stock
                part.CurrentStock = newQuantity; // CurrentStock is always in Stocking UoM

                // 8. Add Transaction and Save Part
                await _unitOfWork.InventoryTransactions.AddAsync(transaction);

                // 9. Save All Changes
                await _unitOfWork.SaveChangesAsync();

                // 10. Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Stock return processed successfully for PartId={PartId}. TransactionId={TransactionId}, NewStock={NewStock} (Stocking UoM).",
                    returnDto.PartId, transaction.Id, newQuantity);

                // 11. Map to DTO and return
                var createdTransactionEntity = await _unitOfWork.InventoryTransactions.GetByIdWithDetailsAsync(transaction.Id);
                if (createdTransactionEntity == null)
                {
                    _logger.LogError("Critical: Failed to re-fetch InventoryTransaction ID {TransactionId} after processing return.", transaction.Id);
                    throw new ServiceException("Stock return was processed, but its details could not be immediately retrieved.");
                }
                return MapTransactionToDto(createdTransactionEntity);
            }
            catch (ValidationException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (ServiceException) { await _unitOfWork.RollbackTransactionAsync(); throw; }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(dbEx, "Database error occurred while processing stock return for PartId {PartId}.", returnDto.PartId);
                throw new ServiceException("A database error occurred while processing the stock return.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An unexpected error occurred while processing stock return for PartId {PartId}.", returnDto.PartId);
                throw new ServiceException($"An error occurred while processing stock return for PartId {returnDto.PartId}.", ex);
            }
        }

        /// <summary>
        /// Gets a paginated list of inventory transactions based on filter criteria.
        /// </summary>
        public async Task<PagedResult<InventoryTransactionDto>> GetFilteredTransactionsAsync(
            InventoryTransactionFilterCriteriaDto filterCriteria,
            int pageNumber,
            int pageSize)
        {
            ArgumentNullException.ThrowIfNull(filterCriteria, nameof(filterCriteria));
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;    // Default page size
            if (pageSize > 200) pageSize = 200;  // Max page size safeguard

            _logger.LogInformation("Attempting to retrieve filtered inventory transactions. Page: {Page}, Size: {Size}, Criteria: {@Criteria}",
                pageNumber, pageSize, filterCriteria);

            try
            {
                // The repository's GetFilteredTransactionsPagedAsync handles the actual filtering based on these parameters.
                // It should also handle including Part and User details.
                var transactionEntities = await _unitOfWork.InventoryTransactions.GetFilteredTransactionsPagedAsync(
                                             pageNumber,
                                             pageSize,
                                             filterPredicate: null,
                                             partIdFilter: filterCriteria.PartId,
                                             userIdFilter: filterCriteria.UserId,
                                             transactionTypeFilter: filterCriteria.TransactionType,
                                             dateFromFilter: filterCriteria.DateFrom,
                                             dateToFilter: filterCriteria.DateTo,
                                             referenceNumberSearch: filterCriteria.ReferenceNumberSearch,
                                             partNumberSearch: filterCriteria.PartNumberSearch, // New
                                             partNameSearch: filterCriteria.PartNameSearch     // New
                                         );

                var totalCount = await _unitOfWork.InventoryTransactions.GetTotalFilteredTransactionsCountAsync(
                                            filterPredicate: null,
                                            partIdFilter: filterCriteria.PartId,
                                            userIdFilter: filterCriteria.UserId,
                                            transactionTypeFilter: filterCriteria.TransactionType,
                                            dateFromFilter: filterCriteria.DateFrom,
                                            dateToFilter: filterCriteria.DateTo,
                                            referenceNumberSearch: filterCriteria.ReferenceNumberSearch,
                                            partNumberSearch: filterCriteria.PartNumberSearch, // New
                                            partNameSearch: filterCriteria.PartNameSearch     // New
                                        );

                // Map entities to DTOs
                var transactionDtos = transactionEntities.Select(MapTransactionToDto).ToList();

                _logger.LogInformation("Successfully retrieved {RetrievedCount} inventory transactions for page {Page}. Total matching: {TotalCount}",
                    transactionDtos.Count, pageNumber, totalCount);

                return new PagedResult<InventoryTransactionDto>
                {
                    Items = transactionDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    // TotalPages can be calculated by PagedResult or the UI
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving filtered inventory transactions with criteria: {@Criteria}", filterCriteria);
                throw new ServiceException("Could not retrieve inventory transactions.", ex);
            }
        }

        /// <summary>
        /// Gets all inventory transactions for a specific part, ordered by date descending.
        /// </summary>
        public async Task<IEnumerable<InventoryTransactionDto>> GetTransactionsForPartAsync(int partId)
        {
            if (partId <= 0)
            {
                _logger.LogWarning("Attempted to get transactions for invalid Part ID: {PartId}", partId);
                return Enumerable.Empty<InventoryTransactionDto>(); // Or throw ArgumentException
            }

            _logger.LogInformation("Attempting to retrieve all transactions for Part ID: {PartId}", partId);
            try
            {
                // The repository method GetByPartIdAsync should include User details and order transactions.
                var transactionEntities = await _unitOfWork.InventoryTransactions.GetByPartIdAsync(partId);

                var dtos = transactionEntities.Select(MapTransactionToDto).ToList();
                _logger.LogInformation("Successfully retrieved {Count} transactions for Part ID {PartId}.", dtos.Count, partId);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving transactions for Part ID {PartId}.", partId);
                throw new ServiceException($"Could not retrieve transactions for Part ID {partId}.", ex);
            }
        }

        /// <summary>
        /// Gets a specific inventory transaction by its ID.
        /// </summary>
        public async Task<InventoryTransactionDto?> GetTransactionByIdAsync(int transactionId)
        {
            if (transactionId <= 0)
            {
                _logger.LogWarning("Attempted to get inventory transaction with invalid ID: {TransactionId}", transactionId);
                return null; // Or throw ArgumentOutOfRangeException depending on desired strictness
            }

            _logger.LogInformation("Attempting to retrieve inventory transaction with ID: {TransactionId}", transactionId);
            try
            {
                // Use the repository method that includes Part and User details
                var transactionEntity = await _unitOfWork.InventoryTransactions.GetByIdWithDetailsAsync(transactionId);

                if (transactionEntity == null)
                {
                    _logger.LogWarning("Inventory transaction with ID {TransactionId} not found.", transactionId);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved inventory transaction with ID {TransactionId} for PartId {PartId}.",
                    transactionId, transactionEntity.PartId);

                return MapTransactionToDto(transactionEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving inventory transaction with ID {TransactionId}.", transactionId);
                throw new ServiceException($"Could not retrieve inventory transaction with ID {transactionId}.", ex);
            }
        }


        #region Helpers

        // --- Private Mapping Helper for InventoryTransaction ---
        private InventoryTransactionDto MapTransactionToDto(InventoryTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            return new InventoryTransactionDto(
                transaction.Id,
                transaction.TransactionDate,
                transaction.PartId,
                transaction.Part?.PartNumber ?? "N/A", // Assumes Part is loaded
                transaction.Part?.Name ?? "N/A",       // Assumes Part is loaded
                transaction.PreviousQuantity,
                transaction.Quantity, // This is the QuantityChanged
                transaction.NewQuantity,
                transaction.TransactionType,
                transaction.TransactionType.ToString(), // Basic display, can be enhanced with localization
                transaction.ReferenceNumber,
                transaction.Notes,
                transaction.UserId,
                transaction.User?.Username ?? "N/A", // Assumes User is loaded (Username or FullName)
                transaction.OrderId,
                transaction.InvoiceId,
                transaction.PurchaseId // This was PurchaseId in your domain model
            );
        }

        #endregion
    }
}
