using AutoFusionPro.Domain.Models.Base;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository.Base
{
    /// <summary>
    /// Define the base for the Data Repositories. 
    /// Note: We didn't reference <typeparam name="TEntity">BaseEntity</typeparam> Because we don't want to make a Circular dependency between Core and Domain
    /// </summary>
    /// <typeparam name="TEntity">Base Entity Class</typeparam>
    public interface IBaseRepository<TModel> where TModel : class // No direct dependency on BaseEntity in Core
    {
        Task<TModel?> GetByIdAsync(int id);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate);
        Task AddRangeAsync(IEnumerable<TModel> entities);
        Task AddAsync(TModel entity);
        void Update(TModel entity);
        void Delete(TModel entity); 
        
        void RemoveRange(IEnumerable<TModel> entities);
        Task<bool> ExistsAsync(Expression<Func<TModel, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TModel, bool>> predicate = null);

        event EventHandler<DataChangedEventArgs<TModel>> DataChanged;

    }
}
