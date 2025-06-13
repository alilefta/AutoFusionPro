using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.VehicleAsset
{
    /// <summary>
    /// DTO for adding a new image to a Vehicle asset.
    /// The image file stream itself is passed separately to the service.
    /// </summary>
    public record CreateVehicleImageDto(
        // VehicleId will be a separate parameter in the service method AddImageToVehicleAsync(int vehicleId, CreateVehicleImageDto dto, Stream imageStream, string fileName)

        [StringLength(255, ErrorMessage = "Caption cannot exceed 255 characters.")]
    string? Caption,
    bool IsPrimary = false,

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be a non-negative number.")]
        int DisplayOrder = 0
    );
}
