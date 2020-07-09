namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class GraphQlSecurityHelper
    {
        public static readonly string SecurityResultFields = $"succeeded errors";
        public static readonly string AllRoleFields = $"id name permissions";
        public static readonly string AllExternalLoginFields = $"";
        public static readonly string AllUserFields = $"id storeId userName password phoneNumber phoneNumberConfirmed email emailConfirmed twoFactorEnabled" +
            $"accessFailedCount lockoutEnabled lockoutEndDateUtc isAdministrator userType userState contactId roles {{{AllRoleFields}}} externalLogins {{{AllExternalLoginFields}}}";

        public static string CreateUserRequest(this SecurityGraphQLProvider service, string selectedFields = null)
        => $@"mutation ($command: InputCreateUserType!)
        {{
          securityResult: createUser(command: $command)
          {{
            { selectedFields ?? SecurityResultFields }
          }}
        }}";

        public static string UpdateUserRequest(this SecurityGraphQLProvider service, string selectedFields = null)
        => $@"mutation ($command: InputUpdateUserType!)
        {{
          securityResult: updateUser(command: $command)
          {{
            { selectedFields ?? SecurityResultFields }
          }}
        }}";

        public static string DeleteUserRequest(this SecurityGraphQLProvider service)
        => $@"mutation ($command: InputDeleteUserType!)
        {{
            deleteContact(command: $command)
        }}";

        public static string GetUserByIdRequest(this SecurityGraphQLProvider service, string id, string selectedFields = null)
            => $@"
        {{
            user(id:""{id}"")
            {{
            { selectedFields ?? AllUserFields }
            }}
        }}";

        public static string GetUserByNameRequest(this SecurityGraphQLProvider service, string name, string selectedFields = null)
            => $@"
        {{
            user(name:""{name}"")
            {{
            { selectedFields ?? AllUserFields }
            }}
        }}";

        public static string GetUserByEmailRequest(this SecurityGraphQLProvider service, string email, string selectedFields = null)
            => $@"
        {{
            user(email:""{email}"")
            {{
            { selectedFields ?? AllUserFields }
            }}
        }}";

        public static string GetUserByLoginRequest(this SecurityGraphQLProvider service, string loginProvider, string providerKey, string selectedFields = null)
            => $@"
        {{
            user(loginProvider:""{loginProvider}"" providerKey:""{providerKey}"")
            {{
            { selectedFields ?? AllUserFields }
            }}
        }}";

        public static string UpdateRoleRequest(this SecurityGraphQLProvider service, string selectedFields = null)
        => $@"mutation ($command: InputUpdateRoleType!)
        {{
          securityResult: updateRole(command: $command)
          {{
            { selectedFields ?? SecurityResultFields }
          }}
        }}";

        public static string GetRoleByIdRequest(this SecurityGraphQLProvider service, string id, string selectedFields = null)
            => $@"
        {{
            role(id:""{id}"")
            {{
            { selectedFields ?? AllRoleFields }
            }}
        }}";
    }
}
