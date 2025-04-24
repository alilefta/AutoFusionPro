using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.Core.Enums.SystemEnum;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace AutoFusionPro.UI.ViewModels.User
{
    public partial class UserAccountViewModel : BaseViewModel
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly ILogger<UserAccountViewModel> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly ISessionManager _sessionManager;
        private readonly INavigationService _navigationService;
        private readonly ShellViewModel _shellViewModel;
        private readonly IWpfToastNotificationService _toastNotificationService;
        private bool _hasUnsavedChanges = false;
        #endregion

        #region General Props

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isUsernameValid = true;

        [ObservableProperty]
        private UserDTO? _user;

        [ObservableProperty]
        private string _profileImage = string.Empty;

        [ObservableProperty]
        private bool _hasChanges = false;

        public List<Gender> GenderOptions { get; } = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
        public List<Languages> LanguageOptions { get; } = Enum.GetValues(typeof(Languages)).Cast<Languages>().ToList();

        #endregion

        #region User Info 

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        private string _confirmNewPassword = string.Empty;

        [ObservableProperty]
        private bool _isAdminAccount;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        [ObservableProperty]
        private string _firstName = string.Empty;

        [ObservableProperty]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _address = string.Empty;

        [ObservableProperty]
        private string _city = string.Empty;

        [ObservableProperty]
        private DateTime _selectedDateOfBirth;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private Gender _selectedGender = Gender.Male;

        [ObservableProperty]
        private Languages? _selectedLanguage = Languages.Arabic;

        #endregion


        #region Tracking Fields

        private UserDTO? _originalUser;
        private string _originalUsername = string.Empty;
        private string _originalFirstName = string.Empty;
        private string _originalLastName = string.Empty;
        private string _originalEmail = string.Empty;
        private string _originalAddress = string.Empty;
        private string _originalCity = string.Empty;
        private string _originalPhoneNumber = string.Empty;
        private DateTime _originalDateOfBirth;
        private Gender _originalGender = Gender.Male;
        private Languages? _originalLanguage = Languages.Arabic;
        private string _originalProfileImage = string.Empty;

        #endregion


        #region Constructor
        public UserAccountViewModel(
            IUserService userService,
            ILogger<UserAccountViewModel> logger,
            ILocalizationService localizationService,
            ISessionManager sessionManager,
            INavigationService navigationService, 
            ShellViewModel shellViewModel, 
            IWpfToastNotificationService toastNotificationService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _shellViewModel = shellViewModel ?? throw new ArgumentNullException(nameof(shellViewModel));
            _toastNotificationService = toastNotificationService ?? throw new ArgumentNullException(nameof(toastNotificationService));


            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            _localizationService.FlowDirectionChanged += OnCurrentWorkFlowChanged;

            RegisterCleanup(() => _localizationService.FlowDirectionChanged -= OnCurrentWorkFlowChanged);

            // Initialize and load data
            _ = InitializeData();

            // Track property changes to enable/disable save button
            PropertyChanged += (s, e) => {
                if (e.PropertyName != nameof(HasChanges) && e.PropertyName != nameof(IsLoading)
                    && e.PropertyName != nameof(HasError) && e.PropertyName != nameof(ErrorMessage))
                {
                    HasChanges = true;
                }
            };
        }
        #endregion

        #region Commands

        [RelayCommand]
        private async Task SaveChangesAsync()
        {
            try
            {
                if (User == null)
                {
                    await MessageBoxHelper.ShowMessageWithTitleAsync("Update User", "Failed", "No user data available to save!", true, CurrentWorkFlow);
                    _logger.LogError("No User exists to save its info");
                    return;
                }

                IsLoading = true;
                HasError = false;

                // Update User properties from form fields
                UpdateUserFromFormFields();

                // Create an UpdateUserDTO from the User object
                var updateDto = new UpdateUserDTO(
                    User.Id,
                    User.FirstName,
                    User.LastName,
                    User.Email,
                    User.PhoneNumber,
                    User.UserRole,
                    User.IsActive,
                    User.Gender,
                    User.PreferredLanguage,
                    User.DateOfBirth,
                    User.Address,
                    User.City
                );

                // Use user service to update user
                await _userService.UpdateUserAsync(updateDto);

                // Refresh session with updated user info
                var updatedUser = await _userService.GetUserByIdAsync(User.Id);
                if (updatedUser != null)
                {
                    User = updatedUser;
                    _sessionManager.SetCurrentUser(MapUserDtoToDomainUser(updatedUser));
                }

                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    "Update User",
                    "Successful",
                    "User data has been updated successfully!",
                    false,
                    CurrentWorkFlow);

                HasChanges = false;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error saving user changes");
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    "Update User",
                    "Error",
                    $"An error has occurred while trying to save user changes: {ex.Message}",
                    true,
                    CurrentWorkFlow);
            }
            finally
            {
                IsLoading = false;
            }
        }


        [RelayCommand]
        private async Task CancelAsync()
        {
            if (HasChanges)
            {
                var result = await MessageBoxHelper.ShowConfirmationAsync(
                    "Warning",
                    "Discard Changes",
                    "You have unsaved changes. Are you sure you want to discard them?",
                    "Yes", "No", Wpf.Ui.Controls.ControlAppearance.Caution, CurrentWorkFlow);

                if (!result)
                    return;

                // Restore original values
                RestoreOriginalValues();
            }

            _navigationService.NavigateTo(ApplicationPage.Dashboard);
        }

        [RelayCommand]
        private async Task ChangeProfileImageAsync()
        {
            try
            {
                if (User == null)
                {
                    await MessageBoxHelper.ShowMessageWithTitleAsync(
                        "Change Profile Image",
                        "Failed",
                        "No user data available!",
                        true,
                        CurrentWorkFlow);
                    return;
                }

                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                    Title = "Select Profile Image"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    // Here you would typically upload the image to your storage service
                    // Update via UserService - we'll need to add this method
                    await _userService.UpdateProfileImageAsync(User.Id, filePath);

                    // Update local state
                    ProfileImage = filePath;
                    User.ProfilePictureUrl = filePath;
                    HasChanges = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing profile image");
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    "Change Profile Image",
                    "Error",
                    $"Failed to change profile image: {ex.Message}",
                    true,
                    CurrentWorkFlow);
            }
        }
        [RelayCommand]
        private async Task ChangePasswordAsync()
        {


            try
            {
                if (User == null)
                {
                    await MessageBoxHelper.ShowMessageWithTitleAsync(
                        "Change Password",
                        "Failed",
                        "No user data available!",
                        true,
                        CurrentWorkFlow);
                    return;
                }

                IsLoading = true;
                HasError = false;

                // Validate inputs
                if (string.IsNullOrWhiteSpace(CurrentPassword))
                {
                    _toastNotificationService.Show("Current password is required", "Current Password is Required", Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 3)
                {
                    _toastNotificationService.Show("New Password must be at least 3 characters", "Incorrect Password", Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                if (NewPassword != ConfirmNewPassword)
                {
                    _toastNotificationService.Show("Check your new password and the confirm fields", "New passwords don't match", Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                // Verify current password and change it using UserService
                if (!await _userService.TryLogUserIn(User.Username, CurrentPassword))
                {
                    _toastNotificationService.Show("Check your current password again!", "Current password is incorrect", Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                // Use UserService to change password
                var success = await _userService.ChangePasswordAsync(User.Id, CurrentPassword, NewPassword);

                if (success)
                {
                    // Re-fetch user to get updated password hash if needed
                    User = await _userService.GetUserByIdAsync(User.Id);

                    _toastNotificationService.Show("Your password has been changed successfully.", "Success", Core.Enums.UI.ToastType.Success, TimeSpan.FromSeconds(8));


                    // Clear password fields
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;
                }
                else
                {
                    _toastNotificationService.Show("Failed to change password!", "Failed", Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));

                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error changing password");
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    "Change Password",
                    "Error",
                    $"Failed to change password: {ex.Message}",
                    true,
                    CurrentWorkFlow);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ChangeUsernameAsync()
        {
            try
            {
                if (User == null)
                {
                    _toastNotificationService.Show("No user data available!", "Change Username Failed", Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                IsLoading = true;
                HasError = false;

                // Validate username
                if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3)
                {
                    HasError = true;
                    ErrorMessage = "Username must be at least 3 characters";
                    return;
                }

                // Check if username is already taken
                if (await _userService.UsernameExistsAsync(Username, User.Id))
                {
                    HasError = true;
                    ErrorMessage = "Username is already taken";
                    await MessageBoxHelper.ShowMessageWithTitleAsync(
                        "Change Username",
                        "Failed",
                        "Username is already taken!",
                        true,
                        CurrentWorkFlow);
                    return;
                }

                // Update username via UserService
                var success = await _userService.UpdateUsernameAsync(User.Id, Username);

                if (success)
                {
                    // Re-fetch user data to get updated username
                    User = await _userService.GetUserByIdAsync(User.Id);

                    // Update session
                    _sessionManager.SetCurrentUser(MapUserDtoToDomainUser(User));

                    await MessageBoxHelper.ShowMessageWithTitleAsync(
                        "Change Username",
                        "Success",
                        "Username has been updated successfully!",
                        false,
                        CurrentWorkFlow);
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Failed to update username";
                    await MessageBoxHelper.ShowMessageWithTitleAsync(
                        "Change Username",
                        "Failed",
                        "Failed to update username in database!",
                        true,
                        CurrentWorkFlow);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error changing username");
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    "Change Username",
                    "Error",
                    $"Failed to change username: {ex.Message}",
                    true,
                    CurrentWorkFlow);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Helper Methods

        private async Task InitializeData()
        {
            try
            {
                IsLoading = true;
                HasError = false;

                if (!_sessionManager.Initialized.IsCompleted)
                {
                    await _sessionManager.Initialize();
                }

                if (!_sessionManager.IsUserLoggedIn)
                {
                    _logger.LogError("No user is logged in");
                    HasError = true;
                    ErrorMessage = "No user is logged in";

                    // Force logout without showing confirmation dialog
                    _shellViewModel.RequestUserLogout();
                    return;
                }

                await LoadUserDataAsync();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error initializing UserAccountViewModel");
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    "Error",
                    "Initialization Failed",
                    $"Failed to load user data: {ex.Message}",
                    true,
                    CurrentWorkFlow);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadUserDataAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;

                var userDto = _sessionManager.CurrentUser;
                if (userDto == null)
                {
                    _logger.LogError("Session Manager has no current user");
                    HasError = true;
                    ErrorMessage = "No user found in session";
                    return;
                }

                // Load fresh user data from database
                User = await _userService.GetUserByIdAsync(userDto.Id);

                if (User == null)
                {
                    _logger.LogError($"User with ID {userDto.Id} not found in database");
                    HasError = true;
                    ErrorMessage = "User not found in database";
                    return;
                }

                // Update view model properties from User object
                UpdateFormFieldsFromUser();

                // Create backup of original values
                BackupOriginalValues();

                // Reset change tracking
                HasChanges = false;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error loading user data");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateFormFieldsFromUser()
        {
            if (User == null) return;

            Username = User.Username;
            FirstName = User.FirstName;
            LastName = User.LastName;
            Email = User.Email;
            PhoneNumber = User.PhoneNumber;
            IsAdminAccount = User.UserRole == UserRole.Admin;
            ProfileImage = User?.ProfilePictureUrl ?? "pack://application:,,,/AutoFusionPro.UI;component/Assets/Photos/Avatars/Avatar1.jpg";

            // Additional properties could be set here as needed
            SelectedGender = User?.Gender ?? Gender.Male;
            SelectedLanguage = User.PreferredLanguage ?? Languages.Arabic;
        }

        private void UpdateUserFromFormFields()
        {
            if (User == null) return;

            User.FirstName = FirstName;
            User.LastName = LastName;
            User.Email = Email;
            User.PhoneNumber = PhoneNumber;
            // Don't update Username here - use the specific command for that

            // Additional properties could be updated here
            User.Gender = SelectedGender;
            User.DateOfBirth = SelectedDateOfBirth;
            User.Address = Address;
            User.City = City;
            User.PreferredLanguage = SelectedLanguage;

            // Recalculate FullName
            User.FullName = $"{FirstName} {LastName}";
        }

        private void BackupOriginalValues()
        {
            if (User == null) return;

            // Create deep copy of user
            _originalUser = new UserDTO
            {
                Id = User.Id,
                Username = User.Username,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email,
                PhoneNumber = User.PhoneNumber,
                UserRole = User.UserRole,
                IsActive = User.IsActive,
                Gender = User.Gender,
                PreferredLanguage = User.PreferredLanguage,
                DateOfBirth = User.DateOfBirth,
                Address = User.Address,
                City = User.City,
                ProfilePictureUrl = User.ProfilePictureUrl,
                PasswordHash = User.PasswordHash,
                Salt = User.Salt,
                FullName = User.FullName,
                DateRegistered = User.DateRegistered,
                LastLoginDate = User.LastLoginDate
            };

            // Backup individual properties
            _originalUsername = Username;
            _originalFirstName = FirstName;
            _originalLastName = LastName;
            _originalEmail = Email;
            _originalAddress = Address;
            _originalCity = City;
            _originalPhoneNumber = PhoneNumber;
            _originalDateOfBirth = SelectedDateOfBirth;
            _originalGender = SelectedGender;
            _originalLanguage = SelectedLanguage;
            _originalProfileImage = ProfileImage;
        }

        private void RestoreOriginalValues()
        {
            if (_originalUser == null) return;

            // Restore User object
            User = _originalUser;

            // Restore individual properties
            Username = _originalUsername;
            FirstName = _originalFirstName;
            LastName = _originalLastName;
            Email = _originalEmail;
            Address = _originalAddress;
            City = _originalCity;
            PhoneNumber = _originalPhoneNumber;
            SelectedDateOfBirth = _originalDateOfBirth;
            SelectedGender = _originalGender;
            SelectedLanguage = _originalLanguage;
            ProfileImage = _originalProfileImage;

            // Reset change tracking
            HasChanges = false;
        }

        private void OnCurrentWorkFlowChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }

        // Added to convert UserDTO to Domain User model
        private Domain.Models.User? MapUserDtoToDomainUser(UserDTO? dto)
        {
            if (dto == null) return null;
            return new Domain.Models.User
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = dto.PasswordHash,
                Salt = dto.Salt,
                IsAdmin = dto.UserRole == UserRole.Admin,
                UserRole = dto.UserRole,
                IsActive = dto.IsActive,
                ProfilePictureUrl = dto.ProfilePictureUrl,
                DateRegistered = dto.DateRegistered,
                LastLoginDate = dto.LastLoginDate
            };
        }

        #endregion
    }
}
