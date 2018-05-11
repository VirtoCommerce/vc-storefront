using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Subscriptions.Services
{
    public interface ISubscriptionService
    {
        Task<IPagedList<Subscription>> SearchSubscriptionsAsync(SubscriptionSearchCriteria criteria);
        IPagedList<Subscription> SearchSubscription(SubscriptionSearchCriteria criteria);
        Task<IList<PaymentPlan>> GetPaymentPlansByIdsAsync(string[] ids);
        Task UpdatePaymentPlanAsync(PaymentPlan paymentplan);
        Task DeletePlansByIdsAsync(string[] ids);
        Task<Subscription> CancelSubscriptionAsync(SubscriptionCancelRequest request);
    }
}
