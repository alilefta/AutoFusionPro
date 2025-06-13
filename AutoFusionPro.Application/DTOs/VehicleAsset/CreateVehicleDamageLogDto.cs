using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for creating a new damage log entry for a Vehicle.
    /// VehicleId will be a separate parameter in the service method:
    /// AddDamageLogToVehicleAsync(int vehicleId, CreateVehicleDamageLogDto damageDto)
    /// </summary>
    public record CreateVehicleDamageLogDto(
        [Required(ErrorMessage = "Date damage was noted is required.")]
        DateTime DateNoted,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Damage description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        string Description,

        [Required(ErrorMessage = "Damage severity is required.")]
        DamageSeverity Severity,

        DateTime? RepairedDate, 

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Estimated repair cost must be a non-negative value.")]
        decimal? EstimatedRepairCost,

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Actual repair cost must be a non-negative value.")]
        decimal? ActualRepairCost,

        [StringLength(1000, ErrorMessage = "Repair notes cannot exceed 1000 characters.")]
        string? RepairNotes,

        bool IsRepaired = false // Default to not repaired = false!
     // List<CreateVehicleDamageImageDto> InitialDamageImages // Optional: if you want to upload images simultaneously
    );
    // FluentValidation will:
    // - Ensure RepairedDate is not set if IsRepaired is false.
    // - Ensure RepairedDate is not in the future.
    // - Ensure ActualRepairCost is only set if IsRepaired is true.
}
