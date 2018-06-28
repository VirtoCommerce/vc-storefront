using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Pricing;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain
{
    public static class PricingWorkContextBuilderExtensions
    {
        public static Task WithPricelistsAsync(this IWorkContextBuilder builder, IMutablePagedList<Pricelist> pricelists)
        {
            builder.WorkContext.CurrentPricelists = pricelists;
            return Task.CompletedTask;
        }

        public static Task WithPricelistsAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var pricingService = serviceProvider.GetRequiredService<IPricingService>();

            Func<int, int, IEnumerable<SortInfo>, IPagedList<Pricelist>> factory = (pageNumber, pageSize, sortInfos) =>
            {
                var pricelists = pricingService.EvaluatePricesLists(builder.WorkContext.ToPriceEvaluationContext(null), builder.WorkContext);
                return new StaticPagedList<Pricelist>(pricelists, pageNumber, pageSize, pricelists.Count);
            };
            return builder.WithPricelistsAsync(new MutablePagedList<Pricelist>(factory, 1, int.MaxValue));
        }
    }
}
