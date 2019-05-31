using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.PaymentModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PaymentModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
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
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        private readonly IPaymentModule _paymentModule;

        public StoreService(IStoreModule storeApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher, IPaymentModule paymentModule)
        {
            _storeApi = storeApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
            _paymentModule = paymentModule;
        }
        public async Task<Model.Stores.Store[]> GetAllStoresAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllStoresAsync");
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StoreCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                return (await _storeApi.GetStoresAsync()).Select(x => x.ToStore()).ToArray();
            }, cacheNullValue: false);
        }

        public async Task<Store> GetStoreByIdAsync(string id, Currency currency)
        {
            var storeDto = await _storeApi.GetStoreByIdAsync(id);
            var result = storeDto.ToStore();

            if (!storeDto.PaymentMethods.IsNullOrEmpty() && currency != null)
            {
                result.PaymentMethods = storeDto.PaymentMethods
                    .Where(pm => pm.IsActive != null && pm.IsActive.Value)
                    .Select(pm => pm.ToPaymentMethod(currency)).ToArray();
            }
            else if (storeDto.PaymentMethods.IsNullOrEmpty() && currency != null)
            {
                var paymentSearchCriteria = new PaymentMethodsSearchCriteria() { StoreId = id };
                var paymentsSearchResult = await _paymentModule.SearchPaymentMethodsAsync(paymentSearchCriteria);

                result.PaymentMethods = paymentsSearchResult.Results
                                                            .Where(pm => pm.IsActive != null && pm.IsActive.Value)
                                                            .Select(pm => pm.ToPaymentMethod(currency)).ToArray();
            }

            return result;
        }
    }
}
