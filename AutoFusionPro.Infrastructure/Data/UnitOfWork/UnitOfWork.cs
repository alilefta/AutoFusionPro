using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Infrastructure.Data.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private ILogger<UnitOfWork> _logger;


        //public IPatientRepository Patients { get; }
        //public IAppointmentRepository Appointments { get; }
        //public IStaffRepository Staff { get; }
        public IUserRepository Users { get; }
        public INotificationRepository Notifications { get; }

        public UnitOfWork(ApplicationDbContext context, 
            IUserRepository userRepository,
            ILogger<UnitOfWork> logger
,
            INotificationRepository notifications)
        {
            _context = context;
            Users = userRepository;
            _logger = logger;
            Notifications = notifications;
        }


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

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
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

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
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
    }
}
