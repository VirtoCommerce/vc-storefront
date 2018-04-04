namespace VirtoCommerce.Storefront.Model.Security
{
    public static class SecurityConstants
    {
        public const string AnonymousUsername = "Anonymous";

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string AllowedStoresClaimType = "allowedstores";
            public const string OperatorUserNameClaimType = "operatorname";
            public const string OperatorUserIdClaimType = "operatornameidentifier";
            public const string CurrencyClaimType = "currency";
        }

        public static class Roles
        {
            public const string Customer = "Customer";
            public const string Operator = "Operator";
            public const string Administrator = "Administrator";
            public const string OrganizationOwner = "OrganizationOwner";
            public const string OrganizationMember = "OrganizationMember";
        }

        public static class Permissions
        {
            public const string CanResetCache = "cache:reset";
        }
    }
}
