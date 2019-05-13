using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Polly;
using Polly.Extensions.Http;
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
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Infrastructure.Autorest;
using VirtoCommerce.Storefront.Model.Common.Bus;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Common.Messages;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.DependencyInjection
{

    public static class PollyPolicyName
    {
        public const string HttpCircuitBreaker = nameof(HttpCircuitBreaker);
        public const string HttpRetry = nameof(HttpRetry);
    }

    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Extention method for add Polly policies to shared policy registry service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddPollyPolicies(this IServiceCollection services, Action<PollyPoliciesOptions> setupAction = null)
        {
            var policyOptions = new PollyPoliciesOptions();
            setupAction?.Invoke(policyOptions);

            var policyRegistry = services.AddPolicyRegistry();

            policyRegistry.Add(
                PollyPolicyName.HttpRetry,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));

            policyRegistry.Add(
                PollyPolicyName.HttpCircuitBreaker,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }

        /// <summary>
        /// Add AutoRest generated module and client to services
        /// </summary>
        /// <typeparam name="TIModule"></typeparam>
        /// <typeparam name="TModuleImplementation"></typeparam>
        /// <typeparam name="TInnerServiceClient"></typeparam>
        /// <param name="services"></param>
        /// <param name="crateModuleImplementationFunc"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoRestClient<TIModule, TModuleImplementation, TInnerServiceClient>(this IServiceCollection services, Func<TInnerServiceClient, TModuleImplementation> crateModuleImplementationFunc)
            where TIModule : class
            where TModuleImplementation : class, TIModule
            where TInnerServiceClient : ServiceClient<TInnerServiceClient>
        {
            if (!services.Any(x => x.ServiceType == typeof(TInnerServiceClient)))
            {
                services.AddHttpClient<TInnerServiceClient>()
                   .ConfigureHttpClient((sp, httpClient) =>
                   {
                       var platformEndpointOptions = sp.GetRequiredService<IOptions<PlatformEndpointOptions>>().Value;
                       httpClient.BaseAddress = platformEndpointOptions.Url;
                       httpClient.Timeout = platformEndpointOptions.RequestTimeout;
                   })
                   .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                   .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
                   .AddHttpMessageHandler(sp => sp.GetService<AuthenticationHandlerFactory>().CreateAuthHandler())
                   .AddPolicyHandlerFromRegistry(PollyPolicyName.HttpRetry)
                   .AddPolicyHandlerFromRegistry(PollyPolicyName.HttpCircuitBreaker);
            }

            services.AddSingleton<TIModule>(sp => crateModuleImplementationFunc(sp.GetRequiredService<TInnerServiceClient>()));

            return services;
        }

        public static void AddPlatformEndpoint(this IServiceCollection services, Action<PlatformEndpointOptions> setupAction = null)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            services.AddSingleton<ServiceClientCredentials>(sp => new EmptyServiceClientCredentials());
            services.AddTransient<ApiKeySecretAuthHandler>();
            services.AddTransient<UserPasswordAuthHandler>();
            services.AddSingleton<AuthenticationHandlerFactory>();
            services.AddHttpClient();
            services.AddAutoRestClient<IStoreModule, StoreModule, VirtoCommerceStoreRESTAPIdocumentation>(x => new StoreModule(x));
            services.AddAutoRestClient<ICommerce, Commerce, VirtoCommerceCoreRESTAPIdocumentation>(x => new Commerce(x));
            services.AddAutoRestClient<ICatalogModuleCategories, CatalogModuleCategories, VirtoCommerceCatalogRESTAPIdocumentation>(x => new CatalogModuleCategories(x));
            services.AddAutoRestClient<ICatalogModuleProducts, CatalogModuleProducts, VirtoCommerceCatalogRESTAPIdocumentation>(x => new CatalogModuleProducts(x));
            services.AddAutoRestClient<ICatalogModuleSearch, CatalogModuleSearch, VirtoCommerceCatalogRESTAPIdocumentation>(x => new CatalogModuleSearch(x));
            services.AddAutoRestClient<ISecurity, Security, VirtoCommercePlatformRESTAPIdocumentation>(x => new Security(x));
            services.AddAutoRestClient<IStorefrontSecurity, StorefrontSecurity, VirtoCommerceCoreRESTAPIdocumentation>(x => new StorefrontSecurity(x));
            services.AddAutoRestClient<ICustomerModule, CustomerModule, VirtoCommerceCustomerRESTAPIdocumentation>(x => new CustomerModule(x));
            services.AddAutoRestClient<IOrderModule, OrderModule, VirtoCommerceOrdersRESTAPIdocumentation>(x => new OrderModule(x));
            services.AddAutoRestClient<IQuoteModule, QuoteModule, VirtoCommerceQuoteRESTAPIdocumentation>(x => new QuoteModule(x));
            //services.AddAutoRestClient<ISubscriptionModule, SubscriptionModule, VirtoCommerceSubscriptionRESTAPIdocumentationExtended>(x => new SubscriptionModule(x));
            services.AddAutoRestClient<IInventoryModule, InventoryModule, VirtoCommerceInventoryRESTAPIdocumentation>(x => new InventoryModule(x));
            services.AddAutoRestClient<IMarketingModulePromotion, MarketingModulePromotion, VirtoCommerceMarketingRESTAPIdocumentation>(x => new MarketingModulePromotion(x));
            services.AddAutoRestClient<IMarketingModuleDynamicContent, MarketingModuleDynamicContent, VirtoCommerceMarketingRESTAPIdocumentation>(x => new MarketingModuleDynamicContent(x));
            services.AddAutoRestClient<IPricingModule, PricingModule, VirtoCommercePricingRESTAPIdocumentation>(x => new PricingModule(x));
            services.AddAutoRestClient<ICartModule, CartModule, VirtoCommerceCartRESTAPIdocumentation>(x => new CartModule(x));
            services.AddAutoRestClient<IMenu, Menu, VirtoCommerceContentRESTAPIdocumentation>(x => new Menu(x));
            services.AddAutoRestClient<IContent, Content, VirtoCommerceContentRESTAPIdocumentation>(x => new Content(x));
            services.AddAutoRestClient<IRecommendations, Recommendations, VirtoCommerceProductRecommendationsRESTAPIdocumentation>(x => new Recommendations(x));
            services.AddAutoRestClient<ISitemapsModuleApiOperations, SitemapsModuleApiOperations, VirtoCommerceSitemapsRESTAPIdocumentation>(x => new SitemapsModuleApiOperations(x));
            services.AddAutoRestClient<ICacheModule, CacheModule, VirtoCommerceCacheRESTAPIdocumentation>(x => new CacheModule(x));
            services.AddAutoRestClient<INotifications, Notifications, VirtoCommercePlatformRESTAPIdocumentation>(x => new Notifications(x));

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }


            //services.AddHttpClient()

            services.AddHttpClient<VirtoCommerceSubscriptionRESTAPIdocumentationExtended>()
                  .ConfigureHttpClient((sp, httpClient) =>
                  {

                      var platformEndpointOptions = sp.GetRequiredService<IOptions<PlatformEndpointOptions>>().Value;
                      httpClient.BaseAddress = platformEndpointOptions.Url;
                      httpClient.Timeout = platformEndpointOptions.RequestTimeout;
                  })
                  .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                  .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
                  .AddHttpMessageHandler(sp => sp.GetService<AuthenticationHandlerFactory>().CreateAuthHandler())
                  .AddPolicyHandlerFromRegistry(PollyPolicyName.HttpRetry)
                  .AddPolicyHandlerFromRegistry(PollyPolicyName.HttpCircuitBreaker);

            services.AddSingleton<ISubscriptionModule>(sp => new SubscriptionModule(sp.GetRequiredService<VirtoCommerceSubscriptionRESTAPIdocumentationExtended>()));


            //var sp2 = services.BuildServiceProvider();
            //var platformOps = sp2.GetRequiredService<IOptions<PlatformEndpointOptions>>().Value;
            //var subsClient = sp2.GetRequiredService<VirtoCommerceSubscriptionRESTAPIdocumentation>();
            //subsClient.BaseUri = platformOps.Url;

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
                    .AddClasses(classes => classes.Where(x =>
                    {
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
