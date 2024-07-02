using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhinaMart.Models;
using System.Security.Claims;

namespace PhinaMart.Controllers
{
    public class CompareController : Controller
    {
       private readonly PhinaMartContext context;
        public CompareController(PhinaMartContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            var Id =int.Parse( User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var Compare = context.Compares.Where(d=>d.IdUser==Id).Select(d => new
            {
                Id = d.Id,
                Name=d.IdProductNavigation.Name,
                Category=d.IdProductNavigation.Category.Name,
                Price=d.IdProductNavigation.Price,
                Url=d.IdProductNavigation.Image,
                Brand=d.IdProductNavigation.Brand.CompanyName
            }).ToList();
            ViewBag.Compares=Compare;

            return View();
        }

        
    }
}
