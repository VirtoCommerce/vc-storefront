using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private static readonly string[] _urlContainingQueryParameters = new string[] { "ReturnUrl", };

        private readonly IStorefrontUrlBuilder _storefrontUrlBuilder;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CustomCookieAuthenticationEvents(IStorefrontUrlBuilder storefrontUrlBuilder, IWorkContextAccessor workContextAccessor)
        {
            _storefrontUrlBuilder = storefrontUrlBuilder;
            _workContextAccessor = workContextAccessor;
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
            // http://localhost/store/Account/Login?ReturnUrl=%2Fstore%2FElectronics%2Fen-US%2Faccount, storeUrl = "http://localhost/store"
            // 1. Should trim store path "/store" from the url path "/store/Account/Login" => path should become "/Account/Login"
            // 2. Check for url params (e.g. ReturnUrl) in query string and trim store url for them too. ReturnUrl=%2Fstore%2FElectronics%2Fen-US%2Faccount => ReturnUrl=%2FElectronics%2Fen-US%2Faccount

            var redirectUri = new UriBuilder(uri);
            var pathWithTrimmedStorePath = new PathString(redirectUri.Path).TrimStorePath(_workContextAccessor?.WorkContext?.CurrentStore);
            var storeBasedRedirectPath = _storefrontUrlBuilder.ToAppAbsolute(pathWithTrimmedStorePath);

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
        /// Trims store path for known url containing params. Encoding/Decoding is handled by HttpUtility.ParseQueryString.
        /// </summary>
        /// <param name="redirectUri">Uri which query params need to be converted.</param>
        private void ConvertParamsUrlsToStoreRelative(UriBuilder redirectUri)
        {
            var queryParams = HttpUtility.ParseQueryString(redirectUri.Query);
            var allParamKeys = queryParams.AllKeys;

            foreach (var paramName in _urlContainingQueryParameters)
            {
                var paramKey = allParamKeys.FirstOrDefault(x => x.EqualsInvariant(paramName));
                var paramValue = paramKey != null ? queryParams[paramKey] : null;

                // Need to check that param value is a valid relative url to avoid exception at PathString creation 
                if (paramKey != null && Uri.TryCreate(paramValue, UriKind.Relative, out var _))
                {
                    queryParams[paramKey] = new PathString(paramValue).TrimStorePath(_workContextAccessor?.WorkContext?.CurrentStore);
                }
            }

            redirectUri.Query = queryParams.ToString();
        }
    }
}
