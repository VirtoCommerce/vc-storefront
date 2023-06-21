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
    public class SpaRoutesService : ISpaRoutesService
    {
        private readonly IContentBlobProvider _contentBlobProvider;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly WorkContext _workContext;

        public SpaRoutesService(IContentBlobProvider contentBlobProvider,
            IStorefrontMemoryCache memoryCache,
            IWorkContextAccessor workContextAccessor)
        {
            _contentBlobProvider = contentBlobProvider;
            _memoryCache = memoryCache;
            _workContext = workContextAccessor.WorkContext;
        }

        public virtual async Task<bool> IsSpaRoute(string route)
        {
            var cacheKey = CacheKey.With(GetType(), "IsSpaRoute", _workContext.CurrentStore.Id, route);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (_) =>
            {
                var routes = await GetSpaRoutes();

                return routes.Any(x => Regex.IsMatch(route, x));
            });

            return result;
        }

        protected virtual async Task<List<string>> GetSpaRoutes()
        {
            var cacheKey = CacheKey.With(GetType(), "GetSpaRoutes", _workContext.CurrentStore.Id);
            var routes = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (_) =>
            {
                var result = new List<string>();
                var currentThemeName = !string.IsNullOrEmpty(_workContext.CurrentStore.ThemeName) ? _workContext.CurrentStore.ThemeName : "default";
                var currentThemePath = Path.Combine("Themes", _workContext.CurrentStore.Id, currentThemeName);
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
