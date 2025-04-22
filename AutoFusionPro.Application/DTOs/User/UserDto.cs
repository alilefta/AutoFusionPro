using AutoFusionPro.Core.Enums.ModelEnum;

namespace AutoFusionPro.Application.DTOs.User
{
    // For displaying user details
    public record UserDTO(
        int Id,
        string FirstName,
        string LastName,
        string FullName, // Convenience property
        string Username,
        string Email,
        string PhoneNumber,
        UserRole UserRole,
        bool IsActive,
        DateTime DateRegistered,
        DateTime? LastLoginDate
    // Add other relevant fields as needed by the UI
    );



    public record AllUserAdmin (
        int Id,
        string username,
        UserRole role
        );



    // For displaying users in lists/summaries
    public record UserSummaryDTO(
        int Id,
        string FullName,
        string Username,
        UserRole Role,
        bool IsActive
    );

    // For creating a new user
    public record CreateUserDTO(
        string FirstName,
        string LastName,
        string Username,
        string Email,
        string PhoneNumber, // Optional? Make nullable if so
        string Password,   // Raw password from UI
        UserRole Role
    );

    // For updating an existing user
    public record UpdateUserDTO(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber, // Optional? Make nullable if so
        UserRole Role,
        bool IsActive
    // Password change should likely be a separate, more secure operation
    );

    // Optional: For password change operations
    public record ChangePasswordDTO(
        int UserId,
        string CurrentPassword,
        string NewPassword
    );

    public record UserProfileDTO(
        int UserId, 
        string Username,
        string FirstName, 
        string LastName, 
        UserRole UserRole,
        string ProfilePhotoURL);
}
