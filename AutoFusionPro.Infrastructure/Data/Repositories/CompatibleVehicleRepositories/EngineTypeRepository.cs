using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class EngineTypeRepository : Repository<EngineType, EngineTypeRepository>, IEngineTypeRepository
    {
        public EngineTypeRepository(ApplicationDbContext context, ILogger<EngineTypeRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeEngineTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string nameLower = name.Trim().ToLowerInvariant();
            var query = _dbSet.Where(et => et.Name.ToLower() == nameLower);
            if (excludeEngineTypeId.HasValue)
            {
                query = query.Where(et => et.Id != excludeEngineTypeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> CodeExistsAsync(string? code, int? excludeEngineTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(code)) return false; // If code is empty, it can't clash (unless DB has empty codes)
            string codeUpper = code.Trim().ToUpperInvariant(); // Normalize code
            var query = _dbSet.Where(et => et.Code != null && et.Code.ToUpper() == codeUpper);
            if (excludeEngineTypeId.HasValue)
            {
                query = query.Where(et => et.Id != excludeEngineTypeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
