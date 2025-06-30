using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories
{
    public interface ITrimLevelRepository : IBaseRepository<TrimLevel>
    {
        Task<bool> NameExistsForModelAsync(string trimLevelName, int modelId, int? excludeTrimLevelId = null);
        Task<IEnumerable<TrimLevel>> GetByModelIdAsync(int modelId);
        Task<TrimLevel?> GetByIdWithModelAsync(int trimLevelId); // Include Model for context
        Task<bool> IsUsedInCompatibilityRuleAttributesAsync(int trimLevelId);
    }
}
