using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class VehicleRepository : Repository<Vehicle, VehicleRepository>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context, ILogger<VehicleRepository> logger) : base(context, logger)
        {
        }

        public async Task<Vehicle?> GetByVinAsync(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin)) return null;
            string vinUpper = vin.Trim().ToUpperInvariant(); // Normalize VIN
            try
            {
                return await _dbSet.FirstOrDefaultAsync(v => v.VIN != null && v.VIN == vinUpper);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Vehicle by VIN '{VIN}'.", vin);
                throw new RepositoryException($"Could not retrieve vehicle with VIN {vin}.", ex);
            }
        }

        /// <summary>
        /// Gets a list of distinct vehicle makes present in the database.
        /// </summary>
        public async Task<IEnumerable<string>> GetDistinctMakesAsync()
        {
            try
            {
                return await _dbSet
                    .Select(v => v.Make)
                    .Where(m => !string.IsNullOrEmpty(m)) // Ensure not null or empty
                    .Distinct()
                    .OrderBy(m => m) // Order alphabetically
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distinct vehicle makes.");
                throw new RepositoryException("Could not retrieve distinct makes.", ex);
            }
        }

        /// <summary>
        /// Gets a list of distinct vehicle models for a given make.
        /// </summary>
        public async Task<IEnumerable<string>> GetDistinctModelsAsync(string make)
        {
            if (string.IsNullOrWhiteSpace(make))
            {
                return Enumerable.Empty<string>();
            }
            string makeLower = make.Trim().ToLowerInvariant();

            try
            {
                return await _dbSet
                    .Where(v => v.Make != null && v.Make.ToLower() == makeLower) // Filter by make
                    .Select(v => v.Model)
                    .Where(m => !string.IsNullOrEmpty(m)) // Ensure not null or empty
                    .Distinct()
                    .OrderBy(m => m) // Order alphabetically
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distinct vehicle models for Make '{Make}'.", make);
                throw new RepositoryException($"Could not retrieve distinct models for make '{make}'.", ex);
            }
        }

        /// <summary>
        /// Gets a list of distinct years present in the database, potentially filtered by make/model.
        /// </summary>
        public async Task<IEnumerable<int>> GetDistinctYearsAsync(string? make = null, string? model = null)
        {
            try
            {
                IQueryable<Vehicle> query = _dbSet;

                if (!string.IsNullOrWhiteSpace(make))
                {
                    string makeLower = make.Trim().ToLowerInvariant();
                    query = query.Where(v => v.Make != null && v.Make.ToLower() == makeLower);
                }

                if (!string.IsNullOrWhiteSpace(model))
                {
                    string modelLower = model.Trim().ToLowerInvariant();
                    query = query.Where(v => v.Model != null && v.Model.ToLower() == modelLower);
                }

                return await query
                    .Select(v => v.Year)
                    .Distinct()
                    .OrderByDescending(y => y) // Show newest years first
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distinct vehicle years (Make: {Make}, Model: {Model}).", make, model);
                throw new RepositoryException("Could not retrieve distinct years.", ex);
            }
        }

        /// <summary>
        /// Gets the total count of vehicles, potentially applying a filter.
        /// NOTE: This simply uses the base class implementation.
        /// Defined explicitly in interface for clarity.
        /// </summary>
        public async Task<int> GetTotalVehiclesCountAsync(Expression<Func<Vehicle, bool>>? filter = null)
        {
            // Leverage the base class implementation
            return await base.CountAsync(filter);
        }

        /// <summary>
        /// Gets a paginated list of vehicles, potentially filtered and ordered.
        /// </summary>
        public async Task<IEnumerable<Vehicle>> GetVehiclesPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Vehicle, bool>>? filter = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; // Or a configurable default

            try
            {
                IQueryable<Vehicle> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                // Apply stable ordering for consistent pagination
                query = query.OrderBy(v => v.Make)
                             .ThenBy(v => v.Model)
                             .ThenBy(v => v.Year)
                             .ThenBy(v => v.Id); // Add ID for final tie-breaking

                return await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged vehicles (Page: {PageNumber}, Size: {PageSize})", pageNumber, pageSize);
                throw new RepositoryException("Could not retrieve paged vehicles.", ex);
            }
        }

        /// <summary>
        /// Checks if any PartCompatibility records exist for a specific Vehicle ID.
        /// </summary>
        public async Task<bool> HasCompatibilityLinksAsync(int compatibileVehicle)
        {
            if (compatibileVehicle <= 0) return false;

            try
            {
                // Access the DbContext (_context field from base class)
                return await _context.Set<PartCompatibility>()
                                     .AnyAsync(pc => pc.CompatibleVehicleId == compatibileVehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for compatibility links for Vehicle ID {VehicleId}", compatibileVehicle);
                throw new RepositoryException($"Could not check compatibility links for Vehicle ID {compatibileVehicle}.", ex);
            }
        }

        /// <summary>
        /// Checks if a Vehicle with the specified Make, Model, and Year already exists.
        /// </summary>
        public async Task<bool> MakeModelYearExistsAsync(string make, string model, int year, int? excludeVehicleId = null)
        {
            // Normalize inputs for reliable comparison
            string makeLower = make?.Trim().ToLowerInvariant() ?? string.Empty;
            string modelLower = model?.Trim().ToLowerInvariant() ?? string.Empty;

            if (string.IsNullOrEmpty(makeLower) || string.IsNullOrEmpty(modelLower) || year < 1900)
            {
                // Cannot check existence with invalid key parts
                _logger.LogWarning("Attempted MakeModelYearExistsAsync check with invalid input: Make='{Make}', Model='{Model}', Year={Year}", make, model, year);
                return false; // Or throw ArgumentException if needed
            }

            try
            {
                var query = _dbSet.Where(v =>
                    v.Make != null && v.Make.ToLower() == makeLower &&
                    v.Model != null && v.Model.ToLower() == modelLower &&
                    v.Year == year);

                if (excludeVehicleId.HasValue)
                {
                    query = query.Where(v => v.Id != excludeVehicleId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence for Vehicle: Make={Make}, Model={Model}, Year={Year}", make, model, year);
                throw new RepositoryException($"Could not check existence for vehicle {make} {model} {year}.", ex);
            }
        }

        public async Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(vin)) return false;
            string vinUpper = vin.Trim().ToUpperInvariant();
            try
            {
                var query = _dbSet.Where(v => v.VIN != null && v.VIN == vinUpper);
                if (excludeVehicleId.HasValue)
                {
                    query = query.Where(v => v.Id != excludeVehicleId.Value);
                }
                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence for VIN '{VIN}'.", vin);
                throw new RepositoryException($"Could not check existence for VIN {vin}.", ex);
            }
        }
    }
}
