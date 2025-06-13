using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class PartRepository : Repository<Part, PartRepository>, IPartRepository
    {

        public PartRepository(ApplicationDbContext context, ILogger<PartRepository> logger) : base(context, logger)
        {
        }

        public async Task<Part?> GetByIdAsync(int id, bool includeUoMs = false)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get part with invalid ID: {PartId} in GetByIdAsync.", id);
                return null;
            }


            try
            {
                IQueryable<Part> query = _dbSet;

                if (includeUoMs) // Or always include them if PartService methods always need them
                {
                    query = query.Include(p => p.StockingUnitOfMeasure)
                              .Include(p => p.SalesUnitOfMeasure)
                                 .Include(p => p.PurchaseUnitOfMeasure);
                }

                return await query.FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving part with Unit of measures for ID {PartId}.", id);
                throw new RepositoryException($"Could not retrieve detailed part with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Gets a single Part by its ID, optionally including detailed related entities
        /// like Category, Suppliers (with Supplier details), and CompatibleVehicles (with Vehicle details).
        /// </summary>
        public async Task<Part?> GetByIdWithDetailsAsync(int id,
            bool includeCategory = true,
            bool includeSuppliers = false,
            bool includeCompatibility = false,
            bool includeUnitOfMeasures = true)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get part with invalid ID: {PartId} in GetByIdWithDetailsAsync.", id);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve part with details for ID: {PartId}. Includes: Category={IncCat}, Suppliers={IncSup}, Compatibility={IncComp}",
                id, includeCategory, includeSuppliers, includeCompatibility);

            try
            {
                IQueryable<Part> query = _dbSet;

                if (includeCategory)
                {
                    query = query.Include(p => p.Category);
                }

                if (includeSuppliers)
                {
                    query = query.Include(p => p.Suppliers)
                                     .ThenInclude(sp => sp.Supplier); // Also load the Supplier details for each SupplierPart
                }

                if (includeCompatibility)
                {
                    query = query.Include(p => p.CompatibleVehicles)
                                     .ThenInclude(pc => pc.CompatibleVehicle)
                                        .ThenInclude(cv => cv.Model)
                                            .ThenInclude(m => m.Make) // Make
                                 .Include(p => p.CompatibleVehicles) // Re-specify root for next ThenInclude chain
                                     .ThenInclude(pc => pc.CompatibleVehicle)
                                        .ThenInclude(cv => cv.TrimLevel) // TrimLevel
                                 .Include(p => p.CompatibleVehicles) // Re-specify root
                                     .ThenInclude(pc => pc.CompatibleVehicle)
                                        .ThenInclude(cv => cv.TransmissionType) // TransmissionType
                                 .Include(p => p.CompatibleVehicles) // Re-specify root
                                     .ThenInclude(pc => pc.CompatibleVehicle)
                                        .ThenInclude(cv => cv.EngineType) // EngineType
                                 .Include(p => p.CompatibleVehicles) // Re-specify root
                                     .ThenInclude(pc => pc.CompatibleVehicle)
                                        .ThenInclude(cv => cv.BodyType); // BodyType
                }


                if (includeUnitOfMeasures) // This flag might be redundant if always needed for PartDetailDto
                {
                    query = query.Include(p => p.StockingUnitOfMeasure);
                    query = query.Include(p => p.SalesUnitOfMeasure);    // Include even if null, EF handles it
                    query = query.Include(p => p.PurchaseUnitOfMeasure); // Include even if null
                }

                return await query.FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving part with details for ID {PartId}.", id);
                throw new RepositoryException($"Could not retrieve detailed part with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Gets a Part by its unique PartNumber, optionally including its Category.
        /// </summary>
        public async Task<Part?> GetByPartNumberAsync(string partNumber, bool includeCategory = true)
        {
            if (string.IsNullOrWhiteSpace(partNumber))
            {
                _logger.LogWarning("Attempted to get part with null or empty PartNumber in GetByPartNumberAsync.");
                return null; // Or throw ArgumentException
            }

            string normalizedPartNumber = partNumber.Trim().ToUpperInvariant(); // Normalize for consistent lookup
            _logger.LogDebug("Attempting to retrieve part by PartNumber: {PartNumber}. IncludeCategory: {IncludeCategory}",
                normalizedPartNumber, includeCategory);

            try
            {
                IQueryable<Part> query = _dbSet;

                if (includeCategory)
                {
                    query = query.Include(p => p.Category);
                }

                return await query.FirstOrDefaultAsync(p => p.PartNumber.ToUpper() == normalizedPartNumber);
                // Consider database collation for case sensitivity if not normalizing here and in DB.
                // Forcing ToUpper on both sides ensures case-insensitivity if DB collation is case-sensitive.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving part by PartNumber '{PartNumber}'.", partNumber);
                throw new RepositoryException($"Could not retrieve part with PartNumber '{partNumber}'.", ex);
            }
        }

        /// <summary>
        /// Gets a collection of Parts based on a flexible filter predicate.
        /// Allows specifying which related entities to include for performance.
        /// </summary>
        public async Task<IEnumerable<Part>> FindPartsAsync(Expression<Func<Part, bool>> predicate,
            bool includeCategory = false,
            bool includeSuppliers = false,
            Func<IQueryable<Part>, IOrderedQueryable<Part>>? orderBy = null,
            int? take = null)
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            _logger.LogDebug("Finding parts with predicate. Includes: Category={IncCat}, Suppliers={IncSup}, OrderBySet={OrderSet}, Take={TakeCount}",
                includeCategory, includeSuppliers, orderBy != null, take);

            try
            {
                IQueryable<Part> query = _dbSet.Where(predicate);

                if (includeCategory)
                {
                    query = query.Include(p => p.Category);
                }

                if (includeSuppliers)
                {
                    query = query.Include(p => p.Suppliers)
                                     .ThenInclude(sp => sp.Supplier);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }
                else // Default ordering if none provided, important for consistent results if 'take' is used
                {
                    query = query.OrderBy(p => p.Name).ThenBy(p => p.Id);
                }

                if (take.HasValue && take.Value > 0)
                {
                    query = query.Take(take.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding parts with predicate.");
                throw new RepositoryException("Could not find parts based on the provided criteria.", ex);
            }
        }

        /// <summary>
        /// Gets a paginated list of Parts based on filter criteria and search term.
        /// </summary>
        public async Task<IEnumerable<Part>> GetFilteredPartsPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Part, bool>>? filterPredicate = null,
            int? categoryId = null,
            string? manufacturer = null,
            int? supplierId = null,
            IEnumerable<int>? restrictToCompatibleVehicleIds = null,
            string? searchTerm = null,
            bool? isActive = true,
            bool? isLowStock = null,
            bool includeCategory = true, // Defaulted to true as it's common for summaries
            bool includeSuppliers = false,
            bool includeStockingUnitOfMeasure = true)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;    // Default page size
            if (pageSize > 200) pageSize = 200; // Max page size safeguard

            _logger.LogDebug("Getting filtered paged parts. Page: {Page}, Size: {Size}, CategoryId: {CatId}, Manufacturer: {Manuf}, SupplierId: {SupId}, Compatible Vehicles Count: {VehId}, Search: '{Search}', IsActive: {Active}, IsLowStock: {LowStock}",
                pageNumber, pageSize, categoryId, manufacturer, supplierId, restrictToCompatibleVehicleIds?.Count(), searchTerm, isActive, isLowStock);

            try
            {
                IQueryable<Part> query = _dbSet;

                // Apply includes first
                if (includeCategory)
                {
                    query = query.Include(p => p.Category);
                }
                if (includeSuppliers) // This include might be heavy for a paged list, use judiciously
                {
                    query = query.Include(p => p.Suppliers)
                                     .ThenInclude(sp => sp.Supplier);
                }

                // Apply direct predicate filter
                if (filterPredicate != null)
                {
                    query = query.Where(filterPredicate);
                }

                // Apply specific filters
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(manufacturer))
                {
                    string manufLower = manufacturer.Trim().ToLowerInvariant();
                    query = query.Where(p => p.Manufacturer != null && p.Manufacturer.ToLower().Contains(manufLower));
                }

                if (supplierId.HasValue && supplierId.Value > 0)
                {
                    query = query.Where(p => p.Suppliers.Any(sp => sp.SupplierId == supplierId.Value));
                }

                if (restrictToCompatibleVehicleIds != null && restrictToCompatibleVehicleIds.Count() > 0)
                {
                    query = query.Where(p => p.CompatibleVehicles.Any(pc => restrictToCompatibleVehicleIds.Contains(pc.CompatibleVehicleId)));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == isActive.Value);
                }

                if (isLowStock.HasValue && isLowStock.Value)
                {
                    query = query.Where(p => p.CurrentStock <= p.ReorderLevel);
                }
                else if (isLowStock.HasValue && !isLowStock.Value) // If user explicitly wants NOT low stock
                {
                    query = query.Where(p => p.CurrentStock > p.ReorderLevel);
                }


                // Apply search term (across multiple fields)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string termLower = searchTerm.Trim().ToLowerInvariant();
                    query = query.Where(p =>
                        (p.PartNumber != null && p.PartNumber.ToLower().Contains(termLower)) ||
                        (p.Name != null && p.Name.ToLower().Contains(termLower)) ||
                        (p.Description != null && p.Description.ToLower().Contains(termLower)) ||
                        (p.Barcode != null && p.Barcode.ToLower().Contains(termLower)) ||
                        (p.Category != null && p.Category.Name != null && p.Category.Name.ToLower().Contains(termLower)) // Search category name if included
                    );
                }

                if (includeStockingUnitOfMeasure)
                {
                    query = query.Include(p => p.StockingUnitOfMeasure);
                }

                // Apply stable ordering for consistent pagination
                // Service layer can provide a more specific OrderBy if needed via the `filterPredicate` or a separate param.
                query = query.OrderBy(p => p.Name) // Default sort
                             .ThenBy(p => p.PartNumber);

                return await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged and filtered parts. Page: {Page}, Size: {Size}", pageNumber, pageSize);
                throw new RepositoryException("Could not retrieve paged parts.", ex);
            }
        }


        /// <summary>
        /// Gets the total count of Parts matching the specified filter criteria.
        /// Parameters should mirror GetFilteredPartsPagedAsync for accurate counts.
        /// </summary>
        public async Task<int> GetTotalFilteredPartsCountAsync(
            Expression<Func<Part, bool>>? filterPredicate = null,
            int? categoryId = null,
            string? manufacturer = null,
            int? supplierId = null,
            IEnumerable<int>? restrictToCompatibleVehicleIds = null,
            string? searchTerm = null,
            bool? isActive = true, // Default to active parts
            bool? isLowStock = null)
        {
            _logger.LogDebug("Getting total filtered parts count. CategoryId: {CatId}, Manufacturer: {Manual}, SupplierId: {SupId}, Compatible Vehicles Count: {VehId}, Search: '{Search}', IsActive: {Active}, IsLowStock: {LowStock}",
                categoryId, manufacturer, supplierId, restrictToCompatibleVehicleIds?.Count(), searchTerm, isActive, isLowStock);

            try
            {
                IQueryable<Part> query = _dbSet;

                // Apply direct predicate filter
                if (filterPredicate != null)
                {
                    query = query.Where(filterPredicate);
                }

                // Apply specific filters - This logic MUST MATCH GetFilteredPartsPagedAsync
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(manufacturer))
                {
                    string manufLower = manufacturer.Trim().ToLowerInvariant();
                    // Note: For CountAsync, if Manufacturer is null, the ToLower will throw.
                    // Ensure Manufacturer is not null before calling ToLower or use EF.Functions.Like for DB-side case-insensitivity if available.
                    query = query.Where(p => p.Manufacturer != null && p.Manufacturer.ToLower().Contains(manufLower));
                }

                if (supplierId.HasValue && supplierId.Value > 0)
                {
                    // This requires a join. For a COUNT, if the relationship is one-to-many (Part to SupplierPart),
                    // using .Any() is correct as it doesn't multiply counts.
                    query = query.Where(p => p.Suppliers.Any(sp => sp.SupplierId == supplierId.Value));
                }

                if (restrictToCompatibleVehicleIds != null && restrictToCompatibleVehicleIds.Count() > 0)
                {
                    query = query.Where(p => p.CompatibleVehicles.Any(pc => restrictToCompatibleVehicleIds.Contains(pc.CompatibleVehicleId)));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == isActive.Value);
                }

                if (isLowStock.HasValue && isLowStock.Value)
                {
                    query = query.Where(p => p.CurrentStock <= p.ReorderLevel);
                }
                else if (isLowStock.HasValue && !isLowStock.Value)
                {
                    query = query.Where(p => p.CurrentStock > p.ReorderLevel);
                }

                // Apply search term (across multiple fields)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string termLower = searchTerm.Trim().ToLowerInvariant();
                    query = query.Where(p =>
                        (p.PartNumber != null && p.PartNumber.ToLower().Contains(termLower)) ||
                        (p.Name != null && p.Name.ToLower().Contains(termLower)) ||
                        (p.Description != null && p.Description.ToLower().Contains(termLower)) ||
                        (p.Barcode != null && p.Barcode.ToLower().Contains(termLower)) ||
                        // For Count, including Category.Name requires a join or that Category is always loaded.
                        // If Category is not guaranteed to be loaded, this could fail or behave unexpectedly.
                        // It's safer if the service maps Category name to CategoryId before calling this for counts,
                        // or if this query explicitly joins.
                        // For simplicity, if Category.Name search is vital for count, an explicit join or subquery might be needed.
                        // Or, if the service translates "category name search" into a categoryId filter first.
                        // Let's assume for now the searchTerm on Category.Name is applied if Category is included by other means,
                        // or we rely on categoryId filter primarily.
                        // A more robust search on related entities for COUNT might require a slightly different query structure
                        // than the paged query if includes are not present.
                        (p.Category != null && p.Category.Name != null && p.Category.Name.ToLower().Contains(termLower))
                    );
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total filtered parts count.");
                throw new RepositoryException("Could not retrieve total parts count.", ex);
            }
        }


        /// <summary>
        /// Checks if a PartNumber already exists, optionally excluding a specific Part ID.
        /// </summary>
        public async Task<bool> PartNumberExistsAsync(string partNumber, int? excludePartId = null)
        {
            if (string.IsNullOrWhiteSpace(partNumber))
            {
                _logger.LogWarning("Attempted to check PartNumber existence with null or empty PartNumber.");
                return false; // An empty/null part number cannot "exist" as a duplicate of a valid one.
                              // Or, based on business rules, an empty part number might not be allowed at all,
                              // which would be caught by DTO validation.
            }

            // Normalize for consistent comparison, matching the database's likely behavior or an index.
            string normalizedPartNumber = partNumber.Trim().ToUpperInvariant();
            _logger.LogDebug("Checking PartNumber existence for: {PartNumber}, ExcludePartId: {ExcludePartId}",
                normalizedPartNumber, excludePartId);

            try
            {
                IQueryable<Part> query = _dbSet.Where(p => p.PartNumber.ToUpper() == normalizedPartNumber);

                if (excludePartId.HasValue && excludePartId.Value > 0)
                {
                    query = query.Where(p => p.Id != excludePartId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking PartNumber existence for '{PartNumber}'.", partNumber);
                throw new RepositoryException($"Could not check existence for PartNumber '{partNumber}'.", ex);
            }
        }

        /// <summary>
        /// Checks if a Barcode already exists, optionally excluding a specific Part ID.
        /// (Assumes Barcode should be unique if not null/empty)
        /// </summary>
        public async Task<bool> BarcodeExistsAsync(string barcode, int? excludePartId = null)
        {
            // Barcodes are often case-sensitive, but trimming is usually good.
            // If barcodes can be null or empty and that's valid, this check assumes
            // we are only interested if a *non-empty* barcode is duplicated.
            if (string.IsNullOrWhiteSpace(barcode))
            {
                _logger.LogDebug("Attempted to check Barcode existence with null or empty Barcode. Returning false as it cannot clash.");
                return false;
            }

            string trimmedBarcode = barcode.Trim(); // Case sensitivity might depend on scanner/system
            _logger.LogDebug("Checking Barcode existence for: {Barcode}, ExcludePartId: {ExcludePartId}",
                trimmedBarcode, excludePartId);

            try
            {
                // Query for non-null and non-empty barcodes that match
                IQueryable<Part> query = _dbSet.Where(p =>
                    !string.IsNullOrEmpty(p.Barcode) && // Ensure we only compare against actual barcodes
                    p.Barcode == trimmedBarcode);

                if (excludePartId.HasValue && excludePartId.Value > 0)
                {
                    query = query.Where(p => p.Id != excludePartId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Barcode existence for '{Barcode}'.", barcode);
                throw new RepositoryException($"Could not check existence for Barcode '{barcode}'.", ex);
            }
        }

        /// <summary>
        /// Gets a list of Parts that are low on stock (CurrentStock <= ReorderLevel).
        /// </summary>
        public async Task<IEnumerable<Part>> GetLowStockPartsAsync(bool includeCategory = true, int? take = null)
        {
            _logger.LogDebug("Attempting to retrieve low stock parts. IncludeCategory: {IncludeCategory}, Take: {TakeCount}",
                includeCategory, take);
            try
            {
                IQueryable<Part> query = _dbSet
                    .Where(p => p.IsActive && p.CurrentStock <= p.ReorderLevel); // Only active parts that are low stock

                if (includeCategory)
                {
                    query = query.Include(p => p.Category);
                }

                // Order by how much they are below reorder level (most critical first), then by name
                query = query.OrderBy(p => p.CurrentStock - p.ReorderLevel) // Smaller (more negative) values are more critical
                             .ThenBy(p => p.Name);

                if (take.HasValue && take.Value > 0)
                {
                    query = query.Take(take.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock parts.");
                throw new RepositoryException("Could not retrieve low stock parts.", ex);
            }
        }


        public async Task<IEnumerable<Part>> GetPartsByCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0) return Enumerable.Empty<Part>();
                return await FindPartsAsync(p => p.CategoryId == categoryId && p.IsActive, includeCategory: true);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parts by category with Id {CategoryId}.", categoryId);
                throw new RepositoryException($"Could not retrieve parts by category with Id {categoryId}.", ex);
            }


        }

    }
}

