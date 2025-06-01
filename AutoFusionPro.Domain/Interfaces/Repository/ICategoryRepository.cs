using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        /// <summary>
        /// Gets a category by its ID, including its ParentCategory and immediate Subcategories.
        /// </summary>
        Task<Category?> GetByIdWithChildrenAndParentAsync(int id);

        /// <summary>
        /// Gets all top-level categories (ParentCategoryId is null), optionally including their immediate subcategories.
        /// </summary>
        Task<IEnumerable<Category>> GetTopLevelCategoriesAsync(bool includeChildren = false);

        /// <summary>
        /// Gets all immediate subcategories for a given parent category ID.
        /// </summary>
        Task<IEnumerable<Category>> GetSubcategoriesAsync(int parentCategoryId);

        /// <summary>
        /// Checks if a category name already exists, optionally under a specific parent.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <param name="parentCategoryId">The ID of the parent category (null for top-level).</param>
        /// <param name="excludeCategoryId">Optional ID of a category to exclude from the check (for updates).</param>
        /// <returns>True if the name exists under the specified parent, false otherwise.</returns>
        Task<bool> NameExistsAsync(string name, int? parentCategoryId, int? excludeCategoryId = null);

        /// <summary>
        /// Checks if a category has any subcategories.
        /// </summary>
        Task<bool> HasSubcategoriesAsync(int categoryId);

        /// <summary>
        /// Checks if any parts are directly assigned to this category.
        /// </summary>
        Task<bool> HasAssociatedPartsAsync(int categoryId);

        /// <summary>
        /// Gets all categories with their full hierarchy (Parent and Subcategories recursively loaded).
        /// Use with caution for very deep hierarchies. Consider GetTopLevelCategoriesAsync with on-demand child loading for UI.
        /// </summary>
        Task<IEnumerable<Category>> GetAllWithFullHierarchyAsync();
    }
}
