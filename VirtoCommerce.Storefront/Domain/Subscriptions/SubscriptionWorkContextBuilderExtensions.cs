using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SubscriptionWorkContextBuilderExtensions
    {
        public static Task WithUserSubscriptionsAsync(this IWorkContextBuilder builder, Func<IMutablePagedList<Subscription>> factory)
        {
            builder.WorkContext.CurrentUser.Subscriptions = factory();
            return Task.CompletedTask;
        }

        public static Task WithUserSubscriptionsAsync(this IWorkContextBuilder builder)
        {
            if (builder.WorkContext.CurrentUser != null)
            {
                var serviceProvider = builder.HttpContext.RequestServices;
                var subscriptionService = serviceProvider.GetRequiredService<ISubscriptionService>();
              
                Func<int, int, IEnumerable<SortInfo>, IPagedList<Subscription>> factory = (pageNumber, pageSize, sortInfos) =>
                {
                    var subscriptionSearchCriteria = new SubscriptionSearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Sort = sortInfos?.ToString(),
                        CustomerId = builder.WorkContext.CurrentUser.Id
                    };
                    return subscriptionService.SearchSubscription(subscriptionSearchCriteria);
                };
                return builder.WithUserSubscriptionsAsync(() => new MutablePagedList<Subscription>(factory, 1, SubscriptionSearchCriteria.DefaultPageSize));
            }
            return Task.CompletedTask;
        }

    }
}
