using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Inventory.Services;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class InventoryController : StorefrontControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IInventoryService inventoryService)
            : base(workContextAccessor, urlBuilder)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// GET: /fulfillmentcenter/{id}
        /// This action is used by storefront to get fulfillment center details by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("fulfillmentcenter/{id}")]
        public async Task<ActionResult> FulfillmentCenterDetails(string id)
        {
            var center = await _inventoryService.GetFulfillmentCenterByIdAsync(id);

            if (center != null)
            {
                WorkContext.CurrentFulfillmentCenter = center;
                return View("fulfillment-center", WorkContext);
            }

            return NotFound();
        }
    }
}
