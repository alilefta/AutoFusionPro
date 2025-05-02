using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Exceptions.Service;
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


        #region Get User Data

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


        #endregion

        #region Create User

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO userDto)
        {
            // --- Validation ---
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));
            // Check for duplicate username/email before hashing password (less work)
            if (await _unitOfWork.Users.ExistsAsync(u => u.Username == userDto.Username))
            {
                throw new ServiceException($"Username '{userDto.Username}' is already taken.");
            }
            if (await _unitOfWork.Users.ExistsAsync(u => u.Email == userDto.Email))
            {
                throw new ServiceException($"Email '{userDto.Email}' is already registered.");
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

        #endregion


        #region Edit User Data

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
            existingUser.Address = userDto.Address;
            existingUser.City = userDto.City;
            existingUser.DateOfBirth = userDto.DateOfBirth;
            existingUser.Gender = userDto.Gender ?? Gender.Male;
            existingUser.PreferredLanguage = userDto.Language;
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


        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID", nameof(userId));
            if (string.IsNullOrEmpty(currentPassword)) throw new ArgumentNullException(nameof(currentPassword));
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentNullException(nameof(newPassword));

            // Get the user
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found when trying to change password");
                return false;
            }

            // Verify current password
            if (!await _authenticationService.AuthenticateAsync(user.Username, currentPassword))
            {
                _logger.LogWarning($"Invalid current password provided for user {user.Username}");
                return false;
            }

            // Generate new password hash and salt
            var (hash, salt) = await _passwordHasher.HashPassword(newPassword);

            // Update password
            user.PasswordHash = hash;
            user.Salt = salt;

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Password changed successfully for user {user.Username}");

            return true;
        }

        public async Task<bool> UpdateUsernameAsync(int userId, string newUsername)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID", nameof(userId));
            if (string.IsNullOrEmpty(newUsername)) throw new ArgumentNullException(nameof(newUsername));

            // Check if username already exists (excluding current user)
            if (await UsernameExistsAsync(newUsername, userId))
            {
                _logger.LogWarning($"Username '{newUsername}' already exists");
                return false;
            }

            // Get the user
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found when trying to update username");
                return false;
            }

            // Update username
            user.Username = newUsername;

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Username updated successfully for user ID {userId}");

            return true;
        }

        public async Task<bool> UpdateProfileImageAsync(int userId, string imageFilePath)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID", nameof(userId));
            if (string.IsNullOrEmpty(imageFilePath)) throw new ArgumentNullException(nameof(imageFilePath));

            // Get the user
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found when trying to update profile image");
                return false;
            }

            // In a real application, you might:
            // 1. Copy the image to a designated storage location
            // 2. Generate a relative path or URL
            // 3. Maybe create thumbnails

            // For now, we'll just store the path
            user.ProfilePictureUrl = imageFilePath;

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Profile image updated successfully for user ID {userId}");

            return true;
        }

        public async Task UpdateSecurityQuestionAsync(UpdateSecurityQuestionDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.UserId <= 0) throw new ArgumentException("Invalid User ID", nameof(dto.UserId));

            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogError("User with ID {UserId} not found for updating security question.", dto.UserId);
                throw new ApplicationException($"User with ID {dto.UserId} not found.");
            }

            _logger.LogInformation("Updating security question for User ID {UserId}.", dto.UserId);

            // Check if clearing or setting
            if (string.IsNullOrWhiteSpace(dto.Question) || string.IsNullOrWhiteSpace(dto.PlainAnswer))
            {
                // Clearing the security question/answer
                user.SecurityQuestion = null;
                user.SecurityAnswerHash = null;
                _logger.LogInformation("Cleared security question for User ID {UserId}.", dto.UserId);
            }
            else
            {
                // Setting a new question/answer - Hash the PLAIN answer
                var (hash, salt) = await _passwordHasher.HashPassword(dto.PlainAnswer); // Use injected service

                user.SecurityQuestion = dto.Question.Trim(); // Trim whitespace
                user.SecurityAnswerHash = hash; // Store the generated hash
                                                // We don't need the salt separately if using BCrypt, but domain model has it? Hash includes it.
                _logger.LogInformation("Set new security question and hashed answer for User ID {UserId}.", dto.UserId);
            }

            // EF Core tracks the changes to the 'user' entity
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Security question update saved successfully for User ID {UserId}.", dto.UserId);
        }

        #endregion

        #region Helpers


        public async Task<bool> TryLogUserIn(string username, string password)
        {
            bool isAuthenticated = await _authenticationService.AuthenticateAsync(username, password);
            if (isAuthenticated) return true;

            return false;
        }

        public async Task<bool> UsernameExistsAsync(string username, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            // Check if another user has this username
            return await _unitOfWork.Users.ExistsAsync(u =>
                u.Username.ToLower() == username.ToLower() && u.Id != currentUserId);
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
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserRole = user.UserRole,
                IsActive = user.IsActive,
                DateRegistered = user.DateRegistered,
                LastLoginDate = user.LastLoginDate,
                ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty,
                PasswordHash = user.PasswordHash,
                Salt = user.Salt,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                SecurityQuestion = user.SecurityQuestion,
                Address = user.Address,
                City = user.City
            };
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
            return new UserProfileDTO
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRole = user.UserRole,
                ProfilePhotoURL = user.ProfilePictureUrl ?? string.Empty
            };
        }

        #endregion
    }
}
