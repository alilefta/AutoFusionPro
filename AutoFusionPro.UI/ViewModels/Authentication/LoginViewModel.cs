using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Services;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Input;

namespace AutoFusionPro.UI.ViewModels.Authentication
{
    public partial class LoginViewModel : BaseViewModel
    {

       // private IAuthenticationService _authenticationService;
        private readonly ISessionManager _sessionManager;
        private readonly ILocalizationService _localizationService;
        private readonly IUserService _userService;
        private readonly ILogger<LoginViewModel> _logger;

        public event EventHandler OnLoginSuccessful;
        public event EventHandler ShowRegisterView;

        [ObservableProperty]
        private bool _loginIsRunning = false;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;


        public ICommand LoginCommand { get; } = null!;
        public ICommand RegisterCommand { get; } = null!;

        public LoginViewModel(ISessionManager sessionManager, 
            ILocalizationService localizationService, 
            IUserService userService,
            ILogger<LoginViewModel> logger)
        {
           // _authenticationService = authenticationService;
            _sessionManager = sessionManager;
            _localizationService = localizationService;
            _userService = userService;
            _logger = logger;

            CurrentWorkFlow = _localizationService.CurrentFlowDirection;

            _localizationService.FlowDirectionChanged += OnFlowDirectionChanged;

            LoginCommand = new RelayCommand(action: async o => { await LoginAsync(); }, canExecute: o => CanLogin());
            RegisterCommand = new RelayCommand(action: o => { ShowRegisterView?.Invoke(this, EventArgs.Empty); ResetFields(); }, canExecute: o => true);

        }


        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void OnFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                var m = System.Windows.Application.Current.Resources["FillAllFieldsStr"] as string ?? "Please fill in all fields.";
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(content: m, isError: true, flowDirection: CurrentWorkFlow);
                return;
            }

            try
            {
                bool isAuthenticated = false;
                LoginIsRunning = true;

                await Task.Run(async () =>
                {
                    isAuthenticated = await _userService.TryLogUserIn(Username, Password);

                });

                await Task.Delay(2000);

                LoginIsRunning = false;



                if (isAuthenticated)
                {

                    var successMessage = System.Windows.Application.Current.Resources["LoginSuccessfulStr"] as string ?? "You have logged in successfully!";
                    await MessageBoxHelper.ShowMessageWithTitleAsync(title: "Login Successful", subtitle: "Success", content: successMessage, flowDirection: CurrentWorkFlow);

                    ResetFields();
                    OnLoginSuccessful?.Invoke(this, EventArgs.Empty);



                }
                else
                {
                    var m = System.Windows.Application.Current.Resources["InvalidCredentialsStr"] as string ?? "Invalid username or password.";
                    await MessageBoxHelper.ShowMessageWithoutTitleAsync(content: m, isError: true, flowDirection: CurrentWorkFlow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while logging in, {ex}");
            }


        }

        public void ResetFields()
        {
            Username = string.Empty;
            Password = string.Empty;
        }
    }
}
