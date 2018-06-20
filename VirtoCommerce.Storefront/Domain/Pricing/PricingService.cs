using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Pricing;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Tax.Services;
using pricingModel = VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class PricingService : IPricingService
    {
        private readonly IPricingModule _pricingApi;
        private readonly ITaxEvaluator _taxEvaluator;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly IInventoryService _inventoryService;
        private readonly IMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public PricingService(IPricingModule pricingApi,
            ITaxEvaluator taxEvaluator,
            IPromotionEvaluator promotionEvaluator,
            IInventoryService inventoryService, IMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _pricingApi = pricingApi;
            _taxEvaluator = taxEvaluator;
            _promotionEvaluator = promotionEvaluator;
            _inventoryService = inventoryService;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }

        #region IPricingService Members
        public IList<Pricelist> EvaluatePricesLists(PriceEvaluationContext evalContext, WorkContext workContext)
        {
            return Task.Factory.StartNew(() => EvaluatePricesListsAsync(evalContext, workContext), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public virtual async Task<IList<Pricelist>> EvaluatePricesListsAsync(PriceEvaluationContext evalContext, WorkContext workContext)
        {
            if (evalContext == null)
            {
                throw new ArgumentNullException(nameof(evalContext));
            }
            if (workContext == null)
            {
                throw new ArgumentNullException(nameof(workContext));
            }
            var cacheKey = CacheKey.With(GetType(), "EvaluatePricesListsAsync", evalContext.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PricingCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                return (await _pricingApi.EvaluatePriceListsAsync(evalContext.ToPriceEvaluationContextDto())).Select(x => x.ToPricelist(workContext.AllCurrencies, workContext.CurrentLanguage)).ToList();
            });

        }

        public virtual async Task EvaluateProductPricesAsync(IEnumerable<Product> products, WorkContext workContext)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }
            if (workContext == null)
            {
                throw new ArgumentNullException(nameof(workContext));
            }

            //Evaluate products prices
            var evalContext = workContext.ToPriceEvaluationContext(products);
            var cacheKey = CacheKey.With(GetType(), "EvaluateProductPricesAsync", evalContext.GetCacheKey());
            var pricesResponse = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PricingCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                return await _pricingApi.EvaluatePricesAsync(evalContext.ToPriceEvaluationContextDto());
            });
            ApplyProductPrices(products, pricesResponse, workContext);

            //Evaluate product discounts
            //Fill product inventories for getting InStockQuantity data for promotion evaluation
            await _inventoryService.EvaluateProductInventoriesAsync(products.ToList(), workContext);
            var promoEvalContext = workContext.ToPromotionEvaluationContext(products);
            await _promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, products);

            //Evaluate product taxes
            var taxEvalContext = workContext.ToTaxEvaluationContext(products);
            await _taxEvaluator.EvaluateTaxesAsync(taxEvalContext, products);
        }

        #endregion

        protected virtual void ApplyProductPrices(IEnumerable<Product> products, IList<pricingModel.Price> prices, WorkContext workContext)
        {
            foreach (var product in products)
            {
                var productPrices = prices.Where(x => x.ProductId == product.Id)
                                          .Select(x => x.ToProductPrice(workContext.AllCurrencies, workContext.CurrentLanguage));
                product.ApplyPrices(productPrices, workContext.CurrentCurrency, workContext.AllCurrencies);
            }
        }
    }
}
