using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Enums.SystemEnum;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;

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
        private ResourceDictionary _resources;
        #endregion

        #region Backup Fields
        private UserDTO? _originalUser;
        private string _originalUsername = string.Empty;
        private string _originalFirstName = string.Empty;
        private string _originalLastName = string.Empty;
        private string _originalEmail = string.Empty;
        private string _originalAddress = string.Empty;
        private string _originalCity = string.Empty;
        private string _originalPhoneNumber = string.Empty;
        private DateTime? _originalDateOfBirth;
        private Gender? _originalGender = Gender.Male;
        private Languages? _originalLanguage = Languages.Arabic;
        private string _originalProfileImage = string.Empty;
        private string? _originalSecurityQuestion = null; // Add backup for question

        private string messageTitle = string.Empty;
        private string message = string.Empty;

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
        private int _currentUserId;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChangeUsernameCommand))]
        private string _username = string.Empty;

        [ObservableProperty]
        private bool _isChangingUsername = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
        private string _confirmNewPassword = string.Empty;

        [ObservableProperty]
        private bool _isChangingPassword = false;

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
        private DateTime? _selectedDateOfBirth;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private Gender? _selectedGender = Gender.Male;

        [ObservableProperty]
        private Languages? _selectedLanguage = Languages.Arabic;

        [ObservableProperty]
        private List<string> _availableSecurityQuestions = new(); // Populate this

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateSecurityQuestionCommand))]
        private string? _selectedSecurityQuestion; // Bound to ComboBox SelectedItem

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateSecurityQuestionCommand))]
        private string _securityAnswer = string.Empty; // Bound to TextBox/PasswordBox

        [ObservableProperty]
        private bool _isUpdatingSecurityQuestion = false;


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

            // Populate Available Security Questions (example)
            LoadAvailableSecurityQuestions();

            _resources = System.Windows.Application.Current.Resources;

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


        #region Property Change Handling (for HasChanges)

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // List of properties that represent the user's editable profile data
            var profileProperties = new List<string> { /* ... FirstName, LastName etc. ... */ };

            // List of properties related to security question UI
            var securityQuestionProperties = new List<string>
            {
                nameof(SelectedSecurityQuestion),
                nameof(SecurityAnswer)
            };

            if (profileProperties.Contains(e.PropertyName))
            {
                CheckForChanges(); // Check general profile changes
            }
            // Optionally track security question changes separately if needed
            // else if (securityQuestionProperties.Contains(e.PropertyName))
            // {
            //    CheckSecurityQuestionChanges(); // Separate check if desired
            // }
        }

        private void CheckForChanges()
        {
            // Compare general profile properties
            HasChanges = /* ... comparisons for FirstName, LastName, etc. ... */
                         // Add comparison for security question selection *only if*
                         // you want the main "Save" button to handle it.
                         // It's cleaner to have a separate save button for security Q/A.
                         // SelectedSecurityQuestion != _originalSecurityQuestion;
                         false; // Let separate logic handle security question changes
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
                    messageTitle = _resources["UpdateUserFailedStr"] as string ?? "Update User Failed";
                    message = _resources["NoUserDataAvailableToSaveStr"] as string ?? "No user data available to save!";

                    _logger.LogError("No User exists to save its info");
                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));

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

                messageTitle = _resources["UpdateUserDataSucceededStr"] as string ?? "Update User Data Succeeded";
                message = _resources["UserDataHasBeenUpdatedSuccessfullyStr"] as string ?? "User data has been updated successfully!";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Success, TimeSpan.FromSeconds(8));


                HasChanges = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user changes");
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    "Update User",
                    "Error",
                    $"An error has occurred while trying to save user changes: {ex.Message}",
                    true,
                    CurrentWorkFlow);
                throw new ViewModelException($"An error has occurred while trying to save user changes: {ex.Message}", nameof(UserAccountViewModel), "SaveChangesAsync()", "Update", ex);

            }
            finally
            {
                IsLoading = false;
                message = string.Empty;
                messageTitle = string.Empty;
            }
        }


        [RelayCommand]
        private async Task CancelAsync()
        {
            if (HasChanges)
            {
                var warningStr = _resources["WarningStr"] as string ?? "Warning";
                messageTitle = _resources["DiscardChangesStr"] as string ?? "Discard Changes";
                message = _resources["YouHaveUnsavedChangesAreYouSureYouWantToDiscardThemStr"] as string ?? "You have unsaved changes. Are you sure you want to discard them?";

                var yesStr = _resources["YesStr"] as string ?? "Yes";
                var noStr = _resources["YesStr"] as string ?? "Yes";

                var result = await MessageBoxHelper.ShowConfirmationAsync(
                    warningStr,
                    messageTitle,
                    message,
                    yesStr, noStr, Wpf.Ui.Controls.ControlAppearance.Caution, CurrentWorkFlow);

                if (!result)
                    return;

                // Restore original values
                RestoreOriginalValues();
            }

            //_navigationService.NavigateTo(ApplicationPage.Dashboard);
        }

        [RelayCommand]
        private async Task ChangeProfileImageAsync()
        {
            try
            {
                if (User == null)
                {
                    messageTitle = _resources["UpdateUserFailedStr"] as string ?? "Update User Failed";
                    message = _resources["NoUserDataAvailableToSaveStr"] as string ?? "No user data available to save!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
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

                    messageTitle = _resources["UpdateUserDataSucceededStr"] as string ?? "Update User Data Succeeded";
                    messageTitle = _resources["UserProfilePhotoUpdatedSuccessfully"] as string ?? "User Profile Photo Has Been Updated Successfully";


                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Success, TimeSpan.FromSeconds(8));

                }
            }
            catch (Exception ex)
            {
               
                _logger.LogError(ex, "Error changing profile image");
                throw new ViewModelException($"Failed to change profile image: {ex.Message}", nameof(UserAccountViewModel), "ChangeChangeProfileImageAsync()", "Update", ex);
            }finally
            {
                message = string.Empty;
                messageTitle = string.Empty;
            }
        }

        [RelayCommand(CanExecute = nameof(CanUpdatePassword))]
        private async Task ChangePasswordAsync()
        {


            try
            {
                if (User == null)
                {
                    messageTitle = _resources["UpdateUserFailedStr"] as string ?? "Update User Failed";
                    message = _resources["NoUserDataAvailableToSaveStr"] as string ?? "No user data available to save!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));                    // throw new ViewModelException("No User data available", nameof(UserAccountViewModel), "ChangePasswordAsync()", "Update");
                    return;
                }

                IsChangingPassword = true;
                HasError = false;

                // Validate inputs
                if (string.IsNullOrWhiteSpace(CurrentPassword))
                {

                    messageTitle = _resources["WarningStr"] as string ?? "Warning";
                    message = _resources["CurrentPasswordIsRequiredStr"] as string ?? "Current password is required";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Warning, TimeSpan.FromSeconds(8));
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 3)
                {
                    messageTitle = _resources["IncorrectPasswordStr"] as string ?? "Incorrect Password";
                    message = _resources["NewPasswordmustbeatleast3charactersStr"] as string ?? "New Password must be at least 3 characters";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                if (NewPassword != ConfirmNewPassword)
                {
                    messageTitle = _resources["NewPasswordsDon'tMatchStr"] as string ?? "New passwords don't match";
                    message = _resources["CheckYourNewPasswordAndTheConfirmFields"] as string ?? "Check your new password and the confirm fields";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                // Verify current password and change it using UserService
                if (!await _userService.TryLogUserIn(User.Username, CurrentPassword))
                {
                    messageTitle = _resources["CurrentPasswordIsIncorrectStr"] as string ?? "Current password is incorrect";
                    message = _resources["CheckYourCurrentPasswordAndTryAgainStr"] as string ?? "Check your current password again!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                // Use UserService to change password
                var success = await _userService.ChangePasswordAsync(User.Id, CurrentPassword, NewPassword);

                if (success)
                {
                    // Re-fetch user to get updated password hash if needed
                    User = await _userService.GetUserByIdAsync(User.Id);


                    messageTitle = _resources["UserDataHasBeenUpdatedSuccessfullyStr"] as string ?? "User data has been updated successfully!";
                    message = _resources["YourPasswordHasBeenChangedSuccessfullyStr"] as string ?? "Your password has been changed successfully.";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Success, TimeSpan.FromSeconds(8));


                    // Clear password fields
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;
                }
                else
                {
                    messageTitle = _resources["FailedToUpdatePasswordStr"] as string ?? "Failed To Update Password";
                    message = _resources["FailedToChangePasswordBecauseOfInternalErrorStr"] as string ?? "Failed to change password because of an internal error!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update password in database!, {ex}", ex.Message);

                messageTitle = _resources["FailedToUpdatePasswordStr"] as string ?? "Failed To Update Password";
                message = _resources["FailedToChangePasswordBecauseOfInternalErrorStr"] as string ?? "Failed to change password because of an internal error!";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                throw new ViewModelException($"Failed to change password: {ex.Message}", nameof(UserAccountViewModel), "ChangePasswordAsync()", "Update", ex);
            }
            finally
            {
                IsChangingPassword = false;
                message = string.Empty;
                messageTitle = string.Empty;
            }
        }

        [RelayCommand(CanExecute = nameof(CanUpdateUsername))]
        private async Task ChangeUsernameAsync()
        {
            try
            {
                if (User == null)
                {
                    messageTitle = _resources["UpdateUserFailedStr"] as string ?? "Update User Failed";
                    message = _resources["NoUserDataAvailableToSaveStr"] as string ?? "No user data available to save!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));                    
                    return;
                }

                IsChangingUsername = true;
                HasError = false;

                // Validate username
                if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3)
                {

                    messageTitle = _resources["ChangeUsernameFailedStr"] as string ?? "Change Username Failed";
                    message = _resources["UsernameMustBeAtLeast3CharactersStr"] as string ?? "Username must be at least 3 characters";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Warning, TimeSpan.FromSeconds(8));
                    return;
                }

                // Check if username is already taken
                if (await _userService.UsernameExistsAsync(Username, User.Id))
                {

                    messageTitle = _resources["UsernameIsTakenStr"] as string ?? "Username is already taken!";
                    message = _resources["PleaseChooseDifferentUsernameStr"] as string ?? "Please, choose a different username!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
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


                    messageTitle = _resources["UserDataHasBeenUpdatedSuccessfullyStr"] as string ?? "User data has been updated successfully!";
                    message = _resources["YourUsernameHasBeenUpdatedStr"] as string ?? "Username has been updated successfully!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Success, TimeSpan.FromSeconds(8));

                    _logger.LogInformation("Username has been updated successfully!");


                }
                else
                {
                    messageTitle = _resources["FailedToUpdateUsernameStr"] as string ?? "Failed To Update Username";
                    message = _resources["FailedToChangeUsernameBecauseOfInternalErrorStr"] as string ?? "Failed to change username because of an internal error!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update username in database!, {ex}",ex.Message);

                messageTitle = _resources["FailedToUpdateUsernameStr"] as string ?? "Failed To Update Username";
                message = _resources["FailedToChangeUsernameBecauseOfInternalErrorStr"] as string ?? "Failed to change username because of an internal error!";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                throw new ViewModelException($"Failed to change username: {ex.Message}", nameof(UserAccountViewModel),"ChangeUsernameAsync()", "Update", ex);
            }
            finally
            {
                IsChangingUsername = false;
                message = string.Empty;
                messageTitle = string.Empty;
            }
        }


        [RelayCommand(CanExecute = nameof(CanUpdateSecurityQuestion))]
        private async Task UpdateSecurityQuestionAsync()
        {
            if (CurrentUserId <= 0) {
                messageTitle = _resources["UpdateUserFailedStr"] as string ?? "Update User Failed";
                message = _resources["NoUserDataAvailableToSaveStr"] as string ?? "No user data available to save!";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                return; 
            }

            // Validate: If a question is selected, an answer is required.
            if (!string.IsNullOrWhiteSpace(SelectedSecurityQuestion) && string.IsNullOrWhiteSpace(SecurityAnswer))
            {

                messageTitle = _resources["UpdateUserFailedStr"] as string ?? "Update User Failed";
                message = _resources["ProvideAnswerForQuestionStr"] as string ?? "Please provide an answer for the selected security question.";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Warning, TimeSpan.FromSeconds(8));

                return;
            }
            // If clearing, ensure answer is also cleared (or ignore it)
            string? plainAnswerToSend = string.IsNullOrWhiteSpace(SelectedSecurityQuestion) ? null : SecurityAnswer;
            string? questionToSend = string.IsNullOrWhiteSpace(SelectedSecurityQuestion) ? null : SelectedSecurityQuestion;

            IsUpdatingSecurityQuestion = true;
            try
            {
                var dto = new UpdateSecurityQuestionDTO(CurrentUserId, questionToSend, plainAnswerToSend);
                await _userService.UpdateSecurityQuestionAsync(dto);


                messageTitle = _resources["UserDataHasBeenUpdatedSuccessfullyStr"] as string ?? "User data has been updated successfully!";
                message = _resources["SecurityQuestionUpdatedSuccessfullyStr"] as string ?? "Security question updated successfully!";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Success, TimeSpan.FromSeconds(8));


                // Refresh user data to get the updated question and reset state
                await LoadUserDataAsync(CurrentUserId);

                // Clear the answer field after successful save
                SecurityAnswer = string.Empty;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating security question for User ID {UserId}", CurrentUserId);


                messageTitle = _resources["FailedToUpdateSecurityQuestionStr"] as string ?? "Failed to update security question";
                message = _resources["FailedToUpdateSecurityQuestionBecauseOfInternalErrorStr"] as string ?? "Failed to update security question because of an internal error";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));

                throw new ViewModelException($"Failed to change security question: {ex.Message}", nameof(UserAccountViewModel), "UpdateSecurityQuestionAsync()", "Update", ex);


            }
            finally
            {
                IsUpdatingSecurityQuestion = false;
                message = string.Empty;
                messageTitle = string.Empty;
                // Re-evaluate CanExecute after operation
                UpdateSecurityQuestionCommand.NotifyCanExecuteChanged();
            }
        }

        #endregion

        #region Initialization Methods

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

                    _shellViewModel.RequestUserLogout();
                    return;
                }

                if (_sessionManager.CurrentUser == null)
                {

                    messageTitle = _resources["InititalizationFailedStr"] as string ?? "Loading User Failed";
                    message = _resources["NoUserIsLoggedInStr"] as string ?? "No user is logged in!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));

                    _shellViewModel.RequestUserLogout();

                    return;
                }

                await LoadUserDataAsync(_sessionManager.CurrentUser.Id);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error initializing UserAccountViewModel");

                messageTitle = _resources["InititalizationFailedStr"] as string ?? "Loading User Failed";
                message = _resources["NoUserIsLoggedInStr"] as string ?? "No user is logged in!";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));

                throw new ViewModelException($"Failed to load user data: {ex.Message}", nameof(UserAccountViewModel), "InitializeData()", "Load", ex);
            }
            finally
            {
                IsLoading = false;
                messageTitle = string.Empty;
                message = string.Empty;
            }
        }

        private async Task LoadUserDataAsync(int userId)
        {
            try
            {
                IsLoading = true;
                HasError = false;

                UserDTO? loadedUserDto = await _userService.GetUserByIdAsync(userId);

                if (loadedUserDto == null)
                {
                    _logger.LogError("Session Manager has no current user");
                    messageTitle = _resources["InititalizationFailedStr"] as string ?? "Loading User Failed";
                    message = _resources["NoUserIsLoggedInStr"] as string ?? "No user is logged in!";

                    _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                    return;
                }

                // Load fresh user data from database
                User = loadedUserDto;

                // Update view model properties from User object
                UpdateFormFieldsFromUser();

                // Create backup of original values
                BackupOriginalValues();

                // Reset change tracking
                HasChanges = false;
            }
            catch (Exception ex)
            {
                messageTitle = _resources["InititalizationFailedStr"] as string ?? "Loading User Failed";
                message = _resources["NoUserIsLoggedInStr"] as string ?? "No user is logged in!";

                _toastNotificationService.Show(message, messageTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(8));
                _logger.LogError(ex, "Error loading user data");

                throw new ViewModelException($"Failed to load user data: {ex.Message}", nameof(UserAccountViewModel), "LoadUserDataAsync(int userId)", "Load", ex);

            }
            finally
            {
                IsLoading = false;
                messageTitle = string.Empty;
                message = string.Empty;
            }
        }

        private void LoadAvailableSecurityQuestions()
        {
            // Example: Load predefined questions
            // In a real app, these might come from config or a service
            AvailableSecurityQuestions = new List<string>
             {
                 "What was the name of your first pet?",
                 "What is your mother's maiden name?",
                 "What was the name of your elementary school?",
                 "In what city were you born?",
                 "What is your favorite movie?"
                 // Add a blank/null option if clearing is desired via selection
                 // "" // Represents "Clear Security Question"
             };
            // Ensure the property notification fires if needed
            // OnPropertyChanged(nameof(AvailableSecurityQuestions));
        }

        #endregion

        #region Helper Methods

        private void UpdateFormFieldsFromUser()
        {
            if (User == null) return;
            CurrentUserId = User.Id;
            Username = User.Username;
            FirstName = User.FirstName;
            LastName = User.LastName;
            Email = User.Email;
            PhoneNumber = User.PhoneNumber;
            IsAdminAccount = User.UserRole == UserRole.Admin;
            SelectedDateOfBirth = User.DateOfBirth;
            SelectedSecurityQuestion = User.SecurityQuestion;

            Address = User.Address ?? string.Empty;
            City = User.City ?? string.Empty;

            ProfileImage = User?.ProfilePictureUrl ?? "pack://application:,,,/AutoFusionPro.UI;component/Assets/Photos/Avatars/Avatar1.jpg";

            // Additional properties could be set here as needed
            SelectedGender = User?.Gender ?? Gender.Male;
            SelectedLanguage = User?.PreferredLanguage ?? Languages.Arabic;
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
            User.Gender = SelectedGender ?? Gender.Male;
            User.DateOfBirth = SelectedDateOfBirth;
            User.Address = Address;
            User.City = City;
            User.PreferredLanguage = SelectedLanguage;

            // Recalculate FullName
            User.FullName = $"{FirstName} {LastName}";
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

        #region Backup and Restore properties
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
                LastLoginDate = User.LastLoginDate,
                SecurityQuestion = User.SecurityQuestion,
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
            _originalSecurityQuestion = SelectedSecurityQuestion;
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
            SelectedGender = _originalGender ?? Gender.Male;
            SelectedLanguage = _originalLanguage;
            ProfileImage = _originalProfileImage;
            SelectedSecurityQuestion = _originalSecurityQuestion;
            

            // Reset change tracking
            HasChanges = false;
        }

        #endregion

        #region Can Execute Command Methods

        private bool CanUpdateSecurityQuestion()
        {
            // Check if either question or answer differs from original/initial state
            // Or enable if user *clears* the question/answer

            bool questionChanged = SelectedSecurityQuestion != _originalSecurityQuestion;
            bool answerEntered = !string.IsNullOrWhiteSpace(SecurityAnswer);
            bool originallyHadQuestion = !string.IsNullOrWhiteSpace(_originalSecurityQuestion);
            bool clearing = originallyHadQuestion && string.IsNullOrWhiteSpace(SelectedSecurityQuestion); // User chose to clear
            bool settingNew = !string.IsNullOrWhiteSpace(SelectedSecurityQuestion) && answerEntered;

            // Enable if setting new, clearing existing, or changing question (requires new answer)
            bool hasRelevantChange = clearing || settingNew;

            // Consider if changing only the question without a new answer is valid - maybe not.

            // Enable command if there's a change OR if clearing is intended, and not busy
            return (hasRelevantChange) && !IsUpdatingSecurityQuestion;
        }
        private bool CanUpdateUsername()
        {
            if (!string.IsNullOrEmpty(Username))
                return true;
            return false;
        }
        private bool CanUpdatePassword()
        {
            if (!string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(ConfirmNewPassword))
                return true;
            return false;
        }

        #endregion
    }
}
