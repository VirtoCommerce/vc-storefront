using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Contracts;
using VirtoCommerce.Storefront.Model.Contracts.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiDerivativeContractController : StorefrontControllerBase
    {
        private readonly IDerivativeContractService _derivativeService;

        public ApiDerivativeContractController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IDerivativeContractService derivativeService)
            : base(workContextAccessor, urlBuilder)
        {
            _derivativeService = derivativeService;
        }

        // storefrontapi/contracts/derivative?ids=...
        [HttpGet]
        public async Task<ActionResult> GetDerivativeContractsByIds(string[] ids)
        {
            var retVal = await _derivativeService.GetDerivativeContractsAsync(ids);
            return Json(retVal);
        }

        // storefrontapi/contracts/derivative/items?ids=...
        [HttpGet]
        public async Task<ActionResult> GetDerivativeContractItemsByIds(string[] ids)
        {
            var retVal = await _derivativeService.GetDerivativeContractsAsync(ids);
            return Json(retVal);
        }

        // storefrontapi/contracts/derivative/search
        [HttpPost]
        public async Task<ActionResult> SearchDerivativeContracts([FromBody] DerivativeContractSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new DerivativeContractSearchCriteria();
            }

            var retVal = await _derivativeService.SearchDerivativeContractsAsync(searchCriteria);

            return Json(new
            {
                Results = retVal,
                TotalCount = retVal.TotalItemCount
            });
        }

        // storefrontapi/contracts/derivative/items/search
        [HttpPost]
        public async Task<ActionResult> SearchDerivativeContractItems([FromBody] DerivativeContractItemSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new DerivativeContractItemSearchCriteria();
            }

            var retVal = await _derivativeService.SearchDerivativeContractItemsAsync(searchCriteria);

            return Json(new
            {
                Results = retVal,
                TotalCount = retVal.TotalItemCount
            });
        }
    }
}
