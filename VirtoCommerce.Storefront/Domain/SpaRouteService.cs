using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class SpaRouteService : ISpaRouteService
    {
        private readonly IContentBlobProvider _contentBlobProvider;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IWorkContextAccessor _workContextAccessor;

        public SpaRouteService(
            IContentBlobProvider contentBlobProvider,
            IStorefrontMemoryCache memoryCache,
            IWorkContextAccessor workContextAccessor)
        {
            _contentBlobProvider = contentBlobProvider;
            _memoryCache = memoryCache;
            _workContextAccessor = workContextAccessor;
        }

        public virtual async Task<bool> IsSpaRoute(string route)
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), nameof(IsSpaRoute), workContext.CurrentStore.Id, route);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (_) =>
            {
                var routes = await GetSpaRoutes();

                var isSpaRoute = routes.Any(jsPattern =>
                {
                    // Input sample: jsPattern = "/^\\/account\\/profile\\/?$/i"
                    // Only the char "i" can be an ending. The others chars are not used
                    // when generating RegExp patterns in the `routes.json` file.
                    var options = jsPattern.EndsWith("i") ? RegexOptions.IgnoreCase : RegexOptions.None;
                    var pattern = Regex.Replace(jsPattern, @"^\/|\/i?$", string.Empty);

                    return Regex.IsMatch(route, pattern, options);
                });

                return isSpaRoute;
            });

            return result;
        }

        protected virtual async Task<List<string>> GetSpaRoutes()
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), nameof(GetSpaRoutes), workContext.CurrentStore.Id);
            var routes = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (_) =>
            {
                var result = new List<string>();
                var currentThemeName = !string.IsNullOrEmpty(workContext.CurrentStore.ThemeName) ? workContext.CurrentStore.ThemeName : "default";
                var currentThemePath = Path.Combine("Themes", workContext.CurrentStore.Id, currentThemeName);
                var currentThemeSettingPath = Path.Combine(currentThemePath, "config", "routes.json");

                if (_contentBlobProvider.PathExists(currentThemeSettingPath))
                {
                    await using var stream = _contentBlobProvider.OpenRead(currentThemeSettingPath);
                    result = JsonConvert.DeserializeObject<List<string>>(await stream.ReadToStringAsync());
                }

                return result;
            });

            return routes;
        }
    }
}
