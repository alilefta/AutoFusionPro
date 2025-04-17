using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Application.Services.DataServices;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFusionPro.Application.DependencyInjection
{
    /// <summary>
    /// Register Application Layer services helper class
    /// </summary>
    public static class ApplicationLayerDI
    {
        /// <summary>
        /// Register Infrastructure Services
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection">Collection of Services</see>/></param>
        public static void AddApplicationServices(
        this IServiceCollection services)
        {
            // Navigation
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISessionManager, SessionManager>();

            // Register repositories
            //services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddSingleton<INotificationService, NotificationService>();

            //services.AddTransient<PatientValidator>();
        }
    }
}
