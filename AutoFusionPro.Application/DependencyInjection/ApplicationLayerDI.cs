using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.DTOs.Vehicle;
using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Application.Services.DataServices;
using AutoFusionPro.Application.Validators.PartValidators;
using AutoFusionPro.Application.Validators.VehicleValidators;
using FluentValidation;
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
            services.AddTransient<IPasswordHashingService, BCryptPasswordHashingService>(); // Often singleton is fine for stateless hashers
            services.AddSingleton<IAuthenticationService, AuthenticationService>(); // Scoped often makes sense

            services.AddSingleton<IUserService, UserService>(); // Scoped

            services.AddScoped<IPartService, PartService>();

            services.AddScoped<IVehicleService, VehicleService>();

            // Register repositories
            //services.AddScoped<IPatientService, PatientService>();
            //services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddSingleton<INotificationService, NotificationService>();

            //services.AddTransient<PatientValidator>();

            // Validators
            services.AddScoped<IValidator<CreatePartDto>, CreatePartDtoValidator>();

            services.AddScoped<IValidator<CreateVehicleDto>, CreateVehicleDtoValidator>();
            services.AddScoped<IValidator<UpdateVehicleDto>, UpdateVehicleDtoValidator>();
        }
    }
}
