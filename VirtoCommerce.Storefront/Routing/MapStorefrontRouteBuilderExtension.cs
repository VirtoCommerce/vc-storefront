using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Routing;
/// <summary>
/// Provides extension methods for <see cref="IRouteBuilder"/> to add routes.
/// </summary>
public static class MapStorefrontRouteBuilderExtension
{
    public static void MapStorefrontRoutes(this IRouteBuilder routes)
    {
        //Errors
        routes.MapStorefrontRoute("Errors", "error/{errCode}", defaults: new { controller = "Error", action = "Errors" });
        routes.MapRoute("NoWorkContextErrors", "error/{errCode}", defaults: new { controller = "Error", action = "Errors" });
        routes.MapStorefrontRoute("StoreMaintenance", "common/maintenance", defaults: new { controller = "Common", action = "Maintenance" });
        routes.MapStorefrontRoute("NoTheme", "common/notheme", defaults: new { controller = "Common", action = "NoTheme" });
        // Account
        routes.MapStorefrontRoute("Account", "account", defaults: new { controller = "Account", action = "GetAccount" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }));
        routes.MapStorefrontRoute("Account.UpdateAccount", "account", defaults: new { controller = "Account", action = "UpdateAccount" }, constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("POST") }));
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
