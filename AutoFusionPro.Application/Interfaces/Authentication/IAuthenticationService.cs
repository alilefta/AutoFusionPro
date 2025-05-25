namespace AutoFusionPro.Application.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        event EventHandler AuthenticationChanged;
        Task<bool> AuthenticateAsync(string username, string password);
        void Logout();
        // Maybe add: Task<bool> IsUserLoggedInAsync();
        // Maybe add: Task<UserDto?> GetCurrentUserAsync(); // If SessionManager exposes this
    }
}

