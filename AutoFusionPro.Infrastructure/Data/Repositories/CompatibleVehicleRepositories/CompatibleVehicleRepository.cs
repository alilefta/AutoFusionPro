using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class CompatibleVehicleRepository : Repository<CompatibleVehicle, CompatibleVehicleRepository>, ICompatibleVehicleRepository
    {
        public CompatibleVehicleRepository(ApplicationDbContext context, ILogger<CompatibleVehicleRepository> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Gets a single CompatibleVehicle configuration by its ID, including its related
        /// Make, Model, TrimLevel, TransmissionType, EngineType, and BodyType details.
        /// </summary>
        public async Task<CompatibleVehicle?> GetByIdWithDetailsAsync(int id)
        {
            if (id <= 0) return null;

            try
            {
                return await _dbSet
                    .Include(cv => cv.Model)
                        .ThenInclude(m => m.Make) // Include Make through Model
                    .Include(cv => cv.TrimLevel)
                    .Include(cv => cv.TransmissionType)
                    .Include(cv => cv.EngineType)
                    .Include(cv => cv.BodyType)
                    .FirstOrDefaultAsync(cv => cv.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CompatibleVehicle with details for ID {CompatibleVehicleId}", id);
                throw new RepositoryException($"Could not retrieve detailed CompatibleVehicle with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Checks if a specific CompatibleVehicle configuration (combination of Model, Years, Trim, etc.)
        /// already exists.
        /// </summary>
        public async Task<bool> SpecificationExistsAsync(
            int modelId,
            int yearStart,
            int yearEnd,
            int? trimLevelId,
            int? transmissionTypeId,
            int? engineTypeId,
            int? bodyTypeId,
            int? excludeCompatibleVehicleId = null)
        {
            if (modelId <= 0 || yearStart <= 0 || yearEnd <= 0 || yearEnd < yearStart)
            {
                _logger.LogWarning("Invalid parameters provided for SpecificationExistsAsync check. ModelId={ModelId}, YearStart={YearStart}, YearEnd={YearEnd}", modelId, yearStart, yearEnd);
                return false; // Or throw ArgumentException
            }

            try
            {
                var query = _dbSet.Where(cv =>
                    cv.ModelId == modelId &&
                    cv.YearStart == yearStart &&
                    cv.YearEnd == yearEnd &&
                    cv.TrimLevelId == trimLevelId &&             // Handles null comparison correctly
                    cv.TransmissionTypeId == transmissionTypeId && // Handles null comparison correctly
                    cv.EngineTypeId == engineTypeId &&           // Handles null comparison correctly
                    cv.BodyTypeId == bodyTypeId);                // Handles null comparison correctly

                if (excludeCompatibleVehicleId.HasValue)
                {
                    query = query.Where(cv => cv.Id != excludeCompatibleVehicleId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for existing CompatibleVehicle specification. ModelId={ModelId}, YearStart={YearStart}", modelId, yearStart);
                throw new RepositoryException("Could not check for existing vehicle specification.", ex);
            }
        }

        /// <summary>
        /// Gets a paginated list of CompatibleVehicle configurations matching the filter,
        /// including details for display (Make Name, Model Name, etc.).
        /// </summary>
        public async Task<IEnumerable<CompatibleVehicle>> GetFilteredWithDetailsPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<CompatibleVehicle, bool>>? filter = null,
            int? makeIdFilter = null,
            string? searchTerm = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                IQueryable<CompatibleVehicle> query = _dbSet
                    .Include(cv => cv.Model)
                        .ThenInclude(m => m.Make)
                    .Include(cv => cv.TrimLevel)
                    .Include(cv => cv.TransmissionType)
                    .Include(cv => cv.EngineType)
                    .Include(cv => cv.BodyType);

                // Apply direct filter on CompatibleVehicle properties
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                // Apply filter by MakeId (requires join through Model)
                if (makeIdFilter.HasValue && makeIdFilter.Value > 0)
                {
                    query = query.Where(cv => cv.Model.MakeId == makeIdFilter.Value);
                }

                // Apply generic search term
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string termLower = searchTerm.Trim().ToLowerInvariant();
                    query = query.Where(cv =>
                        (cv.Model.Make.Name != null && cv.Model.Make.Name.ToLower().Contains(termLower)) ||
                        (cv.Model.Name != null && cv.Model.Name.ToLower().Contains(termLower)) ||
                        (cv.VIN != null && cv.VIN.ToLower().Contains(termLower)) || // Still search VIN if provided
                        (cv.YearStart.ToString().Contains(termLower)) ||
                        (cv.YearEnd.ToString().Contains(termLower)) ||
                        (cv.TrimLevel != null && cv.TrimLevel.Name != null && cv.TrimLevel.Name.ToLower().Contains(termLower)) ||
                        (cv.EngineType != null && cv.EngineType.Name != null && cv.EngineType.Name.ToLower().Contains(termLower)) ||
                        (cv.BodyType != null && cv.BodyType.Name != null && cv.BodyType.Name.ToLower().Contains(termLower)) ||
                        (cv.TransmissionType != null && cv.TransmissionType.Name != null && cv.TransmissionType.Name.ToLower().Contains(termLower))
                    );
                }

                // Apply stable ordering for consistent pagination
                query = query.OrderBy(cv => cv.Model.Make.Name)
                             .ThenBy(cv => cv.Model.Name)
                             .ThenByDescending(cv => cv.YearEnd) // Show newer year ranges first
                             .ThenByDescending(cv => cv.YearStart)
                             .ThenBy(cv => cv.Id); // Final tie-breaker

                return await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged and filtered CompatibleVehicles with details. Page: {Page}, Size: {Size}", pageNumber, pageSize);
                throw new RepositoryException("Could not retrieve paged vehicle specifications.", ex);
            }
        }

        /// <summary>
        /// Gets the total count of CompatibleVehicle configurations matching the filter.
        /// </summary>
        public async Task<int> GetTotalCountAsync(
            Expression<Func<CompatibleVehicle, bool>>? filter = null,
            int? makeIdFilter = null,
            string? searchTerm = null)
        {
            try
            {
                IQueryable<CompatibleVehicle> query = _dbSet;

                // Apply direct filter on CompatibleVehicle properties
                // Note: For joins, the query needs to be built before applying Where on related entities.
                // This is tricky without directly joining here or passing a more complex predicate.
                // A simpler approach is to join first if makeIdFilter or searchTerm are present.

                if (makeIdFilter.HasValue && makeIdFilter.Value > 0)
                {
                    query = query.Where(cv => cv.Model.MakeId == makeIdFilter.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string termLower = searchTerm.Trim().ToLowerInvariant();
                    // To apply search on related tables for Count, you MUST include them or join.
                    // This simplified count might be slightly different if searchTerm relies on related tables
                    // unless those are filtered first. For exact match with GetFiltered, the query structure
                    // for filtering should be identical.
                    query = query.Where(cv =>
                        (cv.Model.Make.Name != null && cv.Model.Make.Name.ToLower().Contains(termLower)) ||
                        (cv.Model.Name != null && cv.Model.Name.ToLower().Contains(termLower)) ||
                        (cv.VIN != null && cv.VIN.ToLower().Contains(termLower)) ||
                        (cv.YearStart.ToString().Contains(termLower)) ||
                        (cv.YearEnd.ToString().Contains(termLower)) ||
                        (cv.TrimLevel != null && cv.TrimLevel.Name != null && cv.TrimLevel.Name.ToLower().Contains(termLower)) ||
                        (cv.EngineType != null && cv.EngineType.Name != null && cv.EngineType.Name.ToLower().Contains(termLower)) ||
                        (cv.BodyType != null && cv.BodyType.Name != null && cv.BodyType.Name.ToLower().Contains(termLower)) ||
                        (cv.TransmissionType != null && cv.TransmissionType.Name != null && cv.TransmissionType.Name.ToLower().Contains(termLower))
                    );
                }

                // Apply direct filter AFTER potential joins or include-based filters for count accuracy
                if (filter != null)
                {
                    query = query.Where(filter);
                }


                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total count of CompatibleVehicles.");
                throw new RepositoryException("Could not retrieve total count of vehicle specifications.", ex);
            }
        }
    }
}
