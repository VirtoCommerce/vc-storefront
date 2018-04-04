using PagedList.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Customer.Services
{
    public interface IMemberService
    {
        Task<Contact> GetContactByIdAsync(string contactId);
        Contact GetContactById(string contactId);
        Task CreateContactAsync(Contact contact);
        Task UpdateContactAsync(string contactId, ContactUpdateInfo updateInfo);
        Task UpdateContactAddressesAsync(string contactId, IList<Address> addresses);

        IPagedList<Contact> SearchOrganizationContacts(OrganizationContactsSearchCriteria criteria);
        Task<IPagedList<Contact>> SearchOrganizationContactsAsync(OrganizationContactsSearchCriteria criteria);

        Task<Organization> GetOrganizationByIdAsync(string organizationId);
        Organization GetOrganizationById(string organizationId);
        Task CreateOrganizationAsync(Organization organization);
        Task UpdateOrganizationAsync(Organization organization);


        Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language,  params string[] vendorIds);
        Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds);
        IPagedList<Vendor> SearchVendors(Store store, Language language, string keyword, int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos);
    }
}
