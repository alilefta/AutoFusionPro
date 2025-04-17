using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        // Basic CRUD methods got inherited
        Task<User?> GetUserByUsernameAsync(string username);
        Task DeleteAsync(User user);
    }
}
