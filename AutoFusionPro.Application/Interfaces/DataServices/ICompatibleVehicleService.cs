using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Core.Models;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    /// <summary>
    /// Defines application service operations for managing CompatibleVehicle configurations.
    /// These configurations are used for determining part compatibility.
    /// </summary>
    public interface ICompatibleVehicleService
    {
        // --- CompatibleVehicle CRUD ---
        Task<CompatibleVehicleDetailDto?> GetCompatibleVehicleByIdAsync(int id);
        Task<PagedResult<CompatibleVehicleSummaryDto>> GetFilteredCompatibleVehiclesAsync(
            CompatibleVehicleFilterCriteriaDto filterCriteria, int pageNumber, int pageSize);
        Task<CompatibleVehicleDetailDto> CreateCompatibleVehicleAsync(CreateCompatibleVehicleDto createDto);
        Task UpdateCompatibleVehicleAsync(UpdateCompatibleVehicleDto updateDto);
        Task DeleteCompatibleVehicleAsync(int id);
        Task<bool> CompatibleVehicleSpecExistsAsync(
                    int modelId, int yearStart, int yearEnd,
                    int? trimLevelId, int? transmissionTypeId, int? engineTypeId, int? bodyTypeId,
                    int? excludeCompatibleVehicleId = null);

        // --- Make Management ---
        Task<IEnumerable<MakeDto>> GetAllMakesAsync();
        Task<MakeDto?> GetMakeByIdAsync(int id);
        Task<MakeDto> CreateMakeAsync(CreateMakeDto createDto);
        Task UpdateMakeAsync(UpdateMakeDto updateDto);
        Task DeleteMakeAsync(int id); // Check for dependent Models

        // --- Model Management ---
        Task<IEnumerable<ModelDto>> GetModelsByMakeIdAsync(int makeId);
        Task<ModelDto?> GetModelByIdAsync(int id);
        Task<ModelDto> CreateModelAsync(CreateModelDto createDto);
        Task UpdateModelAsync(UpdateModelDto updateDto);
        Task DeleteModelAsync(int id); // Check for dependent TrimLevels & CompatibleVehicles

        // --- TrimLevel Management ---
        Task<IEnumerable<TrimLevelDto>> GetTrimLevelsByModelIdAsync(int modelId);
        Task<TrimLevelDto?> GetTrimLevelByIdAsync(int id);
        Task<TrimLevelDto> CreateTrimLevelAsync(CreateTrimLevelDto createDto);
        Task UpdateTrimLevelAsync(UpdateTrimLevelDto updateDto);
        Task DeleteTrimLevelAsync(int id); // Check for dependent CompatibleVehicles

        // --- TransmissionType Management ---
        Task<IEnumerable<TransmissionTypeDto>> GetAllTransmissionTypesAsync();
        Task<TransmissionTypeDto?> GetTransmissionTypeByIdAsync(int id);
        Task<TransmissionTypeDto> CreateTransmissionTypeAsync(CreateLookupDto createDto); // Generic DTO for simple lookups
        Task UpdateTransmissionTypeAsync(UpdateLookupDto updateDto);
        Task DeleteTransmissionTypeAsync(int id); // Check for dependent CompatibleVehicles

        // --- EngineType Management ---
        Task<IEnumerable<EngineTypeDto>> GetAllEngineTypesAsync();
        Task<EngineTypeDto?> GetEngineTypeByIdAsync(int id);
        Task<EngineTypeDto> CreateEngineTypeAsync(CreateEngineTypeDto createDto); // Specific if it has Code
        Task UpdateEngineTypeAsync(UpdateEngineTypeDto updateDto);
        Task DeleteEngineTypeAsync(int id); // Check for dependent CompatibleVehicles

        // --- BodyType Management ---
        Task<IEnumerable<BodyTypeDto>> GetAllBodyTypesAsync();
        Task<BodyTypeDto?> GetBodyTypeByIdAsync(int id);
        Task<BodyTypeDto> CreateBodyTypeAsync(CreateLookupDto createDto);
        Task UpdateBodyTypeAsync(UpdateLookupDto updateDto);
        Task DeleteBodyTypeAsync(int id); // Check for dependent CompatibleVehicles
    }
}
