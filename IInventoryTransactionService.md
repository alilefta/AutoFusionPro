# Documentation: IInventoryTransactionService Interface

**Location:** AutoFusionPro.Application/Interfaces/DataServices/IInventoryTransactionService.cs

This interface defines a set of operations for managing inventory transactions and stock levels within the AutoFusionPro application. It offers methods to record stock receipts, dispatches, adjustments, returns, and to retrieve transaction records based on various criteria.

## Overview

The `IInventoryTransactionService` interface provides contract methods to handle inventory stock movements and maintain accurate stock levels for parts. Its primary focus is on modifying stock quantities while recording the reasons behind these changes for auditability and traceability. This is crucial for a streamlined inventory management system ensuring that stock levels reflect real-world transactions such as receiving new parts, selling parts, adjusting stock for corrections, and processing returns.

## Interface Methods

### ReceiveStockAsync

**Purpose:** To record incoming stock for a part, such as when new items are purchased or initialized as inventory. This method increases the part's current stock count.

**Parameters:** `CreateStockReceiptDto receiptDto` – Contains the data necessary to describe the stock receipt (e.g., part identifier, quantity, date, and any additional details).

**Returns:** A task returning an `InventoryTransactionDto`, representing the newly created inventory transaction record.

**Exceptions:** Throws `ValidationException` if the input data is invalid and `ServiceException` for issues such as missing parts or database errors.

**Usage Example:** When a shipment of parts arrives from a supplier, calling this method updates stock quantities and logs this receipt event.

### DispatchStockAsync

**Purpose:** To record outgoing stock when parts are dispatched. This could be for sales, internal usage, or wastage. The method decreases the current stock count accordingly.

**Parameters:** `CreateStockDispatchDto dispatchDto` – Holds details about the dispatch event, like which part is shipped, the quantity, and the reason.

**Returns:** A task yielding an `InventoryTransactionDto` for the dispatched stock record.

**Exceptions:** Throws `ValidationException` for invalid inputs and `ServiceException` if parts are not found, stock is insufficient, or other errors occur.

**Usage Example:** When a customer order is fulfilled, this method logs the reduction in stock due to the shipment.

### AdjustStockAsync

**Purpose:** To manually adjust a part's stock quantity to a new specified value. This is useful for stock corrections discovered during audits or physical counts.

**Parameters:** `CreateStockAdjustmentDto adjustmentDto` – Contains the adjustment details, notably the new stock quantity.

**Returns:** A task returning an `InventoryTransactionDto` reflecting the adjustment transaction.

**Exceptions:** May throw `ValidationException` for invalid data or `ServiceException` when the part is not found or database issues occur.

**Usage Example:** If a physical stock count reveals discrepancies, you can apply the corrected stock number using this method.

### ProcessReturnAsync

**Purpose:** To log a stock return transaction. This can represent a customer returning stock (increasing inventory) or stock returned to a supplier (decreasing inventory).

**Parameters:** `CreateStockReturnDto returnDto` – Includes details of the return, such as the part, quantity, and return reason.

**Returns:** Task with the newly created `InventoryTransactionDto`.

**Exceptions:** Validation and service exceptions are handled similar to other transaction methods.

**Usage Example:** Use this method to record returns, keeping inventory balanced and providing audit trails.

### GetFilteredTransactionsAsync

**Purpose:** To retrieve a paginated list of inventory transactions filtered by various criteria, such as date ranges, part IDs, or transaction types.

**Parameters:**

- `InventoryTransactionFilterCriteriaDto filterCriteria` – Filter parameters.
- `int pageNumber` – Page index, starting at 1.
- `int pageSize` – Number of transactions per page.

**Returns:** Task yielding a `PagedResult<InventoryTransactionDto>` containing the filtered transactions and total count.

**Usage Example:** When reviewing transaction history with filters, such as all receipts for a specific timeframe, this method supports efficient retrieval.

### GetTransactionsForPartAsync

**Purpose:** To obtain all inventory transactions associated with a particular part, sorted by the most recent first.

**Parameters:** `int partId` – Unique identifier of the part.

**Returns:** A task returning a collection of `InventoryTransactionDto` for that part.

**Usage Example:** Useful for tracing the transaction history of a specific inventory item.

### GetTransactionByIdAsync

**Purpose:** To fetch details of a single inventory transaction by its unique ID.

**Parameters:** `int transactionId` – The identifier of the transaction.

**Returns:** An `InventoryTransactionDto` if found; otherwise, `null`.

**Usage Example:** When you need detailed information about a specific transaction, for audit or troubleshooting.

## Additional Notes

The interface comments mention that methods to directly query current stock levels are located in the `IPartService`, reflecting a clean separation of concerns: `IInventoryTransactionService` focuses strictly on recording changes and reasons, while queries for current state are handled elsewhere.

There is also a placeholder for potential future expansion to support stock transfers between locations, indicating flexible design anticipating evolving requirements.

*End of documentation for* `IInventoryTransactionService`*.*