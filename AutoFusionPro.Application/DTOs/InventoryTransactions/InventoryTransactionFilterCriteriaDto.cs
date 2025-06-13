using AutoFusionPro.Core.Enums.ModelEnum;

namespace AutoFusionPro.Application.DTOs.InventoryTransactions
{
    /// <summary>
    /// DTO for filtering inventory transactions.
    /// </summary>
    public record InventoryTransactionFilterCriteriaDto(
        int? PartId = null,
        string? PartNumberSearch = null, // Search by part number
        string? PartNameSearch = null,   // Search by part name
        InventoryTransactionType? TransactionType = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null,
        int? UserId = null,
        string? ReferenceNumberSearch = null
    );
}
