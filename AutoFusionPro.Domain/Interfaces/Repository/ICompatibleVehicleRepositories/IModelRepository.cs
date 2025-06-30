using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories
{
    public interface IModelRepository : IBaseRepository<Model>
    {
        Task<bool> NameExistsForMakeAsync(string modelName, int makeId, int? excludeModelId = null);
        Task<IEnumerable<Model>> GetByMakeIdAsync(int makeId);
        Task<Model?> GetByIdWithMakeAsync(int modelId); // For getting Make.Name easily
        Task<bool> HasAssociatedTrimLevelsAsync(int modelId);
        Task<bool> IsUsedInCompatibilityRulesAsync(int modelId);
    }
}
