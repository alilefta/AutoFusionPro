using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IVehicleRepository_OLD: IBaseRepository<Vehicle>
    {
        ///// <summary>
        ///// Checks if a Vehicle with the specified Make, Model, and Year already exists.
        ///// Often used before adding a new vehicle to prevent duplicates.
        ///// </summary>
        ///// <param name="make">Vehicle Make.</param>
        ///// <param name="model">Vehicle Model.</param>
        ///// <param name="year">Vehicle Year.</param>
        ///// <param name="excludeVehicleId">Optional ID of a vehicle to exclude from the check (for updates).</param>
        ///// <returns>True if an exact match exists for another vehicle, false otherwise.</returns>
        //Task<bool> MakeModelYearExistsAsync(string make, string model, int year, int? excludeVehicleId = null);

        //// GetByVinAsync can be removed if VIN is not used for lookup
        //Task<Vehicle?> GetByVinAsync(string vin);

        ///// <summary>
        ///// Gets a paginated list of vehicles, potentially filtered and ordered.
        ///// </summary>
        //Task<IEnumerable<Vehicle>> GetVehiclesPagedAsync(
        //    int pageNumber,
        //    int pageSize,
        //    Expression<Func<Vehicle, bool>>? filter = null
        //// Consider adding explicit orderBy parameter if needed (e.g., order by Make, Model, Year)
        //// Func<IQueryable<Vehicle>, IOrderedQueryable<Vehicle>>? orderBy = null
        //);

        ///// <summary>
        ///// Gets the total count of vehicles, potentially applying a filter.
        ///// </summary>
        //Task<int> GetTotalVehiclesCountAsync(Expression<Func<Vehicle, bool>>? filter = null);

        ///// <summary>
        ///// Gets a list of distinct vehicle makes present in the database.
        ///// </summary>
        //Task<IEnumerable<string>> GetDistinctMakesAsync();

        ///// <summary>
        ///// Gets a list of distinct vehicle models for a given make.
        ///// </summary>
        //Task<IEnumerable<string>> GetDistinctModelsAsync(string make);

        ///// <summary>
        ///// Gets a list of distinct years present in the database, potentially filtered by make/model.
        ///// </summary>
        //Task<IEnumerable<int>> GetDistinctYearsAsync(string? make = null, string? model = null);

        ///// <summary>
        ///// Checks if any PartCompatibility records exist for a specific Vehicle ID.
        ///// </summary>
        ///// <param name="vehicleId">The ID of the vehicle to check.</param>
        ///// <returns>True if compatibility links exist, false otherwise.</returns>
        //Task<bool> HasCompatibilityLinksAsync(int vehicleId);

        ///// <summary>
        ///// Checks if a VIN already exists in the system, optionally excluding a specific vehicle ID.
        ///// </summary>
        ///// <param name="vin">The VIN to check.</param>
        ///// <param name="excludeVehicleId">Optional ID to exclude (for updates).</param>
        ///// <returns>True if the VIN exists, false otherwise.</returns>
        //Task<bool> VinExistsAsync(string vin, int? excludeVehicleId = null);
    }
}
