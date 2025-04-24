using AutoFusionPro.Application.DTOs.User;

namespace AutoFusionPro.Application.Interfaces.DataServices
{
    public interface IUserService
    {
        /// <summary>
        /// Gets detailed information for a specific user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>A UserDto or null if not found.</returns>
        Task<UserDTO?> GetUserByIdAsync(int id);

        /// <summary>
        /// Gets a summary list of all users, potentially with filtering/pagination later.
        /// </summary>
        /// <returns>A collection of UserSummaryDto.</returns>
        Task<IEnumerable<UserSummaryDTO>> GetAllUsersSummaryAsync();

        /// <summary>
        /// Gets a summary list of active users.
        /// </summary>
        /// <returns>A collection of UserSummaryDto for active users.</returns>
        Task<IEnumerable<UserSummaryDTO>> GetActiveUsersSummaryAsync();

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userDto">Data for the new user.</param>
        /// <returns>The DTO of the newly created user.</returns>
        /// <exception cref="ApplicationException">Throws if username or email already exists, or validation fails.</exception>
        Task<UserDTO> CreateUserAsync(CreateUserDTO userDto);

        /// <summary>
        /// Updates an existing user's details (excluding password).
        /// </summary>
        /// <param name="userDto">Data to update.</param>
        /// <returns>Task indicating completion.</returns>
        /// <exception cref="ApplicationException">Throws if user not found or validation fails.</exception>
        Task UpdateUserAsync(UpdateUserDTO userDto);

        /// <summary>
        /// Deactivates a user (soft delete).
        /// </summary>
        /// <param name="id">The ID of the user to deactivate.</param>
        /// <returns>Task indicating completion.</returns>
        /// <exception cref="ApplicationException">Throws if user not found.</exception>
        Task DeactivateUserAsync(int id);

        /// <summary>
        /// Activates a previously deactivated user.
        /// </summary>
        /// <param name="id">The ID of the user to activate.</param>
        /// <returns>Task indicating completion.</returns>
        /// <exception cref="ApplicationException">Throws if user not found.</exception>
        Task ActivateUserAsync(int id);

        /// <summary>
        /// Logging in user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        Task<bool> TryLogUserIn(string username, string password);

        /// <summary>
        /// Loading the currently logged in user profile data for the view models. e.g., UserAvatarViewModels
        /// </summary>
        /// <param name="id">Currently logged in User ID</param>
        /// <returns><see cref="UserProfileDTO"/> record</returns>
        /// <exception cref="ApplicationException">Throws if user not found.</exception>
        Task<UserProfileDTO> LoadUserProfileAsync(int id);

        // Optional: Add methods for password management if needed
        // Task ChangePasswordAsync(ChangePasswordDto passwordDto);
        // Task ResetPasswordAsync(int userId);

        /// <summary>
        /// Check if username is already exists.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        Task<bool> UsernameExistsAsync(string username, int currentUserId);


        // New methods needed for UserAccountViewModel
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> UpdateUsernameAsync(int userId, string newUsername);
        Task<bool> UpdateProfileImageAsync(int userId, string imageFilePath);
    }
}
