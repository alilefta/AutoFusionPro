using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for adding a new document to a Vehicle asset.
    /// The document file stream and original file name are passed separately to the service method:
    /// AddDocumentToVehicleAsync(int vehicleId, CreateVehicleDocumentDto docDto, Stream fileStream, string fileName)
    /// </summary>
    public record CreateVehicleDocumentDto(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Document name is required.")]
        [StringLength(100, ErrorMessage = "Document name cannot exceed 100 characters.")]
        string DocumentName,

        [Required(ErrorMessage = "Document type is required.")]
        DocumentType DocumentType, // Enum: Registration, Insurance, Title, ServiceRecordScan, InspectionReport, Other

        DateTime? ExpiryDate, // Optional, relevant for documents like insurance, registration

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        string? Notes // Added optional notes field for the document itself
    );
    // FluentValidation will:
    // - Potentially check if a document with the same name and type already exists for the vehicle.
    // - Ensure ExpiryDate is in the future if provided for certain document types.
}
