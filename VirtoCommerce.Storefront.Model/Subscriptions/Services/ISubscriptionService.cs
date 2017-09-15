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
    }
}
