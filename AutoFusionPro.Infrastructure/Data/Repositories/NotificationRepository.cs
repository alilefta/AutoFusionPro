using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class NotificationRepository : Repository<Notification, NotificationRepository>, INotificationRepository
    {
        
        public NotificationRepository(ApplicationDbContext dbContext, ILogger<NotificationRepository> logger) : base(dbContext, logger) 
        { 
            
            
        }
    }
}
