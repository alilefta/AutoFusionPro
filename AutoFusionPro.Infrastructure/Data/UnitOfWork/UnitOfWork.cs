using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
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

        private IDbContextTransaction _transaction;
        private ILogger<UnitOfWork> _logger;

        public ICategoryRepository Categories { get;}
        public IUserRepository Users { get; }
        public IPartRepository Parts { get; }
        public INotificationRepository Notifications { get; }

        public ICompatibleVehicleRepository CompatibleVehicles { get; }
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


        public IPartCompatibilityRepository PartCompatibilities { get; }
        public ISupplierPartRepository SupplierParts { get; } 

        public UnitOfWork(ApplicationDbContext context, 
                        IUserRepository userRepository,
                        IPartRepository partRepository,
                        INotificationRepository notifications,
                        ICategoryRepository categoryRepository,
                        ICompatibleVehicleRepository compatibleVehicleRepository,
                        IMakeRepository makeRepository,
                        IModelRepository modelRepository,
                        IBodyTypeRepository bodyTypeRepository,
                        IEngineTypeRepository engineTypeRepository,
                        ITransmissionTypeRepository transmissionTypeRepository,
                        ITrimLevelRepository trimLevelRepository, 
                        ISupplierRepository supplierRepository,
                        IPartCompatibilityRepository partCompatibilityRepository,
                        ISupplierPartRepository supplierPartRepository,
                        IInventoryTransactionRepository inventoryTransactionRepository,
                        IUnitOfMeasureRepository unitOfMeasureRepository,

                        IVehicleDocumentRepository vehicleDocumentRepository,
                        IVehicleDamageImageRepository vehicleDamageImageRepository,
                        IVehicleServiceHistoryRepository vehicleServiceHistoryRepository,
                        IVehicleDamageLogRepository vehicleDamageLogRepository,
                        IVehicleImageRepository vehicleImageRepository,
                        IVehicleRepository vehicleRepository,
                        ILogger<UnitOfWork> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Parts = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
            Categories = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            Notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));

            CompatibleVehicles = compatibleVehicleRepository ?? throw new ArgumentNullException(nameof(compatibleVehicleRepository));
            Makes = makeRepository ?? throw new ArgumentNullException(nameof(makeRepository));
            Models = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
            BodyTypes = bodyTypeRepository ?? throw new ArgumentNullException(nameof(bodyTypeRepository));
            EngineTypes = engineTypeRepository ?? throw new ArgumentNullException(nameof(engineTypeRepository));
            TransmissionTypes = transmissionTypeRepository ?? throw new ArgumentNullException(nameof(transmissionTypeRepository));
            TrimLevels = trimLevelRepository ?? throw new ArgumentNullException(nameof(trimLevelRepository));
            Suppliers = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));

            PartCompatibilities = partCompatibilityRepository ?? throw new ArgumentNullException(nameof(partCompatibilityRepository));
            SupplierParts = supplierPartRepository ?? throw new ArgumentNullException(nameof(supplierPartRepository));
            InventoryTransactions = inventoryTransactionRepository ?? throw new ArgumentNullException(nameof(inventoryTransactionRepository));
            UnitOfMeasures = unitOfMeasureRepository ?? throw new ArgumentNullException(nameof(unitOfMeasureRepository));

            VehicleDocuments = vehicleDocumentRepository ?? throw new ArgumentNullException(nameof(vehicleDocumentRepository));
            VehicleDamageImages = vehicleDamageImageRepository ?? throw new ArgumentNullException(nameof(vehicleDamageImageRepository));
            VehicleServiceHistories = vehicleServiceHistoryRepository ?? throw new ArgumentNullException(nameof(vehicleServiceHistoryRepository));
            VehicleDamageLogs = vehicleDamageLogRepository ?? throw new ArgumentNullException(nameof(vehicleDamageLogRepository));
            VehicleImages = vehicleImageRepository ?? throw new ArgumentNullException(nameof(vehicleImageRepository));
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


        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("Transaction begun."); // Changed from Warning to Info
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }
            try
            {
                // Pass cancellationToken to SaveChangesAsync if it's part of the commit logic
                // The RetryOnLockAsync should also ideally accept and pass down the token.
                await RetryOnLockAsync(async () =>
                {
                    await _context.SaveChangesAsync(cancellationToken); // Pass token here
                    await _transaction.CommitAsync(cancellationToken);  // Pass token here
                }, cancellationToken); // Pass token to retry logic
                _logger.LogInformation("Transaction committed."); // Changed from Warning to Info
            }
            finally
            {
                await DisposeTransactionAsync(); // No need for context disposal here usually
                _logger.LogDebug("Transaction disposed after commit/rollback attempt.");
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                try
                {
                    await _transaction.RollbackAsync(cancellationToken); // Pass token here
                    _logger.LogInformation("Transaction rolled back."); // Changed from Warning to Info
                }
                finally
                {
                    await DisposeTransactionAsync();
                    _logger.LogDebug("Transaction disposed after rollback.");
                }
            }
        }




        // Update RetryOnLockAsync to accept and use CancellationToken
        private async Task RetryOnLockAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            int retryCount = 0;
            const int maxRetries = 3;
            const int initialDelayMs = 100;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation before each attempt
                try
                {
                    await operation();
                    _logger.LogDebug("Operation within RetryOnLockAsync succeeded.");
                    return;
                }
                catch (SqliteException ex) when (ex.SqliteErrorCode == SQLitePCL.raw.SQLITE_BUSY && retryCount < maxRetries)
                {
                    retryCount++;
                    int delay = initialDelayMs * (int)Math.Pow(2, retryCount - 1);
                    _logger.LogWarning(ex, "SQLite BUSY error (Code: {ErrorCode}). Retrying attempt {RetryCount}/{MaxRetries} after {Delay}ms...",
                        ex.SqliteErrorCode, retryCount, maxRetries, delay);
                    await Task.Delay(delay, cancellationToken); // Pass token to Task.Delay
                }
                // Removed re-throwing general exceptions, let the specific catch handle SqliteException
                // and let others propagate if not Sqlite BUSY.
            }
        }

        // Remove DisposeTransactionAndContextAsync if context is managed by DI
        // The context is typically disposed by the DI container when the scope ends.
        // The UnitOfWork should generally only manage the lifetime of the transaction it creates.
    

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

        #endregion

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

        // Helper to dispose transaction asynchronously
        private async ValueTask DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null!; // Mark as disposed
            }
        }

    }
}
