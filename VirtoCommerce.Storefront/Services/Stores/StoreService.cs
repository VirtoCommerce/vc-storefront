using CacheManager.Core;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Services
{
    /// <summary>
    /// Data access object for stores implementation
    /// </summary>
    public class StoreService : IStoreService
    {
        private readonly IStoreModule _storeApi;
        private readonly ICacheManager<object> _cache;
        public StoreService(IStoreModule storeApi, ICacheManager<object> cache)
        {
            _storeApi = storeApi;
            _cache = cache;
        }
        public async Task<Model.Stores.Store[]> GetAllStoresAsync()
        {
            var result = await _cache.GetAsync("GetAllStores", StorefrontConstants.CountryCacheRegion, async () => (await _storeApi.GetStoresAsync()).Select(x => x.ToStore()).ToArray(), cacheNullValue: false);
            return result;
        }
    }
}
