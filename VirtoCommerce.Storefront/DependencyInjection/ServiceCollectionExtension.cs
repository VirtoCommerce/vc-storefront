using CacheManager.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.ContentModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.SitemapsModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common.Bus;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Common.Messages;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Services;

namespace VirtoCommerce.Storefront.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddPlatformApi(this IServiceCollection services, StorefrontOptions options)
        {
            //Timeout for all API requests. Should be small on production to prevent platform API flood.
            var apiRequestTimeout = options.Api.RequestTimeout;

            ServicePointManager.UseNagleAlgorithm = false;
            services.AddSingleton<VirtoCommerceApiRequestHandler>();
            var httpHandlerWithCompression = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var baseUri = new Uri(options.Api.Url);
            services.AddSingleton<IStoreModule>(provider => new StoreModule(new VirtoCommerceStoreRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICommerce>(provider => new Commerce(new VirtoCommerceCoreRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICatalogModuleCategories>(provider => new CatalogModuleCategories(new VirtoCommerceCatalogRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICatalogModuleProducts>(provider => new CatalogModuleProducts(new VirtoCommerceCatalogRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICatalogModuleSearch>(provider => new CatalogModuleSearch(new VirtoCommerceCatalogRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ISecurity>(provider => new Security(new VirtoCommercePlatformRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IStorefrontSecurity>(provider => new StorefrontSecurity(new VirtoCommerceCoreRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICustomerModule>(provider => new CustomerModule(new VirtoCommerceCustomerRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IOrderModule>(provider => new OrderModule(new VirtoCommerceOrdersRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IQuoteModule>(provider => new QuoteModule(new VirtoCommerceQuoteRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ISubscriptionModule>(provider => new SubscriptionModule(new VirtoCommerceSubscriptionRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IInventoryModule>(provider => new InventoryModule(new VirtoCommerceInventoryRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IMarketingModulePromotion>(provider => new MarketingModulePromotion(new VirtoCommerceMarketingRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IMarketingModuleDynamicContent>(provider => new MarketingModuleDynamicContent(new VirtoCommerceMarketingRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IPricingModule>(provider => new PricingModule(new VirtoCommercePricingRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ICartModule>(provider => new CartModule(new VirtoCommerceCartRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IMenu>(provider => new Menu(new VirtoCommerceContentRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IContent>(provider => new Content(new VirtoCommerceContentRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<IRecommendations>(provider => new Recommendations(new VirtoCommerceProductRecommendationsRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
            services.AddSingleton<ISitemapsModuleApiOperations>(provider => new SitemapsModuleApiOperations(new VirtoCommerceSitemapsRESTAPIdocumentation(baseUri, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(apiRequestTimeout)));
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

            var configSection = configuration.GetSection("VirtoCommerce:LiquidThemeEngine");
            services.Configure<LiquidThemeEngineOptions>(configSection);

            services.AddSingleton<ShopifyLiquidThemeEngine>();
            var provider = services.BuildServiceProvider();

            services.AddSingleton<ILiquidThemeEngine>(provider.GetService<ShopifyLiquidThemeEngine>());
            var mvcBuilder = services.AddMvc().AddViewOptions(options =>
            {
                options.ViewEngines.Add(new DotLiquidThemedViewEngine(provider.GetService<ShopifyLiquidThemeEngine>()));
            });

        }

        //Register event handlers through reflection
        public static void RegisterAssembliesEventHandlers(this IServiceCollection services, params Type[] typesFromAssemblyContainingMessages)
        {
            //Scan for eventhandlers
            services.Scan(scan => scan
                .FromAssemblies(typesFromAssemblyContainingMessages.Select(x => x.Assembly))
                    .AddClasses(classes => classes.Where(x => {
                        var allInterfaces = x.GetInterfaces();
                        return
                            allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHandler<>)) ||
                            allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableHandler<>));
                    }))
                    .AsSelf()
                    .WithTransientLifetime()
            );

            var serviceProvider = services.BuildServiceProvider();
            var handlerRegistrar = serviceProvider.GetService<IHandlerRegistrar>();

            foreach (var typesFromAssemblyContainingMessage in typesFromAssemblyContainingMessages)
            {
                var executorsAssembly = typesFromAssemblyContainingMessage.GetTypeInfo().Assembly;
                var executorTypes = executorsAssembly
                    .GetTypes()
                    .Select(t => new { Type = t, Interfaces = ResolveMessageHandlerInterface(t) })
                    .Where(e => e.Interfaces != null && e.Interfaces.Any());

                foreach (var executorType in executorTypes)
                {
                    foreach (var @interface in executorType.Interfaces)
                    {
                        InvokeHandler(@interface, handlerRegistrar, executorType.Type, serviceProvider);
                    }
                }
            }
        }

        private static void InvokeHandler(Type @interface, IHandlerRegistrar registrar, Type executorType, ServiceProvider serviceProvider)
        {
            var commandType = @interface.GetGenericArguments()[0];

            var registerExecutorMethod = registrar
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.Name == "RegisterHandler")
                .Where(mi => mi.IsGenericMethod)
                .Where(mi => mi.GetGenericArguments().Length == 1)
                .Single(mi => mi.GetParameters().Length == 1)
                .MakeGenericMethod(commandType);

            Func<dynamic, CancellationToken, Task> del;
            if (IsCancellable(@interface))
            {
                del = (x, token) =>
                {
                    dynamic handler = serviceProvider.GetService(executorType);
                    return handler.Handle(x, token);
                };
            }
            else
            {
                del = (x, token) =>
                {
                    dynamic handler = serviceProvider.GetService(executorType);
                    return handler.Handle(x);
                };
            }

            registerExecutorMethod.Invoke(registrar, new object[] { del });
        }

        private static bool IsCancellable(Type @interface)
        {
            return @interface.GetGenericTypeDefinition() == typeof(ICancellableEventHandler<>);
        }

        private static IEnumerable<Type> ResolveMessageHandlerInterface(Type type)
        {
            return type
                .GetInterfaces()
                .Where(i => i.GetTypeInfo().IsGenericType && (i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                             || i.GetGenericTypeDefinition() == typeof(ICancellableEventHandler<>)));
        }
    }
}
