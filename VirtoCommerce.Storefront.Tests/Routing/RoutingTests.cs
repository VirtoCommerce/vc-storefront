using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        /// <summary>
        /// HTTP methods merged with POST body type. Added to simplify contents of test data in RoutingData.
        /// </summary>
        public enum CustomHttpMethod
        {
            Get,
            PostJson,
            PostForm,
            Put,
            Delete
        }

        private static readonly Currency TestCurrency = new Currency(new Language("en-US"), "XPT");
        private static readonly Money EmptyMoney = new Money(0.0m, TestCurrency);

        private static readonly Payment EmptyPayment = new Payment(TestCurrency)
        {
            Amount = EmptyMoney,
            DiscountAmount = EmptyMoney,
            DiscountAmountWithTax = EmptyMoney,
            Price = EmptyMoney,
            PriceWithTax = EmptyMoney,
            TaxTotal = EmptyMoney,
            Total = EmptyMoney,
            TotalWithTax = EmptyMoney
        };

        private static readonly Shipment EmptyShipment = new Shipment()
        {
            Currency = TestCurrency,
            DiscountAmount = EmptyMoney,
            DiscountAmountWithTax = EmptyMoney,
            Price = EmptyMoney,
            TaxTotal = EmptyMoney,
            TotalWithTax = EmptyMoney,
            Total = EmptyMoney,
            PriceWithTax = EmptyMoney
        };

        private static readonly Dictionary<string, string> EmptyFormData = new Dictionary<string, string>();

        public static readonly IReadOnlyCollection<object[]> RoutingData;

        static RoutingTests()
        {
            var routingData = new List<object[]>();

            // Cart API
            AddApiRequestRoute(routingData, "ApiCart", "GetCart", CustomHttpMethod.Get, "storefrontapi/cart");
            AddApiRequestRoute(routingData, "ApiCart", "GetCartItemsCount", CustomHttpMethod.Get, "storefrontapi/cart/itemscount");
            AddApiRequestRoute(routingData, "ApiCart", "AddItemToCart", CustomHttpMethod.PostJson, "storefrontapi/cart/items");
            AddApiRequestRoute(routingData, "ApiCart", "ChangeCartItem", CustomHttpMethod.Put, "storefrontapi/cart/items");
            AddApiRequestRoute(routingData, "ApiCart", "ChangeCartItemPrice", CustomHttpMethod.Put, "storefrontapi/cart/items/price");

            AddApiRequestRoute(routingData, "ApiCart", "RemoveCartItem", CustomHttpMethod.Delete, "storefrontapi/cart/items");
            AddApiRequestRoute(routingData, "ApiCart", "ClearCart", CustomHttpMethod.PostJson, "storefrontapi/cart/clear");
            AddApiRequestRoute(routingData, "ApiCart", "GetCartShipmentAvailShippingMethods", CustomHttpMethod.Get, "storefrontapi/cart/shipments/100500/shippingmethods");
            AddApiRequestRoute(routingData, "ApiCart", "GetCartAvailPaymentMethods", CustomHttpMethod.Get, "storefrontapi/cart/paymentmethods");
            AddApiRequestRoute(routingData, "ApiCart", "AddCartCoupon", CustomHttpMethod.PostJson, "storefrontapi/cart/coupons/TESTCOUPON");

            AddApiRequestRoute(routingData, "ApiCart", "RemoveCartCoupon", CustomHttpMethod.Delete, "storefrontapi/cart/coupons");
            AddApiRequestRoute(routingData, "ApiCart", "AddOrUpdateCartShipment", CustomHttpMethod.PostJson, "storefrontapi/cart/shipments", EmptyShipment);
            AddApiRequestRoute(routingData, "ApiCart", "AddOrUpdateCartPayment", CustomHttpMethod.PostJson, "storefrontapi/cart/payments", EmptyPayment);
            AddApiRequestRoute(routingData, "ApiCart", "CreateOrder", CustomHttpMethod.PostJson, "storefrontapi/cart/createorder");
            AddApiRequestRoute(routingData, "ApiCart", "AddOrUpdateCartPaymentPlan", CustomHttpMethod.PostJson, "storefrontapi/cart/paymentPlan");

            AddApiRequestRoute(routingData, "ApiCart", "DeleteCartPaymentPlan", CustomHttpMethod.Delete, "storefrontapi/cart/paymentPlan");
            AddApiRequestRoute(routingData, "ApiCart", "UpdateCartComment", CustomHttpMethod.Put, "storefrontapi/cart/comment");

            // API lists
            AddApiRequestRoute(routingData, "ApiLists", "GetListByName", CustomHttpMethod.Get, "storefrontapi/lists/someListName/someType");
            AddApiRequestRoute(routingData, "ApiLists", "AddItemToList", CustomHttpMethod.PostJson, "storefrontapi/lists/items");
            AddApiRequestRoute(routingData, "ApiLists", "RemoveItemFromList", CustomHttpMethod.Delete, "storefrontapi/lists/someListName/someType/items/12345");
            AddApiRequestRoute(routingData, "ApiLists", "GetListsWithProduct", CustomHttpMethod.PostJson, "storefrontapi/lists/getlistswithproduct");
            AddApiRequestRoute(routingData, "ApiLists", "SearchLists", CustomHttpMethod.PostJson, "storefrontapi/lists/search");

            AddApiRequestRoute(routingData, "ApiLists", "CreateList", CustomHttpMethod.PostJson, "storefrontapi/lists/someListName/someType/create");
            AddApiRequestRoute(routingData, "ApiLists", "DeleteListsByIds", CustomHttpMethod.Delete, "storefrontapi/lists/deletelistsbyids");
            AddApiRequestRoute(routingData, "ApiLists", "MergeWithCurrentCart", CustomHttpMethod.PostJson, "storefrontapi/lists/someListName/someType/mergewithcurrentcart");

            // Catalog API
            AddApiRequestRoute(routingData, "ApiCatalog", "SearchProducts", CustomHttpMethod.PostJson, "storefrontapi/catalog/search");
            AddApiRequestRoute(routingData, "ApiCatalog", "GetProductsByIds", CustomHttpMethod.Get, "storefrontapi/products");
            AddApiRequestRoute(routingData, "ApiCatalog", "SearchCategories", CustomHttpMethod.PostJson, "storefrontapi/categories/search");
            AddApiRequestRoute(routingData, "ApiCatalog", "GetCategoriesByIds", CustomHttpMethod.Get, "storefrontapi/categories");

            // Common storefront API
            AddApiRequestRoute(routingData, "ApiCommon", "GetCountries", CustomHttpMethod.Get, "storefrontapi/countries");
            AddApiRequestRoute(routingData, "ApiCommon", "GetCountryRegions", CustomHttpMethod.Get, "storefrontapi/countries/US/regions");
            AddApiRequestRoute(routingData, "ApiCommon", "Feedback", CustomHttpMethod.PostJson, "storefrontapi/feedback");

            // Pricing API
            AddApiRequestRoute(routingData, "ApiPricing", "GetActualProductPrices", CustomHttpMethod.PostJson, "storefrontapi/pricing/actualprices");

            // Marketing API
            AddApiRequestRoute(routingData, "ApiMarketing", "GetDynamicContent", CustomHttpMethod.Get, "storefrontapi/marketing/dynamiccontent/PlaceName");

            // Recommendations API
            AddApiRequestRoute(routingData, "ApiRecommendations", "GetRecommendations", CustomHttpMethod.PostJson, "storefrontapi/recommendations", new RecommendationEvalContext());

            // Account API
            AddApiRequestRoute(routingData, "ApiAccount", "GetCurrentUser", CustomHttpMethod.Get, "storefrontapi/account");
            AddApiRequestRoute(routingData, "ApiAccount", "GetUserById", CustomHttpMethod.Get, "storefrontapi/account/9000");
            AddApiRequestRoute(routingData, "ApiAccount", "UpdateAccount", CustomHttpMethod.PostJson, "storefrontapi/account");
            AddApiRequestRoute(routingData, "ApiAccount", "ChangePassword", CustomHttpMethod.PostJson, "storefrontapi/account/password");
            AddApiRequestRoute(routingData, "ApiAccount", "UpdateAddresses", CustomHttpMethod.PostJson, "storefrontapi/account/addresses");

            AddApiRequestRoute(routingData, "ApiAccount", "SearchOrganizationUsersAsync", CustomHttpMethod.PostJson, "storefrontapi/account/organization/users/search");
            AddApiRequestRoute(routingData, "ApiAccount", "GetCustomerOrganization", CustomHttpMethod.Get, "storefrontapi/account/organization/current");
            AddApiRequestRoute(routingData, "ApiAccount", "CreateUserInvitation", CustomHttpMethod.PostJson, "storefrontapi/account/invitation");
            AddApiRequestRoute(routingData, "ApiAccount", "UpdateOrganization", CustomHttpMethod.Put, "storefrontapi/account/organization");
            AddApiRequestRoute(routingData, "ApiAccount", "RegisterOrganization", CustomHttpMethod.PostJson, "storefrontapi/account/organization");

            AddApiRequestRoute(routingData, "ApiAccount", "RegisterUser", CustomHttpMethod.PostJson, "storefrontapi/account/user");
            AddApiRequestRoute(routingData, "ApiAccount", "LockUser", CustomHttpMethod.PostJson, "storefrontapi/account/9000/lock");
            AddApiRequestRoute(routingData, "ApiAccount", "UnlockUser", CustomHttpMethod.PostJson, "storefrontapi/account/9000/unlock");
            AddApiRequestRoute(routingData, "ApiAccount", "DeleteUser", CustomHttpMethod.Delete, "storefrontapi/account/9000");

            // Quote requests API
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "QuoteSearch", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/search");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "GetItemsCount", CustomHttpMethod.Get, "storefrontapi/quoterequests/111/itemscount");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "Get", CustomHttpMethod.Get, "storefrontapi/quoterequests/111");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "GetCurrent", CustomHttpMethod.Get, "storefrontapi/quoterequest/current");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "AddItem", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/current/items");

            AddApiRequestRoute(routingData, "ApiQuoteRequest", "RemoveItem", CustomHttpMethod.Delete, "storefrontapi/quoterequests/111/items/222222");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "Update", CustomHttpMethod.Put, "storefrontapi/quoterequests/111");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "Submit", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/submit");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "Reject", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/reject");
            AddApiRequestRoute(routingData, "ApiQuoteRequest", "CalculateTotals", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/totals");

            AddApiRequestRoute(routingData, "ApiQuoteRequest", "Confirm", CustomHttpMethod.PostJson, "storefrontapi/quoterequests/111/confirm");

            // Order API
            AddApiRequestRoute(routingData, "ApiOrder", "SearchCustomerOrders", CustomHttpMethod.PostJson, "storefrontapi/orders/search");
            AddApiRequestRoute(routingData, "ApiOrder", "GetCustomerOrder", CustomHttpMethod.Get, "storefrontapi/orders/TORD1234567");
            AddApiRequestRoute(routingData, "ApiOrder", "GetNewPaymentData", CustomHttpMethod.Get, "storefrontapi/orders/TORD1234567/newpaymentdata");
            AddApiRequestRoute(routingData, "ApiOrder", "CancelPayment", CustomHttpMethod.PostJson, "storefrontapi/orders/TORD1234567/payments/1112345/cancel");
            AddApiRequestRoute(routingData, "ApiOrder", "ProcessOrderPayment", CustomHttpMethod.PostJson, "storefrontapi/orders/TORD1234567/payments/1112345/process");

            AddApiRequestRoute(routingData, "ApiOrder", "AddOrUpdateOrderPayment", CustomHttpMethod.PostJson, "storefrontapi/orders/TORD1234567/payments");
            AddApiRequestRoute(routingData, "ApiOrder", "GetInvoicePdf", CustomHttpMethod.Get, "storefrontapi/orders/TORD1234567/invoice");

            // Subscriptions API
            AddApiRequestRoute(routingData, "ApiSubscription", "SearchCustomerSubscriptions", CustomHttpMethod.PostJson, "storefrontapi/subscriptions/search");
            AddApiRequestRoute(routingData, "ApiSubscription", "GetCustomerSubscription", CustomHttpMethod.Get, "storefrontapi/subscriptions/123");
            AddApiRequestRoute(routingData, "ApiSubscription", "CancelSubscription", CustomHttpMethod.PostJson, "storefrontapi/subscriptions/123/cancel");

            // Blog API
            AddApiRequestRoute(routingData, "ApiBlog", "Search", CustomHttpMethod.PostJson, "storefrontapi/blog/generic_blog/search");

            // Inventory API
            AddApiRequestRoute(routingData, "ApiInventory", "SearchFulfillmentCenters", CustomHttpMethod.PostJson, "storefrontapi/fulfillmentcenters/search");

            //Errors
            AddErrorRequestRoutes(routingData, "AccessDenied", "error/AccessDenied");
            AddErrorRequestRoutes(routingData, "Error", "error/500");
            AddErrorRequestRoutes(routingData, "Error", "error/404");
            AddErrorRequestRoutes(routingData, "Error", "error/403");
            AddErrorRequestRoutes(routingData, "Error", "error/401");
            AddErrorRequestRoutes(routingData, "Error", "error/400");

            // Account
            AddRegularRequestRoute(routingData, "Account", "GetAccount", CustomHttpMethod.Get, "account");
            AddRegularRequestRoute(routingData, "Account", "GetOrderDetails", CustomHttpMethod.Get, "account/order/{number}");
            AddRegularRequestRoute(routingData, "Account", "GetAddresses", CustomHttpMethod.Get, "account/addresses");
            AddRegularRequestRoute(routingData, "Account", "Register", CustomHttpMethod.Get, "account/register");
            AddRegularRequestRoute(routingData, "Account", "Register", CustomHttpMethod.PostForm, "account/register");

            AddRegularRequestRoute(routingData, "Account", "Login", CustomHttpMethod.Get, "account/login");
            AddRegularRequestRoute(routingData, "Account", "Login", CustomHttpMethod.PostForm, "account/login");
            AddRegularRequestRoute(routingData, "Account", "Logout", CustomHttpMethod.Get, "account/logout");
            AddRegularRequestRoute(routingData, "Account", "ForgotPassword", CustomHttpMethod.Get, "account/forgotpassword");
            AddRegularRequestRoute(routingData, "Account", "ForgotPassword", CustomHttpMethod.PostForm, "account/forgotpassword");

            AddRegularRequestRoute(routingData, "Account", "ResetPassword", CustomHttpMethod.Get, "account/resetpassword");
            AddRegularRequestRoute(routingData, "Account", "ResetPassword", CustomHttpMethod.PostForm, "account/resetpassword");
            AddRegularRequestRoute(routingData, "Account", "ChangePassword", CustomHttpMethod.PostForm, "account/password");
            AddRegularRequestRoute(routingData, "Account", "ExternalLogin", CustomHttpMethod.Get, "account/externallogin");
            AddRegularRequestRoute(routingData, "Account", "ExternalLoginCallback", CustomHttpMethod.Get, "account/externallogincallback");

            AddRegularRequestRoute(routingData, "Account", "ImpersonateUser", CustomHttpMethod.Get, "account/impersonate/111");
            AddRegularRequestRoute(routingData, "Account", "ConfirmEmail", CustomHttpMethod.Get, "account/confirmemail");
            AddRegularRequestRoute(routingData, "Account", "ConfirmInvitation", CustomHttpMethod.Get, "account/confirminvitation");
            AddRegularRequestRoute(routingData, "Account", "ConfirmInvitation", CustomHttpMethod.PostForm, "account/confirminvitation");
            AddRegularRequestRoute(routingData, "Account", "ForgotLogin", CustomHttpMethod.Get, "account/forgotlogin");

            AddRegularRequestRoute(routingData, "Account", "ForgotLogin", CustomHttpMethod.PostForm, "account/forgotlogin");

            // Cart
            AddRegularRequestRoute(routingData, "Cart", "Index", CustomHttpMethod.Get, "cart");
            AddRegularRequestRoute(routingData, "Cart", "Checkout", CustomHttpMethod.Get, "cart/checkout");
            AddRegularRequestRoute(routingData, "Cart", "ExternalPaymentCallback", CustomHttpMethod.Get, "cart/externalpaymentcallback");
            AddRegularRequestRoute(routingData, "Cart", "ExternalPaymentCallback", CustomHttpMethod.PostJson, "cart/externalpaymentcallback");
            AddRegularRequestRoute(routingData, "Cart", "Thanks", CustomHttpMethod.Get, "cart/thanks/TORD1112345");

            AddRegularRequestRoute(routingData, "Cart", "PaymentForm", CustomHttpMethod.Get, "cart/checkout/paymentform");

            // Cart (Shopify compatible)
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "Cart", CustomHttpMethod.PostJson, "cart?updates=1234,5678&update=90");
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "CartJs", CustomHttpMethod.Get, "cart.js");
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "Add", CustomHttpMethod.PostJson, "cart/add");
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "AddJs", CustomHttpMethod.PostJson, "cart/add.js");
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "Change", CustomHttpMethod.Get, "cart/change");

            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "ChangeJs", CustomHttpMethod.PostJson, "cart/change.js");
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "Clear", CustomHttpMethod.Get, "cart/clear");
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "ClearJs", CustomHttpMethod.PostJson, "cart/clear.js");
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "UpdateJs", CustomHttpMethod.PostJson, "cart/update.js");

            // Collections (Shopify compatible)
            AddRegularRequestRoute(routingData, "ShopifyCompatibility", "Collections", CustomHttpMethod.Get, "collections");

            // QuoteRequest
            AddRegularRequestRoute(routingData, "QuoteRequest", "CurrentQuoteRequest", CustomHttpMethod.Get, "quoterequest");
            AddRegularRequestRoute(routingData, "QuoteRequest", "QuoteRequests", CustomHttpMethod.Get, "account/quoterequests");
            AddRegularRequestRoute(routingData, "QuoteRequest", "QuoteRequestByNumber", CustomHttpMethod.Get, "quoterequest/111");

            // Bulk order
            AddRegularRequestRoute(routingData, "BulkOrder", "Index", CustomHttpMethod.Get, "bulkorder");
            AddRegularRequestRoute(routingData, "BulkOrder", "AddFieldItems", CustomHttpMethod.PostJson, "bulkorder/addfielditems");
            AddRegularRequestRoute(routingData, "BulkOrder", "AddCsvItems", CustomHttpMethod.PostJson, "bulkorder/addcsvitems");

            // CatalogSearch
            AddRegularRequestRoute(routingData, "CatalogSearch", "CategoryBrowsing", CustomHttpMethod.Get, "search/112");
            AddRegularRequestRoute(routingData, "CatalogSearch", "SearchProducts", CustomHttpMethod.Get, "search");

            // Common
            AddRegularRequestRoute(routingData, "Common", "SetCurrency", CustomHttpMethod.Get, "common/setcurrency/USD");
            AddRegularRequestRoute(routingData, "Common", "GetCountries", CustomHttpMethod.Get, "common/getcountries/json");
            AddRegularRequestRoute(routingData, "Common", "GetRegions", CustomHttpMethod.Get, "common/getregions/us/json");
            AddRegularRequestRoute(routingData, "Common", "ContactForm", CustomHttpMethod.PostForm, "contact/page.contact", EmptyFormData);
            AddRegularRequestRoute(routingData, "Common", "Maintenance", CustomHttpMethod.Get, "maintenance");

            AddRegularRequestRoute(routingData, "Common", "Maintenance", CustomHttpMethod.Get, "common/maintenance");
            AddRegularRequestRoute(routingData, "Common", "ResetCache", CustomHttpMethod.Get, "common/resetcache");
            AddRegularRequestRoute(routingData, "Common", "NoTheme", CustomHttpMethod.Get, "common/notheme");

            // Sitemap
            AddRegularRequestRoute(routingData, "Sitemap", "GetSitemapIndex", CustomHttpMethod.Get, "sitemap.xml");
            AddRegularRequestRoute(routingData, "Sitemap", "GetSitemap", CustomHttpMethod.Get, "sitemap/somePath");

            // Category routes
            AddRegularRequestRoute(routingData, "CatalogSearch", "CategoryBrowsing", CustomHttpMethod.Get, "category/1234");

            // Product routes
            AddRegularRequestRoute(routingData, "Product", "ProductDetails", CustomHttpMethod.Get, "product/111");
            AddRegularRequestRoute(routingData, "Product", "Compare", CustomHttpMethod.Get, "compare");

            // Vendor routes
            AddRegularRequestRoute(routingData, "Vendor", "VendorDetails", CustomHttpMethod.Get, "vendor/222");

            // Inventory routes
            AddRegularRequestRoute(routingData, "Inventory", "FulfillmentCenterDetails", CustomHttpMethod.Get, "fulfillmentcenter/12");

            // Assets
            AddRegularRequestRoute(routingData, "Asset", "GetThemeLocalizationJson", CustomHttpMethod.Get, "themes/localization.json");
            AddRegularRequestRoute(routingData, "Asset", "GetThemeAssets", CustomHttpMethod.Get, "themes/assets/some.theme.asset");
            AddRegularRequestRoute(routingData, "Asset", "GetStaticContentAssets", CustomHttpMethod.Get, "assets/some.static.asset");

            // Static content (no cms)
            AddRegularRequestRoute(routingData, "StaticContent", "GetContentPageByName", CustomHttpMethod.Get, "pages/TestPage");

            //Blog
            AddRegularRequestRoute(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blog");
            AddRegularRequestRoute(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blog/category/1");
            AddRegularRequestRoute(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blog/tag/phones");
            AddRegularRequestRoute(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blogs/1");
            AddRegularRequestRoute(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blogs/generic_blog/category/posts");
            AddRegularRequestRoute(routingData, "StaticContent", "GetBlog", CustomHttpMethod.Get, "blogs/generic_blog/tag/shopping");

            AddRegularRequestRoute(routingData, "StaticContent", "Search", CustomHttpMethod.PostJson, "content/search");

            AddRegularRequestRoute(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blogs/generic_blog/rss");
            AddRegularRequestRoute(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blogs/generic_blog/feed");
            AddRegularRequestRoute(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blog/rss");
            AddRegularRequestRoute(routingData, "StaticContent", "BlogRssFeed", CustomHttpMethod.Get, "blog/feed");

            AddRegularRequestRoute(routingData, "Home", "Index", CustomHttpMethod.Get, "");

            RoutingData = routingData;
        }

        private static void AddRegularRequestRoute(ICollection<object[]> routingData, string expectedControllerName,
            string expectedActionName, CustomHttpMethod httpMethod, string baseUrl, object objectToPost = null)
        {
            var expectedControllerMethodName = $"VirtoCommerce.Storefront.Controllers.{expectedControllerName}Controller.{expectedActionName} " +
                                               "(VirtoCommerce.Storefront)";
            routingData.Add(new[] { httpMethod, baseUrl, expectedControllerMethodName, objectToPost });
        }

        private static void AddApiRequestRoute(ICollection<object[]> routingData, string expectedControllerName,
            string expectedActionName, CustomHttpMethod httpMethod, string baseUrl, object objectToPost = null)
        {
            var expectedControllerMethodName = $"VirtoCommerce.Storefront.Controllers.Api.{expectedControllerName}Controller.{expectedActionName} " +
                                               "(VirtoCommerce.Storefront)";
            routingData.Add(new[] { httpMethod, baseUrl, expectedControllerMethodName, objectToPost });
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

            var urlVariations = EnumerateUrlVariations(url);

            // Note: Theoretically, each of these URL variations could be a separate test case, and this test
            // could be a bit simplier. But splitting variations to different test cases also slows test run a lot.
            // So, to speed up tests execution, we reuse the same test server for all URL variations of request.

            foreach (var urlVariation in urlVariations)
            {
                await PerformTestingRouting(method, urlVariation, expectedControllerMethodName, objectToPost);
            }
        }

        private static IEnumerable<string> EnumerateUrlVariations(string baseUrl)
        {
            yield return baseUrl;
            yield return $"en-US/{baseUrl}";
            yield return $"Electronics/{baseUrl}";
            yield return $"Electronics/en-US/{baseUrl}";
        }

        protected static void AddErrorRequestRoutes(ICollection<object[]> routingData, string expectedActionName, string baseUrl)
        {
            var allHttpMethods = Enum.GetValues(typeof(CustomHttpMethod)).Cast<CustomHttpMethod>();
            foreach (var httpMethod in allHttpMethods)
            {
                AddRegularRequestRoute(routingData, "Error", expectedActionName, httpMethod, baseUrl);
            }
        }

        private async Task PerformTestingRouting(CustomHttpMethod method, string url, string expectedControllerMethodName, object objectToPost)
        {
            var response = await PerformSendingRequest(method, url, objectToPost);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var routingData = JsonConvert.DeserializeObject<RoutingDataResult>(responseContent);
            Assert.NotNull(routingData);

            Assert.Equal(expectedControllerMethodName, routingData.ControllerMethodName);
        }

        private async Task<HttpResponseMessage> PerformSendingRequest(CustomHttpMethod method, string url, object objectToPost)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var json = JsonConvert.SerializeObject(objectToPost, settings);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            switch (method)
            {
                case CustomHttpMethod.PostJson:
                    return await Client.PostAsync(url, content);

                case CustomHttpMethod.PostForm:
                    var actualData = (IEnumerable<KeyValuePair<string, string>>)objectToPost ?? EmptyFormData;
                    return await Client.PostAsync(url, new FormUrlEncodedContent(actualData));

                case CustomHttpMethod.Get:
                    return await Client.GetAsync(url);

                case CustomHttpMethod.Put:
                    return await Client.PutAsync(url, content);

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
