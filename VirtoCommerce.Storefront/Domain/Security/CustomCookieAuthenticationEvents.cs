using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

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
            // http://localhost/store/Account/Login?ReturnUrl=%2Fstore%2FElectronics%2Fen-US%2Faccount
            // 1. Should properly handle store path in its url (remove storeUrlPath "/store" for the "http://localhost/store")
            // 2. Check for url params (e.g. ReturnUrl) in query string and trim store url here too

            var redirectUri = new UriBuilder(uri);
            var storeRelativeUrl = TrimStorePathFromPath(redirectUri.Path, _workContextAccessor?.WorkContext?.CurrentStore);
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
        /// Trim store path from known params containng url values. Encoding/Decoding is handled by HttpUtility.ParseQueryString.
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
                    var storeRelativeUrl = TrimStorePathFromPath(paramValue, _workContextAccessor?.WorkContext?.CurrentStore);

                    queryParams[paramKey] = storeRelativeUrl;
                }
            }

            redirectUri.Query = queryParams.ToString();
        }

        /// <summary>
        /// Trims store path ("/store" for "http://localhost/store") from the beginning of the url. Does nothing in case of empty store.Url.
        /// </summary>
        /// <param name="path">Path to trim store path from.</param>
        /// <param name="store">Store which path to trim.</param>
        /// <returns></returns>
        private string TrimStorePathFromPath(string path, Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            // Need to remove store path only if store has url
            var storeUrl = !string.IsNullOrWhiteSpace(store.Url) ? store.Url : store.SecureUrl;

            if (!string.IsNullOrWhiteSpace(storeUrl) && Uri.TryCreate(storeUrl, UriKind.Absolute, out var storeUri))
            {
                var storeUriPath = storeUri.AbsolutePath.Trim('/');

                // Uri.AbsolutePath by default is "/" - no need to trim it
                if (!string.IsNullOrWhiteSpace(storeUriPath) && !storeUriPath.Equals("/"))
                {
                    // Removing store url path from the beginning of path
                    path = Regex.Replace(path, @"^/\b" + storeUriPath + @"\b/", "/", RegexOptions.IgnoreCase);
                }
            }

            return path;
        }
    }
}
