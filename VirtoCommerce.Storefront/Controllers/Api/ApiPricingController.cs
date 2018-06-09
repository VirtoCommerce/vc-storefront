using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [ValidateAntiForgeryToken]
    public class ApiPricingController : StorefrontControllerBase
    {
        private readonly IMarketingService _marketingService;
        private readonly IPricingService _pricingService;
        private readonly IPromotionEvaluator _promotionEvaluator;

        public ApiPricingController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IMarketingService marketingService,
            IPromotionEvaluator promotionEvaluator, IPricingService pricingService)
            : base(workContextAccessor, urlBuilder)
        {
            _marketingService = marketingService;
            _pricingService = pricingService;
            _promotionEvaluator = promotionEvaluator;
        }

        // POST: storefrontapi/pricing/actualprices
        [HttpPost]
        public async Task<ActionResult> GetActualProductPrices([FromBody] Product[] products)
        {
            if (products != null)
            {
                //Evaluate products prices
                await _pricingService.EvaluateProductPricesAsync(products, WorkContext);

                var retVal = products.Select(x => x.Price).ToArray();

                return Json(retVal);
            }
            return Ok();
        }
    }
}
