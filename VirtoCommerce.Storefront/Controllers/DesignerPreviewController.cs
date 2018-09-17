using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class DesignerPreviewController : Controller
    {
        [HttpGet("designer-preview")]
        public IActionResult Index()
        {
            return View("designer-preview");
        }

        [HttpPost("designer-preview/blocks")]
        public IActionResult Blocks([FromBody]JsonPage content)
        {
            // TODO: data does not bind still
            return Content("success");
        }
    }
}
