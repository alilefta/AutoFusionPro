using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region Public props

        public event EventHandler DataChanged;

        #endregion

        #region Private Fields

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        #endregion

        #region Constructor

        public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); ;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                _logger.LogInformation("Retrieved {UserCount} users.", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _logger.LogInformation("Retrieved user with ID {UserId}.", id);
                }
                else
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                }
                return user;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}.", id);
                throw;
            }
        }

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

        public async Task AddAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Added user with ID {UserId}.", user.Id);
                DataChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user with ID {UserId}.", user.Id);
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                user.ModifiedAt = DateTime.Now;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated user with ID {UserId}.", user.Id);
                DataChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}.", user.Id);
                throw;
            }
        }

        public async Task DeleteAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted user with ID {UserId}.", user.Id);
                DataChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}.", user.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {

            var user = await GetByIdAsync(id);

            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted user with ID {UserId}.", user.Id);
                DataChanged?.Invoke(this, EventArgs.Empty);

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
