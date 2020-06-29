using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.PaymentModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PaymentModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.TaxModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.TaxModuleApi.Models;
using VirtoCommerce.Storefront.Common;
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
        private readonly IPaymentModule _paymentModule;
        private readonly ITaxModule _taxModule;
        private readonly StorefrontOptions _storefrontOptions;

        public StoreService(IStoreModule storeApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher, IPaymentModule paymentModule, ITaxModule taxModule, IOptions<StorefrontOptions> storefrontOptions)
        {
            _storeApi = storeApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
            _paymentModule = paymentModule;
            _taxModule = taxModule;
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

        protected virtual async Task<Store> ConvertStoreAndLoadDependeciesAsync(StoreApi.Store storeDto, Currency currency = null)
        {
            var result = storeDto.ToStore();

            if (currency != null)
            {
                var paymentSearchCriteria = new PaymentMethodsSearchCriteria { StoreId = result.Id };
                var paymentsSearchResult = await _paymentModule.SearchPaymentMethodsAsync(paymentSearchCriteria);

                result.PaymentMethods = paymentsSearchResult.Results
                    .Where(pm => pm.IsActive != null && pm.IsActive.Value)
                    .Select(pm =>
                    {
                        var paymentMethod = pm.ToStorePaymentMethod(currency);
                        paymentMethod.Name = pm.TypeName;
                        return paymentMethod;
                    }).ToArray();
            }
            var taxSearchCriteria = new TaxProviderSearchCriteria { StoreId = result.Id };
            var taxProviderSearchResult = await _taxModule.SearchTaxProvidersAsync(taxSearchCriteria);
            result.FixedTaxRate = GetFixedTaxRate(taxProviderSearchResult.Results.Select(xp => xp.JsonConvert<TaxProvider>()).ToArray());

            //use url for stores from configuration file with hight priority than store url defined in manager
            result.Url = _storefrontOptions.StoreUrls[result.Id] ?? result.Url;

            return result;
        }

        private decimal GetFixedTaxRate(IList<TaxProvider> taxProviders)
        {
            var result = 0m;
            var fixedTaxProvider = taxProviders.FirstOrDefault(x => x.IsActive.GetValueOrDefault(false) && x.Code == "FixedRate");
            if (fixedTaxProvider != null && !fixedTaxProvider.Settings.IsNullOrEmpty())
            {
                result = fixedTaxProvider.Settings
                    .Select(x => x.JsonConvert<AutoRestClients.PlatformModuleApi.Models.ObjectSettingEntry>().ToSettingEntry())
                    .GetSettingValue("VirtoCommerce.Core.FixedTaxRateProvider.Rate", 0.00m);
            }

            return result;
        }
    }
}
