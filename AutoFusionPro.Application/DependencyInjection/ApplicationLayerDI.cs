using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.DTOs.InventoryTransactions;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.DTOs.UnitOfMeasure;
using AutoFusionPro.Application.Interfaces.Authentication;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Navigation;
using AutoFusionPro.Application.Interfaces.SessionManagement;
using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Application.Services.DataServices;
using AutoFusionPro.Application.Validators.Category;
using AutoFusionPro.Application.Validators.CompatibleVehicleValidator;
using AutoFusionPro.Application.Validators.Inventory;
using AutoFusionPro.Application.Validators.PartValidators;
using AutoFusionPro.Application.Validators.UnitOfMeasure;
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

            services.AddSingleton<INotificationService, NotificationService>();


            // Data Services
            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<IPartService, PartService>();
            services.AddScoped<ICompatibleVehicleService, CompatibleVehicleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
            services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();

            //services.AddScoped<IVehicleAssetService, VehicleAssetService>(); // TODO To be commented out


            // Validators
            //services.AddScoped<IValidator<CreateVehicleDto>, CreateVehicleDtoValidator>();
            //services.AddScoped<IValidator<UpdateVehicleDto>, UpdateVehicleDtoValidator>();

            services.AddScoped<IValidator<CreateCompatibleVehicleDto>, CreateCompatibleVehicleDtoValidator>();
            services.AddScoped<IValidator<UpdateCompatibleVehicleDto>, UpdateCompatibleVehicleDtoValidator>();

            services.AddScoped<IValidator<CreateMakeDto>, CreateMakeDtoValidator>();
            services.AddScoped<IValidator<UpdateMakeDto>, UpdateMakeDtoValidator>();

            services.AddScoped<IValidator<CreateModelDto>, CreateModelDtoValidator>();
            services.AddScoped<IValidator<UpdateModelDto>, UpdateModelDtoValidator>();

            services.AddScoped<IValidator<CreateLookupDto>, CreateLookupDtoValidator>();
            services.AddScoped<IValidator<UpdateLookupDto>, UpdateLookupDtoValidator>();

            services.AddScoped<IValidator<CreateEngineTypeDto>, CreateEngineTypeDtoValidator>();
            services.AddScoped<IValidator<UpdateEngineTypeDto>, UpdateEngineTypeDtoValidator>();

            services.AddScoped<IValidator<CreateTrimLevelDto>, CreateTrimLevelDtoValidator>();
            services.AddScoped<IValidator<UpdateTrimLevelDto>, UpdateTrimLevelDtoValidator>();

            services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryDtoValidator>();
            services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryDtoValidator>();

            services.AddScoped<IValidator<CreatePartDto>, CreatePartDtoValidator>();
            services.AddScoped<IValidator<UpdatePartDto>, UpdatePartDtoValidator>();

            services.AddScoped<IValidator<CreateStockReceiptDto>, CreateStockReceiptDtoValidator>();
            services.AddScoped<IValidator<CreateStockDispatchDto>, CreateStockDispatchDtoValidator>();

            services.AddScoped<IValidator<CreateStockAdjustmentDto>, CreateStockAdjustmentDtoValidator>();
            services.AddScoped<IValidator<CreateStockReturnDto>, CreateStockReturnDtoValidator>();

            services.AddScoped<IValidator<CreateUnitOfMeasureDto>, CreateUnitOfMeasureDtoValidator>();
            services.AddScoped<IValidator<UpdateUnitOfMeasureDto>, UpdateUnitOfMeasureDtoValidator>();
        }
    }
}
