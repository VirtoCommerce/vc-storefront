using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Marketing.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class DynamicContentEvaluator : IDynamicContentEvaluator
    {
        private readonly IMarketingModuleDynamicContent _dynamicContentApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        private readonly IWorkContextAccessor _workContextAccessor;

        public DynamicContentEvaluator(IMarketingModuleDynamicContent dynamicContentApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher changesWatcher, IWorkContextAccessor workContextAccessor)
        {
            _dynamicContentApi = dynamicContentApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = changesWatcher;
            _workContextAccessor = workContextAccessor;
        }

        public virtual async Task<IEnumerable<DynamicContentItem>> EvaluateDynamicContentItemsAsync(DynamicContentEvaluationContext evalContext)
        {
            var cacheKey = CacheKey.With(GetType(), "EvaluateDynamicContentItemsAsync", evalContext.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(MarketingCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                var evalContextDto = evalContext.ToDynamicContentEvaluationContextDto();
                var dynamicContentItems = (await _dynamicContentApi.EvaluateDynamicContentAsync(evalContextDto)).Select(x => x.ToDynamicContentItem());

                return dynamicContentItems;
            });
        }
    }
}
