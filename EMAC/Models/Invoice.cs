using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMAC.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        public int WorkshopTicketId { get; set; }
        [ForeignKey("WorkshopTicketId")]
        public WorkshopTicket WorkshopTicket { get; set; }

        public decimal LaborCost { get; set; }

        public decimal SparePartsCost { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; } = false;

        public string PaymentMethod { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.Now;

        public void CalculateTotal()
        {
            decimal subtotal = LaborCost + SparePartsCost;
            TaxAmount = subtotal * 0.15m;
            TotalAmount = subtotal + TaxAmount;
        }
    }
}