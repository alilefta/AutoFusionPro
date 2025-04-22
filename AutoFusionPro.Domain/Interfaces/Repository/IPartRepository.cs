using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using System.Linq.Expressions;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IPartRepository : IBaseRepository<Part>
    {
        Task<IEnumerable<Part>> GetPartsByCategory(int categoryId);
        Task<IEnumerable<Part>> GetLowStockParts(int thresholdQuantity);
        Task<IEnumerable<Part>> GetPartsByManufacturer(string manufacturer);
        Task<IEnumerable<Part>> SearchParts(string searchTerm);
        Task<IEnumerable<Part>> GetPartsByVehicle(int vehicleId);
        Task<bool> UpdatePartStock(int partId, int newQuantity, string reason = null);
        Task<Part?> GetPartByPartNumber(string partNumber);
        Task<IEnumerable<Part>> GetActivePartsWithPagination(int pageNumber, int pageSize);
        Task<int> GetTotalPartsCount(Expression<Func<Part, bool>> filter = null);
        Task<IEnumerable<Part>> GetPartsWithSuppliers();
    }
}
