using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Inventory;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using inventoryDto = VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryModule _inventoryApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        public InventoryService(IInventoryModule inventoryApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _inventoryApi = inventoryApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }

        public virtual async Task EvaluateProductInventoriesAsync(IEnumerable<Product> products, WorkContext workContext)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }
            var productIds = products.Select(x => x.Id).ToList();
            var cacheKey = CacheKey.With(GetType(), "EvaluateProductInventoriesAsync", string.Join("-", productIds.OrderBy(x => x)));
            var inventories = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                cacheEntry.AddExpirationToken(InventoryCacheRegion.CreateChangeToken());

                return await _inventoryApi.GetProductsInventoriesByPlentyIdsAsync(productIds);
            });

            var availFullfilmentCentersIds = workContext.CurrentStore.AvailFulfillmentCenterIds;
            foreach (var item in products)
            {
                //TODO: Change these conditions to DDD specification
                item.InventoryAll = inventories.Where(x => x.ProductId == item.Id).Select(x => x.ToInventory()).Where(x => availFullfilmentCentersIds.Contains(x.FulfillmentCenterId)).ToList();
                item.Inventory = item.InventoryAll.OrderByDescending(x => Math.Max(0, (x.InStockQuantity ?? 0L) - (x.ReservedQuantity ?? 0L))).FirstOrDefault();

                if (workContext.CurrentStore.DefaultFulfillmentCenterId != null)
                {
                    item.Inventory = item.InventoryAll.FirstOrDefault(x => x.FulfillmentCenterId == workContext.CurrentStore.DefaultFulfillmentCenterId)
                        ?? item.Inventory;
                }
            }
        }

        public virtual async Task<FulfillmentCenter> GetFulfillmentCenterByIdAsync(string id)
        {
            FulfillmentCenter result = null;
            var centerDto = await _inventoryApi.GetFulfillmentCenterAsync(id);
            if (centerDto != null)
            {
                result = centerDto.ToFulfillmentCenter();
            }
            return result;
        }

        public IPagedList<FulfillmentCenter> SearchFulfillmentCenters(FulfillmentCenterSearchCriteria criteria)
        {
            return SearchFulfillmentCentersAsync(criteria).GetAwaiter().GetResult();
        }

        public async Task<IPagedList<FulfillmentCenter>> SearchFulfillmentCentersAsync(FulfillmentCenterSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchFulfillmentCenters", criteria.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(InventoryCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                var criteriaDto = new inventoryDto.FulfillmentCenterSearchCriteria
                {
                    SearchPhrase = criteria.SearchPhrase,
                    Skip = (criteria.PageNumber - 1) * criteria.PageSize,
                    Take = criteria.PageSize,
                    Sort = criteria.Sort
                };

                var searchResult = await _inventoryApi.SearchFulfillmentCentersAsync(criteriaDto);
                var centers = searchResult.Results.Select(x => x.ToFulfillmentCenter());
                return new StaticPagedList<FulfillmentCenter>(centers, criteria.PageNumber, criteria.PageSize, searchResult.TotalCount.Value);
            });
        }
    }
}
