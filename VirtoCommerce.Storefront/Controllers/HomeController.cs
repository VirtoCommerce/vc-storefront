using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class HomeController : StorefrontControllerBase
    {
        public HomeController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder) : base(workContextAccessor, urlBuilder)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            var contentPage = WorkContext.FindContentPageByName("index");

            if (contentPage != null)
            {
                WorkContext.SetCurrentPage(contentPage);
                return View(contentPage.Template, WorkContext);
            }

            return View("index");
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

    }
}
