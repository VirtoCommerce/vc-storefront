using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Controllers
{
    [Route("error")]
    [StorefrontRoute("error")]
    public class ErrorController : Controller
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        public ErrorController(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        [Route("{errCode}")]
        public IActionResult Error(int? errCode)
        {
            //Returns index page on 404 error when the store.IsSpa flag is activated 
            if (errCode == StatusCodes.Status404NotFound && _workContextAccessor.WorkContext.CurrentStore.IsSpa)
            {
                Response.StatusCode = StatusCodes.Status200OK;
                return View("index");
            }
            var exceptionFeature = base.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null && exceptionFeature.Error is StorefrontException storefrontException && storefrontException.View != null)
            {
                return View(storefrontException.View);
            }
            if (errCode == StatusCodes.Status404NotFound || errCode == StatusCodes.Status500InternalServerError)
            {
                return View(errCode.ToString());
            }
            return View();
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
