using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public static class OrderWorkContextBuilderExtensions
    {
        public static Task WithUserOrdersAsync(this IWorkContextBuilder builder, IMutablePagedList<CustomerOrder> orders)
        {
            builder.WorkContext.CurrentUser.Orders = orders;
            return Task.CompletedTask;
        }

        public static Task WithUserOrdersAsync(this IWorkContextBuilder builder)
        {
            if (builder.WorkContext.CurrentUser != null)
            {
                var serviceProvider = builder.HttpContext.RequestServices;
                var orderService = serviceProvider.GetRequiredService<ICustomerOrderService>();

                Func<int, int, IEnumerable<SortInfo>, IPagedList<CustomerOrder>> factory = (pageNumber, pageSize, sortInfos) =>
                {
                    var orderSearchcriteria = new OrderSearchCriteria
                    {
                        CustomerId = builder.WorkContext.CurrentUser.Id,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Sort = sortInfos?.ToString()
                    };
                    var result = orderService.SearchOrders(orderSearchcriteria);
                    return new StaticPagedList<CustomerOrder>(result, pageNumber, pageSize, result.Count);
                };
                return builder.WithUserOrdersAsync(new MutablePagedList<CustomerOrder>(factory, 1, OrderSearchCriteria.DefaultPageSize));
            }
            return Task.CompletedTask;
        }
    }
}
