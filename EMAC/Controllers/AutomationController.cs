using Microsoft.AspNetCore.Mvc;

namespace EMAC.Controllers
{
    public class AutomationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}