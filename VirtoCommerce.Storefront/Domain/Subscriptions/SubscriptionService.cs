using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
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
        private readonly IMemoryCache _memoryCache;
        public SubscriptionService(ISubscriptionModule subscriptionApi, IWorkContextAccessor workContextAccessor, IMemoryCache memoryCache)
        {
            _subscriptionApi = subscriptionApi;
            _workContextAccessor = workContextAccessor;
            _memoryCache = memoryCache;
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
                return (await _subscriptionApi.GetPaymentPlanByIdsAsync(ids)).Select(x => x.ToPaymentPlan()).ToList();
            });
        }

        public IPagedList<Subscription> SearchSubscription(SubscriptionSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return Task.Factory.StartNew(() => InnerSearchSubscriptionsAsync(criteria, workContext), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async Task<IPagedList<Subscription>> SearchSubscriptionsAsync(SubscriptionSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return await InnerSearchSubscriptionsAsync(criteria, workContext);
        }

        protected virtual async Task<IPagedList<Subscription>> InnerSearchSubscriptionsAsync(SubscriptionSearchCriteria criteria, WorkContext workContext)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            var cacheKey = CacheKey.With(GetType(), "InnerSearchSubscriptionsAsync", criteria.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SubscriptionCacheRegion.CreateCustomerSubscriptionChangeToken(criteria.CustomerId));
                var result = await _subscriptionApi.SearchSubscriptionsAsync(criteria.ToSearchCriteriaDto());
                return new StaticPagedList<Subscription>(result.Subscriptions.Select(x => x.ToSubscription(workContext.AllCurrencies, workContext.CurrentLanguage)),
                                                         criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
            });

        }
    }
}
