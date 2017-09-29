using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Errors(string errCode)
        {
            var view = errCode;
            var exceptionFeature =  base.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if(exceptionFeature != null)
            {
                var storefrontException = exceptionFeature.Error as StorefrontException;
                if(storefrontException != null)
                {
                    view = storefrontException.View ?? view;
                }
            }
            return View(view);
        }

        public IActionResult AccessDenied()
        {          
            return View("AccessDenied");
        }
    }
}
