using System.ComponentModel.DataAnnotations;

namespace EMAC.Models
{
    public class ServiceCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الكود البرمجي مطلوب (مثال: ac_maint)")]
        [StringLength(50)]
        public string Code { get; set; } 

        [Required(ErrorMessage = "اسم الخدمة بالعربية مطلوب")]
        [StringLength(100)]
        public string ArabicName { get; set; }

        [StringLength(200)]
        public string Description { get; set; } 

        public bool IsActive { get; set; } = true; 
    }
}