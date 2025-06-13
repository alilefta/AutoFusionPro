using AutoFusionPro.Core.Enums.ModelEnum.VehicleInventory;
using AutoFusionPro.Domain.Models.Base;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoFusionPro.Domain.Models.VehiclesInventory
{
    public class Vehicle : BaseEntity // This represents an individual, trackable vehicle
    {
        // --- Core Identification & Specification (linking to your taxonomy) ---
        [Required]
        public int MakeId { get; set; }
        public virtual Make Make { get; set; } = null!;

        [Required]
        public int ModelId { get; set; }
        public virtual Model Model { get; set; } = null!;

        [Required]
        [Range(1900, 2100)] // Example year range
        public int Year { get; set; } // Manufacturing year

        public int? EngineTypeId { get; set; }
        public virtual EngineType? EngineType { get; set; }

        public int? TransmissionTypeId { get; set; }
        public virtual TransmissionType? TransmissionType { get; set; }

        public int? TrimLevelId { get; set; }
        public virtual TrimLevel? TrimLevel { get; set; }

        public int? BodyTypeId { get; set; }
        public virtual BodyType? BodyType { get; set; }

        [StringLength(17)]
        public string? VIN { get; set; } // Vehicle Identification Number - should be unique if present

        // --- Physical & Condition Details ---
        [StringLength(50)]
        public string? ExteriorColor { get; set; }

        [StringLength(50)]
        public string? InteriorColor { get; set; }

        [Range(0, 2000000)] // Example mileage range
        public int? Mileage { get; set; }
        public MileageUnit? MileageUnit { get; set; } = Core.Enums.ModelEnum.VehicleInventory.MileageUnit.Kilometers; // Enum: Kilometers, Miles

        public FuelType? FuelType { get; set; }         // Enum: Gasoline, Diesel, Electric, Hybrid, LPG, CNG
        public Core.Enums.ModelEnum.VehicleInventory.DriveType? DriveType { get; set; }       // Enum: FWD, RWD, AWD, FourWD
        public int? NumberOfDoors { get; set; }
        public int? NumberOfSeats { get; set; }

        [StringLength(100)]
        public string? RegistrationPlateNumber { get; set; }
        public DateTime? RegistrationExpiryDate { get; set; }
        [StringLength(50)]
        public string? RegistrationCountryOrState { get; set; }


        // --- Status & Sales Information ---
        public VehicleStatus Status { get; set; } = VehicleStatus.InStock; // Enum: InStock, ForSale, Sold, Reserved, NeedsRepair, Scrapped

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; } // Price at which this vehicle was acquired
        public DateTime? PurchaseDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AskingPrice { get; set; }   // Current listed price if ForSale

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoldPrice { get; set; }
        public DateTime? SoldDate { get; set; }
        public int? SoldToCustomerId { get; set; } // FK to Customer if sold
        public virtual Customer? SoldToCustomer { get; set; }

        // --- Notes & Features ---
        [StringLength(2000)]
        public string? GeneralNotes { get; set; }

        [StringLength(1000)]
        public string? FeaturesList { get; set; } // Could be a comma-separated list, JSON, or a related table for structured features

        // --- Navigation Properties for Related Detailed Information ---
        public virtual ICollection<VehicleImage> Images { get; set; } = new List<VehicleImage>();
        public virtual ICollection<VehicleDamageLog> DamageLogs { get; set; } = new List<VehicleDamageLog>();
        public virtual ICollection<VehicleServiceHistory> ServiceRecords { get; set; } = new List<VehicleServiceHistory>();
        public virtual ICollection<VehicleDocument> Documents { get; set; } = new List<VehicleDocument>();

        // For part compatibility, this vehicle *IS* a specific configuration.
        // It can be *used* to find parts (by matching its spec to CompatibleVehicle specs),
        // but it doesn't *have* "CompatibleParts" in the same way a Part has CompatibleVehicles.
        // If you want to list parts KNOWN to fit THIS SPECIFIC VIN'd car (perhaps beyond general spec),
        // that's a different concept, maybe `FittedParts` or `VerifiedCompatiblePartsForThisVIN`.
        // For now, I'm removing `CompatibleParts` from here to avoid confusion with the `CompatibleVehicle` spec entity.
        // The link from Part -> PartCompatibility -> CompatibleVehicle is how you find parts for a type of vehicle.
        // To find parts for THIS specific vehicle, you'd use its MakeId, ModelId, Year, TrimId etc. to query against CompatibleVehicle specs.
    }
}
