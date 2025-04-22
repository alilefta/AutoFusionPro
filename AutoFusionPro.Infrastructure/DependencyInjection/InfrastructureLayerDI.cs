using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories;
using AutoFusionPro.Infrastructure.Data.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using System.IO;
using System.Configuration;
using AutoFusionPro.Core.Services;
using AutoFusionPro.Infrastructure.Services;

namespace AutoFusionPro.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Register Infrastructure services helper class
    /// </summary>
    public static class InfrastructureLayerDI
    {
        /// <summary>
        /// Register Infrastructure Services
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection">Collection of Services</see>/></param>
        public static void AddInfrastructureServices(
        this IServiceCollection services)
        {

            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            var DbName = ConfigurationManager.AppSettings.Get("DbName") ?? "AutoFusionPro.db"; ;
            string dbPath = Path.Combine(baseDirectory, DbName);

            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPartRepository, PartRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();


        }
    }
}
