using AutoFusionPro.Application.DependencyInjection;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions;
using AutoFusionPro.Core.Services;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.DependencyInjection;
using AutoFusionPro.Infrastructure.HostCreation;
using AutoFusionPro.Infrastructure.Logging;
using AutoFusionPro.UI.Controls.Notifications.ToastNotifications;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels;
using AutoFusionPro.UI.ViewModels.Authentication;
using AutoFusionPro.UI.ViewModels.Dashboard;
using AutoFusionPro.UI.ViewModels.Settings;
using AutoFusionPro.UI.ViewModels.Shell;
using AutoFusionPro.UI.ViewModels.ViewNotification;
using AutoFusionPro.UI.Views;
using AutoFusionPro.UI.Views.Authentication;
using AutoFusionPro.UI.Views.Dashboard;
using AutoFusionPro.UI.Views.Settings;
using AutoFusionPro.UI.Views.Shell;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Wpf.Ui;

namespace AutoFusionPro.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region Private Fields

        private readonly IHost _host = null!;
        private bool _isDesignTimeOrEfTools;

        #endregion

        #region Constructor

        public App()
        {

            // Configure logging
            var loggingService = new LoggingService();
            Log.Logger = loggingService.GetLoggerConfiguration().CreateLogger();

            // Design Time Initialization
            if (IsDesignTimeEnvironment())
            {
                // Skip initialization if we're in EF Tools
                Console.WriteLine("Running Design Time tools, creating basic host");
                _host = CreateApplicationHost.GetDesignTimeHost(Log.Logger);
                return;
            }


            // Normal initialization for actual application
            try
            {


                this.DispatcherUnhandledException += (s, e) =>
                {
                    if (e.Exception is XamlParseException xamlEx)
                    {
                        Log.Error($"XAML Parse Error: {xamlEx.Message}");
                    }
                };

                Log.Information("Starting host");

                var builder = Host.CreateApplicationBuilder();
                builder.Logging.ClearProviders();
                builder.Logging.AddSerilog(Log.Logger);

                AddMainServices(builder.Services);

                InfrastructureLayerDI.AddInfrastructureServices(builder.Services);
                ApplicationLayerDI.AddApplicationServices(builder.Services);

                _host = builder.Build();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
        }

        #endregion

        #region Life-cycle Methods

        protected override async void OnStartup(StartupEventArgs e)
        {

            if (IsDesignTimeEnvironment())
            {
                base.OnStartup(e); // Call base and return immediately in design mode
                Console.WriteLine("Design Time Detected, Running without Service Provider");
            }
            else
            {

                var logger = _host.Services.GetRequiredService<ILogger<App>>();
                logger.LogInformation("Application started");

                await _host.StartAsync();

                var context = _host.Services.GetRequiredService<ApplicationDbContext>();

                try
                {

                    // Check for backups first
                    string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                    string dbName = ConfigurationManager.AppSettings.Get("DbName") ?? "AutoFusionPro.db";
                    string currentDbPath = Path.Combine(baseDirectory, dbName);
                    string backupDir = Path.Combine(baseDirectory, "Backups");

                    if (Directory.Exists(backupDir))
                    {
                        // Get the most recent backup file
                        var latestBackup = Directory
                            .GetFiles(backupDir, "database_backup_*.db")
                            .OrderByDescending(f => File.GetLastWriteTime(f))
                            .FirstOrDefault();

                        if (latestBackup != null)
                        {
                            logger.LogInformation($"Found latest backup: {latestBackup}");

                            // If no current database exists or it's older than the backup
                            if (!File.Exists(currentDbPath) ||
                                File.GetLastWriteTime(latestBackup) > File.GetLastWriteTime(currentDbPath))
                            {
                                logger.LogInformation("Restoring from backup...");
                                File.Copy(latestBackup, currentDbPath, true);
                            }
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(backupDir);
                    }

                    // Check if database exists, if not create it
                    if (!File.Exists(currentDbPath))
                    {
                        logger.LogWarning("Database file not found. Creating and applying migrations...");
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Database created and migrations applied successfully.");
                    }

                    // Load Language Settings
                    var settings = SettingsManager.LoadSettings();

                    var localizationService = _host.Services.GetRequiredService<ILocalizationService<FlowDirection>>();

                    localizationService.ApplyLanguage(settings.Language);
                    localizationService.ApplyTheme(settings.IsDarkThemeEnabled);

                    // Load Splash Screen 
                    // TODO Splash Screen Logic Here
                    //try
                    //{
                    //    var mainWindow = new MainWindow();

                    //    // Create and show splash screen
                    //    var splashScreen = new SplashScreen();
                    //    var splashViewModel = new SplashScreenViewModel(
                    //        _host.Services.GetRequiredService<ILogger<SplashScreenViewModel>>(),
                    //        _host.Services.GetRequiredService<SessionManager>(),
                    //        _host.Services.GetRequiredService<IGlobalSettingsService>(),
                    //        _host.Services,
                    //        _host.Services.GetRequiredService<ActivationManager>(),
                    //        splashScreen,
                    //        mainWindow
                    //    );

                    //    splashScreen.DataContext = splashViewModel;
                    //    splashScreen.Show();
                    //}
                    //catch (Exception ex)
                    //{
                    //    logger.LogError(ex, "Failed to start application");
                    //    MessageBox.Show("Failed to start application. Please check the logs for details.",
                    //        "Startup Error",
                    //        MessageBoxButton.OK,
                    //        MessageBoxImage.Error);
                    //    Current.Shutdown();
                    //}

                    //base.OnStartup(e);



                    Window mainWindow = _host.Services.GetRequiredService<MainWindow>();
                    mainWindow.DataContext = _host.Services.GetRequiredService<MainWindowViewModel>();
                    mainWindow.Show();

                    base.OnStartup(e);


                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initialize database");
                    MessageBox.Show("Failed to initialize database. Please check the logs for details.",
                        "Database Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Current.Shutdown();
                    return;
                }
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            Log.Information("Closing the application");
            await _host.StopAsync();
            await Log.CloseAndFlushAsync();
            base.OnExit(e);
        }

        #endregion

        #region Main Services Registration Method

        private static void AddMainServices(IServiceCollection services)
        {
            try
            {
                // DbContext
                services.AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger), Log.Logger);

                // Services
                services.AddSingleton<ILocalizationService<FlowDirection>, LocalizationService>();
                services.AddSingleton<IGlobalSettingsService<BitmapImage>, GlobalSettingsService>();

                services.AddTransient<IViewFactory, UIViewFactory>();
                services.AddTransient<IViewModelFactory, UIViewModelFactory>();

                services.AddTransient<ILoadingService, LoadingService>();

                services.AddSingleton<IWpfToastNotificationService, ToastNotificationService>();

                // Views
                services.AddSingleton<MainWindow>();

                services.AddScoped<ShellView>();
                services.AddTransient<DashboardView>();

                services.AddScoped<LoginView>();
                services.AddScoped<RegisterView>();

                services.AddTransient<SettingsView>();


                // ViewModels
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<DashboardViewModel>();

                services.AddTransient<NotificationViewModel>();

                services.AddTransient<ShellViewModel>();
                services.AddTransient<SettingsViewModel>();


                services.AddScoped<LoginViewModel>();
                services.AddScoped<RegisterViewModel>();
            }
            catch (Exception ex)
            {
                throw new AutoFusionProException("An error occurred while registering services inside 'App.xaml.cs'", ex);
            }
        }

        #endregion

        #region Helper methods
        private bool IsDesignTimeEnvironment()
        {
            // Check if we're in design mode more reliably
            bool isDesignTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());

            // Check if we're being called from EF Tools
            bool isEfTools = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName?.Contains("EntityFrameworkCore.Tools") == true ||
                         a.FullName?.Contains("EntityFrameworkCore.Design") == true);

            var isDesignTimeOrEfTools = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName?.Contains("Microsoft.EntityFrameworkCore.Design") == true);


            return isDesignTimeOrEfTools;
        }


        #endregion

    }

}
