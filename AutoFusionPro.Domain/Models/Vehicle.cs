using AutoFusionPro.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Domain.Models
{
    public class Vehicle : BaseEntity
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Engine { get; set; }
        public string Transmission { get; set; }
        public string VIN { get; set; }

        public virtual ICollection<PartCompatibility> CompatibleParts { get; set; }
    }
}
