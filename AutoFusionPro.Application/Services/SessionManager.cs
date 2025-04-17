using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Timers;

namespace AutoFusionPro.Application.Services
{
    public class SessionManager : ISessionManager
    {
        private const string SessionFilePath = "session.json";
        private const int SessionTimeoutMinutes = int.MaxValue;

        private TaskCompletionSource<bool> _initializationComplete;
        private bool _isInitialized;

        private readonly ILogger<SessionManager> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private System.Timers.Timer _sessionTimer;

        public DateTime? LoginTime { get; private set; }
        public Task Initialized => _initializationComplete.Task;
        // Can't use Prop change here
        public User? CurrentUser { get; private set; }

        //private User _currentUser;
        //public User CurrentUser
        //{
        //    get => _currentUser;
        //    set
        //    {
        //        _currentUser = value;
        //        OnPropertyChanged(nameof(CurrentUser));
        //    }
        //}



        public bool IsUserLoggedIn => CurrentUser != null && LoginTime.HasValue && !IsSessionExpired();

        //public event PropertyChangedEventHandler? PropertyChanged;


        // Event for session expiration
        public event EventHandler SessionExpired;

        public SessionManager(ILogger<SessionManager> logger, IUnitOfWork unitOfWork)
        {
            _initializationComplete = new TaskCompletionSource<bool>();

            _logger = logger;
            _unitOfWork = unitOfWork;
            StartSessionTimer();
            _logger.LogInformation("SessionManager initialized with session timeout of {SessionTimeoutMinutes} minutes.", SessionTimeoutMinutes);

            // For Testing Purposes. To Be deleted after implementing User auth
            CurrentUser = new User
            {
                Username = "Ali",
                UserRole = Core.Enums.ModelEnum.UserRole.Admin
            };
        }

        public async Task Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                await TryRestoreSession();
                StartSessionTimer();
                _isInitialized = true;
                _initializationComplete.SetResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize SessionManager");
                _initializationComplete.SetException(ex);
                throw;
            }
        }

        public bool IsAdmin()
        {
            var isAdmin = CurrentUser?.IsAdmin ?? false;
            _logger.LogInformation("Checked admin status: {IsAdmin}", isAdmin);
            return isAdmin;
        }

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
            LoginTime = DateTime.Now;

            _logger.LogInformation("User {Username} logged in at {LoginTime}.", user.Username, LoginTime);

            SaveSession();
            ResetSessionTimer();
        }

        public void Logout()
        {
            _logger.LogInformation("User {Username} logged out at {LogoutTime}.", CurrentUser?.Username, DateTime.Now);

            CurrentUser = null;
            LoginTime = null;

            ClearSession();
            StopSessionTimer();
        }


        public async Task<bool> TryRestoreSession()
        {
            if (!File.Exists(SessionFilePath))
            {
                _logger.LogInformation("No session file found");
                return false;
            }

            try
            {
                var sessionData = await File.ReadAllTextAsync(SessionFilePath);
                var decryptedData = DecryptSessionData(sessionData);
                var parts = decryptedData.Split('|');

                if (parts.Length != 3)
                {
                    _logger.LogWarning("Invalid session data format");
                    return false;
                }

                if (!int.TryParse(parts[0], out var userId) ||
                    !DateTime.TryParse(parts[1], out var loginTime))
                {
                    _logger.LogWarning("Could not parse session data");
                    return false;
                }

                // Disabled session timeout for now
                //if ((DateTime.Now - loginTime).TotalMinutes > SessionTimeoutMinutes)
                //{
                //    _logger.LogWarning("Session has expired");
                //    return false;
                //}

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                //var user = await uow.Users.GetUserByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("User not found in database");
                    return false;
                }

                CurrentUser = user;
                LoginTime = loginTime;
                _logger.LogInformation("Session restored for user {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring session");
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
                var sessionData = $"{CurrentUser.Id}|{LoginTime}|{CurrentUser.IsAdmin}";
                var encryptedData = EncryptSessionData(sessionData);
                File.WriteAllText(SessionFilePath, encryptedData);
                _logger.LogInformation("Session saved for user {UserId}.", CurrentUser.Id);
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

        private void OnSessionExpired()
        {
            _logger.LogWarning("Session expired event triggered.");
            StopSessionTimer();
            SessionExpired?.Invoke(this, EventArgs.Empty);
        }

        private string EncryptSessionData(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }

        private string DecryptSessionData(string encryptedData)
        {
            var bytes = Convert.FromBase64String(encryptedData);
            return Encoding.UTF8.GetString(bytes);
        }


        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}


    }
}
