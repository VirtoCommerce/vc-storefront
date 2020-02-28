using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private static readonly string[] _urlContainingQueryParameters = new string[] { "ReturnUrl " };

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

            // Need to be able to properly handle the following case:
            // http://localhost/store/Account/Login?ReturnUrl=%2Fstore%2FElectronics%2Fen-US%2Faccount
            // 1. Should properly handle store paht in its url ("/store" for the "http://localhost/store")
            // 2. Check for url params (e.g. ReturnUrl) in query string and make them store relative too

            var redirectUri = new UriBuilder(uri);
            var storeRelativeUrl = _storefrontUrlBuilder.ToStoreRelativeUrl(redirectUri.Path);
            var storeBasedRedirectPath = _storefrontUrlBuilder.ToAppAbsolute(storeRelativeUrl);

            ConvertParamsUrlsToStoreRelative(redirectUri);

            // Checks whether path is absolute path (starts with scheme), and extract local path if it is
            if (Uri.TryCreate(storeBasedRedirectPath, UriKind.Absolute, out var absoluteUri))
            {
                storeBasedRedirectPath = absoluteUri.AbsolutePath;
            }

            redirectUri.Path = storeBasedRedirectPath;

            return redirectUri.Uri.ToString();
        }

        /// <summary>
        /// Set store relative urls to known url containg params values. Encoding/Decoding is handled by HttpUtility.ParseQueryString.
        /// </summary>
        /// <param name="redirectUri">Uri which query params need to be converted.</param>
        private void ConvertParamsUrlsToStoreRelative(UriBuilder redirectUri)
        {
            var queryParams = HttpUtility.ParseQueryString(redirectUri.Query);
            var allParamKeys = queryParams.AllKeys;

            foreach (var paramName in _urlContainingQueryParameters)
            {
                var paramKey = allParamKeys.FirstOrDefault(x => x.EqualsInvariant(paramName));

                if (paramKey != null)
                {
                    var paramValue = queryParams[paramKey];
                    var storeRelativeUrl = _storefrontUrlBuilder.ToStoreRelativeUrl(paramValue);

                    queryParams[paramKey] = storeRelativeUrl;
                }
            }

            redirectUri.Query = queryParams.ToString();
        }
    }
}
