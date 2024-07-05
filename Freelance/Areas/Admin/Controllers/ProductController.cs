using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhinaMart.Areas.Admin.SubModel;
using PhinaMart.Models;
using PhinaMart.Repository;

namespace PhinaMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Product")]
    public class ProductController : Controller
    {
        private readonly PhinaMartContext _PhinaContext;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public ProductController(PhinaMartContext context, IWebHostEnvironment webHostEnvironment)
        {
            _PhinaContext = context;
            _webHostEnviroment = webHostEnvironment;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _PhinaContext.Products.OrderByDescending(p => p.Id).Include(c => c.Category).Include(b => b.Brand).ToListAsync());
        }

        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_PhinaContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_PhinaContext.Brands, "Id", "CompanyName");
            return View();
        }

        [Route("UpdateQuantity/{id}")]
        public IActionResult UpdateQuantity(int id)
        {
            var product = _PhinaContext.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [Route("UpdateFunctionQuantity/{id}")]
        public async Task<IActionResult> UpdateFunctionQuantity(int id, [FromForm] UpdateQuantity updateQuantity)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _PhinaContext.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("UpdateQuantity", new { id = id });
                }

                existingProduct.Quantity = updateQuantity.Quantity;
                _PhinaContext.Update(existingProduct);
                await _PhinaContext.SaveChangesAsync();
                /*   TempData["Success"] = "Product quantity updated successfully.";*/
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to update product quantity.";
            return View("UpdateQuantity" ,new { id = id });  // Trả về view UpdateQuantity nếu có lỗi
        }


        [Route("Edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            Product product = await _PhinaContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_PhinaContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_PhinaContext.Brands, "Id", "CompanyName", product.BrandId);
            return View(product);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _PhinaContext.Update(product);
                await _PhinaContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            Product pr = await _PhinaContext.Products.FindAsync(Id);
            if (!string.Equals(pr.Image, "noname.jpg"))
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                string oldfilePath = Path.Combine(uploadsDir, pr.Image);
                try
                {
                    if (System.IO.File.Exists(oldfilePath))
                    {
                        System.IO.File.Delete(oldfilePath);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while deleting the product image.");
                }
            }
            _PhinaContext.Products.Remove(pr);
            await _PhinaContext.SaveChangesAsync();
            TempData["success"] = "Sản phẩm đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
