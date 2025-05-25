using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces.SessionManagement;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace AutoFusionPro.Application.Services
{
    public class SessionManager : ISessionManager
    {

        // Consider making path configurable or using AppData folder
        private const string SessionFileName = "autofusionpro.session";
        private static readonly string SessionFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // More robust location
            "AutoFusionPro", // App-specific folder
            SessionFileName);

        // Optional: Add entropy for DPAPI for slightly better security
        private static readonly byte[] s_entropy = Encoding.UTF8.GetBytes("AutoFusionProSessionEntropy");

        private TaskCompletionSource<bool> _initializationComplete = new TaskCompletionSource<bool>();
        private bool _isInitialized = false;

        private const int SessionTimeoutMinutes = int.MaxValue;

        private readonly ILogger<SessionManager> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private System.Timers.Timer _sessionTimer;

        public DateTime? LoginTime { get; private set; }
        public Task Initialized => _initializationComplete.Task;

        // Can't use Prop change here
        public UserDTO? CurrentUser { get; private set; }



        public bool IsUserLoggedIn => CurrentUser != null && LoginTime.HasValue && !IsSessionExpired();

        //public event PropertyChangedEventHandler? PropertyChanged;


        // Event for session expiration
        public event EventHandler SessionExpired;

        public SessionManager(ILogger<SessionManager> logger, IUnitOfWork unitOfWork)
        {
            _initializationComplete = new TaskCompletionSource<bool>();

            _logger = logger;
            _unitOfWork = unitOfWork;
            //StartSessionTimer();

            _logger.LogInformation("SessionManager initialized with session timeout of {SessionTimeoutMinutes} minutes.", SessionTimeoutMinutes);

        }

        public async Task Initialize()
        {
            if (_isInitialized)
            {
                await Initialized; // Ensure caller waits if already initializing/initialized
                return;
            }

            // Ensure target directory exists
            EnsureSessionDirectoryExists();

            try
            {
                _logger.LogInformation("Initializing SessionManager...");

                await TryRestoreSession();
               // StartSessionTimer();
                _isInitialized = true;
                _initializationComplete.SetResult(true);
                _logger.LogInformation("SessionManager initialization complete. User logged in: {IsLoggedIn}", IsUserLoggedIn);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize SessionManager");
                ClearSession(); // Clear potentially corrupt session file on error
                _isInitialized = false; // Mark as not initialized
                _initializationComplete.TrySetException(ex); // Use TrySetException in case Initialize is called multiple times concurrently
                // Don't re-throw here, let the caller check the Task status or handle the exception
            }
        }

        public bool IsUserAdmin()
        {
            // Check Role on the DTO
            return CurrentUser?.UserRole == UserRole.Admin;
        }

        public void SetCurrentUser(User user)
        {
            if (user == null)
            {
                Logout(); // Setting null user is equivalent to logging out
                return;
            }

            CurrentUser = MapUserToUserDto(user); // Map here
            LoginTime = DateTime.UtcNow; // Use UtcNow

            _logger.LogInformation("Session set for user {Username} at {LoginTime}.", CurrentUser.Username, LoginTime);

            SaveSession();
            // REMOVED: ResetSessionTimer();
        }

        public void Logout()
        {
            var username = CurrentUser?.Username; // Get username before clearing
            CurrentUser = null;
            LoginTime = null;

            ClearSession();
            // REMOVED: StopSessionTimer();
            _logger.LogInformation("Session cleared for user {Username} at {LogoutTime}.", username ?? "<unknown>", DateTime.UtcNow);
        }


        public async Task<bool> TryRestoreSession()
        {
            if (!File.Exists(SessionFilePath))
            {
                _logger.LogInformation("No session file found at {SessionFilePath}.", SessionFilePath);
                return false;
            }

            _logger.LogInformation("Attempting to restore session from {SessionFilePath}.", SessionFilePath);
            try
            {
                var encryptedDataBytes = await File.ReadAllBytesAsync(SessionFilePath);
                var sessionData = DecryptSessionData(encryptedDataBytes); // Use DPAPI
                var parts = sessionData.Split('|'); // Use a more robust format like JSON later if needed

                if (parts.Length != 2) // Expecting UserId|LoginTimeTicks
                {
                    _logger.LogWarning("Invalid session data format after decryption.");
                    ClearSession();
                    return false;
                }

                if (!int.TryParse(parts[0], out var userId) ||
                    !long.TryParse(parts[1], out var loginTimeTicks))
                {
                    _logger.LogWarning("Could not parse session data values (UserId or LoginTimeTicks).");
                    ClearSession();
                    return false;
                }

                var loginTime = new DateTime(loginTimeTicks, DateTimeKind.Utc);

                // --- Session Timeout Check (if re-enabled later) ---
                // const int SessionTimeoutMinutes = 30; // Example timeout
                // if ((DateTime.UtcNow - loginTime).TotalMinutes > SessionTimeoutMinutes)
                // {
                //     _logger.LogWarning("Restored session for User ID {UserId} has expired.", userId);
                //     ClearSession();
                //     OnSessionExpired(); // Manually trigger if needed on restore failure
                //     return false;
                // }
                // --- End Timeout Check ---

                // Fetch the full user details from DB using the restored ID
                // This ensures we have the *latest* user data (e.g., roles, active status)
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("User with restored ID {UserId} not found in database. Clearing session.", userId);
                    ClearSession();
                    return false;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("User with restored ID {UserId} is inactive. Clearing session.", userId);
                    ClearSession();
                    return false; // Do not restore session for inactive users
                }

                // Set the current session using the fetched user data
                SetCurrentUser(user); // This maps to DTO and sets LoginTime
                // We might overwrite the restored LoginTime with DateTime.UtcNow here if we want the session timer
                // to reset upon restore, or keep the original LoginTime from the file.
                // Let's keep the original time for accurate tracking of original login.
                this.LoginTime = loginTime; // Re-set LoginTime from file after SetCurrentUser call

                _logger.LogInformation("Session successfully restored for user {Username} (ID: {UserId}). Original login: {LoginTime}",
                    CurrentUser.Username, CurrentUser.Id, LoginTime);

                return true;
            }
            catch (CryptographicException cryptoEx)
            {
                _logger.LogError(cryptoEx, "Cryptographic error restoring session. Session data might be corrupt or from a different context.");
                ClearSession();
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generic error restoring session.");
                ClearSession(); // Clear potentially corrupt session file
                return false;
            }
        }

        private bool IsSessionExpired()
        {
            //if (LoginTime.HasValue && (DateTime.Now - LoginTime.Value).TotalMinutes > SessionTimeoutMinutes)
            //{
            //    _logger.LogWarning("Session expired.");
            //    return true;
            //}

            return false;
        }

        private void SaveSession()
        {
            if (CurrentUser != null && LoginTime.HasValue)
            {
                // Store essential, non-sensitive data needed to re-fetch the user
                // Storing only ID and LoginTime is generally safer than storing more details.
                var sessionData = $"{CurrentUser.Id}|{LoginTime.Value.Ticks}";
                try
                {
                    EnsureSessionDirectoryExists();
                    var encryptedBytes = EncryptSessionData(sessionData); // Use DPAPI
                    File.WriteAllBytes(SessionFilePath, encryptedBytes);
                    _logger.LogDebug("Session saved for user {UserId}.", CurrentUser.Id); // Debug level might be better
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save session file to {SessionFilePath}.", SessionFilePath);
                }
            }
        }

        private void ClearSession()
        {
            if (File.Exists(SessionFilePath))
            {
                File.Delete(SessionFilePath);
                _logger.LogInformation("Session file cleared.");
            }
        }

        private void StartSessionTimer()
        {
            _sessionTimer = new System.Timers.Timer(1000 * 60); // Check every minute
            _sessionTimer.Elapsed += CheckSessionStatus;
            _sessionTimer.Start();
            _logger.LogInformation("Session timer started.");
        }

        private void StopSessionTimer()
        {
            _sessionTimer?.Stop();
            _sessionTimer?.Dispose();
            _logger.LogInformation("Session timer stopped.");
        }

        private void ResetSessionTimer()
        {
            StopSessionTimer();
            StartSessionTimer();
            _logger.LogInformation("Session timer reset.");
        }

        private void CheckSessionStatus(object sender, ElapsedEventArgs e)
        {
            if (IsSessionExpired())
            {
                OnSessionExpired();
                Logout();
            }
        }

        // --- Session Expiry (kept method signature, but logic removed/disabled) ---
        protected virtual void OnSessionExpired()
        {
            // This method won't be called without the timer and timeout logic
            _logger.LogWarning("Session expired event triggered (but timeout logic is currently disabled).");
            // REMOVED: StopSessionTimer();
            SessionExpired?.Invoke(this, EventArgs.Empty);
        }


        // ==== OLD Version ==========
        //private string EncryptSessionData(string data)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(data);
        //    return Convert.ToBase64String(bytes);
        //}


        // --- Encryption using DPAPI (Windows Data Protection API) ---
        // Note: This encrypts data based on the user's Windows credentials or the local machine.
        // It's generally suitable for single-user desktop apps.
        private byte[] EncryptSessionData(string data)
        {
            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                // Use CurrentUser scope if you only want the current logged-in Windows user to decrypt
                // Use LocalMachine scope if any user on the machine should be able to decrypt (less secure)
                byte[] encryptedBytes = ProtectedData.Protect(dataBytes, s_entropy, DataProtectionScope.CurrentUser);
                return encryptedBytes;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "DPAPI encryption failed.");
                throw; // Re-throw to indicate failure
            }
        }

        // ======= OLD 
        //private string DecryptSessionData(string encryptedData)
        //{
        //    var bytes = Convert.FromBase64String(encryptedData);
        //    return Encoding.UTF8.GetString(bytes);
        //}


        private string DecryptSessionData(byte[] encryptedData)
        {
            try
            {
                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedData, s_entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "DPAPI decryption failed. Data might be corrupt or from different user/machine.");
                throw; // Re-throw to indicate failure
            }
        }

        // --- Mapping Helper ---
        private UserDTO MapUserToUserDto(User user)
        {
            // Ensure this mapping includes all fields needed by consumers of CurrentUser DTO

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
            };
        }


        public string GetCurrentUserUsername()
        {
            return CurrentUser?.Username;
        }


        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        private void EnsureSessionDirectoryExists()
        {
            var dir = Path.GetDirectoryName(SessionFilePath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                _logger.LogInformation("Created session directory: {DirectoryPath}", dir);
            }
        }

    }
}
