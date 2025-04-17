using AutoFusionPro.Domain.Models.Base;

namespace AutoFusionPro.Domain.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }

        // Optional: Hierarchical structure
        public int? ParentCategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> Subcategories { get; set; }

        public virtual ICollection<Part> Parts { get; set; }
    }
}
