using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("index");
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return NotFound();
        }
    }
}
