using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.UnitOfMeasure
{
    /// <summary>
    /// DTO for updating an existing Unit of Measure.
    /// </summary>
    public record UpdateUnitOfMeasureDto(
        [Range(1, int.MaxValue)]
        int Id,

        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 1)]
        string Name,

        [Required(AllowEmptyStrings = false)]
        [StringLength(10, MinimumLength = 1)]
        string Symbol,

        [StringLength(255)]
        string? Description
    );
}
