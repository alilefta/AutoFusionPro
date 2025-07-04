using AutoFusionPro.Application.DependencyInjection;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.Settings;
using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.DependencyInjection;
using AutoFusionPro.Infrastructure.HostCreation;
using AutoFusionPro.Infrastructure.Logging;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.Services.Dialogs;
using AutoFusionPro.UI.ViewModels;
using AutoFusionPro.UI.ViewModels.Authentication;
using AutoFusionPro.UI.ViewModels.Categories;
using AutoFusionPro.UI.ViewModels.Categories.Dialogs;
using AutoFusionPro.UI.ViewModels.Categories.Dialogs.Filters;
using AutoFusionPro.UI.ViewModels.Dashboard;
using AutoFusionPro.UI.ViewModels.General.Dialogs;
using AutoFusionPro.UI.ViewModels.Parts;
using AutoFusionPro.UI.ViewModels.Parts.Details;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEdit;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEdit.AddEditPartDialogs;
using AutoFusionPro.UI.ViewModels.Settings;
using AutoFusionPro.UI.ViewModels.Settings.UserManagement;
using AutoFusionPro.UI.ViewModels.Shell;
using AutoFusionPro.UI.ViewModels.User;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.BodyTypes;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.EngineTypes;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.Filters;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.Transmissions;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels;
using AutoFusionPro.UI.ViewModels.Vehicles;
using AutoFusionPro.UI.ViewModels.Vehicles.Dialogs;
using AutoFusionPro.UI.ViewModels.ViewNotification;
using AutoFusionPro.UI.Views;
using AutoFusionPro.UI.Views.Authentication;
using AutoFusionPro.UI.Views.Categories;
using AutoFusionPro.UI.Views.Dashboard;
using AutoFusionPro.UI.Views.Parts;
using AutoFusionPro.UI.Views.Parts.Details;
using AutoFusionPro.UI.Views.Settings;
using AutoFusionPro.UI.Views.Shell;
using AutoFusionPro.UI.Views.User;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement;
using AutoFusionPro.UI.Views.Vehicles;
using CommunityToolkit.Mvvm.Messaging;
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
        private static Serilog.ILogger _logger;
 
        #endregion

        #region Constructor

        public App()
        {

            // Configure logging
            var loggingService = new LoggingService();
            Log.Logger = loggingService.GetLoggerConfiguration().CreateLogger();
            _logger = Log.Logger;

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

                _logger.Information("Starting host");

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
                _logger.Fatal(ex, "Host terminated unexpectedly");
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

                    var localizationService = _host.Services.GetRequiredService<ILocalizationService>();

                    localizationService.ApplyLanguageAndCurrency(settings.Language, settings.SelectedCurrency);
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
                services.AddSingleton<ILocalizationService, LocalizationService>();
                services.AddSingleton<IGlobalSettingsService, GlobalSettingsService>();

                services.AddTransient<IViewFactory, UIViewFactory>();
                services.AddTransient<IViewModelFactory, UIViewModelFactory>();

                services.AddTransient<ILoadingService, LoadingService>();

                services.AddSingleton<IWpfToastNotificationService, ToastNotificationService>();

                services.AddSingleton<IMessenger, WeakReferenceMessenger>();


                // Views
                services.AddSingleton<MainWindow>();

                services.AddScoped<ShellView>();
                services.AddTransient<DashboardView>();

                services.AddScoped<LoginView>();
                services.AddScoped<RegisterView>();

                services.AddScoped<SettingsView>();
                services.AddScoped<PartsView>();
                services.AddScoped<VehiclesView>();

                services.AddTransient<UserManagementView>();
                services.AddTransient<UserAccountView>();

                services.AddTransient<VehicleCompatibilityView>();

                services.AddTransient<CategoriesView>();


                // ViewModels
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<DashboardViewModel>();

                services.AddTransient<NotificationViewModel>();

                services.AddScoped<ShellViewModel>();
                services.AddScoped<SettingsViewModel>();

                services.AddScoped<PartsViewModel>();
                services.AddScoped<VehiclesViewModel>();

                services.AddScoped<LoginViewModel>();
                services.AddScoped<RegisterViewModel>();

                services.AddTransient<CategoriesViewModel>();
                services.AddTransient<CategoryDetailView>();

                services.AddTransient<UserManagementViewModel>();
                services.AddTransient<UserAccountViewModel>();

                services.AddTransient<PartDetailsView>();
                services.AddTransient<PartDetailsViewModel>();



                // Vehicle Compatibility View Models
                services.AddTransient<VehicleCompatibilityShellViewModel>();

                services.AddTransient<MakesModelsTrimsManagementViewModel>();
                services.AddTransient<EngineTypesManagementViewModel>();
                services.AddTransient<BodyTypesManagementViewModel>();
                services.AddTransient<CompatibilityRulesManagementViewModel>();

                services.AddTransient<TransmissionTypesManagementViewModel>();

                services.AddTransient<AddMakeDialogViewModel>();
                services.AddTransient<AddModelDialogViewModel>();
                services.AddTransient<AddTrimLevelDialogViewModel>();

                services.AddTransient<EditMakeDialogViewModel>();
                services.AddTransient<EditModelDialogViewModel>();
                services.AddTransient<EditTrimLevelDialogViewModel>();

                services.AddTransient<AddTransmissionTypeDialogViewModel>();
                services.AddTransient<EditTransmissionTypeDialogViewModel>();

                services.AddTransient<AddEngineTypeDialogViewModel>();
                services.AddTransient<EditEngineTypeDialogViewModel>();

                services.AddTransient<AddBodyTypeDialogViewModel>();
                services.AddTransient<EditBodyTypeDialogViewModel>();


                //services.AddTransient<AddCompatibleVehicleDialogViewModel>();
                //services.AddTransient<EditCompatibleVehicleDialogViewModel>();

                services.AddTransient<PartCompatibilityRuleFilterOptionsDialogViewModel>();

                services.AddTransient<AddRootCategoryDialogViewModel>();
                services.AddTransient<EditRootCategoryDialogViewModel>();

                services.AddTransient<AddSubCategoryDialogViewModel>();
                services.AddTransient<EditSubCategoryDialogViewModel>();

                services.AddTransient<CategoryDetailViewModel>();

                services.AddTransient<CategoryFilterOptionsDialogViewModel>();

                services.AddTransient<LinkNewSupplierDialogViewModel>();
                services.AddTransient<EditSupplierLinkDialogViewModel>();
                services.AddTransient<DefineVehicleSpecificationDialogViewModel>();


                // Dialogs
                services.AddTransient<AddVehicleDialogViewModel>();

                // Parts Dialogs
                services.AddTransient<AddEditPartDialogViewModel>();




                // General
                services.AddTransient<ConfirmDeleteItemsDialogViewModel>();



                // Services
                services.AddScoped<IDialogService, DialogService>();



            }
            catch (Exception ex)
            {
                _logger.Fatal("An error occurred while registering services, {EX}", ex.Message);
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
