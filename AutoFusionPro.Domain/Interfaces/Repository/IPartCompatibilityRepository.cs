using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IPartCompatibilityRepository : IBaseRepository<PartCompatibility>
    {
        Task<IEnumerable<PartCompatibility>> GetByPartIdWithCompatibleVehicleDetailsAsync(int partId);
    }
}
