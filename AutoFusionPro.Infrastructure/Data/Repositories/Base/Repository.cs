using AutoFusionPro.Core.Enums.RepoEnums;
using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Infrastructure.Data.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories.Base
{
    public class Repository<T, Repo> : IBaseRepository<T> where T : class where Repo : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<Repo> _logger;
        protected readonly DbSet<T> _dbSet;

        // Implement event
        public event EventHandler<DataChangedEventArgs<T>> DataChanged;



        public Repository(ApplicationDbContext context, ILogger<Repo> logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        public async Task<T?> GetByIdAsync(int id)
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
                _logger.LogError($"An error occurred while trying to get {typeof(T)}, in 'GetByIdAsync(int id)'");
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
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

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
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

        public async Task AddAsync(T entity)
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

        public async Task AddRangeAsync(IEnumerable<T> entities)
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

        public void Update(T entity)
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

        public void Delete(T entity)
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

        public void RemoveRange(IEnumerable<T> entities)
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

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
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

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
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
        protected virtual void OnDataChanged(T entity, DataChangeType changeType)
        {
            DataChanged?.Invoke(this, new DataChangedEventArgs<T>
            {
                Entity = entity,
                ChangeType = changeType
            });
        }


    }
}
