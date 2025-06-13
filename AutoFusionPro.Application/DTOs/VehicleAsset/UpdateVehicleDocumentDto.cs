using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for updating the metadata of an existing VehicleDocument.
    /// Updating the actual file typically involves deleting the old record and adding a new one.
    /// </summary>
    public record UpdateVehicleDocumentDto(
        [Range(1, int.MaxValue)]
        int Id, // The ID of the VehicleDocument record to update

        [Required(AllowEmptyStrings = false, ErrorMessage = "Document name is required.")]
        [StringLength(100, ErrorMessage = "Document name cannot exceed 100 characters.")]
        string DocumentName,

        [Required(ErrorMessage = "Document type is required.")]
        DocumentType DocumentType,

        DateTime? ExpiryDate,

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        string? Notes // Added for consistency
    );
    // FluentValidation:
    // - Ensure Id exists.
    // - Similar rules as Create for other fields.
}
