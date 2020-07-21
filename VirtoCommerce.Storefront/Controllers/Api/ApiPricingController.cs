using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("pricing")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiPricingController : StorefrontControllerBase
    {
        private readonly IPricingService _pricingService;
        private readonly IPromotionEvaluator _promotionEvaluator;

        public ApiPricingController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IPromotionEvaluator promotionEvaluator, IPricingService pricingService)
            : base(workContextAccessor, urlBuilder)
        {
            _pricingService = pricingService;
            _promotionEvaluator = promotionEvaluator;
        }

        // POST: storefrontapi/pricing/actualprices
        [HttpPost("actualprices")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ProductPrice[]>> GetActualProductPrices([FromBody] Product[] products)
        {
            if (products != null && products.Any())
            {
                //Evaluate products prices
                await _pricingService.EvaluateProductPricesAsync(products, WorkContext);

                var retVal = products.Select(x => x.Price).ToArray();

                return retVal;
            }
            return Ok();
        }
    }
}
