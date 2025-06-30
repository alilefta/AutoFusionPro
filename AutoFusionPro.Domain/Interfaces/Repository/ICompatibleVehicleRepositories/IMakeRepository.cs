using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories
{
    public interface IMakeRepository : IBaseRepository<Make>
    {
        Task<bool> NameExistsAsync(string name, int? excludeMakeId = null);
        Task<bool> IsUsedInCompatibilityRulesAsync(int makeId);


    }
}
