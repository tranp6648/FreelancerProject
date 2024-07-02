using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PhinaMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Home")]

    public class HomeController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
