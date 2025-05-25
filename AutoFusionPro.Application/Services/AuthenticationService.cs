using AutoFusionPro.Application.Interfaces.Authentication;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.SessionManagement;
using AutoFusionPro.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace AutoFusionPro.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ISessionManager _sessionManager;
        private readonly IPasswordHashingService _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthenticationService> _logger;  // Inject logger here


        public event EventHandler AuthenticationChanged;

        public AuthenticationService(IUnitOfWork unitOfWork,
            ISessionManager sessionManager,
            IPasswordHashingService passwordHasher,
            ILogger<AuthenticationService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Authentication failed: Username or password cannot be empty.");
                return false;
            }

            try
            {
                // Fetch the user using the injected Unit of Work
                var user = await _unitOfWork.Users.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    _logger.LogWarning("Authentication failed: User '{Username}' not found.", username);
                    return false; // User not found
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Authentication failed: User '{Username}' is inactive.", username);
                    return false; // User is inactive
                }

                // Verify password using the injected hashing service
                // Assuming IPasswordHashingService uses the stored hash and salt from the user object
                bool isPasswordCorrect = _passwordHasher.VerifyPassword(password, user.PasswordHash, user.Salt);

                if (isPasswordCorrect)
                {
                    _logger.LogInformation("Password verification successful for user '{Username}'.", username);

                    // Update last login and active times (use UtcNow)
                    var now = DateTime.UtcNow;
                    user.LastLoginDate = now;
                    user.LastActive = now;

                    // EF Core tracks the 'user' entity, SaveChangesAsync will persist the updates
                    await _unitOfWork.SaveChangesAsync();

                    // Set user in session manager AFTER successful save
                    // Consider storing UserDto or essential info instead of the full entity
                    _sessionManager.SetCurrentUser(user); // Adapt based on SessionManager needs

                    _logger.LogInformation("User '{Username}' authenticated successfully and session started.", username);
                    AuthenticationChanged?.Invoke(this, EventArgs.Empty); // Notify UI
                    return true;
                }
                else
                {
                    _logger.LogWarning("Authentication failed for user '{Username}': Incorrect password.", username);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during authentication for user '{Username}'.", username);
                return false; // Return false on error
            }
        }


        public void Logout()
        {
            var currentUsername = _sessionManager.GetCurrentUserUsername(); // Example method
            _sessionManager.Logout();
            AuthenticationChanged?.Invoke(this, EventArgs.Empty); // Notify UI
            _logger.LogInformation("User '{Username}' logged out.", currentUsername ?? "Unknown");
        }
    }
}
