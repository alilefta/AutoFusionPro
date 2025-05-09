using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        //public string NameAr { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; } = string.Empty;

        // Optional: Hierarchical structure
        public int? ParentCategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> Subcategories { get; set; }= new List<Category>(); // Or HashSet<Category>

        public virtual ICollection<Part> Parts { get; set; } = new List<Part>(); // Or HashSet<Part>
    }
}
