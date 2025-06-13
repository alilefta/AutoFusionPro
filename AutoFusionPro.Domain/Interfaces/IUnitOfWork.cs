// Ignore Spelling: Denta

using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IUserRepository Users {  get; } 
        IPartRepository Parts {  get; } 
        ICategoryRepository Categories {  get; } 
        INotificationRepository Notifications {  get; } 

        ICompatibleVehicleRepository CompatibleVehicles { get; }
        IMakeRepository Makes { get; }
        IModelRepository Models { get; }
        IBodyTypeRepository BodyTypes { get; }
        IEngineTypeRepository EngineTypes { get; }
        ITransmissionTypeRepository TransmissionTypes { get; }
        ITrimLevelRepository TrimLevels { get; }
        ISupplierRepository Suppliers { get; }
        IInventoryTransactionRepository InventoryTransactions { get; }

        IPartCompatibilityRepository PartCompatibilities { get; }
        ISupplierPartRepository SupplierParts { get; }

        IUnitOfMeasureRepository UnitOfMeasures { get; }

        IVehicleDocumentRepository VehicleDocuments { get; }
        IVehicleDamageImageRepository VehicleDamageImages { get; }
        IVehicleServiceHistoryRepository VehicleServiceHistories { get; }
        IVehicleDamageLogRepository VehicleDamageLogs { get; }
        IVehicleImageRepository VehicleImages { get; }
        IVehicleRepository Vehicles { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        // New overload with CancellationToken
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default); // Add CancellationToken here too
        Task CommitTransactionAsync(CancellationToken cancellationToken = default); // And here
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default); // And here


        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    }
}
