using System;
using System.ComponentModel.DataAnnotations;

namespace EMAC.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "يرجى اختيار الخدمة")]
        public string Service { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}