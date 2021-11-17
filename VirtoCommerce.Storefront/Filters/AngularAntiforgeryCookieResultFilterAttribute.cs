using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics;
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
    public class AngularAntiforgeryCookieResultFilterAttribute : ResultFilterAttribute
    {
        private readonly IAntiforgery antiforgery;
        public AngularAntiforgeryCookieResultFilterAttribute(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {

            var statusCodeReExecuteFeature = context.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            //Reissue antiforgery cookies for follow cases:
            //- when is requested only an ASP.NET View. Since set cookies for each requests such Assets or file resources can leads to stop working the output cache middleware for that controllers
            //- when there are no errors or inner status codes ReExecute occurs (404, 500 etc) (IStatusCodeReExecuteFeature is presents). Since this can leads for reissue an antiforgery cookies .AspNetCore.Antiforgery without AspNetCore.Identity.Application
            //that can leads for 400 errors for antiforgery validation due to different User in the  antiforgery tokens are received as  cookies and request headers
            if (context.Result is ViewResult viewResult && statusCodeReExecuteFeature == null)
            {
                var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
                //We need to set this Cookies XSRF-TOKEN which can be used on the client's side. The most web frameworks such angular can automatically read this cookie add X-XSRF-TOKEN header to each request.
                context.HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = false, IsEssential = true, SameSite = SameSiteMode.Lax });
            }
        }


    }
}
