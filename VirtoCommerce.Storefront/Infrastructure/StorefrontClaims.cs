namespace VirtoCommerce.Storefront.Infrastructure
{
    public static class StorefrontClaims
    {
       public const string AnonymousUsername = "Anonymous";

        public const string AllowedStoresClaimType = "http://schemas.virtocommerce.com/ws/2016/02/identity/claims/allowedstores";
        public const string OperatorUserNameClaimType = "http://schemas.virtocommerce.com/ws/2016/02/identity/claims/operatorname";
        public const string OperatorUserIdClaimType = "http://schemas.virtocommerce.com/ws/2016/02/identity/claims/operatornameidentifier";
        public const string CurrencyClaimType = "http://schemas.virtocommerce.com/ws/2016/02/identity/claims/currency";
    }
}
