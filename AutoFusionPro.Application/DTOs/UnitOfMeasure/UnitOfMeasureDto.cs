namespace AutoFusionPro.Application.DTOs.UnitOfMeasure
{
    /// <summary>
    /// DTO for displaying Unit of Measure information.
    /// </summary>
    public record UnitOfMeasureDto(
        int Id,
        string Name,
        string Symbol,
        string? Description
    );
}
