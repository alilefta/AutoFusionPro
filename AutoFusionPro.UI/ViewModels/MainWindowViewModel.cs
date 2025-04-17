using CommunityToolkit.Mvvm.ComponentModel;
using AutoFusionPro.Core.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Shell;
using AutoFusionPro.UI.Views.Shell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;
using System.Windows;

namespace AutoFusionPro.UI.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private ILocalizationService<FlowDirection> _localizationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MainWindowViewModel> _logger;

        [ObservableProperty]
        private bool _isInitializing = true;

        [ObservableProperty]
        private string _systemName = string.Empty;

        [ObservableProperty]
        private UserControl _currentView = new();

        public MainWindowViewModel(IServiceProvider serviceProvider, ILocalizationService<FlowDirection> localizationService, ILogger<MainWindowViewModel> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _localizationService = localizationService;
            _localizationService.FlowDirectionChanged += OnCurrentFlowDirectionChanged;
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;

            SystemName = "DentaFusion Pro";

            // To be just removed and replaced later, and initialize the view model from the outer usage of MainWindowViewModel (like a Splash screen or App.xaml.cs)
            _ = SetInitialViewForDevelopmentOnlyAsync();



        }

        private void OnCurrentFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }


        private async Task SetInitialViewForDevelopmentOnlyAsync() 
        {
            IsInitializing = true;

            try
            {
                // Create and initialize HomeView
                var shellView = _serviceProvider.GetRequiredService<ShellView>();
                var shellViewModel = _serviceProvider.GetRequiredService<ShellViewModel>();

                // Wait for HomeViewModel initialization
                if (shellViewModel != null)
                {
                    await shellViewModel.InitializeAsync();

                    if (shellViewModel.Initialized.IsCompleted)
                    {
                        shellView.DataContext = shellViewModel;
                        CurrentView = shellView;
                    }
                }
                else
                {
                    _logger.LogError("An error occured while loading 'ShellViewModel'");
                }
            }
            finally 
            {
                IsInitializing = false;
            }
        }

    }
}