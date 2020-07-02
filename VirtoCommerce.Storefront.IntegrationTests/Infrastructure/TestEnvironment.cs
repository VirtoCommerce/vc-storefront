using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

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
        public static string ItemPriceEndpoint => "storefrontapi/cart/items/price";
        public static string PaymentMethodsEndpoint => "storefrontapi/cart/paymentmethods";
        public static string CartPaymentPlanEndpoint => "storefrontapi/cart/paymentPlan";
        public static string CartPaymentEndpoint => "storefrontapi/cart/payments";
        public static string CartShipmentEndpoint => "storefrontapi/cart/shipments";

        public static string DeleteCartItemEndpoint(string lineItemId) =>
            $"storefrontapi/cart/items{(string.IsNullOrWhiteSpace(lineItemId) ? "" : $"?lineItemId={lineItemId}")}";
        public static string AddCouponEndpoint(string couponCode) =>
            $"storefrontapi/cart/coupons/{couponCode}";
        public static string RemoveCouponEndpoint(string couponCode) =>
            $"storefrontapi/cart/coupons{(string.IsNullOrWhiteSpace(couponCode) ? "" : $"?couponCode={couponCode}")}";
        public static string ShippingMethodsEndpoint(string shippmentId) =>
            $"storefrontapi/cart/shipments/{shippmentId}/shippingmethods";

        public static string GetListsWithProduct => "storefrontapi/lists/getlistswithproduct";
        public static string ListItemsEndpoint => "storefrontapi/lists/items";
        public static string ListSearchEndpoint => "storefrontapi/lists/search";

        public static string CreateList(string listName, string type) =>
            $"storefrontapi/lists/{listName}/{type}/create";
        public static string DeleteListByIds(string[] ids) =>
            $"storefrontapi/lists/deletelistsbyids?{ids.Select(x => $"listIds={x}").JoinWithoutWhitespaces("&")}";
        public static string GetList(string listName, string type) =>
            $"storefrontapi/lists/{listName}/{type}";
        public static string ClearList(string listName, string type) =>
            $"storefrontapi/lists/{listName}/{type}/clear";
        public static string MergeWithCurrentCart(string listName, string type) =>
            $"storefrontapi/lists/{listName}/{type}/mergewithcurrentcart";
        public static string DeleteListItemEndpoint(string lineItemId, string listName, string type) =>
            $"storefrontapi/lists/{listName}/{type}/items/{lineItemId}";
    }

    public static class Product
    {
        public static string Octocopter => "e7eee66223da43109502891b54bc33d3";
        public static string Quadcopter => "9cbd8f316e254a679ba34a900fccb076";
    }
}
