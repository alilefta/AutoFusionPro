﻿using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.Core.Exceptions.Navigation;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Categories;
using AutoFusionPro.UI.ViewModels.Dashboard;
using AutoFusionPro.UI.ViewModels.Parts;
using AutoFusionPro.UI.ViewModels.Parts.Details;
using AutoFusionPro.UI.ViewModels.Settings;
using AutoFusionPro.UI.ViewModels.User;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement;
using AutoFusionPro.UI.ViewModels.Vehicles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.Services
{
    public class UIViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UIViewFactory> _logger;

        public UIViewModelFactory(IServiceProvider serviceProvider, ILogger<UIViewFactory> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //public object GetInitializationWrapperClass()
        //{
        //    return typeof(IInitializableViewModel);
        //}

        // Helper methods to resolve ViewModels
        public object ResolveViewModel(ApplicationPage page)
        {
            return page switch
            {
                ApplicationPage.Dashboard => _serviceProvider.GetRequiredService<DashboardViewModel>(),
                ApplicationPage.Settings => _serviceProvider.GetRequiredService<SettingsViewModel>(),
                ApplicationPage.Account => _serviceProvider.GetRequiredService<UserAccountViewModel>(),
                ApplicationPage.Parts => _serviceProvider.GetRequiredService<PartsViewModel>(),
                ApplicationPage.Vehicles => _serviceProvider.GetRequiredService<VehiclesViewModel>(),
                ApplicationPage.VehicleCompatibilityManagement => _serviceProvider.GetRequiredService<VehicleCompatibilityShellViewModel>(),
                ApplicationPage.Categories => _serviceProvider.GetRequiredService<CategoriesViewModel>(),

                // Map other pages to their respective ViewModels
                _ => throw new NavigationException($"No ViewModel found for page {page}. Navigation initialization parameter could be required! which is 'Null' currently, try providing parameter to 'NavigateTo(ApplicationPage page, object param)' and try again.")
            };
        }

        public async Task<object> ResolveViewModelWithInitialization(ApplicationPage page, object parameter)
        {
            return page switch
            {
                ApplicationPage.CategoryDetails =>
                   await ResolveInitializableViewModel<CategoryDetailViewModel>(parameter),
                ApplicationPage.PartDetails =>
                    await ResolveInitializableViewModel<PartDetailsViewModel>(parameter),


                //ApplicationPage.AppointmentDetails =>
                //    _viewModelLocator.Resolve<AppointmentDetailsViewModel>(parameter),
                // Map other initializable ViewModels


                // Default Case
                _ => ResolveViewModel(page)
            };
        }

        private async Task<TViewModel> ResolveInitializableViewModel<TViewModel>(object parameter)
        where TViewModel : class, IInitializableViewModel
        {

            if(parameter == null)
            {
                throw new NavigationException($"An error occurred while initializing {nameof(TViewModel)}, the initialization parameter is required, and it was provided as 'NULL'");
            }

            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // If initialization fails, you might want to handle this more robustly
            await viewModel.InitializeAsync(parameter);
            await viewModel.Initialized;
            return viewModel;
        }
    }
}
