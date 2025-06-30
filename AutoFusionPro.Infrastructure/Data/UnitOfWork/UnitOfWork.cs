using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Interfaces.Repository.PartCompatibilityRulesRepositories;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory;
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
        private IDbContextTransaction? _transaction;
        private ILogger<UnitOfWork> _logger;
        private bool _disposed = false;


        public ICategoryRepository Categories { get;}
        public IUserRepository Users { get; }
        public IPartRepository Parts { get; }
        public INotificationRepository Notifications { get; }

        public IMakeRepository Makes { get; }
        public IModelRepository Models { get; }
        public IBodyTypeRepository BodyTypes { get; }
        public IEngineTypeRepository EngineTypes { get; }
        public ITransmissionTypeRepository TransmissionTypes { get; }
        public ITrimLevelRepository TrimLevels { get; }
        public ISupplierRepository Suppliers { get; }
        public IInventoryTransactionRepository InventoryTransactions { get; }
        public IUnitOfMeasureRepository UnitOfMeasures { get; }

        public IVehicleDocumentRepository VehicleDocuments { get; }
        public IVehicleDamageImageRepository VehicleDamageImages { get; }
        public IVehicleServiceHistoryRepository VehicleServiceHistories { get; }
        public IVehicleDamageLogRepository VehicleDamageLogs { get; }
        public IVehicleImageRepository VehicleImages { get; }
        public IVehicleRepository Vehicles { get; }


        public ISupplierPartRepository SupplierParts { get; }


        public IPartCompatibilityRuleRepository PartCompatibilityRules { get; }
        public IPartCompatibilityRuleBodyTypeRepository PartCompatibilityRuleBodyTypes { get; }
        public IPartCompatibilityRuleEngineTypeRepository PartCompatibilityRuleEngineTypes { get; }
        public IPartCompatibilityRuleTransmissionTypeRepository PartCompatibilityRuleTransmissionTypes { get; }
        public IPartCompatibilityRuleTrimLevelRepository PartCompatibilityRuleTrimLevels { get; }

        public IPartImageRepository PartImages { get; }



        public UnitOfWork(ApplicationDbContext context, 
                        IUserRepository userRepository,
                        IPartRepository partRepository,
                        INotificationRepository notifications,
                        ICategoryRepository categoryRepository,
                        IMakeRepository makeRepository,
                        IModelRepository modelRepository,
                        IBodyTypeRepository bodyTypeRepository,
                        IEngineTypeRepository engineTypeRepository,
                        ITransmissionTypeRepository transmissionTypeRepository,
                        ITrimLevelRepository trimLevelRepository, 
                        ISupplierRepository supplierRepository,
                        ISupplierPartRepository supplierPartRepository,
                        IInventoryTransactionRepository inventoryTransactionRepository,
                        IUnitOfMeasureRepository unitOfMeasureRepository,

                        IVehicleDocumentRepository vehicleDocumentRepository,
                        IVehicleDamageImageRepository vehicleDamageImageRepository,
                        IVehicleServiceHistoryRepository vehicleServiceHistoryRepository,
                        IVehicleDamageLogRepository vehicleDamageLogRepository,
                        IVehicleImageRepository vehicleImageRepository,
                        IVehicleRepository vehicleRepository,


                        IPartCompatibilityRuleRepository partCompatibilityRuleRepository,
                        IPartCompatibilityRuleBodyTypeRepository partCompatibilityRuleBodyTypeRepository,
                        IPartCompatibilityRuleEngineTypeRepository partCompatibilityRuleEngineTypeRepository,
                        IPartCompatibilityRuleTransmissionTypeRepository partCompatibilityRuleTransmissionTypeRepository,
                        IPartCompatibilityRuleTrimLevelRepository partCompatibilityRuleTrimLevelRepository,

                        IPartImageRepository partImageRepository,
            

                        ILogger<UnitOfWork> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Parts = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
            Categories = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            Notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));

            Makes = makeRepository ?? throw new ArgumentNullException(nameof(makeRepository));
            Models = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
            BodyTypes = bodyTypeRepository ?? throw new ArgumentNullException(nameof(bodyTypeRepository));
            EngineTypes = engineTypeRepository ?? throw new ArgumentNullException(nameof(engineTypeRepository));
            TransmissionTypes = transmissionTypeRepository ?? throw new ArgumentNullException(nameof(transmissionTypeRepository));
            TrimLevels = trimLevelRepository ?? throw new ArgumentNullException(nameof(trimLevelRepository));
            Suppliers = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));

            SupplierParts = supplierPartRepository ?? throw new ArgumentNullException(nameof(supplierPartRepository));
            InventoryTransactions = inventoryTransactionRepository ?? throw new ArgumentNullException(nameof(inventoryTransactionRepository));
            UnitOfMeasures = unitOfMeasureRepository ?? throw new ArgumentNullException(nameof(unitOfMeasureRepository));

            VehicleDocuments = vehicleDocumentRepository ?? throw new ArgumentNullException(nameof(vehicleDocumentRepository));
            VehicleDamageImages = vehicleDamageImageRepository ?? throw new ArgumentNullException(nameof(vehicleDamageImageRepository));
            VehicleServiceHistories = vehicleServiceHistoryRepository ?? throw new ArgumentNullException(nameof(vehicleServiceHistoryRepository));
            VehicleDamageLogs = vehicleDamageLogRepository ?? throw new ArgumentNullException(nameof(vehicleDamageLogRepository));
            VehicleImages = vehicleImageRepository ?? throw new ArgumentNullException(nameof(vehicleImageRepository));
            Vehicles = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));

            PartCompatibilityRules = partCompatibilityRuleRepository ?? throw new ArgumentNullException(nameof(partCompatibilityRuleRepository));
            PartCompatibilityRuleBodyTypes = partCompatibilityRuleBodyTypeRepository ?? throw new ArgumentNullException(nameof(partCompatibilityRuleBodyTypeRepository));
            PartCompatibilityRuleEngineTypes = partCompatibilityRuleEngineTypeRepository ?? throw new ArgumentNullException(nameof(partCompatibilityRuleEngineTypeRepository));
            PartCompatibilityRuleTransmissionTypes = partCompatibilityRuleTransmissionTypeRepository ?? throw new ArgumentNullException(nameof(partCompatibilityRuleTransmissionTypeRepository));
            PartCompatibilityRuleTrimLevels = partCompatibilityRuleTrimLevelRepository ?? throw new ArgumentNullException(nameof(partCompatibilityRuleTrimLevelRepository));

            PartImages = partImageRepository ?? throw new ArgumentNullException(nameof(partImageRepository));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Transaction Management

        // Non-CancellationToken overload
        public Task BeginTransactionAsync() => BeginTransactionAsync(CancellationToken.None);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                _logger.LogWarning("Attempting to begin a new transaction while an existing one is active. The existing transaction will be disposed.");
                await DisposeTransactionAsync(); // Dispose previous transaction before starting new
            }
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("Database transaction begun. TransactionId: {TransactionId}", _transaction.TransactionId);
        }

        // Non-CancellationToken overload
        public Task CommitTransactionAsync() => CommitTransactionAsync(CancellationToken.None);

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started or has already been completed.");
            }

            try
            {
                // The service layer should have called SaveChangesAsync BEFORE calling CommitTransactionAsync.
                // This method only commits the underlying DB transaction.
                _logger.LogInformation("Attempting to commit database transaction. TransactionId: {TransactionId}", _transaction.TransactionId);
                await RetryOnLockAsync(async () =>
                {
                    await _transaction.CommitAsync(cancellationToken);
                }, cancellationToken);
                _logger.LogInformation("Database transaction committed successfully. TransactionId: {TransactionId}", _transaction.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing database transaction (TransactionId: {TransactionId}). Attempting to rollback.", _transaction.TransactionId);
                await RollbackInternalAsync(cancellationToken); // Attempt rollback on commit failure
                throw; // Re-throw the original exception that caused commit to fail
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        // Non-CancellationToken overload
        public Task RollbackTransactionAsync() => RollbackTransactionAsync(CancellationToken.None);

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            await RollbackInternalAsync(cancellationToken);
            await DisposeTransactionAsync(); // Always dispose after attempting rollback
        }

        private async Task RollbackInternalAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                var transactionId = _transaction.TransactionId; // Get ID before potential dispose
                try
                {
                    _logger.LogInformation("Attempting to rollback database transaction. TransactionId: {TransactionId}", transactionId);
                    await _transaction.RollbackAsync(cancellationToken);
                    _logger.LogInformation("Database transaction rolled back successfully. TransactionId: {TransactionId}", transactionId);
                }
                catch (Exception rbEx)
                {
                    _logger.LogError(rbEx, "An error occurred during transaction rollback (TransactionId: {TransactionId}). The transaction might be in an indeterminate state.", transactionId);
                    // Do not re-throw here, as we are already in an error handling path for commit/rollback.
                    // The primary error that led to rollback/commit failure is more important.
                }
            }
            else
            {
                _logger.LogWarning("RollbackInternalAsync called but no active transaction found.");
            }
        }

        private async Task RetryOnLockAsync(Func<Task> operation, CancellationToken cancellationToken)
        {
            int retryCount = 0;
            const int maxRetries = 3;
            const int initialDelayMs = 200;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    _logger.LogDebug("Executing operation within RetryOnLockAsync, attempt {AttemptCount}", retryCount + 1);
                    await operation();
                    _logger.LogDebug("Operation within RetryOnLockAsync succeeded.");
                    return; // Success
                }
                catch (SqliteException ex) when ((ex.SqliteErrorCode == SQLitePCL.raw.SQLITE_BUSY || ex.SqliteErrorCode == SQLitePCL.raw.SQLITE_LOCKED) && retryCount < maxRetries)
                {
                    retryCount++;
                    int delay = initialDelayMs * (int)Math.Pow(2, retryCount - 1);
                    _logger.LogWarning(ex, "SQLite BUSY/LOCKED error (Code: {SqliteErrorCode}). Retrying attempt {RetryCount}/{MaxRetries} after {Delay}ms...",
                        ex.SqliteErrorCode, retryCount, maxRetries, delay);
                    await Task.Delay(delay, cancellationToken);
                }
                // If it's not a SqliteException we want to retry, or if maxRetries is reached, the exception will propagate.
            }
        }
        #endregion


        #region DbContext Methods

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _context.Set<TEntity>().AddAsync(entity);
            // Optionally raise DataChanged event if needed for generic adds
        }

        //public async Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        //{
        //    return await _context.Set<TEntity>().AnyAsync(predicate);
        //}

        public async Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            if (_disposed) throw new ObjectDisposedException(nameof(UnitOfWork));
            return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger.LogDebug("SaveChangesAsync called without CancellationToken.");
            // Call the overload with CancellationToken.None or default
            return await _context.SaveChangesAsync();
        }

        // New overload with CancellationToken
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("SaveChangesAsync called with CancellationToken.");
            // Pass the cancellationToken to the DbContext's SaveChangesAsync
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // Can be used
        //public void Delete<TEntity>(TEntity entity) where TEntity : class // or BaseEntity
        //{
        //    if (_disposed) throw new ObjectDisposedException(nameof(UnitOfWork));
        //    _context.Set<TEntity>().Remove(entity);
        //}

        #endregion

        #region IDisposable & IAsyncDisposable

        // Dispose only the transaction, not the DbContext.
        // DbContext lifetime is managed by the DI container.
        private async ValueTask DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null; // Important to nullify after dispose
                _logger.LogDebug("Active database transaction disposed.");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    // _transaction?.Dispose(); // Sync dispose if it had one, but IDbContextTransaction is usually async
                    // Forcibly wait for async dispose if called synchronously (not ideal but better than not disposing)
                    if (_transaction != null)
                    {
                        _logger.LogWarning("Synchronous Dispose called on UnitOfWork with an active async transaction. Disposing transaction now.");
                        _transaction.Dispose(); // Fallback to synchronous dispose
                        _transaction = null;
                    }
                    // Do NOT dispose _context here.
                }
                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(false); // Suppress finalization because async dispose has run
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!_disposed) // Check disposed flag here too for async path
            {
                await DisposeTransactionAsync();
                // Do NOT dispose _context here.
                _disposed = true; // Mark as disposed after async resources handled
            }
        }
        #endregion


    }
}
