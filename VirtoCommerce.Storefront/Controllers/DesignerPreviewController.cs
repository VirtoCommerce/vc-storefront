using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using VirtoCommerce.LiquidThemeEngine;
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
