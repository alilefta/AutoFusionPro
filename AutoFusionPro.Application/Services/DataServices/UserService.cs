using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Application.Services.DataServices
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHashingService _passwordHasher; // Inject password hashing service
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<UserService> _logger;

        // Consider using an AutoMapper instance (IMapper) if mapping becomes complex
        public UserService(IUnitOfWork unitOfWork, 
            IPasswordHashingService passwordHasher, 
            IAuthenticationService authenticationService,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _logger = logger;
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user == null ? null : MapUserToUserDto(user);
        }

        public async Task<IEnumerable<UserSummaryDTO>> GetAllUsersSummaryAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.Select(MapUserToUserSummaryDto); // Use LINQ Select for mapping
        }

        public async Task<IEnumerable<UserSummaryDTO>> GetActiveUsersSummaryAsync()
        {
            // Use FindAsync for filtering in the database
            var activeUsers = await _unitOfWork.Users.FindAsync(u => u.IsActive);
            return activeUsers.Select(MapUserToUserSummaryDto);
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO userDto)
        {
            // --- Validation ---
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));
            // Check for duplicate username/email before hashing password (less work)
            if (await _unitOfWork.Users.ExistsAsync(u => u.Username == userDto.Username))
            {
                throw new ApplicationException($"Username '{userDto.Username}' is already taken.");
            }
            if (await _unitOfWork.Users.ExistsAsync(u => u.Email == userDto.Email))
            {
                throw new ApplicationException($"Email '{userDto.Email}' is already registered.");
            }
            // Add more validation as needed (e.g., password complexity)



            //Check if this is the last admin user
            int adminCount = 0;
            try
            {
                var allUsers = await _unitOfWork.Users.GetAllAsync(); // Get all users to count admins
                adminCount = allUsers.Count(u => u.UserRole == UserRole.Admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting admin users in 'Register View Model'.");
                // Handle error gracefully - maybe assume not last admin to avoid blocking? Or show error and cancel?
                //await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Admin Check Failed", "Could not verify admin users.", true, CurrentWorkFlow);
            }

            UserRole systemRole;
            bool isAdmin = false;

            if (adminCount == 0) // If only one admin left (or none after deletion) and deleting user is admin
            {
                systemRole = UserRole.Admin;
                isAdmin = true;

            }
            else
            {
                systemRole = UserRole.User;
                isAdmin = false;

            }

            // --- Hashing ---
            var (hash, salt) = await _passwordHasher.HashPassword(userDto.Password);

            // --- Mapping & Creation ---
            var newUser = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Username = userDto.Username,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber ?? string.Empty, // Handle potential null
                PasswordHash = hash,
                Salt = salt,
                UserRole = systemRole,
                IsAdmin = isAdmin,
                IsActive = true, // New users are active by default
                DateRegistered = DateTime.UtcNow,
                Address = string.Empty,
                // DateRegistered and LastActive should be set by model default/SaveChanges override
            };

            // --- Persistence ---
            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync(); // Commit the changes for this operation

            // --- Return Result ---
            // newUser entity now has an Id assigned by the database
            return MapUserToUserDto(newUser);
        }

        public async Task UpdateUserAsync(UpdateUserDTO userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

            var existingUser = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
            if (existingUser == null)
            {
                throw new ApplicationException($"User with ID {userDto.Id} not found.");
            }

            // --- Validation (check for email conflict if email changed) ---
            if (existingUser.Email != userDto.Email && await _unitOfWork.Users.ExistsAsync(u => u.Email == userDto.Email && u.Id != userDto.Id))
            {
                throw new ApplicationException($"Email '{userDto.Email}' is already registered by another user.");
            }

            // --- Mapping (Update only allowed fields) ---
            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.Email = userDto.Email;
            existingUser.PhoneNumber = userDto.PhoneNumber ?? existingUser.PhoneNumber; // Keep old if null
            existingUser.UserRole = userDto.Role;
            existingUser.IsActive = userDto.IsActive;
            // DO NOT update Username, PasswordHash, Salt, DateRegistered here.
            // ModifiedAt will be updated by SaveChanges override.

            // --- Persistence ---
            // No need to call _unitOfWork.Users.Update() as the entity is tracked by EF Core
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateUserAsync(int id)
        {
            await SetUserActiveStatusAsync(id, false);
        }

        public async Task ActivateUserAsync(int id)
        {
            await SetUserActiveStatusAsync(id, true);
        }

        public async Task<UserProfileDTO> LoadUserProfileAsync(int id)
        {
            if (id == 0) {
                _logger.LogError($"Loading user profile data was failed due to ID = {id} in 'LoadUserProfileAsync(int id)'");
                throw new ArgumentNullException("id"); 
            }

            var user = await _unitOfWork.Users.GetByIdAsync(id); 

            if (user == null)
            {
                _logger.LogError("The user was not found");
                throw new ApplicationException("The user was not found, in 'LoadUserProfileAsync(int id)' - 'UserService' ");
            }



            return MapUserToUserProfileDTO(user);
        }

        // --- Helper Methods ---
        private async Task SetUserActiveStatusAsync(int id, bool isActive)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException($"User with ID {id} not found.");
            }

            if (user.IsActive == isActive) return; // No change needed

            user.IsActive = isActive;
            // The entity is tracked, SaveChangesAsync will persist the change.
            await _unitOfWork.SaveChangesAsync();
        }

        // Manual Mapping Helpers (Replace with AutoMapper/Mapster if preferred)
        private UserDTO MapUserToUserDto(User user)
        {
            return new UserDTO(
                user.Id,
                user.FirstName,
                user.LastName,
                $"{user.FirstName} {user.LastName}",
                user.Username,
                user.Email,
                user.PhoneNumber,
                user.UserRole,
                user.IsActive,
                user.DateRegistered,
                user.LastLoginDate
            );
        }

        private UserSummaryDTO MapUserToUserSummaryDto(User user)
        {
            return new UserSummaryDTO(
                user.Id,
                $"{user.FirstName} {user.LastName}",
                user.Username,
                user.UserRole,
                user.IsActive
            );
        }

        private UserProfileDTO MapUserToUserProfileDTO(User user)
        {
            return new UserProfileDTO(
                user.Id, user.Username, user.FirstName, user.LastName, user.UserRole, user.ProfilePictureUrl ?? string.Empty);
        }

        public async Task<bool> TryLogUserIn(string username, string password)
        {
            bool isAuthenticated = await _authenticationService.AuthenticateAsync(username, password);
            if (isAuthenticated) return true;

            return false;
        }
    }
}
