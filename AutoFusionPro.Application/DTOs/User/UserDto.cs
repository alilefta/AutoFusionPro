using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.SystemEnum;

namespace AutoFusionPro.Application.DTOs.User
{
    // For displaying user details
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty; // Convenience property
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Male;
        public string? Address { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public Languages? PreferredLanguage { get; set; } = Languages.Arabic;


        public string? SecurityQuestion { get; set; }



        // Added for password management
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }

    }



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
        bool IsActive,
        Gender? Gender,
        Languages? Language,
        DateTime? DateOfBirth,
        string? Address,
        string? City


    // Password change should likely be a separate, more secure operation
    );


    // New DTO for the specific update operation
    public record UpdateSecurityQuestionDTO(
        int UserId,
        string? Question,    // The selected question text (nullable if clearing)
        string? PlainAnswer  // The PLAIN TEXT answer (nullable if clearing)
    );


    // Optional: For password change operations
    public record ChangePasswordDTO(
        int UserId,
        string CurrentPassword,
        string NewPassword
    );

    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string ProfilePhotoURL { get; set; } = string.Empty;
    }
}
