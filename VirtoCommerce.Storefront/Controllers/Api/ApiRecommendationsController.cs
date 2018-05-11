using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Binders;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Recommendations;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiRecommendationsController : StorefrontControllerBase
    {
        private readonly IRecommendationProviderFactory _providerFactory;
        public ApiRecommendationsController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder,
           IRecommendationProviderFactory providerFactory) : base(workContextAccessor, urlBuilder)
        {
            _providerFactory = providerFactory;
        }

        [HttpPost]
        public async Task<ActionResult> GetRecommendations([FromBody] RecommendationEvalContext evalContext)
        {
            var recommendationService = _providerFactory.GetProvider(evalContext.Provider);
            if (recommendationService == null)
            {
                throw new NotSupportedException(evalContext.Provider);
            }
            evalContext.StoreId = WorkContext.CurrentStore.Id;
            evalContext.UserId = WorkContext.CurrentUser.Id;
            var result = await recommendationService.GetRecommendationsAsync(evalContext);

            return Json(result);
        }
    }
}