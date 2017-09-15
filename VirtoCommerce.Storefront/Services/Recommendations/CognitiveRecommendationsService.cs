using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi.Models;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Services.Recommendations
{
    //public class CognitiveRecommendationsService : IRecommendationsService
    //{
    //    private readonly Func<WorkContext> _workContextFactory;
    //    private readonly ICatalogSearchService _catalogSearchService;
    //    private readonly IProductRecommendationsModuleApiClient _productRecommendationsApi;
    //    private readonly ILocalCacheManager _cacheManager;

    //    public CognitiveRecommendationsService(
    //        Func<WorkContext> workContextFactory,
    //        ICatalogSearchService catalogSearchService,
    //        IProductRecommendationsModuleApiClient productRecommendationsApi,
    //        ILocalCacheManager cacheManager)
    //    {
    //        _workContextFactory = workContextFactory;
    //        _catalogSearchService = catalogSearchService;
    //        _productRecommendationsApi = productRecommendationsApi;
    //        _cacheManager = cacheManager;
    //    }

    //    #region IRecommendationsService members
    //    public string ProviderName
    //    {
    //        get
    //        {
    //            return "Cognitive";
    //        }
    //    }

    //    public async Task<Product[]> GetRecommendationsAsync(Model.Recommendations.RecommendationEvalContext context)
    //    {
    //        var cognitiveContext = context as CognitiveRecommendationEvalContext;
    //        if(cognitiveContext == null)
    //        {
    //            throw new InvalidCastException(nameof(context));
    //        }
                
    //        var result = new List<Product>();

    //        var recommendedProductIds = await _productRecommendationsApi.Recommendations.GetRecommendationsAsync(cognitiveContext.ToContextDto());
    //        if (recommendedProductIds != null)
    //        {
    //            result.AddRange(await _catalogSearchService.GetProductsAsync(recommendedProductIds.ToArray(), ItemResponseGroup.Seo | ItemResponseGroup.Outlines | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.Inventory));
    //        }

    //        return result.ToArray();
    //    } 
    //    #endregion
    //}
}