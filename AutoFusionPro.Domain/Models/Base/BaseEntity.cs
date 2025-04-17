using System.ComponentModel.DataAnnotations;

namespace AutoFusionPro.Domain.Models.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // You can add other common properties here LATER if needed, but start simple.
        // For example, you MIGHT consider adding audit properties later, but not initially:

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ModifiedAt { get; set; }

        public int? CreatedByUserId { get; set; }
        public int? ModifiedByUserId { get; set; }
    }
}
