using AutoFusionPro.Application.Interfaces.Storage;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Interfaces.Repository.ICompatibleVehicleRepositories;
using AutoFusionPro.Domain.Interfaces.Repository.PartCompatibilityRulesRepositories;
using AutoFusionPro.Domain.Interfaces.Repository.VehicleInventory;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories;
using AutoFusionPro.Infrastructure.Data.Repositories.CompatibleVehicleRepositories;
using AutoFusionPro.Infrastructure.Data.Repositories.JointTableRepositories;
using AutoFusionPro.Infrastructure.Data.Repositories.PartCompatibilityRules;
using AutoFusionPro.Infrastructure.Data.Repositories.VehicleInventory;
using AutoFusionPro.Infrastructure.Data.UnitOfWork;
using AutoFusionPro.Infrastructure.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.IO;
using System.Reflection;

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

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPartRepository, PartRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();


            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IVehicleDamageImageRepository, VehicleDamageImageRepository>();
            services.AddScoped<IVehicleDamageLogRepository, VehicleDamageLogRepository>();
            services.AddScoped<IVehicleDocumentRepository, VehicleDocumentRepository>();
            services.AddScoped<IVehicleImageRepository, VehicleImageRepository>();
            services.AddScoped<IVehicleServiceHistoryRepository, VehicleServiceHistoryRepository>();


            services.AddScoped<IMakeRepository, MakeRepository>();
            services.AddScoped<IModelRepository, ModelRepository>();
            services.AddScoped<IBodyTypeRepository, BodyTypeRepository>();
            services.AddScoped<IEngineTypeRepository, EngineTypeRepository>();
            services.AddScoped<ITransmissionTypeRepository, TransmissionTypeRepository>();
            services.AddScoped<ITrimLevelRepository, TrimLevelRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();

            services.AddScoped<IPartCompatibilityRuleRepository, PartCompatibilityRuleRepository>();
            services.AddScoped<IPartCompatibilityRuleEngineTypeRepository, PartCompatibilityRuleEngineTypeRepository>();
            services.AddScoped<IPartCompatibilityRuleBodyTypeRepository, PartCompatibilityRuleBodyTypeRepository>();
            services.AddScoped<IPartCompatibilityRuleTransmissionTypeRepository, PartCompatibilityRuleTransmissionTypeRepository>();
            services.AddScoped<IPartCompatibilityRuleTrimLevelRepository, PartCompatibilityRuleTrimLevelRepository>();
            services.AddScoped<IPartImageRepository, PartImageRepository>();


            services.AddScoped<ISupplierPartRepository, SupplierPartRepository>();
            services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
            services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();




            // Services
            services.AddScoped<IImageFileService, LocalImageFileService>();






            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();


        }
    }
}
