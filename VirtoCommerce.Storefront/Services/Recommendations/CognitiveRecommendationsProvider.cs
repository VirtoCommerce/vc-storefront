using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Services;
using dto = VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi.Models;

namespace VirtoCommerce.Storefront.Services.Recommendations
{
    public class CognitiveRecommendationsProvider : IRecommendationsProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICatalogService _catalogService;
        private readonly IRecommendations _recommendationsApi;
        private readonly ICacheManager<object> _cacheManager;

        public CognitiveRecommendationsProvider(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, IRecommendations recommendationsApi,
            ICacheManager<object> cacheManager)
        {
            _workContextAccessor = workContextAccessor;
            _catalogService = catalogService;
            _recommendationsApi = recommendationsApi;
            _cacheManager = cacheManager;
        }

        #region IRecommendationsService members
        public string ProviderName
        {
            get
            {
                return "Cognitive";
            }
        }

        public RecommendationEvalContext CreateEvalContext()
        {
            return new CognitiveRecommendationEvalContext();
        }

        public async Task AddEventAsync(IEnumerable<UsageEvent> events)
        {
            var usageEvents = events.Select(i => i.JsonConvert<dto.UsageEvent>());

            await _recommendationsApi.AddEventAsync(usageEvents.ToList());
        }

        public async Task<Product[]> GetRecommendationsAsync(Model.Recommendations.RecommendationEvalContext context)
        {
            var cognitiveContext = context as CognitiveRecommendationEvalContext;
            if (cognitiveContext == null)
            {
                throw new InvalidCastException(nameof(context));
            }

            var result = new List<Product>();

            var recommendedProductIds = await _recommendationsApi.GetRecommendationsAsync(cognitiveContext.ToContextDto());
            if (recommendedProductIds != null)
            {
                result.AddRange(await _catalogService.GetProductsAsync(recommendedProductIds.ToArray(), ItemResponseGroup.Seo | ItemResponseGroup.Outlines | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.Inventory));
            }

            return result.ToArray();
        }
        #endregion
    }
}