using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for updating the metadata of an existing VehicleImage.
    /// </summary>
    public record UpdateVehicleImageDto(
        [Range(1, int.MaxValue)]
        int Id, // The ID of the VehicleImage record to update

        [StringLength(255, ErrorMessage = "Caption cannot exceed 255 characters.")]
    string? Caption,
    bool IsPrimary,

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be a non-negative number.")]
        int DisplayOrder
    );
}
