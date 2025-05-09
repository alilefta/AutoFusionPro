using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories
{
    public class CompatibleVehicleRepository : Repository<CompatibleVehicle, CompatibleVehicleRepository>, ICompatibleVehicleRepository
    {
        public CompatibleVehicleRepository(ApplicationDbContext context, ILogger<CompatibleVehicleRepository> logger) : base(context, logger)
        {
        }

        public Task<CompatibleVehicle?> GetByIdWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CompatibleVehicle>> GetFilteredWithDetailsPagedAsync(int pageNumber, int pageSize, Expression<Func<CompatibleVehicle, bool>>? filter = null, int? makeIdFilter = null, string? searchTerm = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalCountAsync(Expression<Func<CompatibleVehicle, bool>>? filter = null, int? makeIdFilter = null, string? searchTerm = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SpecificationExistsAsync(int modelId, int yearStart, int yearEnd, int? trimLevelId, int? transmissionTypeId, int? engineTypeId, int? bodyTypeId, int? excludeCompatibleVehicleId = null)
        {
            throw new NotImplementedException();
        }
    }
}
