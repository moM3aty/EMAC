using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using EMAC.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMAC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ... (Login, Logout, Dashboard, Requests, Invoices, InvoicePrint, Services, EditService, SaveService, DeleteService - كما هي) ...
        // (تأكد من وجود باقي الدوال التي أنشأناها سابقاً هنا)

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("IsAdmin") == "true") return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var adminUser = _configuration["AdminSettings:Username"];
            var adminPass = _configuration["AdminSettings:Password"];

            if (username == adminUser && password == adminPass)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "بيانات الدخول غير صحيحة";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var viewModel = new AdminDashboardViewModel
            {
                TotalRequests = _context.ServiceRequests.Count(),
                PendingRequests = _context.ServiceRequests.Count(r => r.Status == "Pending"),
                ActiveWorkshopTickets = _context.Set<WorkshopTicket>().Count(t => t.Status == "InWorkshop"),
                TotalRevenue = _context.Set<Invoice>().Sum(i => i.TotalAmount),

                RecentRequests = _context.ServiceRequests.OrderByDescending(r => r.CreatedAt).Take(5).ToList(),
                RecentWorkshopTickets = _context.Set<WorkshopTicket>().Include(t => t.ServiceRequest).OrderByDescending(t => t.ReceivedAt).Take(5).ToList(),
                RecentInvoices = _context.Set<Invoice>().Include(i => i.WorkshopTicket).ThenInclude(t => t.ServiceRequest).OrderByDescending(i => i.IssuedAt).Take(5).ToList()
            };

            ViewBag.ServiceNames = _context.ServiceCategories.ToDictionary(s => s.Code, s => s.ArabicName);

            return View(viewModel);
        }

        public IActionResult Requests()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var requests = _context.ServiceRequests
                .Include(r => r.AssignedTechnician)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.Technicians = _context.Technicians.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{t.FullName} ({t.Specialty})"
            }).ToList();

            ViewBag.ServiceNames = _context.ServiceCategories.ToDictionary(s => s.Code, s => s.ArabicName);

            return View(requests);
        }

        [HttpPost]
        public IActionResult ReassignTechnician(int requestId, int newTechnicianId)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var request = _context.ServiceRequests.Find(requestId);
            if (request != null)
            {
                request.TechnicianId = newTechnicianId;
                _context.SaveChanges();
            }
            return RedirectToAction("Requests");
        }

        public IActionResult Invoices()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var invoices = _context.Set<Invoice>()
                .Include(i => i.WorkshopTicket)
                .ThenInclude(t => t.ServiceRequest)
                .OrderByDescending(i => i.IssuedAt)
                .ToList();
            return View(invoices);
        }

        public IActionResult InvoicePrint(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var invoice = _context.Set<Invoice>()
                .Include(i => i.WorkshopTicket)
                .ThenInclude(t => t.ServiceRequest)
                .FirstOrDefault(i => i.Id == id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        public IActionResult Services()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var services = _context.ServiceCategories.ToList();
            return View(services);
        }

        [HttpGet]
        public IActionResult EditService(int? id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            if (id == null) return View(new ServiceCategory());
            var service = _context.ServiceCategories.Find(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        public IActionResult SaveService(ServiceCategory model)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            if (ModelState.IsValid)
            {
                if (model.Id == 0) _context.ServiceCategories.Add(model);
                else _context.ServiceCategories.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Services");
            }
            return View("EditService", model);
        }

        [HttpPost]
        public IActionResult DeleteService(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var service = _context.ServiceCategories.Find(id);
            if (service != null) { _context.ServiceCategories.Remove(service); _context.SaveChanges(); }
            return RedirectToAction("Services");
        }

        // --- التعديل هنا: جلب أسماء الخدمات لصفحة الفنيين ---
        public IActionResult Technicians()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var techs = _context.Technicians.ToList();

            // إرسال قاموس الخدمات لترجمة التخصصات
            ViewBag.ServiceNames = _context.ServiceCategories.ToDictionary(s => s.Code, s => s.ArabicName);

            return View(techs);
        }

        public IActionResult TechnicianDetails(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var technician = _context.Technicians.Find(id);
            if (technician == null) return NotFound();

            var tasks = _context.ServiceRequests
                .Where(r => r.TechnicianId == id)
                .OrderByDescending(r => r.AppointmentDate)
                .ToList();

            ViewBag.Tasks = tasks;
            ViewBag.ServiceNames = _context.ServiceCategories.ToDictionary(s => s.Code, s => s.ArabicName);

            return View(technician);
        }

        [HttpGet]
        public IActionResult EditTechnician(int? id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            ViewBag.Specialties = _context.ServiceCategories.Where(s => s.IsActive).ToList();

            if (id == null) return View(new Technician());
            var tech = _context.Technicians.Find(id);
            if (tech == null) return NotFound();
            return View(tech);
        }

        [HttpPost]
        public IActionResult SaveTechnician(Technician model)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            if (!string.IsNullOrEmpty(model.FullName))
            {
                if (model.Id == 0) _context.Technicians.Add(model);
                else _context.Technicians.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Technicians");
            }

            ViewBag.Specialties = _context.ServiceCategories.Where(s => s.IsActive).ToList();
            return View("EditTechnician", model);
        }

        [HttpPost]
        public IActionResult DeleteTechnician(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var tech = _context.Technicians.Find(id);
            if (tech != null) { _context.Technicians.Remove(tech); _context.SaveChanges(); }
            return RedirectToAction("Technicians");
        }
    }
}