using System;
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

            context.RedirectUri = GetStoreAbsoluteUri(context.RedirectUri);

            return base.RedirectToLogin(context);
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            if (context.Request.Path.IsApi())
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }

            context.RedirectUri = GetStoreAbsoluteUri(context.RedirectUri);

            return base.RedirectToAccessDenied(context);
        }


        private string GetStoreAbsoluteUri(string uri)
        {
            //Need to build from an host absolute url a  relative  store-based url
            // http://localhost/Account/Login -> http://localhost/{store}/{lang}/Account/Login
            var redirectUri = new UriBuilder(uri);
            var storeBasedRedirectPath = _storefrontUrlBuilder.ToAppAbsolute(redirectUri.Path);

            // Checks whether path is absolute path (starts with scheme), and extract local path if it is
            if (Uri.TryCreate(storeBasedRedirectPath, UriKind.Absolute, out var absoluteUri))
            {
                storeBasedRedirectPath = absoluteUri.AbsolutePath;
            }

            redirectUri.Path = storeBasedRedirectPath;

            return redirectUri.Uri.ToString();
        }
    }
}
