using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMAC.Models
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم العميل مطلوب")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "نوع الخدمة مطلوب")]
        public string ServiceType { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        public string Location { get; set; }

        [Required(ErrorMessage = "تاريخ الموعد مطلوب")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "الوقت مطلوب")]
        public string TimeSlot { get; set; }

        public string? ProblemDescription { get; set; }

        [StringLength(50)]
        public string? RequestNumber { get; set; }

        [StringLength(50)]
        public string? DeviceCode { get; set; }

        public string? Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? TechnicianId { get; set; }

        [ForeignKey("TechnicianId")]
        public Technician? AssignedTechnician { get; set; } 

        public void GenerateIdentifiers()
        {
            var random = new Random();
            RequestNumber = $"REQ-{DateTime.Now:yyyyMMdd}-{random.Next(1000, 9999)}";

            var sType = string.IsNullOrEmpty(ServiceType) ? "GEN" : ServiceType;
            var prefix = sType.Length >= 2 ? sType.Substring(0, 2).ToUpper() : sType.ToUpper();

            DeviceCode = $"DEV-{prefix}-{random.Next(100, 999)}";
        }
    }
}