using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Inventory.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryModule _inventoryApi;

        public InventoryService(IInventoryModule inventoryApi)
        {
            _inventoryApi = inventoryApi;
        }

        public virtual async Task EvaluateProductInventoriesAsync(ICollection<Product> products, WorkContext workContext)
        {
            var inventories = await _inventoryApi.GetProductsInventoriesByPlentyIdsAsync(products.Select(x => x.Id).ToArray());
            var availFullfilmentCentersIds = workContext.CurrentStore.FulfilmentCenters.Select(x => x.Id).ToArray();
            foreach (var item in products)
            {
                //TODO: Change these conditions to DDD specification
                item.InventoryAll = inventories.Where(x => x.ProductId == item.Id).Select(x => x.ToInventory()).Where(x => availFullfilmentCentersIds.Contains(x.FulfillmentCenterId)).ToList();
                item.Inventory = item.InventoryAll.OrderByDescending(x => Math.Max(0, (x.InStockQuantity ?? 0L) - (x.ReservedQuantity ?? 0L))).FirstOrDefault();
                if (workContext.CurrentStore.PrimaryFullfilmentCenter != null)
                {
                    item.Inventory = item.InventoryAll.FirstOrDefault(x => x.FulfillmentCenterId == workContext.CurrentStore.PrimaryFullfilmentCenter.Id);
                }
            }
        }
    }
}