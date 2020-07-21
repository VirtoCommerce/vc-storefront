using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;

namespace VirtoCommerce.Storefront.Domain.Customer
{
    public static class GraphQlMemberHelper
    {
        public const string AllAddressFields = "city countryCode countryName email firstName key lastName line1 line2 middleName name organization phone postalCode regionId regionName zip addressType";
        public static readonly string AllOrganizationFields = $"id memberType name addresses {{ {AllAddressFields} }} businessCategory description emails groups outerId ownerId parentId phones seoObjectType";
        public static readonly string AllContactFields = $"firstName lastName organizations {{ {AllOrganizationFields} }} organizationsIds id birthDate fullName memberType middleName name outerId addresses {{ {AllAddressFields} }}";
        public static readonly string AllMemberSearchFields = $"totalCount results:items {{ securityAccounts {{ id }} }}";

        public static string CreateContactRequest(this IMemberService service, string selectedFields = null)
        => $@"mutation ($command: InputCreateContactType!)
        {{
          contact: createContact(command: $command)
          {{
            { selectedFields ?? AllContactFields }
          }}
        }}";

        public static string UpdateContactRequest(this IMemberService service, string selectedFields = null)
        => $@"mutation ($command: InputUpdateContactType!)
        {{
          contact: updateContact(command: $command)
          {{
            { selectedFields ?? AllContactFields }
          }}
        }}";

        public static string GetContactRequest(this IMemberService service, string id, string selectedFields = null)
        => $@"
        {{
            contact(id:""{id}"")
            {{
            { selectedFields ?? AllContactFields }
            }}
        }}";

        public static string UpdateContactAddressesRequest(this IMemberService service, string selectedFields = null)
        => $@"mutation ($command: InputUpdateContactAddressType!)
        {{
          contact: updateAddresses(command: $command)
          {{
            { selectedFields ?? AllContactFields }
          }}
        }}";

        public static string CreateOrganizationRequest(this IMemberService service, string selectedFields = null)
        => $@"mutation ($command: InputCreateOrganizationType!)
        {{
          organization: createOrganization(command: $command)
            {{
                { selectedFields ?? AllOrganizationFields }
            }}
        }}";

        public static string UpdateOrganizationRequest(this IMemberService service, string selectedFields = null)
        => $@"mutation ($command: InputUpdateOrganizationType!){{
          organization: updateOrganization(command: $command){{
            { selectedFields ?? AllOrganizationFields }
          }}
        }}";

        public static string OrganizationWithContactsRequest(this IMemberService service, OrganizationContactsSearchCriteria criteria, string selectedFields = null)
        => $@"{{organization(id:""{criteria.OrganizationId}""){{
                contacts(first: {criteria.PageSize}, after: ""{(criteria.PageNumber - 1) * criteria.PageSize}"", searchPhrase: ""{criteria.SearchPhrase}""){{
            { selectedFields ?? AllMemberSearchFields }
            }}
        }} }}";

        public static string GetOrganizationRequest(this IMemberService service, string id, string selectedFields = null)
        => $@"
        {{
            organization(id:""{id}"")
            {{
            { selectedFields ?? AllOrganizationFields }
            }}
        }}";

        public static string DeleteContactRequest(this IMemberService service)
        => $@"mutation ($command: InputDeleteContactType!)
        {{
            deleteContact(command: $command)
        }}";
    }
}
