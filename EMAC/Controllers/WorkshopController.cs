using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EMAC.Controllers
{
    public class WorkshopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkshopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tickets = _context.WorkshopTickets
                .Include(t => t.ServiceRequest)
                .OrderByDescending(t => t.ReceivedAt)
                .ToList();
            return View(tickets);
        }

        [HttpGet]
        public IActionResult ReceiveDevice(int? requestId)
        {
            var model = new WorkshopTicket();
            if (requestId.HasValue)
            {
                var request = _context.ServiceRequests.Find(requestId);
                if (request != null)
                {
                    model.ServiceRequestId = request.Id;
                    model.DeviceCode = request.DeviceCode;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveDevice(WorkshopTicket ticket)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(ticket.DeviceCode)) ticket.GenerateDeviceCode();

                _context.Add(ticket);

                if (ticket.ServiceRequestId != null)
                {
                    var req = await _context.ServiceRequests.FindAsync(ticket.ServiceRequestId);
                    if (req != null) req.Status = "InWorkshop";
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("TicketLabel", new { id = ticket.Id });
            }
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(int ticketId, string fault, string parts, decimal cost, string time)
        {
            var report = new InitialReport
            {
                WorkshopTicketId = ticketId,
                FaultDescription = fault,
                RequiredSpareParts = parts,
                EstimatedCost = cost,
                EstimatedTime = time
            };

            _context.Add(report);
            var ticket = await _context.WorkshopTickets.FindAsync(ticketId);
            if (ticket != null) ticket.Status = "Inspected";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> GenerateInvoice(int ticketId, decimal labor, decimal parts)
        {
            var invoice = new Invoice
            {
                WorkshopTicketId = ticketId,
                LaborCost = labor,
                SparePartsCost = parts
            };
            invoice.CalculateTotal();
            _context.Add(invoice);

            var ticket = await _context.WorkshopTickets.FindAsync(ticketId);
            if (ticket != null) ticket.Status = "Completed";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> TicketLabel(int id)
        {
            var ticket = await _context.WorkshopTickets
                .Include(t => t.ServiceRequest)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();
            return View(ticket);
        }

        public async Task<IActionResult> FinalReport(int id)
        {
            var ticket = await _context.WorkshopTickets
                .Include(t => t.ServiceRequest)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            ViewBag.InitialReport = await _context.InitialReports.FirstOrDefaultAsync(r => r.WorkshopTicketId == id);
            ViewBag.Invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.WorkshopTicketId == id);

            return View(ticket);
        }
    }
}