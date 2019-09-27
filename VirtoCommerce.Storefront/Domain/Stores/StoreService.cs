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
using StorePaymentMethod = VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi.Models.PaymentMethod;
using TaxProvider = VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi.Models.TaxProvider;


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
                    .Select(pm => pm.ToStorePaymentMethod(currency)).ToArray();
            }
            else if (storeDto.PaymentMethods.IsNullOrEmpty() && currency != null)
            {
                var paymentSearchCriteria = new PaymentMethodsSearchCriteria { StoreId = id };
                var paymentsSearchResult = await _paymentModule.SearchPaymentMethodsAsync(paymentSearchCriteria);

                result.PaymentMethods = paymentsSearchResult.Results
                    .Where(pm => pm.IsActive != null && pm.IsActive.Value)
                    .Select(pm =>
                    {
                        var paymentMethod = pm.JsonConvert<StorePaymentMethod>().ToStorePaymentMethod(currency);
                        paymentMethod.Name = pm.TypeName;
                        return paymentMethod;
                    }).ToArray();
            }

            if (storeDto.TaxProviders.IsNullOrEmpty())
            {
                var taxSearchCriteria = new TaxProviderSearchCriteria { StoreId = id };
                var taxProviderSearchResult = await _taxModule.SearchTaxProvidersAsync(taxSearchCriteria);
                result.FixedTaxRate = GetFixedTaxRate(taxProviderSearchResult.Results.Select(xp => xp.JsonConvert<TaxProvider>()).ToArray());
            }
            else
            {
                result.FixedTaxRate = GetFixedTaxRate(storeDto.TaxProviders);
            }
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
                    .Select(x => x.JsonConvert<Setting>().ToSettingEntry())
                    .GetSettingValue("VirtoCommerce.Core.FixedTaxRateProvider.Rate", 0.00m);
            }

            return result;
        }
    }
}
