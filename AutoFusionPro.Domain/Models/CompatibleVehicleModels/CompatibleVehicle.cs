using AutoFusionPro.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Domain.Models.CompatibleVehicleModels
{
    /// <summary>
    /// Represents a specific vehicle configuration (model, year range, trim, etc.)
    /// used for determining part compatibility.
    /// </summary>
    public class CompatibleVehicle : BaseEntity
    {
        public int ModelId { get; set; }      // FK to Model
        public virtual Model Model { get; set; } = null!; // Navigation to Model (which has Make)

        public int YearStart { get; set; }    // e.g., 2018
        public int YearEnd { get; set; }      // e.g., 2022 (can be same as YearStart for single year)

        public int? TrimLevelId { get; set; } // FK to TrimLevel, Nullable
        public virtual TrimLevel? TrimLevel { get; set; }

        public int? TransmissionTypeId { get; set; } // FK to TransmissionType, Nullable
        public virtual TransmissionType? TransmissionType { get; set; }

        public int? EngineTypeId { get; set; } // FK to EngineType, Nullable
        public virtual EngineType? EngineType { get; set; }

        public int? BodyTypeId { get; set; } // FK to BodyType, Nullable
        public virtual BodyType? BodyType { get; set; }

        public string? VIN { get; set; } // Optional, for specific instance matching if needed

        // Navigation property: This specific vehicle configuration is linked by many PartCompatibility records
        public virtual ICollection<PartCompatibility> PartCompatibilities { get; set; } = new List<PartCompatibility>();
    }
}
