using System.Collections.Generic;
using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Contracts.Services
{
    public interface IDerivativeContractService
    {
        Task EvaluateProductDerivativeInfoAsync(List<Product> products, WorkContext workContext);

        Task<IList<DerivativeContract>> GetDerivativeContractsAsync(string[] ids);

        Task<IList<DerivativeContract>> GetDerivativeContractItemsAsync(string[] ids);

        Task<IPagedList<DerivativeContract>> SearchDerivativeContractsAsync(DerivativeContractSearchCriteria criteria);

        Task<IPagedList<DerivativeContractItem>> SearchDerivativeContractItemsAsync(DerivativeContractItemSearchCriteria criteria);
    }
}
