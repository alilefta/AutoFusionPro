using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class TransmissionTypeRepository : Repository<TransmissionType, TransmissionTypeRepository>, ITransmissionTypeRepository
    {
        public TransmissionTypeRepository(ApplicationDbContext context, ILogger<TransmissionTypeRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeTransmissionTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string nameLower = name.Trim().ToLowerInvariant();
            var query = _dbSet.Where(tt => tt.Name.ToLower() == nameLower);
            if (excludeTransmissionTypeId.HasValue)
            {
                query = query.Where(tt => tt.Id != excludeTransmissionTypeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
