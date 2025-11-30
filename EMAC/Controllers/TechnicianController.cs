using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EMAC.Controllers
{
    public class TechnicianController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TechnicianController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("TechId")))
                return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string techName)
        {
            var tech = _context.Technicians.FirstOrDefault(t => t.FullName.Contains(techName));

            if (tech != null)
            {
                HttpContext.Session.SetString("TechId", tech.Id.ToString());
                HttpContext.Session.SetString("TechName", tech.FullName);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "اسم الفني غير مسجل في النظام";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            var techIdStr = HttpContext.Session.GetString("TechId");
            if (string.IsNullOrEmpty(techIdStr)) return RedirectToAction("Login");

            int techId = int.Parse(techIdStr);
            var today = DateTime.Today;

            var tasks = _context.ServiceRequests
                .Where(r => r.TechnicianId == techId
                            && (r.Status == "Confirmed" || r.Status == "InProcess"))
                .OrderBy(r => r.AppointmentDate)
                .ThenBy(r => r.TimeSlot)
                .ToList();

            ViewBag.TechName = HttpContext.Session.GetString("TechName");
            return View(tasks);
        }

        // --- إجراء 1: إتمام الإصلاح في الموقع (مع الفوترة) ---
        [HttpPost]
        public IActionResult FinishOnSite(int requestId, decimal laborCost, decimal partsCost, string notes)
        {
            // التحقق من صحة المدخلات (Validation)
            if (laborCost <= 0)
            {
                TempData["Error"] = "خطأ: يجب إدخال قيمة أجور اليد (أكبر من صفر).";
                return RedirectToAction("Dashboard");
            }

            if (string.IsNullOrWhiteSpace(notes))
            {
                TempData["Error"] = "خطأ: يجب كتابة ملاحظات الفني وتفاصيل الإصلاح.";
                return RedirectToAction("Dashboard");
            }

            var request = _context.ServiceRequests.Find(requestId);
            var currentTechId = HttpContext.Session.GetString("TechId");

            if (request != null && request.TechnicianId.ToString() == currentTechId)
            {
                // 1. إنشاء تذكرة ورشة (سجل فني)
                var ticket = new WorkshopTicket
                {
                    ServiceRequestId = request.Id,
                    DeviceModel = "صيانة خارجية (On-Site)",
                    SerialNumber = "N/A",
                    PhysicalCondition = notes, // نستخدم الملاحظات الحقيقية
                    Accessories = "لا يوجد (صيانة خارجية)",
                    Status = "Completed",
                    ReceivedAt = DateTime.Now
                };
                ticket.GenerateDeviceCode();
                _context.WorkshopTickets.Add(ticket);
                _context.SaveChanges();

                // 2. إنشاء تقرير فني تلقائي
                var report = new InitialReport
                {
                    WorkshopTicketId = ticket.Id,
                    FaultDescription = request.ProblemDescription ?? "صيانة دورية/إصلاح",
                    RequiredSpareParts = partsCost > 0 ? "قطع غيار متنوعة" : "لا يوجد",
                    EstimatedCost = laborCost + partsCost,
                    EstimatedTime = "تم الإنجاز",
                    TechnicianNotes = notes, // نستخدم الملاحظات الحقيقية
                    CustomerApproval = true,
                    CreatedAt = DateTime.Now
                };
                _context.InitialReports.Add(report);

                // 3. إنشاء الفاتورة
                var invoice = new Invoice
                {
                    WorkshopTicketId = ticket.Id,
                    LaborCost = laborCost,
                    SparePartsCost = partsCost,
                    IsPaid = false,
                    PaymentMethod = "Pending", // --- التصحيح: إضافة قيمة افتراضية للحقل الإلزامي ---
                    IssuedAt = DateTime.Now
                };
                invoice.CalculateTotal();
                _context.Invoices.Add(invoice);

                // 4. تحديث حالة الطلب
                request.Status = "PaymentPending";

                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        // --- إجراء 2: نقل الجهاز للورشة ---
        [HttpPost]
        public IActionResult TransferToWorkshop(int requestId, string notes)
        {
            // التحقق من وجود ملاحظات الاستلام
            if (string.IsNullOrWhiteSpace(notes))
            {
                TempData["Error"] = "خطأ: يرجى كتابة ملاحظات الاستلام قبل النقل للورشة.";
                return RedirectToAction("Dashboard");
            }

            var request = _context.ServiceRequests.Find(requestId);
            var currentTechId = HttpContext.Session.GetString("TechId");

            if (request != null && request.TechnicianId.ToString() == currentTechId)
            {
                request.Status = "InWorkshop";
                request.ProblemDescription += $" | ملاحظة الفني: {notes}";

                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}