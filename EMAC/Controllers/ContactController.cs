using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using System.Threading.Tasks;

namespace EMAC.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
               
                TempData["SuccessMessage"] = "تم استلام طلبك بنجاح، سنتواصل معك قريباً.";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "حدث خطأ في البيانات المدخلة.";
            return RedirectToAction("Index", "Home");
        }
    }
}