using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Pricing;
namespace VirtoCommerce.Storefront.Domain
{
    public static class PricingWorkContextBuilderExtensions
    {
        public static Task WithPricelistsAsync(this IWorkContextBuilder builder, IList<Pricelist> pricelists)
        {
            if(pricelists == null)
            {
                throw new ArgumentNullException(nameof(pricelists));
            }
            builder.WorkContext.CurrentPricelists = pricelists;
            return Task.CompletedTask;
        }

        public static async Task WithPricelistsAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var pricingService = serviceProvider.GetRequiredService<IPricingService>();

            var pricelists = await pricingService.EvaluatePricesListsAsync(builder.WorkContext.ToPriceEvaluationContext(), builder.WorkContext);
            await builder.WithPricelistsAsync(pricelists);
        }
    }
}
