using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Storefront.Routing
{
    /// <summary>
    /// Provides extension methods for <see cref="IRouteBuilder"/> to add routes.
    /// </summary>
    public static class MapStorefrontRouteBuilderExtension
    {
        public static void MapStorefrontRoutes(this IRouteBuilder routes)
        {
            #region Storefront API routes

            // API cart
            routes.MapStorefrontRoute("API.GetCart", "storefrontapi/cart", defaults: new { controller = "ApiCart", action = "GetCart" });
            routes.MapStorefrontRoute("API.Cart.GetCartItemsCount", "storefrontapi/cart/itemscount", defaults: new { controller = "ApiCart", action = "GetCartItemsCount" });
            routes.MapStorefrontRoute("API.Cart.AddItemToCart", "storefrontapi/cart/items", defaults: new { controller = "ApiCart", action = "AddItemToCart" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Cart.ChangeCartItem", "storefrontapi/cart/items", defaults: new { controller = "ApiCart", action = "ChangeCartItem" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("PUT") }));
            routes.MapStorefrontRoute("API.Cart.ChangeCartItemPrice", "storefrontapi/cart/items/price", defaults: new { controller = "ApiCart", action = "ChangeCartItemPrice" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("PUT") }));
            routes.MapStorefrontRoute("API.Cart.RemoveCartItem", "storefrontapi/cart/items", defaults: new { controller = "ApiCart", action = "RemoveCartItem" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("DELETE") }));
            routes.MapStorefrontRoute("API.Cart.ClearCart", "storefrontapi/cart/clear", defaults: new { controller = "ApiCart", action = "ClearCart" });
            routes.MapStorefrontRoute("API.Cart.GetCartShipmentAvailShippingMethods", "storefrontapi/cart/shipments/{shipmentId}/shippingmethods", defaults: new { controller = "ApiCart", action = "GetCartShipmentAvailShippingMethods" });
            routes.MapStorefrontRoute("API.Cart.GetCartAvailPaymentMethods", "storefrontapi/cart/paymentmethods", defaults: new { controller = "ApiCart", action = "GetCartAvailPaymentMethods" });
            routes.MapStorefrontRoute("API.Cart.AddCartCoupon", "storefrontapi/cart/coupons/{couponCode}", defaults: new { controller = "ApiCart", action = "AddCartCoupon" });
            routes.MapStorefrontRoute("API.Cart.RemoveCartCoupon", "storefrontapi/cart/coupons", defaults: new { controller = "ApiCart", action = "RemoveCartCoupon" });
            routes.MapStorefrontRoute("API.Cart.AddOrUpdateCartShipment", "storefrontapi/cart/shipments", defaults: new { controller = "ApiCart", action = "AddOrUpdateCartShipment" });
            routes.MapStorefrontRoute("API.Cart.AddOrUpdateCartPayment", "storefrontapi/cart/payments", defaults: new { controller = "ApiCart", action = "AddOrUpdateCartPayment" });
            routes.MapStorefrontRoute("API.Cart.CreateOrder", "storefrontapi/cart/createorder", defaults: new { controller = "ApiCart", action = "CreateOrder" });
            routes.MapStorefrontRoute("API.Cart.UpdatePaymentPlan", "storefrontapi/cart/paymentPlan", new { controller = "ApiCart", action = "AddOrUpdateCartPaymentPlan" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Cart.DeletePaymentPlan", "storefrontapi/cart/paymentPlan", new { controller = "ApiCart", action = "DeleteCartPaymentPlan" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("DELETE") }));
            routes.MapStorefrontRoute("API.Cart.SubmitComment", "storefrontapi/cart/comment", new { controller = "ApiCart", action = "UpdateCartComment" });

            // API lists
            routes.MapStorefrontRoute("API.Lists.GetListByName", "storefrontapi/lists/{listName}/{type}", defaults: new { controller = "ApiLists", action = "GetListByName" });
            routes.MapStorefrontRoute("API.Lists.AddItemToList", "storefrontapi/lists/items", defaults: new { controller = "ApiLists", action = "AddItemToList" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Lists.RemoveItemFromList", "storefrontapi/lists/{listName}/{type}/items/{lineItemId}", defaults: new { controller = "ApiLists", action = "RemoveItemFromList" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("DELETE") }));
            routes.MapStorefrontRoute("API.Lists.GetListsWithProduct", "storefrontapi/lists/getlistswithproduct", defaults: new { controller = "ApiLists", action = "GetListsWithProduct" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Lists.SearchLists", "storefrontapi/lists/search", defaults: new { controller = "ApiLists", action = "SearchLists" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Lists.CreateList", "storefrontapi/lists/{listName}/{type}/create", defaults: new { controller = "ApiLists", action = "CreateList" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Lists.DeleteListsByIds", "storefrontapi/lists/deletelistsbyids", defaults: new { controller = "ApiLists", action = "DeleteListsByIds" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("DELETE") }));
            routes.MapStorefrontRoute("API.Lists.MergeWithCurrentCart", "storefrontapi/lists/{listName}/{type}/mergewithcurrentcart", defaults: new { controller = "ApiLists", action = "MergeWithCurrentCart" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            
            // Catalog API
            routes.MapStorefrontRoute("API.Catalog.SearchProducts", "storefrontapi/catalog/search", defaults: new { controller = "ApiCatalog", action = "SearchProducts" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Catalog.GetProductsByIds", "storefrontapi/products", defaults: new { controller = "ApiCatalog", action = "GetProductsByIds" });
            routes.MapStorefrontRoute("API.Catalog.SearchCategories", "storefrontapi/categories/search", defaults: new { controller = "ApiCatalog", action = "SearchCategories" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Catalog.GetCategoriesByIds", "storefrontapi/categories", defaults: new { controller = "ApiCatalog", action = "GetCategoriesByIds" });

            // Common storefront API
            routes.MapStorefrontRoute("API.Common.GetCountries", "storefrontapi/countries", defaults: new { controller = "ApiCommon", action = "GetCountries" });
            routes.MapStorefrontRoute("API.Common.GetCountryRegions", "storefrontapi/countries/{countryCode}/regions", defaults: new { controller = "ApiCommon", action = "GetCountryRegions" });
            routes.MapStorefrontRoute("API.Common.Feedback", "storefrontapi/feedback", defaults: new { controller = "ApiCommon", action = "Feedback" });

            // Pricing API
            routes.MapStorefrontRoute("API.Pricing.GetActualProductPrices", "storefrontapi/pricing/actualprices", defaults: new { controller = "ApiPricing", action = "GetActualProductPrices" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));

            // Marketing API
            routes.MapStorefrontRoute("API.Marketing.GetDynamicContent", "storefrontapi/marketing/dynamiccontent/{placeName}", defaults: new { controller = "ApiMarketing", action = "GetDynamicContent" });

            // Recommendations API
            routes.MapStorefrontRoute("API.Recommendations.GetRecommendations", "storefrontapi/recommendations", defaults: new { controller = "ApiRecommendations", action = "GetRecommendations" });

            // User actions and events API
            routes.MapStorefrontRoute("API.UserActions.SaveEventInfo", "storefrontapi/useractions/eventinfo", new { controller = "ApiUserActions", action = "SaveEventInfo" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));

            // Account API
            routes.MapStorefrontRoute("API.Account.GetCurrentCustomer", "storefrontapi/account", new { controller = "ApiAccount", action = "GetCurrentCustomer" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }));
            routes.MapStorefrontRoute("API.Account.quotes", "storefrontapi/account/quotes", new { controller = "ApiAccount", action = "GetCustomerQuotes" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }));

            routes.MapStorefrontRoute("API.Account.UpdateAccount", "storefrontapi/account", new { controller = "ApiAccount", action = "UpdateAccount" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Account.ChangePassword", "storefrontapi/account/password", new { controller = "ApiAccount", action = "ChangePassword" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Account.UpdateAddresses", "storefrontapi/account/addresses", new { controller = "ApiAccount", action = "UpdateAddresses" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));

            // Order API
            routes.MapStorefrontRoute("API.Orders", "storefrontapi/orders/search", new { controller = "ApiOrder", action = "SearchCustomerOrders" });
            routes.MapStorefrontRoute("API.OrderByNumber", "storefrontapi/orders/{orderNumber}", new { controller = "ApiOrder", action = "GetCustomerOrder" });
            routes.MapStorefrontRoute("API.Orders.GetNewPaymentData", "storefrontapi/orders/{orderNumber}/newpaymentdata", new { controller = "ApiOrder", action = "GetNewPaymentData" });
            routes.MapStorefrontRoute("API.Orders.CancelPayment", "storefrontapi/orders/{orderNumber}/payments/{paymentNumber}/cancel", new { controller = "ApiOrder", action = "CancelPayment" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Orders.ProcessPayment", "storefrontapi/orders/{orderNumber}/payments/{paymentNumber}/process", new { controller = "ApiOrder", action = "ProcessOrderPayment" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Orders.AddOrUpdateOrderPayment", "storefrontapi/orders/{orderNumber}/payments", new { controller = "ApiOrder", action = "AddOrUpdateOrderPayment" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.Orders.GetInvoicePdf", "storefrontapi/orders/{orderNumber}/invoice", new { controller = "ApiOrder", action = "GetInvoicePdf" });

            // Quote requests API
            routes.MapStorefrontRoute("API.QuoteRequest.GetItemsCount", "storefrontapi/quoterequests/{number}/itemscount", defaults: new { controller = "ApiQuoteRequest", action = "GetItemsCount" });
            routes.MapStorefrontRoute("API.QuoteRequest.Get", "storefrontapi/quoterequests/{number}", defaults: new { controller = "ApiQuoteRequest", action = "Get" });
            routes.MapStorefrontRoute("API.QuoteRequest.GetCurrent", "storefrontapi/quoterequest/current", defaults: new { controller = "ApiQuoteRequest", action = "GetCurrent" });
            routes.MapStorefrontRoute("API.QuoteRequest.AddItem", "storefrontapi/quoterequests/current/items", defaults: new { controller = "ApiQuoteRequest", action = "AddItem" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.QuoteRequest.RemoveItem", "storefrontapi/quoterequests/{number}/items/{itemId}", defaults: new { controller = "ApiQuoteRequest", action = "RemoveItem" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("DELETE") }));
            routes.MapStorefrontRoute("API.QuoteRequest.Update", "storefrontapi/quoterequests/{number}", defaults: new { controller = "ApiQuoteRequest", action = "Update" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("PUT") }));
            routes.MapStorefrontRoute("API.QuoteRequest.Submit", "storefrontapi/quoterequests/{number}/submit", defaults: new { controller = "ApiQuoteRequest", action = "Submit" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.QuoteRequest.Reject", "storefrontapi/quoterequests/{number}/reject", defaults: new { controller = "ApiQuoteRequest", action = "Reject" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.QuoteRequest.CalculateTotals", "storefrontapi/quoterequests/{number}/totals", defaults: new { controller = "ApiQuoteRequest", action = "CalculateTotals" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("API.QuoteRequest.Confirm", "storefrontapi/quoterequests/{number}/confirm", defaults: new { controller = "ApiQuoteRequest", action = "Confirm" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));

            // Subscriptions API
            routes.MapStorefrontRoute("API.Subscriptions", "storefrontapi/subscriptions/search", defaults: new { controller = "ApiSubscription", action = "SearchCustomerSubscriptions" });
            routes.MapStorefrontRoute("API.SubscriptionByNumber", "storefrontapi/subscriptions/{number}", defaults: new { controller = "ApiSubscription", action = "GetCustomerSubscription" });
            routes.MapStorefrontRoute("API.CancelSubscription", "storefrontapi/subscriptions/{number}/cancel", defaults: new { controller = "ApiSubscription", action = "CancelSubscription" });



            // Blog API
            routes.MapStorefrontRoute("API.Blog.Search", "storefrontapi/blog/{blogName}/search", defaults: new { controller = "ApiBlog", action = "Search" });

            #endregion

            //Errors
            routes.MapStorefrontRoute("AccessDenied", "error/AccessDenied", defaults: new { controller = "Error", action = "AccessDenied" });
            routes.MapStorefrontRoute("Error", "error/{errCode}", defaults: new { controller = "Error", action = "Error" });
            routes.MapRoute("NoWorkContextErrors", "error/{errCode}", defaults: new { controller = "Error", action = "Error" });
            routes.MapStorefrontRoute("StoreMaintenance", "common/maintenance", defaults: new { controller = "Common", action = "Maintenance" });
            routes.MapStorefrontRoute("NoTheme", "common/notheme", defaults: new { controller = "Common", action = "NoTheme" });
            // Account
            routes.MapStorefrontRoute("Account", "account", defaults: new { controller = "Account", action = "GetAccount" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }));
            routes.MapStorefrontRoute("Account.GetOrderDetails ", "account/order/{number}", defaults: new { controller = "Account", action = "GetOrderDetails" });
            routes.MapStorefrontRoute("Account.UpdateAddress", "account/addresses/{id?}", defaults: new { controller = "Account", action = "UpdateAddress" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("Account.GetAddresses", "account/addresses", defaults: new { controller = "Account", action = "GetAddresses" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }));
            routes.MapStorefrontRoute("Account.Register", "account/register", defaults: new { controller = "Account", action = "Register" });
            routes.MapStorefrontRoute("Account.Login", "account/login", defaults: new { controller = "Account", action = "Login" });
            routes.MapStorefrontRoute("Account.Logout", "account/logout", defaults: new { controller = "Account", action = "Logout" });
            routes.MapStorefrontRoute("Account.ForgotPassword", "account/forgotpassword", defaults: new { controller = "Account", action = "ForgotPassword" });
            routes.MapStorefrontRoute("Account.ResetPassword", "account/resetpassword", defaults: new { controller = "Account", action = "ResetPassword" });
            routes.MapStorefrontRoute("Account.ChangePassword", "account/password", defaults: new { controller = "Account", action = "ChangePassword" });
            routes.MapStorefrontRoute("Account.ExternalLogin", "account/externallogin", defaults: new { controller = "Account", action = "ExternalLogin" });
            routes.MapStorefrontRoute("Account.ExternalLoginCallback", "account/externallogincallback", defaults: new { controller = "Account", action = "ExternalLoginCallback" });
            routes.MapStorefrontRoute("Account.ImpersonateUser", "account/impersonate/{userId}", defaults: new { controller = "Account", action = "ImpersonateUser" });
            routes.MapStorefrontRoute("Account.ConfirmEmail", "account/confirmemail", defaults: new { controller = "Account", action = "ConfirmEmail" });

            // Cart
            routes.MapStorefrontRoute("Cart.Index", "cart", defaults: new { controller = "Cart", action = "Index" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }));
            routes.MapStorefrontRoute("Cart.Checkout", "cart/checkout", defaults: new { controller = "Cart", action = "Checkout" });
            routes.MapStorefrontRoute("Cart.ExternalPaymentCallback", "cart/externalpaymentcallback", defaults: new { controller = "Cart", action = "ExternalPaymentCallback" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET", "POST") }));
            routes.MapStorefrontRoute("Cart.Thanks", "cart/thanks/{orderNumber}", defaults: new { controller = "Cart", action = "Thanks" });
            routes.MapStorefrontRoute("Cart.PaymentForm", "cart/checkout/paymentform", defaults: new { controller = "Cart", action = "PaymentForm" });

            // Cart (Shopify compatible)
            routes.MapStorefrontRoute("ShopifyCart.Cart", "cart", defaults: new { controller = "ShopifyCompatibility", action = "Cart" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("ShopifyCart.CartJs", "cart.js", defaults: new { controller = "ShopifyCompatibility", action = "CartJs" });
            routes.MapStorefrontRoute("ShopifyCart.Add", "cart/add", defaults: new { controller = "ShopifyCompatibility", action = "Add" });
            routes.MapStorefrontRoute("ShopifyCart.AddJs", "cart/add.js", defaults: new { controller = "ShopifyCompatibility", action = "AddJs" });
            routes.MapStorefrontRoute("ShopifyCart.Change", "cart/change", defaults: new { controller = "ShopifyCompatibility", action = "Change" });
            routes.MapStorefrontRoute("ShopifyCart.ChangeJs", "cart/change.js", defaults: new { controller = "ShopifyCompatibility", action = "ChangeJs" });
            routes.MapStorefrontRoute("ShopifyCart.Clear", "cart/clear", defaults: new { controller = "ShopifyCompatibility", action = "Clear" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }));
            routes.MapStorefrontRoute("ShopifyCart.ClearJs", "cart/clear.js", defaults: new { controller = "ShopifyCompatibility", action = "ClearJs" });
            routes.MapStorefrontRoute("ShopifyCart.UpdateJs", "cart/update.js", defaults: new { controller = "ShopifyCompatibility", action = "UpdateJs" });
            //Collections  (Shopify compatible)
            routes.MapStorefrontRoute("Shopify.Collections", "collections", defaults: new { controller = "ShopifyCompatibility", action = "Collections" });

            // QuoteRequest
            routes.MapStorefrontRoute("QuoteRequest.CurrentQuoteRequest", "quoterequest", defaults: new { controller = "QuoteRequest", action = "CurrentQuoteRequest" });
            routes.MapStorefrontRoute("Account.QuoteRequests", "account/quoterequests", defaults: new { controller = "QuoteRequest", action = "QuoteRequests" });
            routes.MapStorefrontRoute("Account.QuoteRequestByNumber", "quoterequest/{number}", defaults: new { controller = "QuoteRequest", action = "QuoteRequestByNumber" });

            // Bulk order
            routes.MapStorefrontRoute("BulkOrder.Index", "bulkorder", defaults: new { controller = "BulkOrder", action = "Index" });
            routes.MapStorefrontRoute("BulkOrder.AddFieldItems", "bulkorder/addfielditems", defaults: new { controller = "BulkOrder", action = "AddFieldItems" });
            routes.MapStorefrontRoute("BulkOrder.AddCsvItems", "bulkorder/addcsvitems", defaults: new { controller = "BulkOrder", action = "AddCsvItems" });

            // CatalogSearch
            routes.MapStorefrontRoute("CatalogSearch.CategoryBrowsing", "search/{categoryId}", defaults: new { controller = "CatalogSearch", action = "CategoryBrowsing" });
            routes.MapStorefrontRoute("CatalogSearch.SearchProducts", "search", defaults: new { controller = "CatalogSearch", action = "SearchProducts" });

            // Common            
            routes.MapStorefrontRoute("Common.SetCurrency", "common/setcurrency/{currency}", defaults: new { controller = "Common", action = "SetCurrency" });
            routes.MapStorefrontRoute("Common.ContactUsPost", "contact/{viewName?}", defaults: new { controller = "Common", action = "СontactForm" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
            routes.MapStorefrontRoute("Common.NoStore", "common/nostore", defaults: new { controller = "Common", action = "NoStore" });
            routes.MapStorefrontRoute("Common.Maintenance", "maintenance", defaults: new { controller = "Common", action = "Maintenance" });
            routes.MapStorefrontRoute("Common.ResetCache", "common/resetcache", defaults: new { controller = "Common", action = "ResetCache" });

            //Sitemap
            routes.MapStorefrontRoute("Sitemap.GetSitemapIndex", "sitemap.xml", defaults: new { controller = "Sitemap", action = "GetSitemapIndex" });
            routes.MapStorefrontRoute("Sitemap.GetSitemap", "sitemap/{sitemapPath}", defaults: new { controller = "Sitemap", action = "GetSitemap" });


            // Category routes
            routes.MapStorefrontRoute("Category.BrowseCategory", "category/{categoryId}", defaults: new { controller = "CatalogSearch", action = "CategoryBrowsing" });

            // Product routes
            routes.MapStorefrontRoute("Product.GetProduct", "product/{productId}", defaults: new { controller = "Product", action = "ProductDetails" });
            routes.MapStorefrontRoute("Product.Compare", "compare", defaults: new { controller = "Product", action = "Compare" });

            // Vendor routes
            routes.MapStorefrontRoute("Vendor.GetVendor", "vendor/{vendorId}", defaults: new { controller = "Vendor", action = "VendorDetails" });

            // Assets
            routes.MapStorefrontRoute("ThemeLocalization", "themes/localization.json", defaults: new { controller = "Asset", action = "GetThemeLocalizationJson" });
            routes.MapStorefrontRoute("ThemeAssets", "themes/assets/{*path}", defaults: new { controller = "Asset", action = "GetThemeAssets" });
            routes.MapStorefrontRoute("StaticContentAssets", "assets/{*path}", defaults: new { controller = "Asset", action = "GetStaticContentAssets" });


            // Static content (no cms)
            routes.MapStorefrontRoute("Pages.GetPage", "pages/{*page}", defaults: new { controller = "StaticContent", action = "GetContentPageByName" });
            //Blog
            routes.MapStorefrontRoute("Blogs.GetDefaultBlog", "blog", defaults: new { controller = "StaticContent", action = "GetBlog" });
            routes.MapStorefrontRoute("Blogs.GetDefaultBlogWithFilterByCategory", "blog/category/{category}", defaults: new { controller = "StaticContent", action = "GetBlog" });
            routes.MapStorefrontRoute("Blogs.GetDefaultBlogWithFilterByTag", "blog/tag/{tag}", defaults: new { controller = "StaticContent", action = "GetBlog" });
            routes.MapStorefrontRoute("Blogs.GetBlogByName", "blogs/{blog}", defaults: new { controller = "StaticContent", action = "GetBlog" });
            routes.MapStorefrontRoute("Blogs.GetBlogWithFilterByCategory", "blogs/{blogname}/category/{category}", defaults: new { controller = "StaticContent", action = "GetBlog" });
            routes.MapStorefrontRoute("Blogs.GetBlogWithFilterByTag", "blogs/{blogname}/tag/{tag}", defaults: new { controller = "StaticContent", action = "GetBlog" });
            routes.MapStorefrontRoute("StaticContent.Search", "content/search", defaults: new { controller = "StaticContent", action = "Search" });

            routes.MapStorefrontRoute("StaticContent.BlogByName.RssFeed", "blogs/{blogname}/{rss}", defaults: new { controller = "StaticContent", action = "BlogRssFeed" }, constraints: new { rss = @"(rss|feed)" });
            routes.MapStorefrontRoute("StaticContent.Blog.RssFeed", "blog/{rss}", defaults: new { controller = "StaticContent", action = "BlogRssFeed" }, constraints: new { rss = @"(rss|feed)" });

            //Register  slug route based on dynamic data
            //It a default route and must be always on end of routes
            routes.MapSlugRoute("{*path}", defaults: new { controller = "Home", action = "Index" });
        }

        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template)
        {
            return MapStorefrontRoute(routeBuilder, name, template, defaults: null);
        }

        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template,
            object defaults)
        {
            return MapStorefrontRoute(routeBuilder, name, template, defaults, constraints: null);
        }


        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template,
            object defaults,
            object constraints)
        {
            return MapStorefrontRoute(routeBuilder, name, template, defaults, constraints, dataTokens: null);
        }


        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template,
            object defaults,
            object constraints,
            object dataTokens)
        {
            if (routeBuilder.DefaultHandler == null)
            {
                throw new RouteCreationException($"Must be set { nameof(IRouteBuilder) }");
            }

            var inlineConstraintResolver = routeBuilder
                .ServiceProvider
                .GetRequiredService<IInlineConstraintResolver>();

            routeBuilder.Routes.Add(new StorefrontRoute(
                routeBuilder.DefaultHandler,
                name,
                template,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                new RouteValueDictionary(dataTokens),
                inlineConstraintResolver));

            return routeBuilder;
        }

        public static IRouteBuilder MapSlugRoute(
           this IRouteBuilder routeBuilder,
           string template,
           object defaults)
        {
            if (routeBuilder.DefaultHandler == null)
            {
                throw new RouteCreationException($"Must be set { nameof(IRouteBuilder) }");
            }

            var inlineConstraintResolver = routeBuilder
                .ServiceProvider
                .GetRequiredService<IInlineConstraintResolver>();

            routeBuilder.Routes.Add(new SlugRoute(
                routeBuilder.DefaultHandler,
                template,
                new RouteValueDictionary(defaults),
                inlineConstraintResolver));

            return routeBuilder;
        }
    }
}
