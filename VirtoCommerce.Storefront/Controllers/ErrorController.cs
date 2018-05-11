using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error(int? errCode)
        {
            var exceptionFeature = base.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null && exceptionFeature.Error is StorefrontException storefrontException && storefrontException.View != null)
            {
                return View(storefrontException.View);
            }
            if (errCode == 404 || errCode == 500)
            {
                return View(errCode.ToString());
            }
            return View();
        }

        public IActionResult AccessDenied()
        {          
            return View("AccessDenied");
        }
    }
}
