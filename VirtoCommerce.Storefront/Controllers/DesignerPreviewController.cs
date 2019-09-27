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
        readonly IStaticContentService contentService;

        public DesignerPreviewController(IWorkContextAccessor workContextAccessor,
            IStorefrontUrlBuilder urlBuilder,
            IStaticContentService contentService) :
            base(workContextAccessor, urlBuilder)
        {
            this.contentService = contentService;
        }

        [HttpGet("designer-preview")]
        public IActionResult Index()
        {
            //Response.Headers.Add("Content-Security-Policy", "frame-src http://localhost:4200/ http://dev-cms-vm.westeurope.cloudapp.azure.com;");
            //Response.Headers.Add("Content-Security-Policy", "frame-src https://vc-com-new-initial-platform.azurewebsites.net;");
            WorkContext.Layout = Request.Query["layout"].ToString();
            return View("json-preview", WorkContext);
        }

        [HttpPost("designer-preview/reset-cache")]
        public IActionResult ResetCache()
        {
            contentService.ResetCache(WorkContext.CurrentStore);
            return Ok();
        }

        [HttpPost("designer-preview/block")]
        public IActionResult Block([FromBody]JObject block)
        {
            var content = new JsonPage
            {
                Blocks = new List<JObject> { block }
            };
            WorkContext.CurrentJsonPage = content;
            var viewName = "json-blocks";

            return PartialView(viewName, WorkContext);
        }

        //[HttpPost("designer-preview/blocks")]
        //public IActionResult Blocks([FromBody]JsonPage content)
        //{
        //    WorkContext.CurrentJsonPage = content;
        //    return PartialView("json-page", WorkContext);
        //}
    }
}
