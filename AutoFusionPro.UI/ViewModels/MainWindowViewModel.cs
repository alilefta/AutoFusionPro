using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Authentication;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Shell;
using AutoFusionPro.UI.Views.Authentication;
using AutoFusionPro.UI.Views.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;

namespace AutoFusionPro.UI.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel<MainWindowViewModel>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISessionManager _sessionManager;


        [ObservableProperty]
        private bool _isInitializing = true;

        [ObservableProperty]
        private string _systemName = string.Empty;

        [ObservableProperty]
        private UserControl _currentView = new();

        public MainWindowViewModel(IServiceProvider serviceProvider, 
            ILocalizationService localizationService, 
            ILogger<MainWindowViewModel> logger,
            ISessionManager sessionManager) : base(localizationService, logger)
        {
            _serviceProvider = serviceProvider;
            _sessionManager = sessionManager;

            _sessionManager.SessionExpired += OnSessionExpired;
            var homeViewModel = _serviceProvider.GetRequiredService<ShellViewModel>();
            homeViewModel.NavigateOnLogoutEvent += OnLogoutChangeCurrentViewHandler;

            SystemName = "AutoFusion Pro";

            // To be just removed and replaced later, and initialize the view model from the outer usage of MainWindowViewModel (like a Splash screen or App.xaml.cs)
            //_ = SetInitialViewForDevelopmentOnlyAsync();

            _ = InitializeAsync();


        }
        public async Task InitializeAsync()
        {
            try
            {
                IsInitializing = true;

                // First initialize session manager
                await _sessionManager.Initialize();

                await Task.Delay(1000);

                // Then set up view models
                SetupViewModels();

                // Finally set the appropriate view
                await SetInitialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MainViewModel initialization");
                CurrentView = _serviceProvider.GetRequiredService<LoginView>();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private async Task SetInitialView()
        {
            if (_sessionManager.IsUserLoggedIn)
            {
                _logger.LogInformation("Valid session found for user {Username}", _sessionManager.CurrentUser?.Username);

                // Create and initialize HomeView
                var homeView = _serviceProvider.GetRequiredService<ShellView>();
                var homeViewModel = _serviceProvider.GetRequiredService<ShellViewModel>();

                // Wait for HomeViewModel initialization
                if (homeViewModel != null)
                {
                    await homeViewModel.InitializeAsync();
                }

                homeView.DataContext = homeViewModel;
                CurrentView = homeView;
            }
            else
            {
                _logger.LogInformation("No valid session found, showing login view");
                CurrentView = _serviceProvider.GetRequiredService<LoginView>();
            }
        }

        private void SetupViewModels()
        {
            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            loginViewModel.OnLoginSuccessful += HandleLoginSuccessful;
            loginViewModel.ShowRegisterView += OnShowRegisterViewRequest;

            var registerViewModel = _serviceProvider.GetRequiredService<RegisterViewModel>();
            registerViewModel.OnRegisterSuccessful += HandleRegisterSuccessful;
            registerViewModel.ShowLoginView += OnShowLoginViewRequest;

            var homeViewModel = _serviceProvider.GetRequiredService<ShellViewModel>();
            homeViewModel.NavigateOnLogoutEvent += OnLogoutChangeCurrentViewHandler;
        }


        private void OnLogoutChangeCurrentViewHandler(object? sender, EventArgs e)
        {
            _logger.LogInformation("Logout event received, changing view to Login.");
            CurrentView = _serviceProvider.GetRequiredService<LoginView>();
        }

        //private async Task SetInitialViewForDevelopmentOnlyAsync() 
        //{
        //    IsInitializing = true;

        //    try
        //    {
        //        // Create and initialize HomeView
        //        var shellView = _serviceProvider.GetRequiredService<ShellView>();
        //        var shellViewModel = _serviceProvider.GetRequiredService<ShellViewModel>();

        //        // Wait for HomeViewModel initialization
        //        if (shellViewModel != null)
        //        {
        //            await shellViewModel.InitializeAsync();

        //            if (shellViewModel.Initialized.IsCompleted)
        //            {
        //                shellView.DataContext = shellViewModel;
        //                CurrentView = shellView;
        //            }
        //        }
        //        else
        //        {
        //            _logger.LogError("An error occured while loading 'ShellViewModel'");
        //        }
        //    }
        //    finally 
        //    {
        //        IsInitializing = false;
        //    }
        //}

        private void OnSessionExpired(object? sender, EventArgs e)
        {
            _logger.LogWarning("Session expired event received.");

            try
            {
                IsInitializing = true;

                var message = System.Windows.Application.Current.Resources["SessionExpiredWarningStr"] as String
                    ?? "Your session has expired. Please log in again.";

                System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                {
                    await MessageBoxHelper.ShowMessageWithoutTitleAsync(message, true, flowDirection: CurrentWorkFlow);

                    await _sessionManager.Initialize();

                    await Task.Delay(1000);


                    // Reset the session manager

                    // Get and setup login view model
                    var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
                    loginViewModel.OnLoginSuccessful += HandleLoginSuccessful;
                    loginViewModel.ShowRegisterView += OnShowRegisterViewRequest;

                    // Ensure any existing HomeViewModel is reset
                    if (_serviceProvider.GetRequiredService<ShellViewModel>() is ShellViewModel shellVM)
                    {
                        shellVM.Reset();
                    }

                    CurrentView = _serviceProvider.GetRequiredService<LoginView>();

                });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling session expiry");
                // Handle any errors during session expiry handling
            }
            finally
            {
                IsInitializing = false;
            }


        }

        private void OnShowLoginViewRequest(object? sender, EventArgs e)
        {
            _logger.LogInformation("ShowLoginViewRequest received.");
            CurrentView = _serviceProvider.GetRequiredService<LoginView>();
        }

        private void HandleRegisterSuccessful(object? sender, EventArgs e)
        {
            _logger.LogInformation("Registration successful, changing view to Login.");
            CurrentView = _serviceProvider.GetRequiredService<LoginView>();
        }

        private void OnShowRegisterViewRequest(object? sender, EventArgs e)
        {
            _logger.LogInformation("ShowRegisterViewRequest received.");
            CurrentView = _serviceProvider.GetRequiredService<RegisterView>();
        }

        private void HandleLoginSuccessful(object? sender, EventArgs e)
        {
            _logger.LogInformation("Login successful, initializing and changing view to Home.");

            try
            {

                System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                {
                    // Get both view and view model
                    var homeView = _serviceProvider.GetRequiredService<ShellView>();
                    var homeViewModel = _serviceProvider.GetRequiredService<ShellViewModel>();

                    homeViewModel.Reset();


                    // Show loading indicator if you have one
                    IsInitializing = true;


                    // Initialize the HomeViewModel - this will run on the UI thread
                    await homeViewModel.InitializeAsync();

                    // Update the view
                    CurrentView = homeView;
                });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing HomeViewModel after login");
                CurrentView = _serviceProvider.GetRequiredService<LoginView>();

                if (!System.Windows.Application.Current.MainWindow.IsLoaded) return;

                System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                {
                    // Optionally show error message to user
                    await MessageBoxHelper.ShowMessageWithoutTitleAsync(
                    "An error occurred while initializing the application. Please try again.",
                    true,
                    flowDirection: CurrentWorkFlow);
                });


            }
            finally
            {
                IsInitializing = false;
            }
        }
    }
}