using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product pr)
        {
            ViewBag.Categories = new SelectList(_PhinaContext.Categories, "Id", "Name", pr.CategoryId);
            ViewBag.Brands = new SelectList(_PhinaContext.Brands, "Id", "CompanyName", pr.BrandId);

            if (ModelState.IsValid)
            {
                pr.Slug = pr.Name.Replace(" ", "-");
                var slug = await _PhinaContext.Products.FirstOrDefaultAsync(p => p.Slug == pr.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(pr);
                }

                if (pr.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + pr.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await pr.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    pr.Image = imageName;
                }

                _PhinaContext.Add(pr);
                await _PhinaContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(pr);
        }


        [Route("Edit")]

        public async Task<IActionResult> Edit(int Id)
        {
            Product product = await _PhinaContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_PhinaContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_PhinaContext.Brands, "Id", "CompanyName", product.BrandId);

            return View(product);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product pr)
        {
            ViewBag.Categories = new SelectList(_PhinaContext.Categories, "Id", "Name", pr.CategoryId);
            ViewBag.Brands = new SelectList(_PhinaContext.Brands, "Id", "CompanyName", pr.BrandId);
            var existed_product = _PhinaContext.Products.Find(pr.Id);

            if (ModelState.IsValid)
            {
                pr.Slug = pr.Name.Replace(" ", "-");

                if (pr.ImageUpload != null)
                {
                    //upload new file
                    string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + pr.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    //delete file pictureOld
                    string oldfilePath = Path.Combine(uploadsDir, existed_product.Image);
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


                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await pr.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_product.Image = imageName;
                }
                // Update other product properties
                existed_product.Name = pr.Name;
                existed_product.Description = pr.Description;
                existed_product.Price = pr.Price;
                existed_product.CategoryId = pr.CategoryId;
                existed_product.BrandId = pr.BrandId;
                //delete old picture



                _PhinaContext.Update(existed_product); // Update the existing product object


                await _PhinaContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(pr);
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
            TempData["success"] = "sản phẩm đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
