using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhinaMart.Models;
using PhinaMart.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace PhinaMart.Controllers
{
    public class ProductController : Controller
    {
        private readonly PhinaMartContext db;

        public ProductController(PhinaMartContext context)
        {
            db = context;
        }
        public IActionResult Index(int? category)
        {
            var products = db.Products.AsQueryable();
            if (category.HasValue)
            {
                products = products.Where(p => p.CategoryId == category.Value);
            }

            var result = products.Select(p => new ProductVm
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price ?? 0,
                Image = p.Image ?? "",
                DescriptionShort = p.DescriptionUnit ?? string.Empty,
                NameCategory = p.Category.Name
            });

            return View(result);
        }

        public IActionResult Search(string query)
        {
            var products = db.Products.AsQueryable();

            if (query != null)
            {
                products = products.Where(p => p.Name.Contains(query));
            }

            var result = products.Select(p => new ProductVm
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price ?? 0,
                Image = p.Image ?? "",
                DescriptionShort = p.DescriptionUnit ?? string.Empty,
                NameCategory = p.Category.Name
            });

            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.Products
                .Include(p => p.Category)
                .SingleOrDefault(p => p.Id == id);
            if (data == null)
            {
                TempData["error"] = $"{id}";
                return Redirect("/404");
            }

            var result = new DetailProductVm
            {
                Id = data.Id,
                Name = data.Name,
                Price = data.Price ?? 0,
                Description = data.Description ?? string.Empty,
                Image = data.Image ?? string.Empty,
                DescriptionShort = data.DescriptionUnit ?? string.Empty,
                NameCategory = data.Category.Name,
                StockQuantity = 10, // Placeholder
                StarRating = 5, // Placeholder

            };
            var email=User.FindFirst(ClaimTypes.Email)?.Value;
            ViewBag.Email = email;  

            return View(result);
        }
       

    }
}
