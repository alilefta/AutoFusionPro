using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class BodyTypeRepository : Repository<BodyType, BodyTypeRepository>, IBodyTypeRepository
    {
        public BodyTypeRepository(ApplicationDbContext context, ILogger<BodyTypeRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeBodyTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string nameLower = name.Trim().ToLowerInvariant();
            var query = _dbSet.Where(bt => bt.Name.ToLower() == nameLower);
            if (excludeBodyTypeId.HasValue)
            {
                query = query.Where(bt => bt.Id != excludeBodyTypeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
