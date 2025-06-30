using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories
{
    public interface IEngineTypeRepository : IBaseRepository<EngineType>
    {
        Task<bool> NameExistsAsync(string name, int? excludeEngineTypeId = null);
        Task<bool> CodeExistsAsync(string? code, int? excludeEngineTypeId = null); // Code can be nullable
        Task<bool> IsUsedInCompatibilityRuleAttributesAsync(int engineTypeId);

    }
}
