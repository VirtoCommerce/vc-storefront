namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public static class TestEnvironment
    {
        public static string CartEndpoint => "storefrontapi/cart";
        public static string CartItemsEndpoint => "storefrontapi/cart/items";
        public static string CartItemsCountEndpoint => "storefrontapi/cart/itemscount";
        public static string CartClearEndpoint => "storefrontapi/cart/clear";
        public static string LoginEndpoint => "account/login";
        public static string LogoutEndpoint => "account/logout";

    }

    public static class Product
    {
        public static string Octocopter => "e7eee66223da43109502891b54bc33d3";
        public static string Quadcopter => "9cbd8f316e254a679ba34a900fccb076";
    }
}
