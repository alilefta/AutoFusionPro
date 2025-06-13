using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.UnitOfMeasure
{
    /// <summary>
    /// DTO for creating a new Unit of Measure.
    /// </summary>
    public record CreateUnitOfMeasureDto(
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
