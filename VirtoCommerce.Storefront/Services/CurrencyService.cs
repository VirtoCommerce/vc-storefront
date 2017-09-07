using CacheManager.Core;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICommerce _commerceApi;
        private readonly ICacheManager<object> _cache;

        public CurrencyService(ICommerce commerceApi, ICacheManager<object> cache)
        {
            _commerceApi = commerceApi;
            _cache = cache;
        }

        public async Task<Currency[]> GetAllCurrenciesAsync(Language language)
        {
            var result = await _cache.GetAsync("GetAllCurrencies", StorefrontConstants.CurrencyCacheRegion, async () => (await _commerceApi.GetAllCurrenciesAsync()).Select(x => x.ToCurrency(language)).ToArray(), cacheNullValue: false);
            return result;
        }

    }
}
