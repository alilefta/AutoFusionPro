using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Application.Interfaces
{
    public interface ISessionManager
    {
        // Properties
        DateTime? LoginTime { get; }
        UserDTO? CurrentUser { get; } // Changed to UserDto?
        Task Initialized { get; }
        bool IsUserLoggedIn { get; }

        // Events
        event EventHandler SessionExpired; // Keep for potential future use

        // Methods
        Task Initialize();
        bool IsUserAdmin(); // Renamed for clarity, checks Role
        void SetCurrentUser(User user); // Takes Domain User, maps internally
        void Logout();
        Task<bool> TryRestoreSession();
        string? GetCurrentUserUsername(); // Changed return type to nullable string

    }
}
