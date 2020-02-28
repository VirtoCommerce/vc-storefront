using System;
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
        private readonly IUrlBuilder _urlBuilder;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IHostingEnvironment _hostEnv;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StorefrontUrlBuilder(IUrlBuilder urlBuilder, IWorkContextAccessor workContextAccessor, IHostingEnvironment hostEnv, IHttpContextAccessor httpContextAccessor)
        {
            _urlBuilder = urlBuilder;
            _workContextAccessor = workContextAccessor;
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

        public string ToStoreRelativeUrl(string virtualPath)
        {
            var workContext = _workContextAccessor.WorkContext;
            return ToStoreRelativeUrl(virtualPath, workContext.CurrentStore);
        }

        public string ToStoreRelativeUrl(string virtualPath, Store store)
        {
            var result = virtualPath;
            var storeUrl = !string.IsNullOrWhiteSpace(store.Url) ? store.Url : store.SecureUrl;

            if (!string.IsNullOrWhiteSpace(storeUrl) && !Uri.TryCreate(virtualPath, UriKind.Absolute, out var _))
            {
                var storeUri = new UriBuilder(storeUrl);
                var storeUriPath = storeUri.Path;

                // Uri.Path by default is "/" - no need to remove starting "/"
                if (!string.IsNullOrWhiteSpace(storeUriPath) && !storeUriPath.EqualsInvariant("/"))
                {
                    result = RemoveUrlPartFromBeginning(result, storeUri.Path);
                }
            }

            return result;
        }

        private static string RemoveUrlPartFromBeginning(string url, string urlPartToRemove)
        {
            var result = url;

            if (!string.IsNullOrWhiteSpace(urlPartToRemove))
            {
                result = url.IndexOf(urlPartToRemove, StringComparison.OrdinalIgnoreCase) == 0 ? url.Remove(0, urlPartToRemove.Length) : url;
            }

            return result;
        }
    }
}
