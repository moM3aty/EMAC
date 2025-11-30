using Microsoft.AspNetCore.Mvc;

namespace EMAC.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}