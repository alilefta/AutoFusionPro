using AutoFusionPro.Domain.Models.Base;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository.Base
{
    /// <summary>
    /// Define the base for the Data Repositories. 
    /// Note: We didn't reference <typeparam name="TEntity">BaseEntity</typeparam> Because we don't want to make a Circular dependency between Core and Domain
    /// </summary>
    /// <typeparam name="TEntity">Base Entity Class</typeparam>
    public interface IBaseRepository<T> where T : class // No direct dependency on BaseEntity in Core
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity); 
        
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        event EventHandler<DataChangedEventArgs<T>> DataChanged;

    }
}
