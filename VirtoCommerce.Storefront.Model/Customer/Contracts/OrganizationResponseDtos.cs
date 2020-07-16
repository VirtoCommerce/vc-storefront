using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class OrganizationResponseDto
    {
        public Organization Organization { get; set; }
    }

    public class OrganizationContactsResponseDto
    {
        public OrganizationResponseDtos Organization { get; set; }
    }

    public class OrganizationResponseDtos
    {
        public ContactSearchResult Contacts { get; set; }
    }

    public class ContactSearchResult : GenericSearchResult<Contact>
    {
    }
}
