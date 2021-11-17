using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Routing;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class ThemeViewController : StorefrontControllerBase
    {
        public ThemeViewController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder)
            : base(workContextAccessor, urlBuilder)
        {
        }

        public IActionResult ThemeView(string viewName)
        {
            WorkContext.SlugRoutingData = RouteData.Values.GetValueOrDefault("routing") as SlugRoutingData;
            return View(viewName);
        }

        [HttpGet("checkout")]
        public IActionResult CheckoutView()
        {
            return View("checkout");
        }
    }
}
