using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class PartRepository : Repository<Part, PartRepository>, IPartRepository
    {

        public PartRepository(ApplicationDbContext context, ILogger<PartRepository> logger) : base(context, logger)
        {
        }


        public async Task<IEnumerable<Part>> GetPartsByCategory(int categoryId)
        {
            return await _context.Set<Part>()
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Part>> GetLowStockParts(int thresholdQuantity)
        {
            return await _context.Set<Part>()
                .Where(p => p.CurrentStock <= thresholdQuantity && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Part>> GetPartsByManufacturer(string manufacturer)
        {
            return await _context.Set<Part>()
                .Where(p => p.Manufacturer == manufacturer && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Part>> SearchParts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _context.Set<Part>().Where(p => p.IsActive).Take(50).ToListAsync();

            searchTerm = searchTerm.ToLower();
            return await _context.Set<Part>()
                .Where(p => (p.Name.ToLower().Contains(searchTerm) ||
                           p.PartNumber.ToLower().Contains(searchTerm) ||
                           p.Description.ToLower().Contains(searchTerm) ||
                           p.Manufacturer.ToLower().Contains(searchTerm)) &&
                           p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Part>> GetPartsByVehicle(int vehicleId)
        {
            return await _context.Set<Part>()
                .Where(p => p.CompatibleVehicles.Any(v => v.VehicleId == vehicleId) && p.IsActive)
                .ToListAsync();
        }

        public async Task<bool> UpdatePartStock(int partId, int newQuantity, string reason = null)
        {
            var part = await _context.Set<Part>().FindAsync(partId);
            if (part == null)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Record the transaction in inventory history
                var inventoryTransaction = new InventoryTransaction
                {
                    PartId = partId,
                    TransactionDate = DateTime.Now,
                    PreviousQuantity = part.CurrentStock,
                    Quantity = newQuantity - part.CurrentStock, // This can be positive or negative
                    NewQuantity = newQuantity,
                    TransactionType = newQuantity > part.CurrentStock ?
                        InventoryTransactionType.Adjustment :
                        InventoryTransactionType.Adjustment,
                    Notes = reason ??
                        (newQuantity > part.CurrentStock
                            ? "Stock adjustment (increase)"
                            : "Stock adjustment (decrease)"),
                    UserId = 1, // TODO: Get actual user ID from current user context
                };

                part.CurrentStock = newQuantity;
                part.LastRestockDate = newQuantity > part.CurrentStock ? DateTime.Now : part.LastRestockDate;

                await _context.Set<InventoryTransaction>().AddAsync(inventoryTransaction);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Part?> GetPartByPartNumber(string partNumber)
        {
            try
            {
                var part = await _context.Set<Part>().FirstOrDefaultAsync(p => p.PartNumber == partNumber);

                if (part == null)
                {
                    return null;
                }

                return part;
                

            }catch(RepositoryException ex)
            {
                _logger.LogError($"An error occurred while getting part by its number in 'GetPartByPartName(string name)' ");
                return null;
            }
        }

        public async Task<IEnumerable<Part>> GetActivePartsWithPagination(int pageNumber, int pageSize)
        {
            return await _context.Set<Part>()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPartsCount(Expression<Func<Part, bool>> filter = null)
        {
            if (filter == null)
                return await _context.Set<Part>().CountAsync();

            return await _context.Set<Part>().CountAsync(filter);
        }

        public async Task<IEnumerable<Part>> GetPartsWithSuppliers()
        {
            return await _context.Set<Part>()
                .Include(p => p.Suppliers)
                .ThenInclude(sp => sp.Supplier)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

    }
}

