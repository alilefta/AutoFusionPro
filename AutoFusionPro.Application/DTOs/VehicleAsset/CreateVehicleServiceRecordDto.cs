using AutoFusionPro.Core.Helpers.ErrorMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for creating a new service history record for a Vehicle.
    /// VehicleId will be a separate parameter in the service method:
    /// AddServiceRecordToVehicleAsync(int vehicleId, CreateVehicleServiceRecordDto recordDto)
    /// </summary>
    public record CreateVehicleServiceRecordDto(
    [Required(ErrorMessage = "Service date is required.")]
    DateTime ServiceDate,

        [Range(0, 2000000, ErrorMessage = "Mileage at service must be a non-negative number.")] // Example range
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
    // FluentValidation:
    // - Ensure ServiceDate is not in the future.
    // - Ensure MileageAtService is plausible (e.g., not less than previous service mileage if available).
}
