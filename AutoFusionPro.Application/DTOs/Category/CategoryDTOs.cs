using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Application.DTOs.Category
{
    /// <summary>
    /// DTO for displaying category information, potentially in a hierarchical structure.
    /// </summary>
    public class CategoryDto // Using class to easily support hierarchy and potential INPC if needed
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; } // For display
        public bool IsActive { get; set; } // Added IsActive

        // For hierarchical display (e.g., TreeView)
        public List<CategoryDto> Children { get; set; } = new List<CategoryDto>();

        // Optional: For quick UI checks without loading children
        public bool HasChildren { get; set; }
        public int PartCount { get; set; } // How many parts are directly in this category
    }

    /// <summary>
    /// DTO for creating a new category.
    /// </summary>
    public record CreateCategoryDto(
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 2)]
        string Name,
        string? Description,
        string? ImagePath, // This would be a source path from UI
        int? ParentCategoryId, // Null for top-level category
        bool IsActive = true
    );


    /// <summary>
    /// DTO for updating an existing category.
    /// </summary>
    public record UpdateCategoryDto(
        [Range(1, int.MaxValue)]
        int Id,
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 2)]
        string Name,
        string? Description,
        string? ImagePath, // Source path from UI if changed, or existing path, or null to clear
        int? ParentCategoryId,
        bool IsActive
    );

    /// <summary>
    /// Simple DTO for category selection (e.g., in ComboBoxes).
    /// </summary>
    public record CategorySelectionDto(int Id, string Name, int? ParentId); // ParentId can help in building dependent dropdowns
}
