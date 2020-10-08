using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Stores;
using StoreApi = VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi.Models;

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
        private readonly StorefrontOptions _storefrontOptions;

        public StoreService(IStoreModule storeApi
            ,IStorefrontMemoryCache memoryCache
            ,IApiChangesWatcher apiChangesWatcher
            ,IOptions<StorefrontOptions> storefrontOptions)
        {
            _storeApi = storeApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
            _storefrontOptions = storefrontOptions.Value;
        }
        public async Task<Model.Stores.Store[]> GetAllStoresAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllStoresAsync");
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StoreCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                var storeDtos = await _storeApi.GetStoresAsync();
                return await Task.WhenAll(storeDtos.Select(x => ConvertStoreAndLoadDependeciesAsync(x)));
            }, cacheNullValue: false);
        }

        public async Task<Store> GetStoreByIdAsync(string id, Currency currency = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetStoreByIdAsync", id, currency?.ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StoreCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                var storeDto = await _storeApi.GetStoreByIdAsync(id);
                return await ConvertStoreAndLoadDependeciesAsync(storeDto, currency);
            }, cacheNullValue: false);
        }

        protected virtual Task<Store> ConvertStoreAndLoadDependeciesAsync(StoreApi.Store storeDto, Currency currency = null)
        {
            var result = storeDto.ToStore();          
        
            //use url for stores from configuration file with hight priority than store url defined in manager
            result.Url = _storefrontOptions.StoreUrls[result.Id] ?? result.Url;

            return Task.FromResult(result);
        }

    }
}
