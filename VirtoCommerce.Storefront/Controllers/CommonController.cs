using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Models;

namespace VirtoCommerce.Storefront.Controllers
{
    public class CommonController : Controller
    {

        // GET: common/maintenance
        [HttpGet]
        public ActionResult Maintenance()
        {
            return View("Maintenance");
        }

        // GET: common/notheme
        [HttpGet]
        public ActionResult NoTheme()
        {
            object viewModel;
            if(!HttpContext.Items.TryGetValue(NoLiquidThemeMiddleware.NoThemeModelKey, out viewModel))
            {
                viewModel = new NoThemeViewModel();
            }
            return View("NoTheme", viewModel);
        }

    }
}
