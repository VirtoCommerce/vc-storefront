using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using marketingModel = VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class PromotionEvaluator : IPromotionEvaluator
    {
        private readonly IMarketingModulePromotion _promiotionApi;
        private readonly IMemoryCache _memoryCache;

        public PromotionEvaluator(IMarketingModulePromotion promiotionApi, IMemoryCache memoryCache)
        {
            _promiotionApi = promiotionApi;
            _memoryCache = memoryCache;
        }

        #region IPromotionEvaluator Members
        public virtual void EvaluateDiscounts(PromotionEvaluationContext context, IEnumerable<IDiscountable> owners)
        {
            Task.Factory.StartNew(() => EvaluateDiscountsAsync(context, owners), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public virtual async Task EvaluateDiscountsAsync(PromotionEvaluationContext context, IEnumerable<IDiscountable> owners)
        {
            var cacheKey = CacheKey.With(GetType(), "EvaluateDiscountsAsync", context.GetCacheKey());
            var rewards = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(MarketingCacheRegion.CreateChangeToken());
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

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
