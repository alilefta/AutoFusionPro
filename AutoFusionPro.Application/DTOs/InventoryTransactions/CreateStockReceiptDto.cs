using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.InventoryTransactions
{
    /// <summary>
    /// DTO for recording a stock receipt (e.g., from a purchase or initial stock).
    /// </summary>
    public record CreateStockReceiptDto(
    [Range(1, int.MaxValue, ErrorMessage = "Part ID is required.")]
        int PartId,

        [Range(1, int.MaxValue, ErrorMessage = "Quantity received must be positive.")]
        int QuantityReceived,

        [Required(AllowEmptyStrings = false, ErrorMessage = "A reference (e.g., PO Number, 'Initial Stock') is required.")]
    [StringLength(50)]
    string Reference,

        [Range(1, int.MaxValue, ErrorMessage = "User ID performing the transaction is required.")]
        int UserId,

        [StringLength(500)]
        string? Notes,

        int? PurchaseOrderId = null // Optional link to a specific Purchase Order
    );
}
