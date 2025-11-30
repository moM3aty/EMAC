using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EMAC.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrackingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(string requestNumber)
        {
            if (string.IsNullOrEmpty(requestNumber))
            {
                ViewBag.Error = "الرجاء إدخال رقم الطلب";
                return View("Index");
            }

            var request = _context.ServiceRequests.FirstOrDefault(r => r.RequestNumber == requestNumber);

            if (request == null)
            {
                ViewBag.Error = "رقم الطلب غير موجود، يرجى التأكد والمحاولة مرة أخرى";
                return View("Index");
            }

            // --- التعديل الجديد: جلب الاسم العربي للخدمة بدلاً من الكود ---
            var serviceCategory = _context.ServiceCategories.FirstOrDefault(s => s.Code == request.ServiceType);
            // نخزن الاسم العربي في ViewBag، وإذا لم يوجد نستخدم الكود كاحتياطي
            ViewBag.ServiceName = serviceCategory != null ? serviceCategory.ArabicName : request.ServiceType;

            var workshopTicket = _context.WorkshopTickets
                                         .FirstOrDefault(t => t.ServiceRequestId == request.Id);

            InitialReport report = null;
            Invoice invoice = null;

            if (workshopTicket != null)
            {
                report = _context.InitialReports
                                 .FirstOrDefault(r => r.WorkshopTicketId == workshopTicket.Id);

                invoice = _context.Invoices
                                  .FirstOrDefault(i => i.WorkshopTicketId == workshopTicket.Id);
            }

            ViewBag.WorkshopTicket = workshopTicket;
            ViewBag.InitialReport = report;
            ViewBag.Invoice = invoice;

            return View("Index", request);
        }

        [HttpPost]
        public async Task<IActionResult> CustomerDecision(int reportId, string decision)
        {
            var report = await _context.InitialReports.FindAsync(reportId);
            if (report != null)
            {
                var ticket = await _context.WorkshopTickets.FindAsync(report.WorkshopTicketId);
                if (decision == "approve")
                {
                    report.CustomerApproval = true;
                    if (ticket != null) ticket.Status = "Approved_Fixing";
                }
                else
                {
                    if (ticket != null) ticket.Status = "Rejected";
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم تسجيل قرارك بنجاح.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PayInvoice(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                invoice.IsPaid = true;
                invoice.PaymentMethod = "CreditCard";

                var ticket = await _context.WorkshopTickets.FindAsync(invoice.WorkshopTicketId);
                if (ticket != null)
                {
                    ticket.Status = "Closed";
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم سداد الفاتورة بنجاح! شكراً لتعاملكم معنا.";
            }
            return RedirectToAction("Index");
        }
    }
}