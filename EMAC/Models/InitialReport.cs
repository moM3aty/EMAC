using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMAC.Models
{
    public class InitialReport
    {
        [Key]
        public int Id { get; set; }

        public int WorkshopTicketId { get; set; }
        [ForeignKey("WorkshopTicketId")]
        public WorkshopTicket WorkshopTicket { get; set; }

        [Required]
        public string FaultDescription { get; set; }

        public string RequiredSpareParts { get; set; }

        public decimal EstimatedCost { get; set; }

        public string EstimatedTime { get; set; }

        public string TechnicianNotes { get; set; }

        public bool CustomerApproval { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}