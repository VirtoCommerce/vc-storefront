using System.Collections.Generic;
using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Customer.Services
{
    public interface IMemberService
    {
        Task<Contact> GetContactByIdAsync(string contactId);
        Task<Contact> CreateContactAsync(Contact contact);
        Task UpdateContactAsync(Contact contact);
        Task UpdateContactAddressesAsync(string contactId, IList<Address> addresses);
        Task DeleteContactAsync(string contactId);

        IPagedList<Contact> SearchOrganizationContacts(OrganizationContactsSearchCriteria criteria);
        Task<IPagedList<Contact>> SearchOrganizationContactsAsync(OrganizationContactsSearchCriteria criteria);

        Task<Organization> GetOrganizationByIdAsync(string organizationId);
        Task<Organization> CreateOrganizationAsync(Organization organization);
        Task UpdateOrganizationAsync(Organization organization);
    }
}
