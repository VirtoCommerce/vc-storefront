using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Security
{
    public static class SecurityConstants
    {
        public const string AnonymousUsername = "Anonymous";

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string OperatorUserNameClaimType = "operatorname";
            public const string OperatorUserIdClaimType = "operatornameidentifier";
            public const string CurrencyClaimType = "currency";
        }

        public static class Roles
        {
            public const string Customer = "Customer";
            public const string Operator = "Operator";
            public const string Administrator = "Administrator";
            public static readonly Role OrganizationMaintainer = new Role
            {
                Id = "org-maintainer",
                Name = "Organization maintainer",
                Permissions = new string[] { Permissions.CanViewUsers, Permissions.CanCreateUsers, Permissions.CanSeeOrganizationDetail, Permissions.CanDeleteUsers, Permissions.CanEditOrganization, Permissions.CanEditUsers, Permissions.CanInviteUsers }
            };
            public static readonly Role OrganizationEmployee = new Role
            {
                Id = "org-employee",
                Name = "Organization employee",
                Permissions = new string[] { Permissions.CanSeeOrganizationDetail } 
            };

            public static readonly IEnumerable<Role> AllRoles = new[] { OrganizationMaintainer, OrganizationEmployee };
        }

        public static class Permissions
        {
            public const string CanResetCache = "cache:reset";
            public const string CanSeeOrganizationDetail = "storefront:organization:view";
            public const string CanEditOrganization = "storefront:organization:edit";
            public const string CanInviteUsers = "storefront:user:invite";
            public const string CanCreateUsers = "storefront:user:create";
            public const string CanEditUsers = "storefront:user:edit";
            public const string CanDeleteUsers = "storefront:user:delete";
            public const string CanViewUsers = "storefront:user:view";

            public static readonly IEnumerable<string> AllPermissions = new [] { CanViewUsers, CanResetCache, CanSeeOrganizationDetail, CanEditOrganization, CanInviteUsers, CanEditUsers, CanDeleteUsers, CanCreateUsers };
        }
    }
}
