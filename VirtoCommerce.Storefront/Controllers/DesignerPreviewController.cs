using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class DesignerPreviewController : StorefrontControllerBase
    {
        public DesignerPreviewController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder)
            : base(workContextAccessor, urlBuilder)
        {
        }

        [HttpGet("designer-preview")]
        public IActionResult Index()
        {
            WorkContext.Layout = Request.Query["layout"].ToString();
            return View("json-preview", WorkContext);
        }

        [HttpPost("designer-preview/block")]
        [ValidateAntiForgeryToken]
        public IActionResult Block([FromBody]dynamic data)
        {
            var page = new ContentPage
            {
                Content = $"[{data}]"
            };

            WorkContext.CurrentPage = page;
            var viewName = "json-blocks";

            return PartialView(viewName, WorkContext);
        }
    }
}
