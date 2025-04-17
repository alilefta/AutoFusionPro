using AutoFusionPro.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Domain.Models
{
    public class PartCompatibility : BaseEntity
    {
        public int PartId { get; set; }
        public virtual Part Part { get; set; }

        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public string Notes { get; set; }
    }
}
