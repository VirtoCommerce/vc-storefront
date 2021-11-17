using System;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Tools;

namespace VirtoCommerce.Storefront.Infrastructure
{
    /// <summary>
    /// Create storefront url contains language and store information
    /// </summary>
    public class StorefrontUrlBuilder : IStorefrontUrlBuilder
    {
        const string FILE_SCHEME = "file";

        private readonly IUrlBuilder _urlBuilder;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly string[] UrlContainingQueryParameters = { "ReturnUrl", };

        public StorefrontUrlBuilder(IUrlBuilder urlBuilder, IWorkContextAccessor workContextAccessor, IWebHostEnvironment hostEnv, IHttpContextAccessor httpContextAccessor)
        {
            _urlBuilder = urlBuilder;
            _workContextAccessor = workContextAccessor;
            //_urlBuilderContext = workContext.ToToolsContext();
            _hostEnv = hostEnv;
            _httpContextAccessor = httpContextAccessor;
        }

        #region IStorefrontUrlBuilder members

        public string ToAppAbsolute(string virtualPath)
        {
            var workContext = _workContextAccessor.WorkContext;
            return ToAppAbsolute(virtualPath, workContext.CurrentStore, workContext.CurrentLanguage);
        }

        public string ToAppAbsolute(string virtualPath, Store store, Language language)
        {
            var appRelativePath = ToAppRelative(virtualPath, store, language);
            //TODO:
            var result = appRelativePath != null && appRelativePath.StartsWith("~")
                ? _httpContextAccessor.HttpContext.Request.PathBase + appRelativePath.Replace("~", string.Empty)
                : appRelativePath;
            return result;
        }

        public string ToStoreAbsolute(string virtualPath, Store store = null, Language language = null)
        {
            // Need to build from an host absolute url a  relative  store-based url
            // http://localhost/Account/Login -> http://localhost/{store}/{lang}/Account/Login

            // Need to be able to properly handle the following case:
            // http://localhost/store/Account/Login?ReturnUrl=%2Fstore%2FElectronics%2Fen-US%2Faccount, storeUrl = "http://localhost/store"
            // 1. Should trim store path "/store" from the url path "/store/Account/Login" => path should become "/Account/Login"
            // 2. Check for url params (e.g. ReturnUrl) in query string and trim store url for them too. ReturnUrl=%2Fstore%2FElectronics%2Fen-US%2Faccount => ReturnUrl=%2FElectronics%2Fen-US%2Faccount

            var uri = new Uri(virtualPath, UriKind.RelativeOrAbsolute);
            var absolutePathOrUrl = ConvertPathToStoreAbsolutePathOrUrl(uri.IsAbsoluteUri ? uri.AbsolutePath : virtualPath, store, language);
            var storeRelativeOrAbsoluteUrl = new Uri(absolutePathOrUrl, UriKind.RelativeOrAbsolute);
            var urlBuilder = new UriBuilder(new Uri(uri, storeRelativeOrAbsoluteUrl))
            {
                Query = ConvertQueryUrlsToStoreAbsolutePaths(uri.Query, store, language)
            };
            return urlBuilder.Uri.ToString();
        }

        private string ConvertPathToStoreAbsolutePathOrUrl(PathString path, Store store = null, Language language = null)
        {
            var relativeToAppPath = path.TrimStorePath(_workContextAccessor.WorkContext.CurrentStore);
            var storeUrl = store != null || language != null
                ? ToAppAbsolute(relativeToAppPath.TrimStoreAndLangSegment(_workContextAccessor.WorkContext.CurrentStore, _workContextAccessor.WorkContext.CurrentLanguage),
                    store ?? _workContextAccessor.WorkContext.CurrentStore,
                    language ?? store.DefaultLanguage ?? _workContextAccessor.WorkContext.CurrentLanguage)
                : ToAppAbsolute(relativeToAppPath);

            return storeUrl;
        }

        /// <summary>
        /// Trims store path for known url containing params. Encoding/Decoding is handled by HttpUtility.ParseQueryString.
        /// </summary>
        /// <param name="query">Query params need to be converted.</param>
        private string ConvertQueryUrlsToStoreAbsolutePaths(string query, Store store = null, Language language = null)
        {
            var queryParams = HttpUtility.ParseQueryString(query);
            var allParamKeys = queryParams.AllKeys;

            foreach (var paramName in UrlContainingQueryParameters)
            {
                var paramKey = allParamKeys.FirstOrDefault(x => x.EqualsInvariant(paramName));
                var paramValue = paramKey != null ? queryParams[paramKey] : null;

                // Need to check that param value is a valid relative url to avoid exception at PathString creation 
                if (paramKey != null && Uri.TryCreate(paramValue, UriKind.Relative, out _))
                {
                    queryParams[paramKey] = ConvertPathToStoreAbsolutePathOrUrl(new PathString(paramValue), store, language).ToAbsolutePath();
                }
            }

            return queryParams.ToString();
        }

        public string ToAppRelative(string virtualPath)
        {
            var workContext = _workContextAccessor.WorkContext;
            return ToAppRelative(virtualPath, workContext.CurrentStore, workContext.CurrentLanguage);
        }

        public string ToAppRelative(string virtualPath, Store store, Language language)
        {
            var workContext = _workContextAccessor.WorkContext;
            var urlBuilderContext = workContext.ToToolsContext();
            return _urlBuilder.BuildStoreUrl(urlBuilderContext, virtualPath, store.ToToolsStore(), language?.CultureName);
        }

        public string ToLocalPath(string virtualPath)
        {
            return _hostEnv.MapPath(virtualPath);
        }
        #endregion
    }
}
