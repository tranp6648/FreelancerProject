using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhinaMart.Models;
using PhinaMart.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using PhinaMart.Services;
using PhinaMart.Helpers;
using System.Xml.Linq;

namespace PhinaMart.Controllers
{
    public class ProductController : Controller
    {
        private readonly PhinaMartContext db;
        private readonly CommentService commentService;
        public ProductController(PhinaMartContext context,CommentService commentService)
        {
            db = context;
            this.commentService = commentService;
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
                Price = p.Price,
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
                Price = p.Price,
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
            var Comment = db.Comments.Where(d=>d.ProductId==id).Select(d => new
            {
                Id=d.Id,
                User=d.User.Username,
                CreateDate=d.CreatedAt,
                ContenText=d.CommentText
            }).OrderByDescending(d=>d.Id).ToList();
            ViewBag.Comments =Comment;
            var viewModel = new ProductDetailAndCommentViewModel
            {
                ProductDetail = new DetailProductVm
                {
                    Id = data.Id,
                    Name = data.Name,
                    Price = data.Price,
                    Description = data.Description ?? string.Empty,
                    Image = data.Image ?? string.Empty,
                    DescriptionShort = data.DescriptionUnit ?? string.Empty,
                    NameCategory = data.Category.Name,
                    StockQuantity = 10, // Placeholder
                    StarRating = 5, // Placeholder
                }
            }; 
            var email=User.FindFirst(ClaimTypes.Email)?.Value;
            ViewBag.Email = email;  

            return View(viewModel);
        }
        [HttpPost]
        [Route("CreateCompare/{id}")]
          [ValidateAntiForgeryToken]
        public IActionResult CreateCompare(int id)
        {
            var idUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (db.Compares.Any(d => d.IdUser == idUser && d.IdProduct == id))
            {
                TempData["Exist"] = "this Product is already exists in Compare";
                return RedirectToAction("Detail", new { id = id });
            }
            var result=commentService.CreateCompare(id);
            if (result)
            {
                TempData["Success"] = "Compare Created Successfully";
            }
            else
            {
                
                
                    TempData["Error"] = "Failed to create comment";
                
              
            }
            return RedirectToAction("Detail", new { id = id });
        }
        [HttpPost]
        [Route("CreateComment/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateComment(int id,CreateComment createComment)
        {
            var result = commentService.CreateComment(createComment, id);
            if (result)
            {
                TempData["Message"] = "Comment created successfully";
            }
            else
            {
                TempData["Error"] = "Failed to create comment";
            }
            return RedirectToAction("Detail", new { id = id });
        }

    }
}
