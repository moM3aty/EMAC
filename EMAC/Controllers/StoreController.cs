using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;

namespace EMAC.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Where(c => c.ParentId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Products(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null) return NotFound();

            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId || p.Category.ParentId == categoryId)
                .OrderByDescending(p => p.IsFeatured)
                .ToListAsync();

            ViewBag.CategoryName = category.Name;
            ViewBag.SubCategories = category.SubCategories;

            ViewBag.FavIds = GetFavoriteIds();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewBag.RelatedProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(4)
                .ToListAsync();

            return View(product);
        }

        public async Task<IActionResult> Favorites()
        {
            var favIds = GetFavoriteIds();
            var products = new List<Product>();

            if (favIds.Any())
            {
                products = await _context.Products
                    .Where(p => favIds.Contains(p.Id))
                    .ToListAsync();
            }

            return View(products);
        }

        [HttpPost]
        public IActionResult ToggleFavorite(int id)
        {
            var favIds = GetFavoriteIds();
            bool isAdded = false;

            if (favIds.Contains(id))
            {
                favIds.Remove(id);
                isAdded = false;
            }
            else
            {
                favIds.Add(id);
                isAdded = true;
            }

            var options = new CookieOptions { Expires = DateTime.Now.AddDays(30) };
            Response.Cookies.Append("EmacFavs", string.Join(",", favIds), options);

            return Json(new { success = true, isAdded = isAdded, count = favIds.Count });
        }

        private List<int> GetFavoriteIds()
        {
            var favCookie = Request.Cookies["EmacFavs"];
            if (!string.IsNullOrEmpty(favCookie))
            {
                return favCookie.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse)
                                .ToList();
            }
            return new List<int>();
        }
    }
}