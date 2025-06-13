using AutoFusionPro.Application.DTOs.UnitOfMeasure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    /// <summary>
    /// Defines application service operations for managing Units of Measure.
    /// </summary>
    public interface IUnitOfMeasureService
    {
        /// <summary>
        /// Gets a single Unit of Measure by its ID.
        /// </summary>
        /// <param name="id">The ID of the Unit of Measure.</param>
        /// <returns>A UnitOfMeasureDto or null if not found.</returns>
        Task<UnitOfMeasureDto?> GetUnitOfMeasureByIdAsync(int id);

        /// <summary>
        /// Gets all Units of Measure, typically for populating selection controls.
        /// Results are usually ordered by Name.
        /// </summary>
        /// <returns>A collection of UnitOfMeasureDto.</returns>
        Task<IEnumerable<UnitOfMeasureDto>> GetAllUnitOfMeasuresAsync();

        /// <summary>
        /// Creates a new Unit of Measure.
        /// </summary>
        /// <param name="createDto">Data for the new Unit of Measure.</param>
        /// <returns>The DTO of the newly created Unit of Measure.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid (e.g., name/symbol not unique).</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown for other errors.</exception>
        Task<UnitOfMeasureDto> CreateUnitOfMeasureAsync(CreateUnitOfMeasureDto createDto);

        /// <summary>
        /// Updates an existing Unit of Measure's details.
        /// </summary>
        /// <param name="updateDto">Data containing the ID and updated fields.</param>
        /// <returns>Task indicating completion.</returns>
        /// <exception cref="FluentValidation.ValidationException">Thrown if input data is invalid.</exception>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown if Unit of Measure not found or other errors.</exception>
        Task UpdateUnitOfMeasureAsync(UpdateUnitOfMeasureDto updateDto);

        /// <summary>
        /// Deletes a Unit of Measure.
        /// Checks for dependencies (e.g., if used by any Part) before deletion.
        /// </summary>
        /// <param name="id">The ID of the Unit of Measure to delete.</param>
        /// <returns>Task indicating completion.</returns>
        /// <exception cref="AutoFusionPro.Core.Exceptions.ServiceException">Thrown if not found or cannot be deleted due to dependencies.</exception>
        Task DeleteUnitOfMeasureAsync(int id);

        // Optional: Specific validation helpers if needed by other services/UI directly,
        // though usually Create/Update will handle this.
        // Task<bool> IsUnitOfMeasureNameUniqueAsync(string name, int? excludeId = null);
        // Task<bool> IsUnitOfMeasureSymbolUniqueAsync(string symbol, int? excludeId = null);
    }
}
