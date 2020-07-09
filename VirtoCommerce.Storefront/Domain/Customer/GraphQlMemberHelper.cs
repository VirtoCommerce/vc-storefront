using VirtoCommerce.Storefront.Model.Customer.Services;

namespace VirtoCommerce.Storefront.Domain.Customer
{
    public static class GraphQlMemberHelper
    {
        public const string AllAddressFields = "city countryCode countryName email firstName key lastName line1 line2 middleName name organization phone postalCode regionId regionName zip";
        public static readonly string AllOrganizationFields = $"id memberType name addresses {{ {AllAddressFields} }} businessCategory description emails groups outerId ownerId parentId phones seoObjectType";
        public static readonly string AllContactFields = $"firstName lastName organizations {{ {AllOrganizationFields} }} id birthDate fullName memberType middleName name outerId addresses {{ {AllAddressFields} }}";
        public static readonly string AllMemberSearchFields = $"totalCount items {{ {AllContactFields } }}";

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

        public static string SearchOrganizationContacts(this IMemberService service, int first = 20, int after = 0, string selectedFields = null)
        => $@"query ($criteria: SearchOrganizationMembersInputType!){{
          searchOrganizationMembers(command: $criteria, first: {first}, after: ""{after}""){{
            { selectedFields ?? AllMemberSearchFields }
            }}
        }}";

        public static string GetOrganizationRequest(this IMemberService service, string id, string selectedFields = null)
        => $@"
        {{
            organization(id:""{id}"")
            {{
            { selectedFields ?? AllContactFields }
            }}
        }}";

        public static string DeleteContactRequest(this IMemberService service)
        => $@"mutation ($command: InputDeleteContactType!)
        {{
            deleteContact(command: $command)
        }}";
    }
}
