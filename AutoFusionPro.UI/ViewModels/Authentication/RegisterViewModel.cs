using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.DTOs.User;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.Core.Services;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;

namespace AutoFusionPro.UI.ViewModels.Authentication
{
    public partial class RegisterViewModel : BaseViewModel<RegisterViewModel>
    {
        public event EventHandler OnRegisterSuccessful;
        public event EventHandler ShowLoginView;


        //#region Validation
        ///// <summary>
        ///// Data Validation
        ///// </summary>
        //private readonly Dictionary<string, List<string>> _propertyErrors = new Dictionary<string, List<string>>();

        //public bool HasErrors => _propertyErrors.Any();
        //public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;


        //#endregion

        private bool _registerIsRunning = false;
        public bool RegisterIsRunning
        {
            get => _registerIsRunning;
            set
            {
                _registerIsRunning = value;
                OnPropertyChanged(nameof(RegisterIsRunning));
            }
        }

        private readonly IUserService _userService;

        // Use authentication service only if you need to add login immediately after successful registration
        //private readonly IAuthenticationService _authenticationService;


        #region

        // Define all properties with attributes
        [ObservableProperty]
        [Required(ErrorMessage = "Username cannot be empty")]
        [RegularExpression(@"^(?![0-9]+$)[A-Za-z][A-Za-z0-9]{1,19}$",
            ErrorMessage = "Username must be 2-20 characters and cannot be numbers only")]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Phone number cannot be empty")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        private string _phoneNumber = string.Empty;


        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Password must be between 2-20 characters")]
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value, true);
        }


        [Required(ErrorMessage = "Confirm password cannot be empty")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value, true);
        }

        #endregion


        #region Commands

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        //public ICommand ResetPasswordCommand { get; }

        #endregion

        public RegisterViewModel(IUserService userService, 
            ILocalizationService localizationService, 
            ILogger<RegisterViewModel> logger) : base(localizationService, logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            //_authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            RegisterCommand = new RelayCommand(action: async o => await RegisterAsync(), canExecute: o => CanRegister());
            LoginCommand = new RelayCommand(action: o => { ShowLoginView?.Invoke(this, EventArgs.Empty); ResetFields(); }, canExecute: o => true);
        }


        private async Task RegisterAsync()
        {

            RegisterIsRunning = true;


            if (Password != ConfirmPassword)
            {
                var m = System.Windows.Application.Current.Resources["PasswordsDontMatchStr"] as String ?? "Passwords do not match.";

                await MessageBoxHelper.ShowMessageWithoutTitleAsync(content: m, true, flowDirection: CurrentWorkFlow);

                return;
            }


            //var existingUser = await _userService.GetUserByUsername(Username);
            //if (existingUser != null)
            //{
            //    var m1 = System.Windows.Application.Current.Resources["UserExistStr"] as String ?? "User already exists!";

            //    await MessageBoxHelper.ShowMessageWithoutTitleAsync(content: m1, false, flowDirection: CurrentWorkFlow);

            //    RegisterIsRunning = false;

            //    return;
            //}

            try
            {

                var newUser = new CreateUserDTO
                (
                    string.Empty,
                    string.Empty,
                    Username,
                    Email,
                    PhoneNumber,
                    Password,
                    UserRole.User
                );

                var userDTO = await _userService.CreateUserAsync(newUser);

                if(userDTO == null )
                {
                    var message = System.Windows.Application.Current.Resources["RegisterationFailedStr"] as String ?? "Registration process failed!";

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Registration Failed.", true, flowDirection: CurrentWorkFlow);
                }



                var m = System.Windows.Application.Current.Resources["RegisterationSucceedStr"] as String ?? "Registration process was successful!";
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(m, false, flowDirection: CurrentWorkFlow);


                ResetFields();
                // Optionally, navigate to login or main view
                OnRegisterSuccessful?.Invoke(this, EventArgs.Empty);

                // --- Optional: Auto-Login ---
                // if (_authenticationService != null) {
                //     bool loggedIn = await _authenticationService.AuthenticateAsync(Username, Password);
                //     if (!loggedIn) {
                //         // Log warning or show message that auto-login failed
                //     }
                // }
                // --- End Optional ---

            }
            catch (Exception ex)
            {

                var message = System.Windows.Application.Current.Resources["RegisterationFailedStr"] as String ?? "Registration process failed!";

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"{message}: {ex.Message}", true, flowDirection: CurrentWorkFlow);

            }
            finally { RegisterIsRunning = false; }
        }

        private bool CanRegister()
        {
            ValidateAllProperties();
            return !HasErrors &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }


        public void ResetFields()
        {
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;

        }
    }
}
