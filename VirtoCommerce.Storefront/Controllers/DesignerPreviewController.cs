using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;

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
    }
}
