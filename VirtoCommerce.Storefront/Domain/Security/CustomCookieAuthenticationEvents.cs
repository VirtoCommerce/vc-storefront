using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IStorefrontUrlBuilder _storefrontUrlBuilder;

        public CustomCookieAuthenticationEvents(IStorefrontUrlBuilder storefrontUrlBuilder)
        {
            _storefrontUrlBuilder = storefrontUrlBuilder;
        }

        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            if (context.Request.Path.IsApi())
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            }

            var absolutePath = _storefrontUrlBuilder.ToStoreAbsolute(context.RedirectUri).ToAbsolutePath();
            context.RedirectUri = WebUtility.UrlDecode(absolutePath);

            return base.RedirectToLogin(context);
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            if (context.Request.Path.IsApi())
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }

            context.RedirectUri = _storefrontUrlBuilder.ToStoreAbsolute(context.RedirectUri).ToAbsolutePath();

            return base.RedirectToAccessDenied(context);
        }
    }
}
