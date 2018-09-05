using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Domain.Subscriptions;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionModule _subscriptionApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly StorefrontOptions _options;
        public SubscriptionService(ISubscriptionModule subscriptionApi, IWorkContextAccessor workContextAccessor, IStorefrontMemoryCache memoryCache, IOptions<StorefrontOptions> options)
        {
            _subscriptionApi = subscriptionApi;
            _workContextAccessor = workContextAccessor;
            _memoryCache = memoryCache;
            _options = options.Value;
        }
        public async Task<Subscription> CancelSubscriptionAsync(SubscriptionCancelRequest request)
        {
            var workContext = _workContextAccessor.WorkContext;
            var result = (await _subscriptionApi.CancelSubscriptionAsync(new AutoRestClients.SubscriptionModuleApi.Models.SubscriptionCancelRequest
            {
                CancelReason = request.CancelReason,
                SubscriptionId = request.SubscriptionId
            })).ToSubscription(workContext.AllCurrencies, workContext.CurrentLanguage);

            SubscriptionCacheRegion.ExpireCustomerSubscription(request.CustomerId);

            return result;

        }
        public async Task DeletePlansByIdsAsync(string[] ids)
        {
            await _subscriptionApi.DeletePlansByIdsAsync(ids);
            PaymentPlanCacheRegion.ExpireRegion();
        }
        public async Task UpdatePaymentPlanAsync(PaymentPlan plan)
        {
            var paymentPlanDto = plan.ToPaymentPlanDto();

            await _subscriptionApi.UpdatePaymentPlanAsync(paymentPlanDto);
            PaymentPlanCacheRegion.ExpireRegion();
        }

        public async Task<IList<PaymentPlan>> GetPaymentPlansByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetPaymentPlansByIdsAsync", string.Join("-", ids.OrderBy(x => x)));
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PaymentPlanCacheRegion.CreateChangeToken());
                return (await _subscriptionApi.GetPaymentPlansByPlentyIdsAsync(ids)).Select(x => x.ToPaymentPlan()).ToList();
            });
        }

        public IPagedList<Subscription> SearchSubscription(SubscriptionSearchCriteria criteria)
        {
            return SearchSubscriptionsAsync(criteria).GetAwaiter().GetResult();
        }

        public async Task<IPagedList<Subscription>> SearchSubscriptionsAsync(SubscriptionSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), "SearchSubscription", criteria.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                //Observe all subscriptions for current user and invalidate them in cache if any changed (one limitation - this toke doesn't detect subscriptions deletions)
                cacheEntry.AddExpirationToken(new PoolingApiSubscriptionsChangeToken(_subscriptionApi, _options.ChangesPoolingInterval));
                cacheEntry.AddExpirationToken(SubscriptionCacheRegion.CreateCustomerSubscriptionChangeToken(criteria.CustomerId));
                var result = await _subscriptionApi.SearchSubscriptionsAsync(criteria.ToSearchCriteriaDto());
                return new StaticPagedList<Subscription>(result.Subscriptions.Select(x => x.ToSubscription(workContext.AllCurrencies, workContext.CurrentLanguage)),
                                                         criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
            });
        }
    }
}
