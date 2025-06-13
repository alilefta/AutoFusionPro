using AutoFusionPro.Core.Enums.ModelEnum;

namespace AutoFusionPro.Application.DTOs.InventoryTransactions
{
    /// <summary>
    /// DTO for displaying details of an inventory transaction.
    /// </summary>
    public record InventoryTransactionDto(
        int Id,
        DateTime TransactionDate,
        int PartId,
        string PartNumber,          // For display
        string PartName,            // For display
        int PreviousQuantity,
        int QuantityChanged,        // The actual amount added or removed (e.g., +10 or -5)
        int NewQuantity,
        InventoryTransactionType TransactionType,
        string TransactionTypeDisplay, // User-friendly display of the type
        string? ReferenceNumber,    // e.g., PO Number, SO Number, Adjustment ID
        string? Notes,
        int UserId,
        string UserName,            // For display
        int? OrderId,
        int? InvoiceId,
        int? PurchaseOrderId        // Renamed from PurchaseId for clarity if it refers to PurchaseOrder
    );
}
