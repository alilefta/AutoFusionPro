using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    internal class MakeRepository : Repository<Make, MakeRepository>, IMakeRepository
    {
        public MakeRepository(ApplicationDbContext context, ILogger<MakeRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeMakeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string nameLower = name.Trim().ToLowerInvariant();
            var query = _dbSet.Where(m => m.Name.ToLower() == nameLower);
            if (excludeMakeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeMakeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
