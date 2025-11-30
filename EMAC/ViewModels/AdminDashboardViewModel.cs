using System.Collections.Generic;
using EMAC.Models;

namespace EMAC.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ActiveWorkshopTickets { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<ServiceRequest> RecentRequests { get; set; }
        public List<WorkshopTicket> RecentWorkshopTickets { get; set; }
        public List<Invoice> RecentInvoices { get; set; }
    }
}