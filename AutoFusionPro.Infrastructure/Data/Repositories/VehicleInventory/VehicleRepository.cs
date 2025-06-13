using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Linq;

namespace AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory
{
    public class VehicleRepository : Repository<Vehicle, VehicleRepository>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context, ILogger<VehicleRepository> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Gets a single Vehicle by its ID, including all its detailed collections
        /// (Images, DamageLogs with their Images, ServiceRecords, Documents) and
        /// related lookup entity names (Make, Model, Trim, EngineType, etc.).
        /// </summary>
        public async Task<Vehicle?> GetByIdWithAllDetailsAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to get vehicle with all details using invalid ID: {VehicleId}. Returning null.", vehicleId);
                return null;
            }

            _logger.LogDebug("Attempting to retrieve vehicle with all details for ID: {VehicleId}", vehicleId);
            try
            {
                return await _dbSet
                    // Include direct lookup properties
                    .Include(v => v.Make)
                    .Include(v => v.Model) // Model already includes Make by convention if MakeId is FK, but explicit can be clearer or useful if Make is also a direct nav prop on Vehicle
                    .Include(v => v.TrimLevel)
                    .Include(v => v.TransmissionType)
                    .Include(v => v.EngineType)
                    .Include(v => v.BodyType)
                    .Include(v => v.SoldToCustomer) // Include customer if sold

                    // Include collections
                    .Include(v => v.Images.OrderBy(img => img.DisplayOrder).ThenBy(img => img.Id)) // Order images
                    .Include(v => v.ServiceRecords.OrderByDescending(sr => sr.ServiceDate).ThenByDescending(sr => sr.Id)) // Order service records
                    .Include(v => v.Documents.OrderBy(d => d.DocumentType).ThenBy(d => d.DocumentName)) // Order documents

                    // Include DamageLogs and their nested Images
                    .Include(v => v.DamageLogs.OrderByDescending(dl => dl.DateNoted).ThenByDescending(dl => dl.Id)) // Order damage logs
                        .ThenInclude(dl => dl.DamageImages.OrderBy(di => di.Id)) // Order images within each damage log

                    .AsNoTracking() // Recommended for complex read-only queries if entities are not modified
                    .FirstOrDefaultAsync(v => v.Id == vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle with all details for ID {VehicleId}.", vehicleId);
                throw new RepositoryException($"Could not retrieve vehicle with all details for ID {vehicleId}.", ex);
            }
        }

        /// <summary>
        /// Gets a single Vehicle by its VIN, optionally including all details.
        /// </summary>
        public async Task<Vehicle?> GetByVinAsync(string vin, bool includeAllDetails = false)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                _logger.LogWarning("Attempted to get vehicle with null or empty VIN. Returning null.");
                return null;
            }

            string normalizedVin = vin.Trim().ToUpperInvariant(); // Normalize VIN for consistent lookup
            _logger.LogDebug("Attempting to retrieve vehicle by VIN: {VIN}, IncludeAllDetails: {IncludeDetails}", normalizedVin, includeAllDetails);

            try
            {
                if (includeAllDetails)
                {
                    // If all details are requested, we find the ID first then use the more comprehensive method
                    // This avoids duplicating the complex Include logic.
                    var vehicleId = await _dbSet
                                        .Where(v => v.VIN == normalizedVin)
                                        .Select(v => v.Id)
                                        .FirstOrDefaultAsync();

                    if (vehicleId > 0)
                    {
                        return await GetByIdWithAllDetailsAsync(vehicleId);
                    }
                    return null; // VIN not found
                }
                else
                {
                    // Fetch basic vehicle info plus its direct lookups (Make, Model, etc.)
                    // No collections are loaded in this case for performance.
                    return await _dbSet
                        .Include(v => v.Make)
                        .Include(v => v.Model)
                        .Include(v => v.TrimLevel)
                        .Include(v => v.TransmissionType)
                        .Include(v => v.EngineType)
                        .Include(v => v.BodyType)
                        .AsNoTracking() // Good for read-only
                        .FirstOrDefaultAsync(v => v.VIN == normalizedVin);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle by VIN '{VIN}'. IncludeAllDetails: {IncludeDetails}", vin, includeAllDetails);
                throw new RepositoryException($"Could not retrieve vehicle with VIN '{vin}'.", ex);
            }
        }


        /// <summary>
        /// Checks if a VIN already exists, optionally excluding a specific Vehicle ID.
        /// Only considers non-null, non-empty VINs for uniqueness checks.
        /// </summary>
        public async Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null)
        {
            // If VIN is optional and can be empty/null, an empty/null VIN doesn't typically participate in uniqueness.
            // This check is for when a non-empty VIN is provided.
            if (string.IsNullOrWhiteSpace(vin))
            {
                _logger.LogDebug("VinExistsAsync called with null or empty VIN. Returning false as it cannot clash if not provided.");
                return false;
            }

            string normalizedVin = vin.Trim().ToUpperInvariant(); // Normalize VIN for consistent comparison
            _logger.LogDebug("Checking VIN existence for: {NormalizedVIN}, ExcludeVehicleId: {ExcludeVehicleId}",
                normalizedVin, excludeVehicleId ?? 0);

            try
            {
                IQueryable<Vehicle> query = _dbSet
                    .Where(v => v.VIN != null && v.VIN.ToUpper() == normalizedVin);

                if (excludeVehicleId.HasValue && excludeVehicleId.Value > 0)
                {
                    query = query.Where(v => v.Id != excludeVehicleId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking VIN existence for '{VIN}'.", vin);
                throw new RepositoryException($"Could not check existence for VIN '{vin}'.", ex);
            }
        }

        /// <summary>
        /// Checks if a Registration Plate Number already exists, optionally scoped by country/state
        /// and excluding a specific Vehicle ID.
        /// Only considers non-null, non-empty plate numbers for uniqueness checks.
        /// </summary>
        public async Task<bool> RegistrationPlateExistsAsync(string plateNumber, string? countryOrState, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                _logger.LogDebug("RegistrationPlateExistsAsync called with null or empty plate number. Returning false.");
                return false;
            }

            // Normalize plate number: typically to uppercase and remove spaces/hyphens depending on regional rules
            // For this example, just trimming and uppercasing. You might need more sophisticated normalization.
            string normalizedPlateNumber = plateNumber.Trim().ToUpperInvariant();
            string? normalizedCountryOrState = string.IsNullOrWhiteSpace(countryOrState) ? null : countryOrState.Trim().ToUpperInvariant();

            _logger.LogDebug("Checking Registration Plate existence for: Plate='{NormalizedPlate}', Country/State='{NormalizedCountryState}', ExcludeVehicleId: {ExcludeVehicleId}",
                normalizedPlateNumber, normalizedCountryOrState ?? "<any>", excludeVehicleId ?? 0);

            try
            {
                IQueryable<Vehicle> query = _dbSet
                    .Where(v => v.RegistrationPlateNumber != null &&
                                v.RegistrationPlateNumber.ToUpper() == normalizedPlateNumber);

                // Scope by country/state if provided
                if (normalizedCountryOrState != null)
                {
                    query = query.Where(v => v.RegistrationCountryOrState != null &&
                                             v.RegistrationCountryOrState.ToUpper() == normalizedCountryOrState);
                }
                // If normalizedCountryOrState is null, we are checking for global uniqueness of the plate number,
                // or uniqueness among vehicles that also have a null/empty RegistrationCountryOrState.
                // To be truly global regardless of country/state being set or not (if that's the business rule):
                // This part depends on whether a plate number can be unique globally or only within a country/state.
                // The current logic checks for uniqueness given the provided country/state (or among those with no country/state if `countryOrState` is null).

                if (excludeVehicleId.HasValue && excludeVehicleId.Value > 0)
                {
                    query = query.Where(v => v.Id != excludeVehicleId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Registration Plate existence for Plate='{PlateNumber}', Country/State='{CountryOrState}'.", plateNumber, countryOrState);
                throw new RepositoryException($"Could not check existence for Registration Plate '{plateNumber}'.", ex);
            }
        }


        /// <summary>
        /// Gets a paginated list of Vehicle entities based on filter criteria.
        /// Includes summary-level details (MakeName, ModelName, PrimaryImage, etc.).
        /// </summary>
        public async Task<IEnumerable<Vehicle>> GetFilteredVehiclesPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Vehicle, bool>>? filterPredicate = null,
            int? makeId = null,
            int? modelId = null,
            int? minYear = null,
            int? maxYear = null,
            VehicleStatus? status = null,
            string? searchTerm = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 200) pageSize = 200; // Max page size safeguard

            _logger.LogDebug("Getting filtered paged vehicles. Page: {Page}, Size: {Size}, MakeId: {MakeId}, ModelId: {ModelId}, MinYear: {MinY}, MaxYear: {MaxY}, Status: {Status}, Search: '{Search}'",
                pageNumber, pageSize, makeId, modelId, minYear, maxYear, status, searchTerm);

            try
            {
                IQueryable<Vehicle> query = _dbSet
                    .Include(v => v.Make)  // For MakeName in SummaryDto
                    .Include(v => v.Model)  // For ModelName in SummaryDto
                    .Include(v => v.Images.Where(img => img.IsPrimary).OrderBy(img => img.Id).Take(1)); // To get the primary image for SummaryDto
                                                                                                        // Using .Where().OrderBy().Take(1) is a common way to ensure only one primary is loaded per vehicle efficiently
                                                                                                        // Some DB providers might require AsSplitQuery() for optimal performance with collection includes in .NET 6+

                // Apply direct predicate filter first (if any)
                if (filterPredicate != null)
                {
                    query = query.Where(filterPredicate);
                }

                // Apply specific criteria
                if (makeId.HasValue && makeId.Value > 0)
                {
                    query = query.Where(v => v.MakeId == makeId.Value);
                }

                if (modelId.HasValue && modelId.Value > 0)
                {
                    query = query.Where(v => v.ModelId == modelId.Value);
                }

                if (minYear.HasValue)
                {
                    query = query.Where(v => v.Year >= minYear.Value);
                }

                if (maxYear.HasValue)
                {
                    query = query.Where(v => v.Year <= maxYear.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(v => v.Status == status.Value);
                }

                // Apply generic search term
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string termLower = searchTerm.Trim().ToLowerInvariant();
                    query = query.Where(v =>
                        (v.VIN != null && v.VIN.ToLower().Contains(termLower)) ||
                        (v.Make.Name != null && v.Make.Name.ToLower().Contains(termLower)) || // Relies on Make being included
                        (v.Model.Name != null && v.Model.Name.ToLower().Contains(termLower)) || // Relies on Model being included
                        (v.RegistrationPlateNumber != null && v.RegistrationPlateNumber.ToLower().Contains(termLower)) ||
                        (v.ExteriorColor != null && v.ExteriorColor.ToLower().Contains(termLower)) ||
                        (v.Year.ToString().Contains(termLower)) // Search year as string
                    );
                }

                // Apply stable ordering for consistent pagination
                query = query.OrderBy(v => v.Make.Name)    // Order by Make Name
                             .ThenBy(v => v.Model.Name) // Then by Model Name
                             .ThenByDescending(v => v.Year) // Then by Year (newest first)
                             .ThenBy(v => v.Id);           // Final tie-breaker

                return await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking() // Good for read-only lists
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged and filtered vehicles. Page: {Page}, Size: {Size}, Filters: MakeId={MakeId},ModelId={ModelId},YearRange={MinY}-{MaxY},Status={Status},Search='{Search}'",
                    pageNumber, pageSize, makeId, modelId, minYear, maxYear, status, searchTerm);
                throw new RepositoryException("Could not retrieve paged vehicles.", ex);
            }
        }

        /// <summary>
        /// Gets the total count of Vehicle entities matching the specified filter criteria.
        /// Parameters should mirror GetFilteredVehiclesPagedAsync.
        /// </summary>
        public async Task<int> GetTotalFilteredVehiclesCountAsync(
            Expression<Func<Vehicle, bool>>? filterPredicate = null,
            int? makeId = null,
            int? modelId = null,
            int? minYear = null,
            int? maxYear = null,
            VehicleStatus? status = null,
            string? searchTerm = null)
        {
            _logger.LogDebug("Getting total filtered vehicles count. Filters: MakeId={MakeId},ModelId={ModelId},YearRange={MinY}-{MaxY},Status={Status},Search='{Search}'",
                 makeId, modelId, minYear, maxYear, status, searchTerm);
            try
            {
                IQueryable<Vehicle> query = _dbSet; // Start with the base DbSet, no includes needed for Count

                // Apply direct predicate filter first
                if (filterPredicate != null)
                {
                    query = query.Where(filterPredicate);
                }

                // Apply specific criteria
                if (makeId.HasValue && makeId.Value > 0)
                {
                    query = query.Where(v => v.MakeId == makeId.Value);
                }

                if (modelId.HasValue && modelId.Value > 0)
                {
                    query = query.Where(v => v.ModelId == modelId.Value);
                }

                if (minYear.HasValue)
                {
                    query = query.Where(v => v.Year >= minYear.Value);
                }

                if (maxYear.HasValue)
                {
                    query = query.Where(v => v.Year <= maxYear.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(v => v.Status == status.Value);
                }

                // Apply generic search term
                // IMPORTANT: For CountAsync, if searching related entity properties (Make.Name, Model.Name),
                // EF Core needs to know about the join.
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string termLower = searchTerm.Trim().ToLowerInvariant();
                    query = query.Where(v =>
                        (v.VIN != null && v.VIN.ToLower().Contains(termLower)) ||
                        // For related entities in a COUNT query, if not explicitly joining,
                        // ensure the DB can optimize or the filter is on indexed FKs.
                        // Here, we assume Make and Model are related via FKs and EF Core can translate this.
                        // If performance is an issue, this might need explicit joins or separate checks.
                        (v.Make.Name != null && v.Make.Name.ToLower().Contains(termLower)) ||
                        (v.Model.Name != null && v.Model.Name.ToLower().Contains(termLower)) ||
                        (v.RegistrationPlateNumber != null && v.RegistrationPlateNumber.ToLower().Contains(termLower)) ||
                        (v.ExteriorColor != null && v.ExteriorColor.ToLower().Contains(termLower)) ||
                        (v.Year.ToString().Contains(termLower))
                    );
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total count of filtered vehicles. Filters: MakeId={MakeId},ModelId={ModelId},YearRange={MinY}-{MaxY},Status={Status},Search='{Search}'",
                    makeId, modelId, minYear, maxYear, status, searchTerm);
                throw new RepositoryException("Could not retrieve total count of vehicles.", ex);
            }
        }

    }
}
