using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.AutoRestClients.CacheModuleApi;
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
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Common.Bus;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Common.Messages;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddPlatformEndpoint(this IServiceCollection services, Action<PlatformEndpointOptions> setupAction = null)
        {
       
            ServicePointManager.UseNagleAlgorithm = false;
            services.AddSingleton<VirtoCommerceApiRequestHandler>();
            var httpHandlerWithCompression = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            services.AddSingleton<IStoreModule>(provider => new StoreModule(new VirtoCommerceStoreRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ICommerce>(provider => new Commerce(new VirtoCommerceCoreRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ICatalogModuleCategories>(provider => new CatalogModuleCategories(new VirtoCommerceCatalogRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ICatalogModuleProducts>(provider => new CatalogModuleProducts(new VirtoCommerceCatalogRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ICatalogModuleSearch>(provider => new CatalogModuleSearch(new VirtoCommerceCatalogRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ISecurity>(provider => new Security(new VirtoCommercePlatformRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IStorefrontSecurity>(provider => new StorefrontSecurity(new VirtoCommerceCoreRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ICustomerModule>(provider => new CustomerModule(new VirtoCommerceCustomerRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IOrderModule>(provider => new OrderModule(new VirtoCommerceOrdersRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IQuoteModule>(provider => new QuoteModule(new VirtoCommerceQuoteRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ISubscriptionModule>(provider => new SubscriptionModule(new VirtoCommerceSubscriptionRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IInventoryModule>(provider => new InventoryModule(new VirtoCommerceInventoryRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IMarketingModulePromotion>(provider => new MarketingModulePromotion(new VirtoCommerceMarketingRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IMarketingModuleDynamicContent>(provider => new MarketingModuleDynamicContent(new VirtoCommerceMarketingRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IPricingModule>(provider => new PricingModule(new VirtoCommercePricingRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ICartModule>(provider => new CartModule(new VirtoCommerceCartRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IMenu>(provider => new Menu(new VirtoCommerceContentRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IContent>(provider => new Content(new VirtoCommerceContentRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<IRecommendations>(provider => new Recommendations(new VirtoCommerceProductRecommendationsRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ISitemapsModuleApiOperations>(provider => new SitemapsModuleApiOperations(new VirtoCommerceSitemapsRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<ICacheModule>(provider => new CacheModule(new VirtoCommerceCacheRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));
            services.AddSingleton<INotifications>(provider => new Notifications(new VirtoCommercePlatformRESTAPIdocumentation(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.Url, provider.GetService<VirtoCommerceApiRequestHandler>(), httpHandlerWithCompression).DisableRetries().WithTimeout(provider.GetService<IOptions<PlatformEndpointOptions>>().Value.RequestTimeout)));

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }     

        public static void AddFileSystemBlobContent(this IServiceCollection services, Action<FileSystemBlobContentOptions> setupAction = null)
        {
            services.AddSingleton<IContentBlobProvider, FileSystemContentBlobProvider>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }
   
        public static void AddAzureBlobContent(this IServiceCollection services, Action<AzureBlobContentOptions> setupAction = null)
        {
            services.AddSingleton<IContentBlobProvider, AzureBlobContentProvider>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }
     
        public static void AddLiquidViewEngine(this IServiceCollection services, Action<LiquidThemeEngineOptions> setupAction = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ILiquidThemeEngine, ShopifyLiquidThemeEngine>();
            services.AddSingleton<ILiquidViewEngine, DotLiquidThemedViewEngine>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
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
