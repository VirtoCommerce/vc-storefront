namespace VirtoCommerce.Storefront.Model.Security
{
    public static class SecurityConstants
    {
        public const string AnonymousUsername = "Anonymous";

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string IsAdministratorClaimType = "isadministrator";
            public const string AllowedStoresClaimType = "allowedstores";
            public const string OperatorUserNameClaimType = "operatorname";
            public const string OperatorUserIdClaimType = "operatornameidentifier";
            public const string CurrencyClaimType = "currency";
        }

        public static class Permissions
        {
            public const string CanResetCache = "cache:reset";
        }
    }
}
