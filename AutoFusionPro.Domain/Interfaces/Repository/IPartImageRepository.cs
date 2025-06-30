using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IPartImageRepository : IBaseRepository<PartImage>
    {
        Task ClearPrimaryFlagsForPartAsync(int partId, int? excludeImageId = null);
        Task<IEnumerable<PartImage>> GetByPartIdAsync(int partId); // For GetImagesForPartAsync
        Task<PartImage?> GetPrimaryImageForPartAsync(int partId); // For GetImagesForPartAsync or summaries
    }
}
