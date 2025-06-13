using AutoFusionPro.Core.Enums.RepoEnums;
using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories.Base
{
    /// <summary>
    /// Base Repository Wrapper
    /// </summary>
    /// <typeparam name="TEntity">Child Domain Model</typeparam>>
    /// <typeparam name="TChildRepo">Child Repository</typeparam>>
    public abstract class Repository<TEntity, TChildRepo> : IBaseRepository<TEntity> where TEntity : class where TChildRepo : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger<TChildRepo> _logger; // Generic logger

        // Implement event
        public event EventHandler<DataChangedEventArgs<TEntity>> DataChanged;



        public Repository(ApplicationDbContext context, ILogger<TChildRepo> logger)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _logger = logger;
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            try
            {
                var entity =  await _dbSet.FindAsync(id);

                if (entity == null)
                    return null;

                return entity;
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while trying to get {typeof(TEntity)}, in 'GetByIdAsync(int id)'");
                return null;
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
           try
           {
                return await _dbSet.Where(predicate).ToListAsync();

           }
            catch (Exception ex)
            {
               throw new RepositoryException(ex.Message, ex);
            }
        }

        public async Task AddAsync(TEntity entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                OnDataChanged(entity, DataChangeType.Added);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }

        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);

            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                OnDataChanged(entity, DataChangeType.Updated);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }

        }

        public void Delete(TEntity entity)
        {
            try
            {
                _dbSet.Remove(entity);
                OnDataChanged(entity, DataChangeType.Deleted);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }

        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }

        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            try
            {
                if (predicate == null)
                    return await _dbSet.CountAsync();

                return await _dbSet.CountAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
        }



        // Trigger event in methods that modify data
        protected virtual void OnDataChanged(TEntity entity, DataChangeType changeType)
        {
            DataChanged?.Invoke(this, new DataChangedEventArgs<TEntity>
            {
                Entity = entity,
                ChangeType = changeType
            });
        }


    }
}
