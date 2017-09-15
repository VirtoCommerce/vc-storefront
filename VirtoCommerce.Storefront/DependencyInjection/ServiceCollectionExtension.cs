using CacheManager.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Services;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;

namespace VirtoCommerce.Storefront.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddPlatformApi(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var apiUrl = configuration.GetValue<string>("VirtoCommerce:Api:Url");
            //TODO: Add validation and throw friendly error
            var apiAppId = configuration.GetValue<string>("VirtoCommerce:Api:AppId");
            var apiSecretKey = configuration.GetValue<string>("VirtoCommerce:Api:SecretKey");
            //Timeout for all API requests. Should be small on production to prevent platform API flood.
            var apiRequestTimeout = TimeSpan.Parse(configuration.GetValue("VirtoCommerce:Api:RequestTimeout", "0:0:30"));
            //container.RegisterInstance(new HmacCredentials(apiAppId, apiSecretKey));

            //container.RegisterType<VirtoCommerceApiRequestHandler>(new PerRequestLifetimeManager());

            ServicePointManager.UseNagleAlgorithm = false;
            services.AddSingleton<VirtoCommerceApiRequestHandler>();
            services.AddSingleton(provider => new HmacCredentials(apiAppId, apiSecretKey));
            services.AddTransient(provider => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            var baseUri = new Uri(apiUrl);
            services.AddSingleton<IStoreModule>(provider => new StoreModule(new VirtoCommerceStoreRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICommerce>(provider => new Commerce(new VirtoCommerceCoreRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICatalogModuleCategories>(provider => new CatalogModuleCategories(new VirtoCommerceCatalogRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICatalogModuleProducts>(provider => new CatalogModuleProducts(new VirtoCommerceCatalogRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ISecurity>(provider => new Security(new VirtoCommercePlatformRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IStorefrontSecurity>(provider => new StorefrontSecurity(new VirtoCommerceCoreRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICustomerModule>(provider => new CustomerModule(new VirtoCommerceCustomerRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IOrderModule>(provider => new OrderModule(new VirtoCommerceOrdersRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IQuoteModule>(provider => new QuoteModule(new VirtoCommerceQuoteRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ISubscriptionModule>(provider => new SubscriptionModule(new VirtoCommerceSubscriptionRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>()).DisableRetries().WithTimeout(apiRequestTimeout)));


        }

        public static void AddCache(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            //TODO: Load from configuration
            var localCache = CacheFactory.Build("storefrontCache", settings =>
            {
                settings.WithUpdateMode(CacheUpdateMode.Up)
                        .WithMicrosoftMemoryCacheHandle("memCacheHandle")
                        .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(5));
            });
            services.AddSingleton<ICacheManager<object>>(localCache);
        }

        public static void AddContentBlobServices(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var cmsContentConnectionString = BlobConnectionString.Parse(configuration.GetConnectionString("ContentConnectionString"));
            var themesBasePath = cmsContentConnectionString.RootPath.TrimEnd('/') + "/" + "Themes";
            var staticContentBasePath = cmsContentConnectionString.RootPath.TrimEnd('/') + "/" + "Pages";
            IThemesContentBlobProvider themesBlobProvider;
            IStaticContentBlobProvider staticContentBlobProvider;
            //TODO: Azure blob provider
            //if ("AzureBlobStorage".Equals(cmsContentConnectionString.Provider, StringComparison.OrdinalIgnoreCase))
            //{
            //    themesBlobProvider = new AzureBlobContentProvider(cmsContentConnectionString.ConnectionString, themesBasePath, localCacheManager);
            //    staticContentBlobProvider = new AzureBlobContentProvider(cmsContentConnectionString.ConnectionString, staticContentBasePath, localCacheManager);
            //}
            //else
            //{
            themesBlobProvider = new FileSystemContentBlobProvider(hostingEnvironment.MapPath(themesBasePath));
            staticContentBlobProvider = new FileSystemContentBlobProvider(hostingEnvironment.MapPath(staticContentBasePath));
            //}
            services.AddSingleton(themesBlobProvider);
            services.AddSingleton(staticContentBlobProvider);
        }

        public static void AddLiquidViewEngine(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var liquidThemeEngineOptions = new LiquidThemeEngineSettings
            {
                ThemesAssetsRelativeUrl = "~/themes/assets",
                RethrowLiquidRenderErrors = configuration.GetValue("VirtoCommerce:LiquidThemeEngine:RethrowErrors", false)
            };
            services.AddSingleton(liquidThemeEngineOptions);
            services.AddSingleton<ShopifyLiquidThemeEngine>();
            var tmpProvider = services.BuildServiceProvider();

            services.AddSingleton<ILiquidThemeEngine>(tmpProvider.GetService<ShopifyLiquidThemeEngine>());
            var mvcBuilder = services.AddMvc().AddViewOptions(options =>
            {
                options.ViewEngines.Add(new DotLiquidThemedViewEngine(tmpProvider.GetService<ShopifyLiquidThemeEngine>()));
            });

        }
    }
}
