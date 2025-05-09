using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class TrimLevelRepository : Repository<TrimLevel, TrimLevelRepository>, ITrimLevelRepository
    {
        public TrimLevelRepository(ApplicationDbContext context, ILogger<TrimLevelRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsForModelAsync(string trimLevelName, int modelId, int? excludeTrimLevelId = null)
        {
            if (string.IsNullOrWhiteSpace(trimLevelName) || modelId <= 0) return false;
            string nameLower = trimLevelName.Trim().ToLowerInvariant();
            var query = _dbSet.Where(t => t.ModelId == modelId && t.Name.ToLower() == nameLower);
            if (excludeTrimLevelId.HasValue)
            {
                query = query.Where(t => t.Id != excludeTrimLevelId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<TrimLevel>> GetByModelIdAsync(int modelId)
        {
            if (modelId <= 0) return Enumerable.Empty<TrimLevel>();
            return await _dbSet.Include(t => t.Model) // Include Model for ModelName in DTO
                               .Where(t => t.ModelId == modelId)
                               .OrderBy(t => t.Name)
                               .ToListAsync();
        }

        public async Task<TrimLevel?> GetByIdWithModelAsync(int trimLevelId)
        {
            if (trimLevelId <= 0) return null;
            return await _dbSet.Include(t => t.Model) // Include Model
                               .ThenInclude(m => m.Make) // Optionally include Model's Make
                               .FirstOrDefaultAsync(t => t.Id == trimLevelId);
        }

        public async Task<bool> HasAssociatedCompatibleVehiclesAsync(int trimLevelId)
        {
            return await _context.Set<CompatibleVehicle>().AnyAsync(cv => cv.TrimLevelId == trimLevelId);
        }
    }
}
