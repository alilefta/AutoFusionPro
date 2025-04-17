using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        public event EventHandler DataChanged;

        public Task AddAsync(Notification entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Notification>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Notification?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Notification entity)
        {
            throw new NotImplementedException();
        }
    }
}
