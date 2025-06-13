using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
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
    /// DTO for updating an existing VehicleDamageLog entry.
    /// </summary>
    public record UpdateVehicleDamageLogDto(
        [Range(1, int.MaxValue)]
        int Id, // ID of the VehicleDamageLog to update

        [Required(ErrorMessage = "Date damage was noted is required.")]
        DateTime DateNoted,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Damage description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        string Description,

        [Required(ErrorMessage = "Damage severity is required.")]
    DamageSeverity Severity,

        bool IsRepaired,
        DateTime? RepairedDate,
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Estimated repair cost must be a non-negative value.")]
        decimal? EstimatedRepairCost,

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Actual repair cost must be a non-negative value.")]
        decimal? ActualRepairCost,

        [StringLength(1000, ErrorMessage = "Repair notes cannot exceed 1000 characters.")]
        string? RepairNotes
    );
    // FluentValidation: Similar rules as Create, plus ensuring Id exists.
}
