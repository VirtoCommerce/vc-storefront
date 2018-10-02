using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Inventory;
using VirtoCommerce.Storefront.Model.Inventory.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("")]
    public class ApiInventoryController : StorefrontControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public ApiInventoryController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IInventoryService inventoryService)
            : base(workContextAccessor, urlBuilder)
        {
            _inventoryService = inventoryService;
        }

        // POST: storefrontapi/fulfillmentcenters/search
        [HttpPost("fulfillmentcenters/search")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchFulfillmentCenters([FromBody] FulfillmentCenterSearchCriteria criteria)
        {
            if (criteria != null)
            {
                var result = await _inventoryService.SearchFulfillmentCentersAsync(criteria);
                return Json(new { TotalCount = result.TotalItemCount, Results = result.ToArray() });
            }
            return Ok();
        }
    }
}
