using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Tax;
using VirtoCommerce.Storefront.Model.Tax.Services;
using coreService = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class TaxEvaluator : ITaxEvaluator
    {
        private readonly ICommerce _commerceApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        public TaxEvaluator(ICommerce commerceApi, IStorefrontMemoryCache memoryCache)
        {
            _commerceApi = commerceApi;
            _memoryCache = memoryCache;
        }

        #region ITaxEvaluator Members

        public virtual async Task EvaluateTaxesAsync(TaxEvaluationContext context, IEnumerable<ITaxable> owners)
        {
            IList<coreService.TaxRate> taxRates = new List<coreService.TaxRate>();
            if (context.StoreTaxCalculationEnabled)
            {
                var cacheKey = CacheKey.With(GetType(), context.GetCacheKey());

                taxRates = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, (cacheEntry) =>
                {
                    cacheEntry.AddExpirationToken(TaxCacheRegion.CreateChangeToken());
                    return _commerceApi.EvaluateTaxesAsync(context.StoreId, context.ToTaxEvaluationContextDto());
                });
            }
            ApplyTaxRates(taxRates, owners);
        }

        #endregion

        private static void ApplyTaxRates(IList<coreService.TaxRate> taxRates, IEnumerable<ITaxable> owners)
        {
            if (taxRates == null)
            {
                return;
            }
            var taxRatesMap = owners.Select(x => x.Currency).Distinct().ToDictionary(x => x, x => taxRates.Select(r => r.ToTaxRate(x)).ToArray());

            foreach (var owner in owners)
            {
                owner.ApplyTaxRates(taxRatesMap[owner.Currency]);
            }
        }
    }
}
