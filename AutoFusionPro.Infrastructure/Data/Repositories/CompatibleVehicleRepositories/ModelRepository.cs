using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class ModelRepository : Repository<Model, ModelRepository>, IModelRepository
    {
        public ModelRepository(ApplicationDbContext context, ILogger<ModelRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsForMakeAsync(string modelName, int makeId, int? excludeModelId = null)
        {
            if (string.IsNullOrWhiteSpace(modelName) || makeId <= 0) return false;
            string nameLower = modelName.Trim().ToLowerInvariant();
            var query = _dbSet.Where(m => m.MakeId == makeId && m.Name.ToLower() == nameLower);
            if (excludeModelId.HasValue)
            {
                query = query.Where(m => m.Id != excludeModelId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Model>> GetByMakeIdAsync(int makeId)
        {
            if (makeId <= 0) return Enumerable.Empty<Model>();
            // Include Make for mapping to DTO if needed, or handle in service
            return await _dbSet.Include(m => m.Make)
                               .Where(m => m.MakeId == makeId)
                               .OrderBy(m => m.Name)
                               .ToListAsync();
        }

        public async Task<Model?> GetByIdWithMakeAsync(int modelId)
        {
            if (modelId <= 0) return null;
            return await _dbSet.Include(m => m.Make)
                               .FirstOrDefaultAsync(m => m.Id == modelId);
        }

        public async Task<bool> HasAssociatedTrimLevelsAsync(int modelId)
        {
            return await _context.Set<TrimLevel>().AnyAsync(t => t.ModelId == modelId);
        }

        public async Task<bool> HasAssociatedCompatibleVehiclesAsync(int modelId)
        {
            return await _context.Set<CompatibleVehicle>().AnyAsync(cv => cv.ModelId == modelId);
        }
    }
}
