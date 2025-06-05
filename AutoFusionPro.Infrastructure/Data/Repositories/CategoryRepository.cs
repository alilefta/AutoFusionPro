using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class CategoryRepository : Repository<Category, CategoryRepository>, ICategoryRepository
    {
        #region Constructor
        /// <summary>
        /// Default Constructor for <see cref="CategoryRepository"/>
        /// Context and Logger are provided by <see cref="Repository{T, Repo}"/>
        /// </summary>
        /// <param name="context"><see cref="ApplicationDbContext"/></param>
        /// <param name="logger"><see cref="ILogger"/></param>
        public CategoryRepository(ApplicationDbContext context, ILogger<CategoryRepository> logger) : base(context, logger) { }
        #endregion


        /// <summary>
        /// Gets a category by its ID, including its ParentCategory and immediate Subcategories.
        /// </summary>
        public async Task<Category?> GetByIdWithChildrenAndParentAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get category with invalid ID: {CategoryId} in GetByIdWithChildrenAndParentAsync.", id);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve category with children and parent for ID: {CategoryId}", id);
            try
            {
                return await _dbSet // _dbSet is from your base Repository<T, TRepo>
                    .Include(c => c.ParentCategory)
                    .Include(c => c.Subcategories)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with children and parent for ID {CategoryId}.", id);
                throw new RepositoryException($"Could not retrieve category with details for ID {id}.", ex);
            }
        }

        /// <summary>
        /// Gets all top-level categories (ParentCategoryId is null), optionally including their immediate subcategories.
        /// </summary>
        public async Task<IEnumerable<Category>> GetTopLevelCategoriesAsync(bool includeChildren = false)
        {
            _logger.LogDebug("Attempting to retrieve top-level categories. IncludeChildren: {IncludeChildren}", includeChildren);
            try
            {
                IQueryable<Category> query = _dbSet.Where(c => c.ParentCategoryId == null);

                if (includeChildren)
                {
                    query = query.Include(c => c.Subcategories);
                }

                return await query.OrderBy(c => c.Name).ToListAsync(); // Added OrderBy
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top-level categories.");
                throw new RepositoryException("Could not retrieve top-level categories.", ex);
            }
        }

        /// <summary>
        /// Gets all immediate subcategories for a given parent category ID.
        /// </summary>
        public async Task<IEnumerable<Category>> GetSubcategoriesAsync(int parentCategoryId)
        {
            if (parentCategoryId <= 0)
            {
                _logger.LogWarning("Attempted to get subcategories with invalid ParentCategoryID: {ParentCategoryId}", parentCategoryId);
                return Enumerable.Empty<Category>();
            }

            _logger.LogDebug("Attempting to retrieve subcategories for ParentCategoryID: {ParentCategoryId}", parentCategoryId);
            try
            {
                return await _dbSet
                    .Where(c => c.ParentCategoryId == parentCategoryId)
                    .OrderBy(c => c.Name) // Added OrderBy
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subcategories for ParentCategoryID {ParentCategoryId}.", parentCategoryId);
                throw new RepositoryException($"Could not retrieve subcategories for ParentCategoryID {parentCategoryId}.", ex);
            }
        }

        /// <summary>
        /// Checks if a category name already exists, optionally under a specific parent.
        /// </summary>
        public async Task<bool> NameExistsAsync(string name, int? parentCategoryId, int? excludeCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Attempted to check name existence with null or empty name.");
                return false; // Or throw ArgumentException
            }

            string nameLower = name.Trim().ToLowerInvariant();
            _logger.LogDebug("Checking name existence for: Name='{Name}', ParentId='{ParentId}', ExcludeId='{ExcludeId}'",
                nameLower, parentCategoryId, excludeCategoryId);
            try
            {
                var query = _dbSet.Where(c => c.Name.ToLower() == nameLower && c.ParentCategoryId == parentCategoryId);

                if (excludeCategoryId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeCategoryId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking category name existence for Name '{Name}', ParentId '{ParentId}'.", name, parentCategoryId);
                throw new RepositoryException($"Could not check existence for category name '{name}'.", ex);
            }
        }

        /// <summary>
        /// Checks if a category has any subcategories.
        /// </summary>
        public async Task<bool> HasSubcategoriesAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                _logger.LogWarning("Attempted to check for subcategories with invalid CategoryID: {CategoryId}", categoryId);
                return false;
            }

            _logger.LogDebug("Checking if category ID {CategoryId} has subcategories.", categoryId);
            try
            {
                return await _dbSet.AnyAsync(c => c.ParentCategoryId == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for subcategories for CategoryID {CategoryId}.", categoryId);
                throw new RepositoryException($"Could not check for subcategories for CategoryID {categoryId}.", ex);
            }
        }

        /// <summary>
        /// Checks if any parts are directly assigned to this category.
        /// </summary>
        public async Task<bool> HasAssociatedPartsAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                _logger.LogWarning("Attempted to check for associated parts with invalid CategoryID: {CategoryId}", categoryId);
                return false;
            }

            _logger.LogDebug("Checking if category ID {CategoryId} has associated parts.", categoryId);
            try
            {
                // Accessing _context directly for a different DbSet
                // Or if PartRepository is on IUnitOfWork: _unitOfWork.Parts.ExistsAsync(p => p.CategoryId == categoryId);
                return await _context.Parts.AnyAsync(p => p.Id == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for associated parts for CategoryID {CategoryId}.", categoryId);
                throw new RepositoryException($"Could not check for associated parts for CategoryID {categoryId}.", ex);
            }
        }

        /// <summary>
        /// Gets all categories with their full hierarchy (Parent and Subcategories recursively loaded).
        /// This implementation loads all categories and relies on EF Core's change tracking to fix up navigation properties
        /// if individual entities with their Parent/Subcategories are loaded.
        /// For very deep hierarchies, a more optimized approach (e.g., recursive CTEs if DB supports, or iterative loading) might be needed
        /// if performance becomes an issue or if only a specific branch is needed.
        /// This simplified version fetches all categories and their *immediate* parent/children.
        /// True recursive loading often happens in the service or by constructing a tree DTO.
        /// </summary>
        public async Task<IEnumerable<Category>> GetAllWithFullHierarchyAsync()
        {
            _logger.LogDebug("Attempting to retrieve all categories with their hierarchy.");
            try
            {
                // This will fetch all categories. EF Core's change tracker will help link
                // ParentCategory and Subcategories for entities that are loaded into the context.
                // However, it doesn't guarantee full deep recursive loading in one go for disconnected scenarios.
                // For connected scenarios (same DbContext instance), it works well.
                var allCategories = await _context.Categories
                    .Include(c => c.ParentCategory) // Include parent
                    .Include(c => c.Subcategories)  // Include immediate children
                    .OrderBy(c => c.ParentCategoryId) // Order to help with tree building if done manually
                    .ThenBy(c => c.Name)
                    .ToListAsync();

                // The service layer would typically be responsible for assembling these into a true hierarchical DTO structure.
                // This repository method provides the raw data with one level of parent/children loaded.
                return allCategories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories with hierarchy.");
                throw new RepositoryException("Could not retrieve all categories with hierarchy.", ex);
            }
        }
    }
}
