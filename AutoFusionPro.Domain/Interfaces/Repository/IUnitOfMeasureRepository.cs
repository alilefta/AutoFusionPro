using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IUnitOfMeasureRepository : IBaseRepository<UnitOfMeasure>
    {

        /// <summary>
        /// Checks if a Unit of Measure name already exists, optionally under a specific parent.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <param name="excludeUnitOfMeasureId">Optional ID of a UnitOfMeasure to exclude from the check (for updates).</param>
        /// <returns>True if the name exists under the specified parent, false otherwise.</returns>
        Task<bool> NameExistsAsync(string name, int? excludeUnitOfMeasureId = null);

        /// <summary>
        /// Checks if a Unit of Measure symbol already exists, optionally under a specific parent.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <param name="excludeUnitOfMeasureId">Optional ID of a UnitOfMeasure to exclude from the check (for updates).</param>
        /// <returns>True if the symbol exists under the specified parent, false otherwise.</returns>
        Task<bool> SymbolExistsAsync(string symbol, int? excludeUnitOfMeasureId = null);
    }
}
