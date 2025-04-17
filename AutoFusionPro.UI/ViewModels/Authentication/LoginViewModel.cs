using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Input;

namespace AutoFusionPro.UI.ViewModels.Authentication
{
    public class LoginViewModel : BaseViewModel
    {

       // private IAuthenticationService _authenticationService;
        private readonly ISessionManager _sessionManager;
        private readonly ILocalizationService<FlowDirection> _localizationService;
        private readonly ILogger<LoginViewModel> _logger;

        public event EventHandler OnLoginSuccessful = null!;
        public event EventHandler ShowRegisterView = null!;


        public ICommand LoginCommand { get; } = null!;
        public ICommand RegisterCommand { get; } = null!;

        public LoginViewModel(ISessionManager sessionManager, ILocalizationService<FlowDirection> localizationService, ILogger<LoginViewModel> logger)
        {
           // _authenticationService = authenticationService;
            _sessionManager = sessionManager;
            _localizationService = localizationService;
            _logger = logger;

            CurrentWorkFlow = _localizationService.CurrentFlowDirection;

            _localizationService.FlowDirectionChanged += OnFlowDirectionChanged;

            //LoginCommand = new RelayCommand(action: async o => { await LoginAsync(); }, canExecute: o => CanLogin());
           // RegisterCommand = new RelayCommand(action: o => { ShowRegisterView?.Invoke(this, EventArgs.Empty); ResetFields(); }, canExecute: o => true);

        }


        private bool CanLogin()
        {
            //return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
            return true;
        }

        private void OnFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }
    }
}
