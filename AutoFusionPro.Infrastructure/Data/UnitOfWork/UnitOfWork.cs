using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Infrastructure.Data.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private ILogger<UnitOfWork> _logger;

        public ICategoryRepository Categories { get;}
        public IUserRepository Users { get; }
        public IPartRepository Parts { get; }
        public INotificationRepository Notifications { get; }
        public IVehicleRepository Vehicles { get; }

        public UnitOfWork(ApplicationDbContext context, 
                        IUserRepository userRepository,
                        IPartRepository partRepository,
                        INotificationRepository notifications,
                        ICategoryRepository categoryRepository,
                        IVehicleRepository vehicleRepository,
                        ILogger<UnitOfWork> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Parts = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
            Categories = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            Notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));
            Vehicles = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Transaction Management

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            _logger.LogWarning("Begin Transaction");
        }

        public async Task CommitTransactionAsync()
        {
            //await context.Database.CommitTransactionAsync();

            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }
            try
            {
                await RetryOnLockAsync(async () =>
                {
                    await _context.SaveChangesAsync();
                    await _transaction.CommitAsync();
                });
                _logger.LogWarning("Commuting Transaction");

            }
            finally
            {
                await DisposeTransactionAndContextAsync();

                _logger.LogWarning("Disposing context and transaction");

            }
        }

        public async Task RollbackTransactionAsync()
        {

            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await DisposeTransactionAndContextAsync();
                _logger.LogWarning("Rollback the Transaction");
                _logger.LogWarning("Disposing context and transaction");

            }


        }

        private async Task RetryOnLockAsync(Func<Task> operation)
        {
            int retryCount = 0;
            const int maxRetries = 3;
            const int initialDelayMs = 100;

            while (true)
            {
                try
                {

                    await operation();
                    _logger.LogWarning("Retrying ... ");

                    return;
                }
                catch (SqliteException ex) when (ex.SqliteErrorCode == SQLitePCL.raw.SQLITE_BUSY && retryCount < maxRetries)
                {
                    retryCount++;
                    int delay = initialDelayMs * (int)Math.Pow(2, retryCount - 1);
                    await Task.Delay(delay);

                    _logger.LogWarning("Retry is awaited");

                }
            }
        }

        private async Task DisposeTransactionAndContextAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }

            if (_context != null)
            {
                await _context.DisposeAsync();
            }
        }

        #endregion


        #region DbContext Methods

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _context.Set<TEntity>().AddAsync(entity);
            // Optionally raise DataChanged event if needed for generic adds
        }

        public async Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
