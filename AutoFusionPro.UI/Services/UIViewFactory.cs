using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.Core.Exceptions.Navigation;
using AutoFusionPro.UI.Views.Categories;
using AutoFusionPro.UI.Views.Dashboard;
using AutoFusionPro.UI.Views.Parts;
using AutoFusionPro.UI.Views.Parts.Details;
using AutoFusionPro.UI.Views.Settings;
using AutoFusionPro.UI.Views.User;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement;
using AutoFusionPro.UI.Views.Vehicles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Services
{
    public class UIViewFactory : IViewFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UIViewFactory> _logger;

        public UIViewFactory(IServiceProvider serviceProvider, ILogger<UIViewFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public UserControl CreateView(ApplicationPage page)
        {
            try
            {
                UserControl view = page switch
                {
                    ApplicationPage.Dashboard => _serviceProvider.GetRequiredService<DashboardView>(),
                    ApplicationPage.Settings => _serviceProvider.GetRequiredService<SettingsView>(),
                    ApplicationPage.Account => _serviceProvider.GetRequiredService<UserAccountView>(),
                    ApplicationPage.Parts => _serviceProvider.GetRequiredService<PartsView>(),
                    ApplicationPage.Vehicles => _serviceProvider.GetRequiredService<VehiclesView>(),
                    ApplicationPage.VehicleCompatibilityManagement => _serviceProvider.GetRequiredService<VehicleCompatibilityView>(),
                    ApplicationPage.Categories => _serviceProvider.GetRequiredService<CategoriesView>(),
                    ApplicationPage.CategoryDetails => _serviceProvider.GetRequiredService<CategoryDetailView>(),
                    ApplicationPage.PartDetails => _serviceProvider.GetRequiredService<PartDetailsView>(),
                                      _ => throw new ArgumentOutOfRangeException(nameof(page), page, "Unsupported page type.")
                };
                
                _logger.LogInformation("Retrieved view for page: {Page}", page);
                return view;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, "Unsupported page type: {Page}", page);
                throw new NavigationException(page, $"Unsupported page type: {page}", ex);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to resolve view for page: {Page}", page);
                throw new NavigationException(page, $"Failed to resolve view for page: {page}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving view for page: {Page}", page);

                return _serviceProvider.GetRequiredService<DashboardView>();

                throw new NavigationException(page, $"Unexpected error retrieving view for page: {page}", ex);

            }
        }
    }
}

