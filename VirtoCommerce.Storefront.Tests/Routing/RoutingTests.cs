using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Tests.Routing.Infrastructure;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Routing
{
    public class RoutingTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        public enum CustomHttpMethod
        {
            Get,
            PostJson,
            PostForm,
            Put,
            Delete
        }

        public static readonly IReadOnlyCollection<object[]> RoutingData;

        static RoutingTests()
        {
            var routingData = new List<object[]>();

            // Cart API
            AddApiRequestRoutes(routingData, "ApiCart", "GetCart", CustomHttpMethod.Get, "storefrontapi/cart");
            AddApiRequestRoutes(routingData, "ApiCart", "GetCartItemsCount", CustomHttpMethod.Get, "storefrontapi/cart/itemscount");
            AddApiRequestRoutes(routingData, "ApiCart", "AddItemToCart", CustomHttpMethod.PostJson, "storefrontapi/cart/items");
            AddApiRequestRoutes(routingData, "ApiCart", "ChangeCartItem", CustomHttpMethod.Put, "storefrontapi/cart/items");
            AddApiRequestRoutes(routingData, "ApiCart", "ChangeCartItemPrice", CustomHttpMethod.Put, "storefrontapi/cart/items/price");

            AddApiRequestRoutes(routingData, "ApiCart", "RemoveCartItem", CustomHttpMethod.Delete, "storefrontapi/cart/items");
            AddApiRequestRoutes(routingData, "ApiCart", "ClearCart", CustomHttpMethod.PostJson, "storefrontapi/cart/clear");
            AddApiRequestRoutes(routingData, "ApiCart", "GetCartShipmentAvailShippingMethods", CustomHttpMethod.Get, "storefrontapi/cart/shipments/100500/shippingmethods");
            AddApiRequestRoutes(routingData, "ApiCart", "GetCartAvailPaymentMethods", CustomHttpMethod.Get, "storefrontapi/cart/paymentmethods");
            AddApiRequestRoutes(routingData, "ApiCart", "AddCartCoupon", CustomHttpMethod.PostJson, "storefrontapi/cart/coupons/TESTCOUPON");

            AddApiRequestRoutes(routingData, "ApiCart", "RemoveCartCoupon", CustomHttpMethod.Delete, "storefrontapi/cart/coupons");
            AddApiRequestRoutes(routingData, "ApiCart", "AddOrUpdateCartShipment", CustomHttpMethod.PostJson, "storefrontapi/cart/shipments", new Shipment());
            AddApiRequestRoutes(routingData, "ApiCart", "CreateOrder", CustomHttpMethod.PostJson, "storefrontapi/cart/createorder");
            AddApiRequestRoutes(routingData, "ApiCart", "AddOrUpdateCartPaymentPlan", CustomHttpMethod.PostJson, "storefrontapi/cart/paymentPlan");
            AddApiRequestRoutes(routingData, "ApiCart", "DeleteCartPaymentPlan", CustomHttpMethod.Delete, "storefrontapi/cart/paymentPlan");

            AddApiRequestRoutes(routingData, "ApiCart", "UpdateCartComment", CustomHttpMethod.Put, "storefrontapi/cart/comment");

            // API lists
            AddApiRequestRoutes(routingData, "ApiLists", "GetListByName", CustomHttpMethod.Get, "storefrontapi/lists/someListName/someType");
            AddApiRequestRoutes(routingData, "ApiLists", "AddItemToList", CustomHttpMethod.PostJson, "storefrontapi/lists/items");
            AddApiRequestRoutes(routingData, "ApiLists", "RemoveItemFromList", CustomHttpMethod.Delete, "storefrontapi/lists/someListName/someType/items/12345");
            AddApiRequestRoutes(routingData, "ApiLists", "GetListsWithProduct", CustomHttpMethod.PostJson, "storefrontapi/lists/getlistswithproduct");
            AddApiRequestRoutes(routingData, "ApiLists", "SearchLists", CustomHttpMethod.PostJson, "storefrontapi/lists/search");

            AddApiRequestRoutes(routingData, "ApiLists", "CreateList", CustomHttpMethod.PostJson, "storefrontapi/lists/someListName/someType/create");
            AddApiRequestRoutes(routingData, "ApiLists", "DeleteListsByIds", CustomHttpMethod.Delete, "storefrontapi/lists/deletelistsbyids");
            AddApiRequestRoutes(routingData, "ApiLists", "MergeWithCurrentCart", CustomHttpMethod.PostJson, "storefrontapi/lists/someListName/someType/mergewithcurrentcart");

            // Catalog API
            AddApiRequestRoutes(routingData, "ApiCatalog", "SearchProducts", CustomHttpMethod.PostJson, "storefrontapi/catalog/search");
            AddApiRequestRoutes(routingData, "ApiCatalog", "GetProductsByIds", CustomHttpMethod.Get, "storefrontapi/products");
            AddApiRequestRoutes(routingData, "ApiCatalog", "SearchCategories", CustomHttpMethod.PostJson, "storefrontapi/categories/search");
            AddApiRequestRoutes(routingData, "ApiCatalog", "GetCategoriesByIds", CustomHttpMethod.Get, "storefrontapi/categories");

            // Common storefront API
            AddApiRequestRoutes(routingData, "ApiCommon", "GetCountries", CustomHttpMethod.Get, "storefrontapi/countries");
            AddApiRequestRoutes(routingData, "ApiCommon", "GetCountryRegions", CustomHttpMethod.Get, "storefrontapi/countries/US/regions");
            AddApiRequestRoutes(routingData, "ApiCommon", "Feedback", CustomHttpMethod.PostJson, "storefrontapi/feedback");

            // Pricing API
            AddApiRequestRoutes(routingData, "ApiPricing", "GetActualProductPrices", CustomHttpMethod.PostJson, "storefrontapi/pricing/actualprices");

            // Marketing API
            AddApiRequestRoutes(routingData, "ApiMarketing", "GetDynamicContent", CustomHttpMethod.Get, "storefrontapi/marketing/dynamiccontent/PlaceName");

            // Recommendations API
            AddApiRequestRoutes(routingData, "ApiRecommendations", "GetRecommendations", CustomHttpMethod.PostJson, "storefrontapi/recommendations", new RecommendationEvalContext());

            // Account API
            AddApiRequestRoutes(routingData, "ApiAccount", "GetCurrentUser", CustomHttpMethod.Get, "storefrontapi/account");
            AddApiRequestRoutes(routingData, "ApiAccount", "GetUserById", CustomHttpMethod.Get, "storefrontapi/account/9000");
            AddApiRequestRoutes(routingData, "ApiAccount", "UpdateAccount", CustomHttpMethod.PostJson, "storefrontapi/account");
            AddApiRequestRoutes(routingData, "ApiAccount", "ChangePassword", CustomHttpMethod.PostJson, "storefrontapi/account/password");
            AddApiRequestRoutes(routingData, "ApiAccount", "UpdateAddresses", CustomHttpMethod.PostJson, "storefrontapi/account/addresses");

            AddApiRequestRoutes(routingData, "ApiAccount", "SearchOrganizationUsersAsync", CustomHttpMethod.PostJson, "storefrontapi/account/organization/users/search");
            AddApiRequestRoutes(routingData, "ApiAccount", "GetCustomerOrganization", CustomHttpMethod.Get, "storefrontapi/account/organization/current");
            AddApiRequestRoutes(routingData, "ApiAccount", "CreateUserInvitation", CustomHttpMethod.PostJson, "storefrontapi/account/invitation");
            AddApiRequestRoutes(routingData, "ApiAccount", "UpdateOrganization", CustomHttpMethod.Put, "storefrontapi/account/organization");
            AddApiRequestRoutes(routingData, "ApiAccount", "RegisterOrganization", CustomHttpMethod.PostJson, "storefrontapi/account/organization");

            AddApiRequestRoutes(routingData, "ApiAccount", "RegisterUser", CustomHttpMethod.PostJson, "storefrontapi/account/user");
            AddApiRequestRoutes(routingData, "ApiAccount", "LockUser", CustomHttpMethod.PostJson, "storefrontapi/account/9000/lock");
            AddApiRequestRoutes(routingData, "ApiAccount", "UnlockUser", CustomHttpMethod.PostJson, "storefrontapi/account/9000/unlock");
            AddApiRequestRoutes(routingData, "ApiAccount", "DeleteUser", CustomHttpMethod.Delete, "storefrontapi/account/9000");

            // Quote requests API
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "QuoteSearch", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/search");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "GetItemsCount", CustomHttpMethod.Get, "storefrontapi/quoterequests/111/itemscount");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "Get", CustomHttpMethod.Get, "storefrontapi/quoterequests/111");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "GetCurrent", CustomHttpMethod.Get, "storefrontapi/quoterequest/current");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "AddItem", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/current/items");

            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "RemoveItem", CustomHttpMethod.Delete, "storefrontapi/quoterequests/111/items/222222");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "Update", CustomHttpMethod.Put, "storefrontapi/quoterequests/111");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "Submit", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/submit");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "Reject", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/reject");
            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "CalculateTotals", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/totals");

            AddApiRequestRoutes(routingData, "ApiQuoteRequest", "Confirm", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/confirm");

            // Order API
            AddApiRequestRoutes(routingData, "ApiOrder", "SearchCustomerOrders", CustomHttpMethod.PostJson, "storefrontapi/orders/search");
            AddApiRequestRoutes(routingData, "ApiOrder", "GetCustomerOrder", CustomHttpMethod.Get, "storefrontapi/orders/TORD1234567");
            AddApiRequestRoutes(routingData, "ApiOrder", "GetNewPaymentData", CustomHttpMethod.Get, "storefrontapi/orders/TORD1234567/newpaymentdata");
            AddApiRequestRoutes(routingData, "ApiOrder", "CancelPayment", CustomHttpMethod.PostJson, "storefrontapi/orders/TORD1234567/payments/1112345/cancel");
            AddApiRequestRoutes(routingData, "ApiOrder", "ProcessOrderPayment", CustomHttpMethod.PostJson, "storefrontapi/orders/TORD1234567/payments/1112345/process");

            AddApiRequestRoutes(routingData, "ApiOrder", "AddOrUpdateOrderPayment", CustomHttpMethod.PostJson, "storefrontapi/orders/TORD1234567/payments");
            AddApiRequestRoutes(routingData, "ApiOrder", "GetInvoicePdf", CustomHttpMethod.Get, "storefrontapi/orders/TORD1234567/invoice");

            // Subscriptions API
            AddApiRequestRoutes(routingData, "ApiSubscription", "SearchCustomerSubscriptions", CustomHttpMethod.PostJson, "storefrontapi/subscriptions/search");
            AddApiRequestRoutes(routingData, "ApiSubscription", "GetCustomerSubscription", CustomHttpMethod.Get, "storefrontapi/subscriptions/123");
            AddApiRequestRoutes(routingData, "ApiSubscription", "CancelSubscription", CustomHttpMethod.PostJson, "storefrontapi/subscriptions/123/cancel");

            // Blog API
            AddApiRequestRoutes(routingData, "ApiBlog", "Search", CustomHttpMethod.PostJson, "storefrontapi/blog/generic_blog/search");

            // Inventory API
            AddApiRequestRoutes(routingData, "ApiInventory", "SearchFulfillmentCenters", CustomHttpMethod.PostJson, "storefrontapi/fulfillmentcenters/search");

            //Errors
            AddRegularRequestRoutes(routingData, "Error", "AccessDenied", CustomHttpMethod.Get, "error/AccessDenied");
            AddRegularRequestRoutes(routingData, "Error", "Error", CustomHttpMethod.Get, "error/500");
            AddRegularRequestRoutes(routingData, "Error", "Error", CustomHttpMethod.Get, "error/404");

            // Account
            AddRegularRequestRoutes(routingData, "Account", "GetAccount", CustomHttpMethod.Get, "account");
            AddRegularRequestRoutes(routingData, "Account", "GetOrderDetails", CustomHttpMethod.Get, "account/order/{number}");
            AddRegularRequestRoutes(routingData, "Account", "GetAddresses", CustomHttpMethod.Get, "account/addresses");
            AddRegularRequestRoutes(routingData, "Account", "Register", CustomHttpMethod.Get, "account/register");
            AddRegularRequestRoutes(routingData, "Account", "Login", CustomHttpMethod.Get, "account/login");

            AddRegularRequestRoutes(routingData, "Account", "Login", CustomHttpMethod.PostForm, "account/login", new Login());
            AddRegularRequestRoutes(routingData, "Account", "Logout", CustomHttpMethod.Get, "account/logout");
            AddRegularRequestRoutes(routingData, "Account", "ForgotPassword", CustomHttpMethod.Get, "account/forgotpassword");
            AddRegularRequestRoutes(routingData, "Account", "ForgotPassword", CustomHttpMethod.PostForm, "account/forgotpassword", new ForgotPassword());
            AddRegularRequestRoutes(routingData, "Account", "ResetPassword", CustomHttpMethod.Get, "account/resetpassword");

            AddRegularRequestRoutes(routingData, "Account", "ResetPassword", CustomHttpMethod.PostForm, "account/resetpassword", new ResetPassword());
            AddRegularRequestRoutes(routingData, "Account", "ChangePassword", CustomHttpMethod.PostForm, "account/password", new ChangePassword());
            AddRegularRequestRoutes(routingData, "Account", "ExternalLogin", CustomHttpMethod.Get, "account/externallogin");
            AddRegularRequestRoutes(routingData, "Account", "ExternalLoginCallback", CustomHttpMethod.Get, "account/externallogincallback");
            AddRegularRequestRoutes(routingData, "Account", "ImpersonateUser", CustomHttpMethod.Get, "account/impersonate/111");

            AddRegularRequestRoutes(routingData, "Account", "ConfirmEmail", CustomHttpMethod.Get, "account/confirmemail");
            AddRegularRequestRoutes(routingData, "Account", "ConfirmInvitation", CustomHttpMethod.Get, "account/confirminvitation");
            AddRegularRequestRoutes(routingData, "Account", "ForgotLogin", CustomHttpMethod.Get, "account/forgotlogin");
            AddRegularRequestRoutes(routingData, "Account", "ForgotLogin", CustomHttpMethod.PostForm, "account/forgotlogin", new ForgotPassword());

            // Cart
            AddRegularRequestRoutes(routingData, "Cart", "Index", CustomHttpMethod.Get, "cart");
            AddRegularRequestRoutes(routingData, "Cart", "Checkout", CustomHttpMethod.Get, "cart/checkout");
            AddRegularRequestRoutes(routingData, "Cart", "ExternalPaymentCallback", CustomHttpMethod.Get, "cart/externalpaymentcallback");
            AddRegularRequestRoutes(routingData, "Cart", "ExternalPaymentCallback", CustomHttpMethod.PostJson, "cart/externalpaymentcallback");
            AddRegularRequestRoutes(routingData, "Cart", "Thanks", CustomHttpMethod.Get, "cart/thanks/TORD1112345");

            AddRegularRequestRoutes(routingData, "Cart", "PaymentForm", CustomHttpMethod.Get, "cart/checkout/paymentform");

            // Cart (Shopify compatible)
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "Cart", CustomHttpMethod.PostJson, "cart?updates=1234,5678&update=90");
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "CartJs", CustomHttpMethod.Get, "cart.js");
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "Add", CustomHttpMethod.PostJson, "cart/add");
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "AddJs", CustomHttpMethod.PostJson, "cart/add.js");
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "Change", CustomHttpMethod.Get, "cart/change");

            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "ChangeJs", CustomHttpMethod.PostJson, "cart/change.js");
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "Clear", CustomHttpMethod.Get, "cart/clear");
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "ClearJs", CustomHttpMethod.PostJson, "cart/clear.js");
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "UpdateJs", CustomHttpMethod.PostJson, "cart/update.js");

            // Collections (Shopify compatible)
            AddRegularRequestRoutes(routingData, "ShopifyCompatibility", "Collections", CustomHttpMethod.Get, "collections");

            // QuoteRequest
            AddRegularRequestRoutes(routingData, "QuoteRequest", "CurrentQuoteRequest", CustomHttpMethod.Get, "quoterequest");
            AddRegularRequestRoutes(routingData, "QuoteRequest", "QuoteRequests", CustomHttpMethod.Get, "account/quoterequests");
            AddRegularRequestRoutes(routingData, "QuoteRequest", "QuoteRequestByNumber", CustomHttpMethod.Get, "quoterequest/111");

            // Bulk order
            AddRegularRequestRoutes(routingData, "BulkOrder", "Index", CustomHttpMethod.Get, "bulkorder");
            AddRegularRequestRoutes(routingData, "BulkOrder", "AddFieldItems", CustomHttpMethod.PostJson, "bulkorder/addfielditems");
            AddRegularRequestRoutes(routingData, "BulkOrder", "AddCsvItems", CustomHttpMethod.PostJson, "bulkorder/addcsvitems");

            // CatalogSearch
            AddRegularRequestRoutes(routingData, "CatalogSearch", "CategoryBrowsing", CustomHttpMethod.Get, "search/112");
            AddRegularRequestRoutes(routingData, "CatalogSearch", "SearchProducts", CustomHttpMethod.Get, "search");

            // Common
            AddRegularRequestRoutes(routingData, "Common", "SetCurrency", CustomHttpMethod.Get, "common/setcurrency/USD");
            AddRegularRequestRoutes(routingData, "Common", "ContactForm", CustomHttpMethod.PostForm, "contact/page.contact", new ContactForm());
            AddRegularRequestRoutes(routingData, "Common", "Maintenance", CustomHttpMethod.Get, "maintenance");
            AddRegularRequestRoutes(routingData, "Common", "Maintenance", CustomHttpMethod.Get, "common/maintenance");
            AddRegularRequestRoutes(routingData, "Common", "ResetCache", CustomHttpMethod.Get, "common/resetcache");

            AddRegularRequestRoutes(routingData, "Common", "NoTheme", CustomHttpMethod.Get, "common/notheme");

            // Sitemap
            AddRegularRequestRoutes(routingData, "Sitemap", "GetSitemapIndex", CustomHttpMethod.Get, "sitemap.xml");
            AddRegularRequestRoutes(routingData, "Sitemap", "GetSitemap", CustomHttpMethod.Get, "sitemap/somePath");

            // Category routes
            AddRegularRequestRoutes(routingData, "CatalogSearch", "CategoryBrowsing", CustomHttpMethod.Get, "category/1234");

            // Product routes
            AddRegularRequestRoutes(routingData, "Product", "ProductDetails", CustomHttpMethod.Get, "product/111");
            AddRegularRequestRoutes(routingData, "Product", "Compare", CustomHttpMethod.Get, "compare");

            // Vendor routes
            AddRegularRequestRoutes(routingData, "Vendor", "VendorDetails", CustomHttpMethod.Get, "vendor/222");

            // Inventory routes
            AddRegularRequestRoutes(routingData, "Inventory", "FulfillmentCenterDetails", CustomHttpMethod.Get, "fulfillmentcenter/12");

            // Assets
            AddRegularRequestRoutes(routingData, "Asset", "GetThemeLocalizationJson", CustomHttpMethod.Get, "themes/localization.json");
            AddRegularRequestRoutes(routingData, "Asset", "GetThemeAssets", CustomHttpMethod.Get, "themes/assets/some.theme.asset");
            AddRegularRequestRoutes(routingData, "Asset", "GetStaticContentAssets", CustomHttpMethod.Get, "assets/some.static.asset");

            // Static content (no cms)
            AddRegularRequestRoutes(routingData, "StaticContent", "GetContentPageByName", CustomHttpMethod.Get, "pages/TestPage");

            //Blog
            AddRegularRequestRoutes(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blog");
            AddRegularRequestRoutes(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blog/category/1");
            AddRegularRequestRoutes(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blog/tag/phones");
            AddRegularRequestRoutes(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blogs/1");
            AddRegularRequestRoutes(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blogs/generic_blog/category/posts");
            AddRegularRequestRoutes(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blogs/generic_blog/tag/shopping");

            AddRegularRequestRoutes(routingData, "StaticContent", "Search", CustomHttpMethod.PostJson, "content/search");

            AddRegularRequestRoutes(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blogs/generic_blog/rss");
            AddRegularRequestRoutes(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blogs/generic_blog/feed");
            AddRegularRequestRoutes(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blog/rss");
            AddRegularRequestRoutes(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blog/feed");

            AddRegularRequestRoutes(routingData, "Home", "Index", CustomHttpMethod.Get, "");

            RoutingData = routingData;
        }

        private static void AddRegularRequestRoutes(ICollection<object[]> routingData, string expectedControllerName,
            string expectedActionName, CustomHttpMethod httpMethod, string baseUrl, object objectToPost = null)
        {
            var expectedControllerMethodName = BuildStorefrontControllerMethodName(expectedControllerName, expectedActionName);
            AddRequestRouteVariations(routingData, expectedControllerMethodName, httpMethod, baseUrl, objectToPost);
        }

        protected static string BuildStorefrontControllerMethodName(string controllerName, string methodName)
        {
            return $"VirtoCommerce.Storefront.Controllers.{controllerName}Controller.{methodName} " +
                   "(VirtoCommerce.Storefront)";
        }

        private static void AddApiRequestRoutes(ICollection<object[]> routingData, string expectedControllerName,
            string expectedActionName, CustomHttpMethod httpMethod, string baseUrl, object objectToPost = null)
        {
            var expectedControllerMethodName = BuildStorefrontApiControllerMethodName(expectedControllerName, expectedActionName);
            AddRequestRouteVariations(routingData, expectedControllerMethodName, httpMethod, baseUrl, objectToPost);
        }

        protected static string BuildStorefrontApiControllerMethodName(string controllerName, string methodName)
        {
            return $"VirtoCommerce.Storefront.Controllers.Api.{controllerName}Controller.{methodName} " +
                   "(VirtoCommerce.Storefront)";
        }

        private static void AddRequestRouteVariations(ICollection<object[]> routingData, string expectedControllerMethodName,
            CustomHttpMethod httpMethod, string baseUrl, object objectToPost)
        {
            routingData.Add(new object[] { httpMethod, baseUrl, expectedControllerMethodName, objectToPost });
            routingData.Add(new object[] { httpMethod, $"en-US/{baseUrl}", expectedControllerMethodName, objectToPost });
            routingData.Add(new object[] { httpMethod, $"Electronics/{baseUrl}", expectedControllerMethodName, objectToPost });
            routingData.Add(new object[] { httpMethod, $"Electronics/en-US/{baseUrl}", expectedControllerMethodName, objectToPost });
        }


        public RoutingTests(WebApplicationFactory<Startup> factory)
        {
            Client = factory
                .WithWebHostBuilder(
                    builder => builder.ConfigureServices(ConfigureServices)
                ).CreateClient();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                options =>
                {
                    options.Filters.Add<RoutingTestingActionFilter>();
                    options.Filters.Add(typeof(AllowAnonymousFilter), 0);
                });

            var dummyAntiforgeryDescriptor = ServiceDescriptor.Singleton<IAntiforgery, DummyAntiforgery>();
            services.Add(dummyAntiforgeryDescriptor);
        }

        public HttpClient Client { get; }


        [Theory]
        [MemberData(nameof(RoutingData))]
        public async Task TestRouting(CustomHttpMethod method, string url, string expectedControllerMethodName, object objectToPost)
        {
            // Act & Assert
            var response = await PerformSendingRequest(method, url, objectToPost);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var routingData = JsonConvert.DeserializeObject<RoutingDataResult>(responseContent);
            Assert.NotNull(routingData);

            Assert.Equal(expectedControllerMethodName, routingData.ControllerMethodName);
        }

        private async Task<HttpResponseMessage> PerformSendingRequest(CustomHttpMethod method, string url, object objectToPost)
        {
            switch (method)
            {
                case CustomHttpMethod.PostJson:
                    return await Client.PostAsJsonAsync(url, objectToPost);

                case CustomHttpMethod.PostForm:
                    return await Client.PostAsync(url, new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()));

                case CustomHttpMethod.Get:
                    return await Client.GetAsync(url);

                case CustomHttpMethod.Put:
                    return await Client.PutAsJsonAsync(url, objectToPost);

                case CustomHttpMethod.Delete:
                    return await Client.DeleteAsync(url);

                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
