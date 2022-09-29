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
    }
}
