using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VirtoCommerce.Storefront.Filters
{
    /// <summary>
    ///  Angular will be using their $http service for sending AJAX requests and will automatically include a header with the name X-XSRF-TOKEN
    ///  if it can find the token value as a cookie with the name XSRF-TOKEN.
    ///  This filter automatically set XSRF-TOKEN cookies for each request routed to the view
    /// </summary>
    public class AngularAntiforgeryCookieResultFilter : ResultFilterAttribute
    {
        private IAntiforgery antiforgery;
        public AngularAntiforgeryCookieResultFilter(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            //Do not include XSRF-TOKEN in each request, since it can leads to stop working the output cache middleware
            if (context.Result is ViewResult)
            {
                var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
                context.HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = false, IsEssential = true });
            }
        }


    }
}
