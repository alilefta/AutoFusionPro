﻿using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.Core.Exceptions.Navigation;
using AutoFusionPro.UI.Views.Dashboard;
using AutoFusionPro.UI.Views.Settings;
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

