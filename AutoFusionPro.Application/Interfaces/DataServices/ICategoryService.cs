using AutoFusionPro.Application.DTOs.Category;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    /// <summary>
    /// Defines application service operations for managing product/part categories.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Gets a single category by its ID, including details suitable for display or editing.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>A CategoryDto or null if not found.</returns>
        Task<CategoryDto?> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Gets all categories, potentially structured hierarchically.
        /// Primarily for administrative UIs or full tree displays.
        /// </summary>
        /// <param name="onlyActive">Flag to retrieve only active categories.</param>
        /// <returns>A collection of CategoryDto, possibly nested for hierarchy.</returns>
        Task<IEnumerable<CategoryDto>> GetAllCategoriesHierarchicalAsync(bool onlyActive = true);

        /// <summary>
        /// Gets all top-level categories (those without a parent).
        /// </summary>
        /// <param name="onlyActive">Flag to retrieve only active categories.</param>
        /// <returns>A collection of CategoryDto representing top-level categories.</returns>
        Task<IEnumerable<CategoryDto>> GetTopLevelCategoriesAsync(bool onlyActive = true);

        /// <summary>
        /// Gets the immediate subcategories for a given parent category ID.
        /// </summary>
        /// <param name="parentCategoryId">The ID of the parent category.</param>
        /// <param name="onlyActive">Flag to retrieve only active subcategories.</param>
        /// <returns>A collection of CategoryDto representing subcategories.</returns>
        Task<IEnumerable<CategoryDto>> GetSubcategoriesAsync(int parentCategoryId, bool onlyActive = true);

        /// <summary>
        /// Gets a flat list of all categories suitable for selection (e.g., in a ComboBox when assigning a category to a part).
        /// Names might be prefixed with parent names for clarity in a flat list (e.g., "Engine > Filters").
        /// </summary>
        /// <param name="onlyActive">Flag to retrieve only active categories.</param>
        /// <returns>A collection of CategorySelectionDto.</returns>
        Task<IEnumerable<CategorySelectionDto>> GetAllCategoriesForSelectionAsync(bool onlyActive = true);


        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="createDto">Data for the new category.</param>
        /// <returns>The DTO of the newly created category.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid.</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown for other errors (e.g., duplication, database issues).</exception>
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);

        /// <summary>
        /// Updates an existing category's details.
        /// </summary>
        /// <param name="updateDto">Data containing the ID and updated fields.</param>
        /// <returns>Task indicating completion.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid.</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown if category not found or other errors.</exception>
        Task UpdateCategoryAsync(UpdateCategoryDto updateDto);

        /// <summary>
        /// Deletes a category. Checks for dependencies (subcategories, parts) before deletion.
        /// This might be a soft delete (setting IsActive=false) or a hard delete.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>Task indicating completion.</returns>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown if category not found or cannot be deleted due to dependencies.</exception>
        Task DeleteCategoryAsync(int id);

        /// <summary>
        /// Checks if a category name is unique, optionally within a specific parent category.
        /// </summary>
        /// <param name="name">The category name.</param>
        /// <param name="parentCategoryId">Optional parent category ID. Null for top-level.</param>
        /// <param name="excludeCategoryId">Optional category ID to exclude (for updates).</param>
        /// <returns>True if the name is unique, false otherwise.</returns>
        Task<bool> IsCategoryNameUniqueAsync(string name, int? parentCategoryId, int? excludeCategoryId = null);

        // Optional: If you implement an IsActive flag
        // Task SetCategoryActiveStatusAsync(int id, bool isActive);
    }
}
