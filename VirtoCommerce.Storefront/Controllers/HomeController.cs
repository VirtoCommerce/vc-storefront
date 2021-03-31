using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

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
            var contentPage = WorkContext.Pages
                .OfType<ContentPage>()
                .Where(x => string.Equals(x.Url, "index", StringComparison.OrdinalIgnoreCase) && Path.GetExtension(x.FileName) == ".page")
                .FindWithLanguage(WorkContext.CurrentLanguage);

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
