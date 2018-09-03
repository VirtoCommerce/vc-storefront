using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Extensions;
using System.Collections.Generic;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain
{
    /// <summary>
    /// Data access object for stores implementation
    /// </summary>
    public class StoreService : IStoreService
    {
        private readonly IStoreModule _storeApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        public StoreService(IStoreModule storeApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _storeApi = storeApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }
        public async Task<Model.Stores.Store[]> GetAllStoresAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllStoresAsync");
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StoreCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                return (await _storeApi.GetStoresAsync()).Select(x => x.ToStore()).ToArray();
            }, cacheNullValue : false);
        }
    }
}
