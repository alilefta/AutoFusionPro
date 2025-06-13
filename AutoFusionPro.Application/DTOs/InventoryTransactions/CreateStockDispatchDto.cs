using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.InventoryTransactions
{
    /// <summary>
    /// DTO for recording a stock dispatch (e.g., for a sale or internal use).
    /// </summary>
    public record CreateStockDispatchDto(
    [Range(1, int.MaxValue, ErrorMessage = "Part ID is required.")]
        int PartId,

        [Range(1, int.MaxValue, ErrorMessage = "Quantity dispatched must be positive.")]
        int QuantityDispatched, // This will be treated as a negative change to stock

        [Required(AllowEmptyStrings = false, ErrorMessage = "A reference (e.g., Sales Order Number, Internal Requisition) is required.")]
    [StringLength(50)]
    string Reference,

        [Range(1, int.MaxValue, ErrorMessage = "User ID performing the transaction is required.")]
        int UserId,

        [StringLength(500)]
        string? Notes,

        int? SalesOrderId = null, // Optional link to a specific Sales Order
        int? InvoiceId = null    // Optional link to a specific Invoice
    );
}

