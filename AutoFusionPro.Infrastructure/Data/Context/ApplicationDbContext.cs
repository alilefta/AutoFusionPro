/*
 * Data: # Data access layer
 * EF DbContext
 */

using AutoFusionPro.Domain.Models;
using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using AutoFusionPro.Domain.Models.VehiclesInventory;
using Microsoft.EntityFrameworkCore;

namespace AutoFusionPro.Infrastructure.Data.Context
{

    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Part> Parts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierPart> SupplierParts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }

        public DbSet<Make> Makes { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<TrimLevel> TrimLevels { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }
        public DbSet<EngineType> EngineTypes { get; set; }
        public DbSet<TransmissionType> TransmissionTypes { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleDamageImage> VehicleDamageImages { get; set; }
        public DbSet<VehicleDamageLog> VehicleDamageLogs { get; set; }
        public DbSet<VehicleDocument> VehicleDocuments { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<VehicleServiceHistory> VehicleServiceHistories { get; set; }

        public DbSet<PartCompatibilityRule> PartCompatibilityRules { get; set; }
        public DbSet<PartCompatibilityRuleTrimLevel> PartCompatibilityRuleTrimLevels { get; set; }
        public DbSet<PartCompatibilityRuleEngineType> PartCompatibilityRuleEngineTypes { get; set; }
        public DbSet<PartCompatibilityRuleTransmissionType> PartCompatibilityRuleTransmissionTypes { get; set; }
        public DbSet<PartCompatibilityRuleBodyType> PartCompatibilityRuleBodyTypes { get; set; }
        public DbSet<PartImage> PartImages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // --- Seed Data ---
            SeedVehicleTaxonomyData(modelBuilder);
            SeedLookupData(modelBuilder);
            SeedUnitOfMeasures(modelBuilder);
        }

        #region Context Methods
        // Override SaveChanges to handle audit trails and timestamps
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added || e.State == EntityState.Modified
                ));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else
                {
                    entry.Property("CreatedAt").IsModified = false;
                    entity.ModifiedAt = DateTime.UtcNow;
                }
            }
        }

        #endregion

        #region Seed Initial Data

        private void SeedLookupData(ModelBuilder modelBuilder)
        {
            // Seed Transmission Types
            modelBuilder.Entity<TransmissionType>().HasData(
                new TransmissionType { Id = 1, Name = "Automatic", CreatedAt = DateTime.UtcNow },
                new TransmissionType { Id = 2, Name = "Manual", CreatedAt = DateTime.UtcNow },
                new TransmissionType { Id = 3, Name = "CVT (Continuously Variable Transmission)", CreatedAt = DateTime.UtcNow },
                new TransmissionType { Id = 4, Name = "Semi-Automatic", CreatedAt = DateTime.UtcNow },
                new TransmissionType { Id = 5, Name = "Dual-Clutch (DCT)", CreatedAt = DateTime.UtcNow }
            );

            // Seed Body Types (Example)
            modelBuilder.Entity<BodyType>().HasData(
                new BodyType { Id = 1, Name = "Sedan", CreatedAt = DateTime.UtcNow },
                new BodyType { Id = 2, Name = "SUV (Sport Utility Vehicle)", CreatedAt = DateTime.UtcNow },
                new BodyType { Id = 3, Name = "Hatchback", CreatedAt = DateTime.UtcNow },
                new BodyType { Id = 4, Name = "Coupe", CreatedAt = DateTime.UtcNow },
                new BodyType { Id = 5, Name = "Convertible", CreatedAt = DateTime.UtcNow },
                new BodyType { Id = 6, Name = "Minivan", CreatedAt = DateTime.UtcNow },
                new BodyType { Id = 7, Name = "Truck (Pickup)", CreatedAt = DateTime.UtcNow },
                new BodyType { Id = 8, Name = "Wagon (Estate)", CreatedAt = DateTime.UtcNow }
            );

            // Seed Engine Types (Example - you'll have more)
            modelBuilder.Entity<EngineType>().HasData(
                new EngineType { Id = 1, Name = "Gasoline - Inline 4 (I4)", Code = "I4_GAS", CreatedAt = DateTime.UtcNow },
                new EngineType { Id = 2, Name = "Gasoline - V6", Code = "V6_GAS", CreatedAt = DateTime.UtcNow },
                new EngineType { Id = 3, Name = "Diesel - Inline 4 (I4)", Code = "I4_DSL", CreatedAt = DateTime.UtcNow },
                new EngineType { Id = 4, Name = "Electric", Code = "ELEC", CreatedAt = DateTime.UtcNow },
                new EngineType { Id = 5, Name = "Hybrid", Code = "HYB", CreatedAt = DateTime.UtcNow }
            );

            // You could also seed some common Makes if desired
            // modelBuilder.Entity<Make>().HasData(
            // new Make { Id = 1, Name = "Toyota", CreatedAt = DateTime.UtcNow },
            // new Make { Id = 2, Name = "Honda", CreatedAt = DateTime.UtcNow },
            // new Make { Id = 3, Name = "Nissan", CreatedAt = DateTime.UtcNow },
            // new Make { Id = 4, Name = "Ford", CreatedAt = DateTime.UtcNow }
            // );

            // IMPORTANT:
            // 1. You MUST provide primary key (Id) values for seeded data.
            // 2. These IDs must be unique and typically start from 1.
            // 3. If your BaseEntity has `CreatedAt`, `CreatedByUserId` etc., provide sensible default values.
            //    Here I've added `CreatedAt = DateTime.UtcNow`.
            // 4. The data is inserted during migrations. If you change seeded data, it might
            //    require careful migration management or manual DB updates if IDs clash.
        }

        private void SeedVehicleTaxonomyData(ModelBuilder modelBuilder)
        {
            var utcNow = DateTime.UtcNow;

            // --- Seed Makes ---
            var nissanMake = new Make { Id = 1, Name = "نيسان", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Nissan.jpg", CreatedAt = utcNow };
            var toyotaMake = new Make { Id = 2, Name = "تويوتا", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Toyota.png", CreatedAt = utcNow };
            var kiaMake = new Make { Id = 3, Name = "كيا", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Kia.jpg", CreatedAt = utcNow };
            var hyundaiMake = new Make { Id = 4, Name = "هيونداي", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Hyundai.png", CreatedAt = utcNow };

            modelBuilder.Entity<Make>().HasData(
                nissanMake,
                toyotaMake,
                kiaMake,
                hyundaiMake
            );

            // --- Seed Models (Focus on Nissan, then maybe a few top models for other seeded makes) ---

            // Nissan Models
            var altimaModel = new Model { Id = 1, MakeId = nissanMake.Id, Name = "التيما", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Altima.png", CreatedAt = utcNow };
            var rogueModel = new Model { Id = 2, MakeId = nissanMake.Id, Name = "روج", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Rogue.png", CreatedAt = utcNow };
            var sentraModel = new Model { Id = 3, MakeId = nissanMake.Id, Name = "سنترا", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Sentra.png", CreatedAt = utcNow };
            var sunnyModel = new Model { Id = 4, MakeId = nissanMake.Id, Name = "صني", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Sunny.jpg", CreatedAt = utcNow };
            var kicksModel = new Model { Id = 5, MakeId = nissanMake.Id, Name = "كيكس", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Kicks.jpg", CreatedAt = utcNow };
            var maximaModel = new Model { Id = 6, MakeId = nissanMake.Id, Name = "ماكزيما", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Maxima.png", CreatedAt = utcNow };
            var qashqqaiModel = new Model { Id = 7, MakeId = nissanMake.Id, Name = "قاشقاي", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Qashqqai.png", CreatedAt = utcNow };

            // Example: Toyota Models
            //var camryModel = new Model { Id = 101, MakeId = toyotaMake.Id, Name = "Camry", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/toyota_camry.jpg", CreatedAt = utcNow };
            //var rav4Model = new Model { Id = 102, MakeId = toyotaMake.Id, Name = "RAV4", ImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/toyota_rav4.jpg", CreatedAt = utcNow };

            modelBuilder.Entity<Model>().HasData(
                altimaModel,
                rogueModel,
                sentraModel,
                sunnyModel,
                kicksModel,
                maximaModel,
                qashqqaiModel
            );

            // --- Seed Trim Levels (Optional, could be very extensive. Maybe a few common ones for key models) ---
            //Example for Nissan Altima

            modelBuilder.Entity<TrimLevel>().HasData(
                new TrimLevel { Id = 1, ModelId = altimaModel.Id, Name = "S", CreatedAt = utcNow },
                new TrimLevel { Id = 2, ModelId = altimaModel.Id, Name = "SV", CreatedAt = utcNow },
                new TrimLevel { Id = 3, ModelId = altimaModel.Id, Name = "SR", CreatedAt = utcNow },
                new TrimLevel { Id = 4, ModelId = altimaModel.Id, Name = "SL", CreatedAt = utcNow },
                new TrimLevel { Id = 5, ModelId = altimaModel.Id, Name = "Platinum", CreatedAt = utcNow },

                new TrimLevel { Id = 6, ModelId = sentraModel.Id, Name = "S", CreatedAt = utcNow },
                new TrimLevel { Id = 7, ModelId = sentraModel.Id, Name = "SV", CreatedAt = utcNow },
                new TrimLevel { Id = 8, ModelId = sentraModel.Id, Name = "SR", CreatedAt = utcNow },

                new TrimLevel { Id = 9, ModelId = rogueModel.Id, Name = "S", CreatedAt = utcNow },
                new TrimLevel { Id = 10, ModelId = rogueModel.Id, Name = "SV", CreatedAt = utcNow },
                new TrimLevel { Id = 11, ModelId = rogueModel.Id, Name = "SL", CreatedAt = utcNow },

                new TrimLevel { Id = 12, ModelId = sunnyModel.Id, Name = "S", CreatedAt = utcNow },
                new TrimLevel { Id = 13, ModelId = sunnyModel.Id, Name = "SV", CreatedAt = utcNow },
                new TrimLevel { Id = 14, ModelId = sunnyModel.Id, Name = "SL", CreatedAt = utcNow },

                new TrimLevel { Id = 15, ModelId = maximaModel.Id, Name = "S", CreatedAt = utcNow },
                new TrimLevel { Id = 16, ModelId = maximaModel.Id, Name = "SV", CreatedAt = utcNow },
                new TrimLevel { Id = 17, ModelId = maximaModel.Id, Name = "SL", CreatedAt = utcNow },
                new TrimLevel { Id = 18, ModelId = maximaModel.Id, Name = "SR", CreatedAt = utcNow },
                new TrimLevel { Id = 19, ModelId = maximaModel.Id, Name = "Platinum", CreatedAt = utcNow },
                new TrimLevel { Id = 20, ModelId = maximaModel.Id, Name = "Platinum Reserve", CreatedAt = utcNow }

            );

            //// --- Seed Transmission Types, Engine Types, Body Types (as done previously) ---
            //// Ensure these use distinct ID ranges from Makes/Models if you're manually assigning IDs.
            //modelBuilder.Entity<TransmissionType>().HasData(
            //    new TransmissionType { Id = 1001, Name = "Automatic", CreatedAt = utcNow },
            //    new TransmissionType { Id = 1002, Name = "Manual", CreatedAt = utcNow },
            //    new TransmissionType { Id = 1003, Name = "CVT", CreatedAt = utcNow }
            //// ...
        }

        private void SeedUnitOfMeasures(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitOfMeasure>().HasData(
                new UnitOfMeasure { Id = 1, Name = "Piece", Symbol = "pcs", CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = 2, Name = "Liter", Symbol = "L", CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = 3, Name = "Kilogram", Symbol = "kg", CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = 4, Name = "Box", Symbol = "box", CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = 5, Name = "Meter", Symbol = "m", CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = 6, Name = "Pair", Symbol = "pr", CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = 7, Name = "Set", Symbol = "set", CreatedAt = DateTime.UtcNow }
            );
        }

        #endregion
    }
}
