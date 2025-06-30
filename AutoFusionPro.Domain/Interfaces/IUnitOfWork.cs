// Ignore Spelling: Denta

using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Interfaces.Repository.PartCompatibilityRulesRepositories;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable // Ensure these are inherited
    {
        // Repository Properties
        ICategoryRepository Categories { get; }
        IUserRepository Users { get; }
        IPartRepository Parts { get; }
        INotificationRepository Notifications { get; } // Assuming this exists
        IMakeRepository Makes { get; }
        IModelRepository Models { get; }
        IBodyTypeRepository BodyTypes { get; }
        IEngineTypeRepository EngineTypes { get; }
        ITransmissionTypeRepository TransmissionTypes { get; }
        ITrimLevelRepository TrimLevels { get; }
        ISupplierRepository Suppliers { get; }
        IInventoryTransactionRepository InventoryTransactions { get; }
        IUnitOfMeasureRepository UnitOfMeasures { get; }
        IVehicleDocumentRepository VehicleDocuments { get; }
        IVehicleDamageImageRepository VehicleDamageImages { get; }
        IVehicleServiceHistoryRepository VehicleServiceHistories { get; }
        IVehicleDamageLogRepository VehicleDamageLogs { get; }
        IVehicleImageRepository VehicleImages { get; }
        IVehicleRepository Vehicles { get; }
        ISupplierPartRepository SupplierParts { get; }

        IPartCompatibilityRuleRepository PartCompatibilityRules { get; }
        IPartCompatibilityRuleBodyTypeRepository PartCompatibilityRuleBodyTypes { get; }
        IPartCompatibilityRuleEngineTypeRepository PartCompatibilityRuleEngineTypes { get; }
        IPartCompatibilityRuleTransmissionTypeRepository PartCompatibilityRuleTransmissionTypes { get; }
        IPartCompatibilityRuleTrimLevelRepository PartCompatibilityRuleTrimLevels { get; }

        IPartImageRepository PartImages { get; }


        // Transaction Management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);

        // Data Modification & Querying Helpers (directly on UoW for convenience)
        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class;
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        // Can be used for general delete
        // void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity; // If it's synchronous marking
    }
}
