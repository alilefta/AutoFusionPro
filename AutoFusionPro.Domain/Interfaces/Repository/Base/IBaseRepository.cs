namespace AutoFusionPro.Domain.Interfaces.Repository.Base
{
    /// <summary>
    /// Define the base for the Data Repositories. 
    /// Note: We didn't reference <typeparam name="TEntity">BaseEntity</typeparam> Because we don't want to make a Circular dependency between Core and Domain
    /// </summary>
    /// <typeparam name="TEntity">Base Entity Class</typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class // No direct dependency on BaseEntity in Core
    {
        Task<TEntity?> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);

        public event EventHandler DataChanged;

    }
}
