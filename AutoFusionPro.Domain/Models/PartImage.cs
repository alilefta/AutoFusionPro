using AutoFusionPro.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Domain.Models
{
    public class PartImage : BaseEntity
    {
        [Required]
        public int PartId { get; set; }
        public virtual Part Part { get; set; } = null!;

        [Required]
        [StringLength(500)] // Path or URL
        public string ImagePath { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Caption { get; set; }
        public bool IsPrimary { get; set; } = false; // Mark one image as the main display image
        public int DisplayOrder { get; set; } = 0; // For ordering images in a gallery
    }
}
