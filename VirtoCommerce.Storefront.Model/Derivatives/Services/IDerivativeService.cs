using System.Collections.Generic;
using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Derivatives.Services
{
    public interface IDerivativeService
    {
        Task EvaluateProductDerivativeInfoAsync(List<Product> products, WorkContext workContext);

        Task<IList<Derivative>> GetDerivativesAsync(string[] ids);

        Task<IPagedList<Derivative>> SearchDerivativesAsync(DerivativeSearchCriteria criteria);

        Task<IPagedList<DerivativeItem>> SearchDerivativeItemsAsync(DerivativeItemSearchCriteria criteria);
    }
}
