using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhinaMart.Models;
using PhinaMart.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhinaMart.Controllers
{
    [Authorize]
    public class WishListController : Controller
    {
        private readonly PhinaMartContext _context;

        public WishListController(PhinaMartContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User is not logged in.");
            }

            var userId = int.Parse(userIdClaim);
            var wishListItems = await _context.WishLists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .Select(w => new WishListVm
                {
                    ProductId = w.ProductId.Value,
                    ProductName = w.Product.Name,
                    ProductImage = w.Product.Image,
                    ProductPrice = w.Product.Price ,
                    InStock = w.Product.StockQuantity > 0
                })
                .ToListAsync();

            // Add logging here
            Console.WriteLine($"Found {wishListItems.Count} wishlist items for user {userId}");

            return View(wishListItems);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddToWishList(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "You need to log in to add products to your wishlist." });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User is not logged in." });
            }

            var userId = int.Parse(userIdClaim);
            Console.WriteLine($"User ID: {userId}, Product ID: {productId}");

            if (!_context.WishLists.Any(w => w.UserId == userId && w.ProductId == productId))
            {
                var wishList = new WishList
                {
                    UserId = userId,
                    ProductId = productId,
                    SelectDate = DateTime.Now
                };
                _context.WishLists.Add(wishList);
                await _context.SaveChangesAsync();
                Console.WriteLine("Wishlist item added to database.");
            }
            else
            {
                Console.WriteLine("Wishlist item already exists.");
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishList(int productId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User is not logged in.");
            }

            var userId = int.Parse(userIdClaim);
            var wishListItem = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishListItem != null)
            {
                _context.WishLists.Remove(wishListItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
