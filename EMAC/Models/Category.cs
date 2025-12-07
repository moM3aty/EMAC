using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMAC.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب")]
        public string Name { get; set; }

        public string? ImageUrl { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Category? ParentCategory { get; set; }

        public ICollection<Category>? SubCategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}