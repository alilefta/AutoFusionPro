﻿using AutoFusionPro.Application.Commands;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.Core.Services;
using AutoFusionPro.UI.Controls.Notifications.ToastNotifications;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.UserControls;
using AutoFusionPro.UI.ViewModels.ViewNotification;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.ViewModels.Shell
{
    /// <summary>
    /// Shell View Model: Represent the main content wrapper of the application, It shows the [Side Panel] and the business logic views [Dashboard, Patients, Reports, ...]. It handles the navigation of the application.
    /// The only view model that is responsible for navigation.
    /// </summary>
    public partial class ShellViewModel : InitializableViewModel
    {

        #region Private Fields

        private ILocalizationService<FlowDirection> _localizationService;
        private INavigationService _navigationService;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger<ShellViewModel> _logger;
        private readonly IGlobalSettingsService<BitmapImage> _globalSettingsService;

        [ObservableProperty]
        private NotificationViewModel _notificationViewModel;
        private readonly IWpfToastNotificationService _toastNotificationService;

        #endregion

        #region General Props

        public event EventHandler NavigateOnLogoutEvent = null!;


        [ObservableProperty]
        private UserControl _currentView;

        [ObservableProperty]
        private ApplicationPage _currentViewName = ApplicationPage.Dashboard;

        [ObservableProperty]
        private bool _isLoadingContent = false;

        [ObservableProperty]
        private BitmapImage _systemLogo = new();

        [ObservableProperty]
        private string _systemName = string.Empty;

        [ObservableProperty]
        private DateTime _currentTime = DateTime.Now;

        [ObservableProperty]
        private UserAvatarViewModel _userAvatarViewModel;

        #endregion

        #region SideMenu Props 

        [ObservableProperty]
        private SymbolRegular _collapseButtonIcon = SymbolRegular.PanelLeftContract28;

        [ObservableProperty]
        private bool _collapsed;

        // Add boolean properties for each navigable page
        public bool IsDashboardSelected => SelectedPage == ApplicationPage.Dashboard;
        public bool IsSettingsSelected => SelectedPage == ApplicationPage.Settings;
        // Add properties for Schedule, Billing, Inventory, Staff, Reports, etc.

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDashboardSelected))]
        [NotifyPropertyChangedFor(nameof(IsSettingsSelected))]
        public ApplicationPage _selectedPage = ApplicationPage.Dashboard;

        [ObservableProperty]
        private ApplicationPage _loadingPage; // Unused To be removed 

        private void UpdateSideMenuState()
        {
            Collapsed = !Collapsed;

            // Toggle side menu width
            //CurrentSideMenuWidth = Collapsed ? 56 : 220; // Not Needed, Set by the animation

            //CurrentSideMenuMargin = Collapsed ? new Thickness(0, 20, 0, 0) : new Thickness(0, 20, 5, 0); // Not Needed, Set by the animation

            //CurrentLogoPadding = Collapsed ? new Thickness(0, 12, 0, 12) : new Thickness(8, 12, 8, 12); // Not Needed, Set by the animation

            CollapseButtonIcon = Collapsed ? SymbolRegular.PanelLeftExpand28 : SymbolRegular.PanelLeftContract28;
        }


        //[ObservableProperty]
        //private double _currentSideMenuWidth = 220;


        //[ObservableProperty]
        //private Thickness _currentLogoPadding = new Thickness(8, 12, 8, 12);

        //[ObservableProperty]
        //private Thickness _currentSideMenuMargin = new Thickness(0, 20, 5, 0);

        #endregion

        public ICommand NavigateCommand { get; set; }
        public ICommand NavigateBack { get; set; }
        public ICommand ToggleSideMenuCollapse { get; set; }


        public ShellViewModel(ILogger<ShellViewModel> logger,
            ILocalizationService<FlowDirection> localizationService,
            IGlobalSettingsService<BitmapImage> globalSettingsService,
            INavigationService navigationService, 
            ISessionManager sessionManager, 
            IServiceProvider serviceProvider,
            NotificationViewModel notificationViewModel, 
            IWpfToastNotificationService toastNotificationService)
        {

            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _globalSettingsService = globalSettingsService ?? throw new ArgumentNullException(nameof(globalSettingsService));

            _notificationViewModel = notificationViewModel ?? throw new ArgumentNullException(nameof(notificationViewModel));

            _toastNotificationService = toastNotificationService ?? throw new ArgumentNullException(nameof(toastNotificationService));

            CurrentWorkFlow = localizationService.CurrentFlowDirection;
            localizationService.FlowDirectionChanged += OnCurrentFlowDirectionChanged;
            _navigationService.NavigationChanged += OnNavigationChanged;

            RegisterCleanup(() => localizationService.FlowDirectionChanged -= OnCurrentFlowDirectionChanged);
            RegisterCleanup(() => _navigationService.NavigationChanged -= OnNavigationChanged);

            NavigateCommand = new RelayCommand(o => Navigate(o), o => true);
            NavigateBack = new RelayCommand(o => _navigationService.GoBack(), o => _navigationService.CanGoBack);
            ToggleSideMenuCollapse = new RelayCommand(o => UpdateSideMenuState(), o => true);


            // Initialize User Avatar
            ILogger<UserAvatarViewModel> userAvatarLogger = serviceProvider.GetRequiredService<ILogger<UserAvatarViewModel>>();
            UserAvatarViewModel UserAvatarViewModel = new UserAvatarViewModel(_localizationService, sessionManager, serviceProvider, userAvatarLogger, this);

        }

        private async void Navigate(object pageParam)
        {
            if (pageParam is ApplicationPage page)
            {
                try
                {
                    if (page == SelectedPage) return;

                    LoadingPage = page;

                    IsLoadingContent = true;

                    await Task.Delay(500);
                    _navigationService.NavigateTo(page);


                }
                finally
                {
                    IsLoadingContent = false;
                    LoadingPage = SelectedPage;
                }
            }
        }

        private void OnCurrentFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }

        private void OnNavigationChanged(object? sender, ApplicationPage e)
        {
            CurrentView = _navigationService.CurrentView;
            CurrentViewName = _navigationService.CurrentViewName;
            SelectedPage = _navigationService.CurrentViewName;

            _toastNotificationService.Show("Test", "Hello",Core.Enums.UI.ToastType.Success , duration: TimeSpan.FromSeconds(90));

        }

        public override async Task InitializeAsync(object? parameter = null)
        {
            if (IsInitialized)
            {
                return;
            }

            try
            {
                IsLoadingContent = true;


                // TODO : To be uncommented after development
                // Ensure we have a valid session
                //await _sessionManager.Initialized;

                //if (!_sessionManager.IsUserLoggedIn)
                //{
                //    _logger.LogWarning("HomeViewModel initialized without a valid user");
                //    NavigateOnLogoutEvent?.Invoke(this, EventArgs.Empty);

                //    return;
                //}

                await RunOnUiThread(async () =>
                {
                    // Initialize basic properties
                    InitializeBasicProperties();

                    // Initialize navigation
                    _navigationService.InitializeDefaultView(ApplicationPage.Dashboard);
                    CurrentViewName = _navigationService.CurrentViewName;
                    CurrentView = _navigationService.CurrentView;
                    SelectedPage = _navigationService.CurrentViewName;

                    await Task.CompletedTask;
                });

                // IMPORTANT: Call base method to complete initialization
                await base.InitializeAsync(parameter);


                _logger.LogInformation($"Current View ===== : {CurrentView}");
                _logger.LogInformation($"Is Initialized ===== : {Initialized}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing HomeViewModel");
                throw;
            }
            finally
            {
                IsLoadingContent = false;
            }
        }

        private void InitializeBasicProperties()
        {
            SystemName = _globalSettingsService.SystemName;
            SystemLogo = _globalSettingsService.GetLogoImage();
        }
    }
}

