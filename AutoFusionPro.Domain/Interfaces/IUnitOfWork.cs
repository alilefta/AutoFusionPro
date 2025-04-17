// Ignore Spelling: Denta

using AutoFusionPro.Domain.Interfaces.Repository;

namespace AutoFusionPro.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        //IPatientRepository Patients { get; }
        //IAppointmentRepository Appointments { get; }
        //IStaffRepository Staff {  get; } 
        IUserRepository Users {  get; } 
        INotificationRepository Notifications {  get; } 

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
