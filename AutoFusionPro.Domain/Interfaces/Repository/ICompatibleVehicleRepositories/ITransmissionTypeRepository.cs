using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;

namespace AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories
{
    public interface ITransmissionTypeRepository : IBaseRepository<TransmissionType>
    {
        Task<bool> NameExistsAsync(string name, int? excludeTransmissionTypeId = null);

    }
}
