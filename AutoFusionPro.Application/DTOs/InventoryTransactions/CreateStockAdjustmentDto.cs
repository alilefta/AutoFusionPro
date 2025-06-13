using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.InventoryTransactions
{
    /// <summary>
    /// DTO for recording a manual stock adjustment (e.g., cycle count correction, damage).
    /// </summary>
    public record CreateStockAdjustmentDto(
    [Range(1, int.MaxValue, ErrorMessage = "Part ID is required.")]
    int PartId,

        [Range(0, int.MaxValue, ErrorMessage = "New quantity on hand must be zero or positive.")] // User enters the *new actual total*
        int NewActualQuantityOnHand,

        [Required(AllowEmptyStrings = false, ErrorMessage = "A reason for the adjustment is required.")]
        [StringLength(200)]
        string Reason, // More descriptive than just "Notes" for the primary purpose

        [Range(1, int.MaxValue, ErrorMessage = "User ID performing the transaction is required.")]
        int UserId,

        [StringLength(500)]
        string? Notes // Additional details
    );
}
