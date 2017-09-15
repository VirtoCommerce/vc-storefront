using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Recommendations;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    //[HandleJsonError]
    public class ApiRecommendationsController : StorefrontControllerBase
    {
        private readonly IRecommendationsService[] _services;
        public ApiRecommendationsController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder,
           IRecommendationsService[] services) : base(workContextAccessor, urlBuilder)
        {
            _services = services;
        }

        [HttpPost]
        public async Task<ActionResult> GetRecommendations(RecommendationEvalContext evalContext)
        {
            var recommendationService = _services.FirstOrDefault(x => x.ProviderName.EqualsInvariant(evalContext.Provider));
            if (recommendationService == null)
            {
                throw new NotSupportedException(evalContext.Provider);
            }
            evalContext.StoreId = WorkContext.CurrentStore.Id;
            evalContext.UserId = WorkContext.CurrentCustomer.Id;
            var result = await recommendationService.GetRecommendationsAsync(evalContext);

            return Json(result);
        }
    }
}