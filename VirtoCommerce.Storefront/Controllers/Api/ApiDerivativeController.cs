using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Derivatives;
using VirtoCommerce.Storefront.Model.Derivatives.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiDerivativeController : StorefrontControllerBase
    {
        private readonly IDerivativeService _derivativeService;

        public ApiDerivativeController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IDerivativeService derivativeService)
            : base(workContextAccessor, urlBuilder)
        {
            _derivativeService = derivativeService;
        }

        // storefrontapi/derivatives?ids=...
        [HttpGet]
        public async Task<ActionResult> GetDerivativesByIds(string[] ids)
        {
            var retVal = await _derivativeService.GetDerivativesAsync(ids);
            return Json(retVal);
        }

        // storefrontapi/derivatives/search
        [HttpPost]
        public async Task<ActionResult> SearchDerivatives([FromBody] DerivativeSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new DerivativeSearchCriteria();
            }

            var retVal = await _derivativeService.SearchDerivativesAsync(searchCriteria);

            return Json(new
            {
                Results = retVal,
                TotalCount = retVal.TotalItemCount
            });
        }

        // storefrontapi/derivatives/searchitems
        [HttpPost]
        public async Task<ActionResult> SearchDerivativeItems([FromBody] DerivativeItemSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new DerivativeItemSearchCriteria();
            }

            var retVal = await _derivativeService.SearchDerivativeItemsAsync(searchCriteria);

            return Json(new
            {
                Results = retVal,
                TotalCount = retVal.TotalItemCount
            });
        }
    }
}
