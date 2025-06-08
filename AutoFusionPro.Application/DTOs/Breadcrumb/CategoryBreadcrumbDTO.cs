using AutoFusionPro.Application.DTOs.Category;

namespace AutoFusionPro.Application.DTOs.Breadcrumb
{
    public record CategoryBreadcrumbDTO
    (
        string? CategoryName,
        CategoryDto? CategoryItemDTO
    );
}
