using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    /// <summary>
    /// Data access object for stores implementation
    /// </summary>
    public class StoreService : IStoreService
    {
        private readonly IStoreModule _storeApi;
        private readonly IMemoryCache _memoryCache;
        public StoreService(IStoreModule storeApi, IMemoryCache memoryCache)
        {
            _storeApi = storeApi;
            _memoryCache = memoryCache;
        }
        public async Task<Model.Stores.Store[]> GetAllStoresAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllStoresAsync");
            return await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StoreCacheRegion.GetChangeToken());

                return (await _storeApi.GetStoresAsync()).Select(x => x.ToStore()).ToArray();
            });
        }
    }
}
