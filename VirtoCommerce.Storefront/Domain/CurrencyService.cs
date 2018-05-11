using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICommerce _commerceApi;
        private readonly IMemoryCache _memoryCache;

        public CurrencyService(ICommerce commerceApi, IMemoryCache memoryCache)
        {
            _commerceApi = commerceApi;
            _memoryCache = memoryCache;
        }

        public async Task<Currency[]> GetAllCurrenciesAsync(Language language)
        {
            var cacheKey = CacheKey.With(GetType(), language.CultureName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                return (await _commerceApi.GetAllCurrenciesAsync()).Select(x => x.ToCurrency(language)).ToArray();
            });
        }

    }
}
