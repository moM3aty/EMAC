using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMAC.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون رقماً موجباً")]
        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        public string ImageUrl { get; set; }

        public string Brand { get; set; } 

        public string SizeOrCapacity { get; set; } 

        public bool IsFeatured { get; set; } = false;

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int DiscountPercentage
        {
            get
            {
                if (OldPrice.HasValue && OldPrice.Value > Price)
                {
                    return (int)((1 - (Price / OldPrice.Value)) * 100);
                }
                return 0;
            }
        }
    }
}