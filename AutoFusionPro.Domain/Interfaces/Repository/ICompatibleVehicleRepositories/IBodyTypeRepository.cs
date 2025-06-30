using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories
{
    public interface IBodyTypeRepository : IBaseRepository<BodyType>
    {
        Task<bool> NameExistsAsync(string name, int? excludeBodyTypeId = null);
        Task<bool> IsUsedInCompatibilityRuleAttributesAsync(int bodyTypeId);

    }
}
