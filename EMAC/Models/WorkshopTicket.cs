using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMAC.Models
{
    public class WorkshopTicket
    {
        [Key]
        public int Id { get; set; }

        public int? ServiceRequestId { get; set; }
        [ForeignKey("ServiceRequestId")]
        public ServiceRequest ServiceRequest { get; set; }

        [Required]
        public string DeviceModel { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        public string DeviceCode { get; set; }

        public string PhysicalCondition { get; set; }

        public string Accessories { get; set; }

        public DateTime ReceivedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "InWorkshop";

        public void GenerateDeviceCode()
        {
            var random = new Random();
            DeviceCode = $"WS-{DateTime.Now:MMdd}-{random.Next(1000, 9999)}";
        }
    }
}