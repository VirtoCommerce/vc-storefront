namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public static class TestEnvironment
    {
        public static string DefaultCultureName => "en-US";
        public static string DefaultCurrencyCode => "USD";
        public static string DefaultCurrencyName => "";
        public static string DefaultCurrencySymbol => "$";
        public static decimal DefaultExchangeRate => 0.0m;
        public static string CartEndpoint => "storefrontapi/cart";
        public static string CartItemsEndpoint => "storefrontapi/cart/items";
        public static string CartItemsCountEndpoint => "storefrontapi/cart/itemscount";
        public static string CartClearEndpoint => "storefrontapi/cart/clear";
        public static string LoginEndpoint => "account/login";
        public static string LogoutEndpoint => "account/logout";
    }
}
