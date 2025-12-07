using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using EMAC.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMAC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

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

        public IActionResult Technicians()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var techs = _context.Technicians.ToList();
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


        public IActionResult Categories()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var categories = _context.Categories.Include(c => c.ParentCategory).ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            ViewBag.ParentCategories = _context.Categories.Where(c => c.ParentId == null && c.Id != id).ToList();

            if (id == null) return View(new Category());
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory(Category model, IFormFile imageFile)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categories");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    model.ImageUrl = fileName;
                }

                if (model.Id == 0) _context.Categories.Add(model);
                else _context.Categories.Update(model);

                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }

            ViewBag.ParentCategories = _context.Categories.Where(c => c.ParentId == null && c.Id != model.Id).ToList();
            return View("EditCategory", model);
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var category = _context.Categories.Find(id);
            if (category != null) { _context.Categories.Remove(category); _context.SaveChanges(); }
            return RedirectToAction("Categories");
        }


        public IActionResult Products()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var products = _context.Products.Include(p => p.Category).OrderByDescending(p => p.CreatedAt).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult EditProduct(int? id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ParentId != null ? $"{c.ParentCategory.Name} > {c.Name}" : c.Name
                })
                .ToList();
            ViewBag.Categories = categories;

            if (id == null) return View(new Product());
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(Product model, IFormFile imageFile)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            if (model.Id > 0) ModelState.Remove("ImageUrl");

            ModelState.Remove("Category");
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    model.ImageUrl = fileName;
                }
                else if (model.Id > 0)
                {
                    var existingProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == model.Id);
                    if (existingProduct != null) model.ImageUrl = existingProduct.ImageUrl;
                }

                if (string.IsNullOrEmpty(model.ImageUrl)) model.ImageUrl = "default.png";

                if (model.Id == 0) _context.Products.Add(model);
                else _context.Products.Update(model);

                await _context.SaveChangesAsync();
                return RedirectToAction("Products");
            }

            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ParentId != null ? $"{c.ParentCategory.Name} > {c.Name}" : c.Name
                })
                .ToList();
            ViewBag.Categories = categories;

            return View("EditProduct", model);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            var product = _context.Products.Find(id);
            if (product != null) { _context.Products.Remove(product); _context.SaveChanges(); }
            return RedirectToAction("Products");
        }

        [HttpGet]
        public IActionResult AdGenerator()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");
            ViewBag.Products = _context.Products.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult GenerateAdContent(int productId, string platform)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound();

            string adText = "";
            string phone = "+966564133825";

            if (platform == "whatsapp")
            {
                adText = $"*عرض خاص من مؤسسة الإخلاص الحديثة (EMAC)* 🔥\n\n" +
                         $"*{product.Name}*\n" +
                         $"✅ الماركة: {product.Brand}\n" +
                         $"✅ السعة: {product.SizeOrCapacity}\n" +
                         $"----------------------------\n" +
                         $"💰 *السعر: {product.Price} ريال فقط* (شامل الضريبة)\n" +
                         (product.OldPrice.HasValue ? $"❌ ~كان {product.OldPrice}~ \n" : "") +
                         $"----------------------------\n" +
                         $"{product.Description}\n\n" +
                         $"🚚 التوصيل والتركيب متاح!\n" +
                         $"🛒 للطلب تواصل معنا مباشرة: https://wa.me/{phone.Replace("+", "")}";
            }
            else
            {
                adText = $"تحديثات عروض الصيف من EMAC! ❄️☀️\n\n" +
                         $"استمتع بأفضل تبريد وأداء مع {product.Name} \n\n" +
                         $"🔹 الموديل: {product.Brand} - {product.SizeOrCapacity}\n" +
                         $"🔹 السعر: {product.Price} ريال 💵\n" +
                         $"🔹 ضمان الوكيل + تركيب احترافي\n\n" +
                         $"📍 موقعنا: الأحساء\n" +
                         $"📞 للحجز والطلب: {phone}\n\n" +
                         $"#تكييف #تبريد #EMAC #عروض #{product.Brand.Replace(" ", "_")} #السعودية";
            }

            return Json(new { success = true, text = adText, image = product.ImageUrl });
        }
    }
}