using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using marketingModel = VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class PromotionEvaluator : IPromotionEvaluator
    {
        private readonly IMarketingModulePromotion _promiotionApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly StorefrontOptions _storefrontOptions;
        public PromotionEvaluator(IMarketingModulePromotion promiotionApi, IStorefrontMemoryCache memoryCache, IOptions<StorefrontOptions> storefrontOptions)
        {
            _promiotionApi = promiotionApi;
            _memoryCache = memoryCache;
            _storefrontOptions = storefrontOptions.Value;
        }

        #region IPromotionEvaluator Members
        public virtual async Task EvaluateDiscountsAsync(PromotionEvaluationContext context, IEnumerable<IDiscountable> owners)
        {
            var cacheKey = CacheKey.With(GetType(), "EvaluateDiscountsAsync", context.GetCacheKey());
            var rewards = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(MarketingCacheRegion.CreateChangeToken());
                //Workaround: Use lifetime for promotions from ChangesPollingInterval setting to be able manage this value
                cacheEntry.SetAbsoluteExpiration(_storefrontOptions.ChangesPollingInterval);

                var contextDto = context.ToPromotionEvaluationContextDto();
                return await _promiotionApi.EvaluatePromotionsAsync(contextDto);
            });
            ApplyRewards(rewards, owners);
        }
        #endregion

        protected virtual void ApplyRewards(IList<marketingModel.PromotionReward> rewards, IEnumerable<IDiscountable> owners)
        {
            if (rewards != null)
            {
                var rewardsMap = owners.Select(x => x.Currency).Distinct().ToDictionary(x => x, x => rewards.Select(r => r.ToPromotionReward(x)).ToArray());

                foreach (var owner in owners)
                {
                    owner.ApplyRewards(rewardsMap[owner.Currency]);
                }
            }
        }
    }
}
