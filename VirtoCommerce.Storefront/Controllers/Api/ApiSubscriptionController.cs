using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("subscriptions")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiSubscriptionController : StorefrontControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public ApiSubscriptionController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ISubscriptionService subscriptionService)
            : base(workContextAccessor, urlBuilder)
        {
            _subscriptionService = subscriptionService;
        }

        // POST: storefrontapi/subscriptions/search
        [HttpPost("search")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<SubscriptionSearchResult>> SearchCustomerSubscriptions([FromBody] SubscriptionSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new SubscriptionSearchCriteria();
            }

            //Does not allow to see a other subscriptions
            searchCriteria.CustomerId = WorkContext.CurrentUser.Id;

            var result = await _subscriptionService.SearchSubscriptionsAsync(searchCriteria);

            return new SubscriptionSearchResult
            {
                TotalCount = result.TotalItemCount,
                Results = result.ToArray()
            };
        }

        // GET: storefrontapi/subscriptions/{number}
        [HttpGet("{number}")]
        public async Task<ActionResult<Subscription>> GetCustomerSubscription(string number)
        {
            var retVal = await GetSubscriptionByNumberAsync(number);
            return retVal;
        }

        // POST: storefrontapi/subscriptions/cancel
        [HttpPost("{number}/cancel")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Subscription>> CancelSubscription([FromBody] SubscriptionCancelRequest cancelRequest)
        {
            var subscription = await GetSubscriptionByNumberAsync(cancelRequest.Number);
            var retVal = (await _subscriptionService.CancelSubscriptionAsync(new SubscriptionCancelRequest
            {
                CancelReason = cancelRequest.CancelReason,
                SubscriptionId = subscription.Id,
                CustomerId = WorkContext.CurrentUser.Id
            }));

            return retVal;
        }

        private async Task<Subscription> GetSubscriptionByNumberAsync(string number)
        {
            var criteria = new SubscriptionSearchCriteria
            {
                Number = number,
                CustomerId = WorkContext.CurrentUser.Id
            };
            var retVal = (await _subscriptionService.SearchSubscriptionsAsync(criteria)).FirstOrDefault();

            if (retVal == null || retVal.CustomerId != WorkContext.CurrentUser.Id)
            {
                throw new StorefrontException($"Subscription with number {{ number }} not found (or not belongs to current user)");
            }
            return retVal;
        }
    }
}
