using AutoFusionPro.Core.Enums.DTOEnums.Inventory;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.InventoryTransactions
{
    /// <summary>
    /// DTO for recording a stock return (e.g., customer return, return to supplier).
    /// </summary>
    public record CreateStockReturnDto(
    [Range(1, int.MaxValue, ErrorMessage = "Part ID is required.")]
        int PartId,

        [Range(1, int.MaxValue, ErrorMessage = "Quantity returned must be positive.")]
        int QuantityReturned,
        [Range(1, int.MaxValue)] 
        int ReturnedUnitOfMeasureId, // ID of the UoM for QuantityReturned

        // Indicates if it's a customer return (increases stock) or return to supplier (decreases stock)
        // The service will determine if QuantityReturned is positive or negative for stock based on this.
        // Alternatively, have two DTOs: CreateCustomerReturnDto, CreateReturnToSupplierDto.
        // For now, one DTO with a flag.
        StockReturnType ReturnType, // e.g., CustomerReturn, ReturnToSupplier

        [Required(AllowEmptyStrings = false, ErrorMessage = "A reference (e.g., RMA Number, Original SO/PO) is required.")]
    [StringLength(50)]
    string Reference,

        [Range(1, int.MaxValue, ErrorMessage = "User ID performing the transaction is required.")]
        int UserId,

        [StringLength(500)]
        string? Notes,

        int? OriginalSalesOrderId = null,
        int? OriginalPurchaseOrderId = null
    );

}
