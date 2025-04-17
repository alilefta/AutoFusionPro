using AutoFusionPro.Domain.Models;

namespace AutoFusionPro.Application.Interfaces
{
    public interface ISessionManager
    {
        // Properties
        DateTime? LoginTime { get; }
        User? CurrentUser { get; }
        Task Initialized { get; }
        bool IsUserLoggedIn { get; }

        // Events
        event EventHandler SessionExpired;

        // Methods
        Task Initialize();
        bool IsAdmin();
        void SetCurrentUser(User user);
        void Logout();
        Task<bool> TryRestoreSession();

    }
}
