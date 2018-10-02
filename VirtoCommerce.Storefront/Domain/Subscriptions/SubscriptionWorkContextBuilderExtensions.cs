using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static Task WithUserSubscriptionsAsync(this IWorkContextBuilder builder, IMutablePagedList<Subscription> subscriptions)
        {
            builder.WorkContext.CurrentUser.Subscriptions = subscriptions;
            return Task.CompletedTask;
        }

        public static Task WithUserSubscriptionsAsync(this IWorkContextBuilder builder)
        {
            if (builder.WorkContext.CurrentUser != null)
            {
                var serviceProvider = builder.HttpContext.RequestServices;
                var subscriptionService = serviceProvider.GetRequiredService<ISubscriptionService>();

                Func<int, int, IEnumerable<SortInfo>, NameValueCollection, IPagedList<Subscription>> factory = (pageNumber, pageSize, sortInfos, @params) =>
                {
                    var subscriptionSearchCriteria = new SubscriptionSearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Sort = sortInfos?.ToString(),
                        CustomerId = builder.WorkContext.CurrentUser.Id
                    };
                    if (@params != null)
                    {
                        subscriptionSearchCriteria.CopyFrom(@params);
                    }
                    return subscriptionService.SearchSubscription(subscriptionSearchCriteria);
                };
                return builder.WithUserSubscriptionsAsync(new MutablePagedList<Subscription>(factory, 1, SubscriptionSearchCriteria.DefaultPageSize));
            }
            return Task.CompletedTask;
        }

    }
}
