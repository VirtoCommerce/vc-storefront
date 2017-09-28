using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Inventory.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryModule _inventoryApi;
        private readonly IMemoryCache _memoryCache;
        public InventoryService(IInventoryModule inventoryApi, IMemoryCache memoryCache)
        {
            _inventoryApi = inventoryApi;
            _memoryCache = memoryCache;
        }

        public virtual async Task EvaluateProductInventoriesAsync(ICollection<Product> products, WorkContext workContext)
        {
            if(products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }
            var productIds = products.Select(x => x.Id).ToList();
            var cacheKey = CacheKey.With(GetType(), "EvaluateProductInventoriesAsync", productIds.GetOrderIndependentHashCode().ToString());
            var inventories = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                cacheEntry.AddExpirationToken(InventoryCacheRegion.CreateChangeToken());

                return await _inventoryApi.GetProductsInventoriesByPlentyIdsAsync(productIds);
            });

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