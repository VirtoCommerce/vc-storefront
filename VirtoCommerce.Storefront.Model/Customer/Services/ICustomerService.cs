using PagedList.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Customer.Services
{
    public interface ICustomerService
    {
        Task<CustomerInfo> GetCustomerByIdAsync(string customerId);
        Task CreateCustomerAsync(CustomerInfo customer);
        Task UpdateCustomerAsync(CustomerInfo customer);
        Task UpdateAddressesAsync(CustomerInfo customer);
        Task<bool> CanLoginOnBehalfAsync(string storeId, string customerId);
        Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language,  params string[] vendorIds);
        Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds);
        IPagedList<Vendor> SearchVendors(string keyword, int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos);
    }
}
