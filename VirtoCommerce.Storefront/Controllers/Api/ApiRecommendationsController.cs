using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Recommendations;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("recommendations")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiRecommendationsController : StorefrontControllerBase
    {
        private readonly IRecommendationProviderFactory _providerFactory;
        public ApiRecommendationsController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder,
           IRecommendationProviderFactory providerFactory) : base(workContextAccessor, urlBuilder)
        {
            _providerFactory = providerFactory;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Product[]>> GetRecommendations([FromBody] RecommendationEvalContext evalContext)
        {
            var recommendationService = _providerFactory.GetProvider(evalContext.Provider);
            if (recommendationService == null)
            {
                throw new NotSupportedException(evalContext.Provider);
            }
            evalContext.StoreId = WorkContext.CurrentStore.Id;
            evalContext.UserId = WorkContext.CurrentUser.Id;
            var result = await recommendationService.GetRecommendationsAsync(evalContext);

            return result;
        }
    }
}
