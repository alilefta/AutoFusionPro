using AutoFusionPro.Core.Enums.RepoEnums;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class UserRepository : Repository<User, UserRepository>, IUserRepository
    {


        #region Constructor

        public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger) : base(dbContext, logger)
        {
        }

        #endregion

        #region Public Methods

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    _logger.LogInformation("Retrieved user with username {Username}.", username);
                }
                else
                {
                    _logger.LogWarning("User with username {Username} not found.", username);
                }
                return user;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with username {Username}.", username);
                throw;
            }
        }

        public async void Delete(int id)
        {

            var user = await GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogError("No User was found to delete with ID {id}", id);
                return;
            }


            try
            {
                _dbSet.Remove(user);
                OnDataChanged(user, DataChangeType.Deleted);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}.", user.Id);
                throw;
            }
        }

        #endregion
    }
}
