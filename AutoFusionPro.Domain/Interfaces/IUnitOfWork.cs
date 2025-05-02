// Ignore Spelling: Denta

using AutoFusionPro.Domain.Interfaces.Repository;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IUserRepository Users {  get; } 
        IPartRepository Parts {  get; } 
        ICategoryRepository Categories {  get; } 
        IVehicleRepository Vehicles {  get; } 
        INotificationRepository Notifications {  get; } 

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    }
}
