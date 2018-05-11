using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Pricing.Services
{
    public interface IPricingService
    {
        Task EvaluateProductPricesAsync(IEnumerable<Product> products, WorkContext workContext);
        Task<IList<Pricelist>> EvaluatePricesListsAsync(PriceEvaluationContext evalContext, WorkContext workContext);
        IList<Pricelist> EvaluatePricesLists(PriceEvaluationContext evalContext, WorkContext workContext);
    }
}
