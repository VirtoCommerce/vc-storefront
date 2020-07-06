using System;
using VirtoCommerce.Storefront.Model.Customer.Services;

namespace VirtoCommerce.Storefront.Domain.Customer
{
    public static class GraphQlMemberHelper
    {
        public const string AllAddressFields = "city countryCode countryName email firstName key lastName line1 line2 middleName name organization phone postalCode regionId regionName zip";
        public static readonly string AllOrganizationFields = $"addresses {{ {AllAddressFields} }} businessCategory description emails groups memberType name outerId ownerId parentId phones seoObjectType";
        public static readonly string AllCustomerFields = $"firstName lastName organizations id birthDate fullName middleName name outerId addresses {{ {AllAddressFields} }} organization {{ {AllOrganizationFields} }}";
        public static readonly string AllMemberSearchFields = $"totalCount items {{ {AllCustomerFields} }}";

        public static string UpdateContactAddressesRequest(this IMemberService service, string selectedFields = null)
        => $@"mutation ($id: String!, $addresses: [AddressInputType]!)
        {{
          updateAddresses(addresses: $addresses, customerId: $id)
          {{
            { selectedFields ?? AllCustomerFields }
          }}
        }}";

        public static string UpdateOrganizationRequest(this IMemberService service, string selectedFields = null)
        => $@"mutation ($command: OrganizationInputType!){{
          updateOrganization(command: $command){{
            { selectedFields ?? AllOrganizationFields }
          }}
        }}";

        public static string SearchOrganizationContacts(this IMemberService service, int first = 20, int after = 0, string selectedFields = null)
        => $@"query ($criteria: SearchOrganizationMembersInputType!){{
          searchOrganizationMembers(command: $criteria, first: {first}, after: ""{after}""){{
            { selectedFields ?? AllMemberSearchFields }
            }}
        }}";

        public static string UpdateContactRequest(this IMemberService service, string selectedFields = null)
        => throw new NotImplementedException();
    }
}
