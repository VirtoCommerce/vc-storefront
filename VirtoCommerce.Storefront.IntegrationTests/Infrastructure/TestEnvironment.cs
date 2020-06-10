namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public static class TestEnvironment
    {
        public static string DefaultCultureName => "en-US";
        public static string DefaultCurrencyCode => "USD";
        public static string DefaultCurrencyName => "";
        public static string DefaultCurrencySymbol => "$";
        public static decimal DefaultExchangeRate => 0.0m;
        public static string GetCartEndPoint => "storefrontapi/cart";
    }
}
