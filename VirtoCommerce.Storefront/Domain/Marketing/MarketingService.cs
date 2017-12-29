using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class MarketingService : IMarketingService
    {
        private readonly IMarketingModuleDynamicContent _dynamicContentApi;
        private readonly IMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public MarketingService(IMarketingModuleDynamicContent dynamicContentApi, IMemoryCache memoryCache, IApiChangesWatcher changesWatcher)
        {
            _dynamicContentApi = dynamicContentApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = changesWatcher;
        }

        public virtual async Task<string> GetDynamicContentHtmlAsync(string storeId, string placeholderName)
        {
            string htmlContent = null;

            //TODO: make full context
            var evaluationContext = new DynamicContentEvaluationContext
            {
                StoreId = storeId,
                PlaceName = placeholderName
            };

            var cacheKey = CacheKey.With(GetType(), "GetDynamicContentHtmlAsync", storeId, placeholderName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(MarketingCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                var dynamicContentItems = (await _dynamicContentApi.EvaluateDynamicContentAsync(evaluationContext)).Select(x => x.ToDynamicContentItem());

                if (dynamicContentItems != null)
                {
                    var htmlContentSpec = new HtmlDynamicContentSpecification();
                    var htmlDynamicContent = dynamicContentItems.FirstOrDefault(htmlContentSpec.IsSatisfiedBy);
                    if (htmlDynamicContent != null)
                    {
                        var dynamicProperty = htmlDynamicContent.DynamicProperties.FirstOrDefault(htmlContentSpec.IsSatisfiedBy);
                        if (dynamicProperty != null && dynamicProperty.Values.Any(v => v.Value != null))
                        {
                            htmlContent = dynamicProperty.Values.First().Value.ToString();
                        }
                    }
                }
                return htmlContent;
            });
        }
    }
}
