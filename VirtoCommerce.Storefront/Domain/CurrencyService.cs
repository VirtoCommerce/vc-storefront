using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICommerce _commerceApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public CurrencyService(ICommerce commerceApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _commerceApi = commerceApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }

        public async Task<Currency[]> GetAllCurrenciesAsync(Language language)
        {
            var cacheKey = CacheKey.With(GetType(), language.CultureName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                return (await _commerceApi.GetAllCurrenciesAsync()).Select(x => x.ToCurrency(language)).ToArray();
            });
        }

    }
}
