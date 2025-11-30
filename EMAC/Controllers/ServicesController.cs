using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMAC.Data;
using EMAC.ViewModels;
using System.Linq;
using System;

namespace EMAC.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new ServicePageViewModel();

            // 1. جلب الخدمات من قاعدة البيانات
            viewModel.ServicesList = _context.ServiceCategories
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.Code,
                    Text = s.ArabicName
                }).ToList();

            // 2. جلب المناطق ومعالجتها في الذاكرة
            var rawRegions = _context.Technicians
                .Select(t => t.CoveredRegions)
                .ToList();

            var distinctRegions = rawRegions
                .Where(r => !string.IsNullOrEmpty(r))
                .SelectMany(r => r.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(r => r.Trim())
                .Distinct()
                .ToList();

            viewModel.LocationsList = distinctRegions.Select(r => new SelectListItem
            {
                Value = r,
                Text = GetArabicLocationName(r)
            }).ToList();

            return View(viewModel);
        }

        private string GetArabicLocationName(string code)
        {
            return code.ToLower() switch
            {
                "hassa" => "الأحساء",
                "dammam" => "الدمام / الخبر",
                "riyadh" => "الرياض",
                "jeddah" => "جدة",
                "makkah" => "مكة المكرمة",
                "all" => "جميع المناطق", 
                "other" => "مدن أخرى",  
                _ => code 
            };
        }
    }
}