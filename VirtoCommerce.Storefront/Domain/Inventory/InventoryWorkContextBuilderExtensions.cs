using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Inventory;
using VirtoCommerce.Storefront.Model.Inventory.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public static class InventoryWorkContextBuilderExtensions
    {
        public static Task WithFulfillmentCentersAsync(this IWorkContextBuilder builder, Func<IMutablePagedList<FulfillmentCenter>> factory)
        {
            builder.WorkContext.FulfillmentCenters = factory();
            return Task.CompletedTask;
        }

        public static Task WithFulfillmentCentersAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var inventoryService = serviceProvider.GetRequiredService<IInventoryService>();
        
            Func<int, int, IEnumerable<SortInfo>, IPagedList<FulfillmentCenter>> factory = (pageNumber, pageSize, sortInfos) =>
            {
                return inventoryService.SearchFulfillmentCenters(new FulfillmentCenterSearchCriteria { PageNumber = pageNumber, PageSize = pageSize, Sort = SortInfo.ToString(sortInfos) });
               
            };
            return builder.WithFulfillmentCentersAsync(() => new MutablePagedList<FulfillmentCenter>(factory, 1, FulfillmentCenterSearchCriteria.DefaultPageSize));
        }
    }
}
