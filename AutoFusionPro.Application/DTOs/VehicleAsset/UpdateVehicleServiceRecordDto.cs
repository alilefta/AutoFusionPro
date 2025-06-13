using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for updating an existing VehicleServiceRecord.
    /// </summary>
    public record UpdateVehicleServiceRecordDto(
        [Range(1, int.MaxValue)]
        int Id, // ID of the VehicleServiceRecord to update

        [Required(ErrorMessage = "Service date is required.")]
    DateTime ServiceDate,

        [Range(0, 2000000, ErrorMessage = "Mileage at service must be a non-negative number.")]
        int? MileageAtService,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Service description is required.")]
        [StringLength(500, ErrorMessage = "Service description cannot exceed 500 characters.")]
        string ServiceDescription,

        [StringLength(255, ErrorMessage = "Service provider name cannot exceed 255 characters.")]
    string? ServiceProviderName,

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Service cost must be a non-negative value.")]
        decimal? Cost,

        [StringLength(1000, ErrorMessage = "Service notes cannot exceed 1000 characters.")]
        string? Notes
    );
    // FluentValidation: Similar rules as Create, plus ensuring Id exists.
}