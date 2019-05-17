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
        private const string PlatformEndpointHttpClientName = "PlatformEndpoint";

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
        /// Add common http clients handlers and pollicy that will be used to communicate with platform
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddPlatformEnpointHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient(PlatformEndpointHttpClientName)
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

            return services;
        }
        /// <summary>
        ///  init autorest generated ServiceClient instance with platform enpoint HttpClient and add it into DI services as singlton
        /// </summary>
        /// <typeparam name="TServiceClient"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceClietnFactory"></param>
        /// <returns></returns>
        private static IServiceCollection AddAutoRestClient<TServiceClient>(this IServiceCollection services, Func<ServiceClientCredentials, HttpClient, bool, TServiceClient> serviceClietnFactory)
            where TServiceClient : ServiceClient<TServiceClient>
        {
            services.AddSingleton<TServiceClient>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(PlatformEndpointHttpClientName);
                var serviceClient = serviceClietnFactory(new EmptyServiceClientCredentials(), httpClient, true);
                return serviceClient;
            });

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
            services.AddPlatformEnpointHttpClient();
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceStoreRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IStoreModule>(sp => new StoreModule(sp.GetRequiredService<VirtoCommerceStoreRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceCoreRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IStorefrontSecurity>(sp => new StorefrontSecurity(sp.GetRequiredService<VirtoCommerceCoreRESTAPIdocumentation>()));
            services.AddSingleton<ICommerce>(sp => new Commerce(sp.GetRequiredService<VirtoCommerceCoreRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceCatalogRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<ICatalogModuleCategories>(sp => new CatalogModuleCategories(sp.GetRequiredService<VirtoCommerceCatalogRESTAPIdocumentation>()));
            services.AddSingleton<ICatalogModuleProducts>(sp => new CatalogModuleProducts(sp.GetRequiredService<VirtoCommerceCatalogRESTAPIdocumentation>()));
            services.AddSingleton<ICatalogModuleSearch>(sp => new CatalogModuleSearch(sp.GetRequiredService<VirtoCommerceCatalogRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommercePlatformRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<ISecurity>(sp => new Security(sp.GetRequiredService<VirtoCommercePlatformRESTAPIdocumentation>()));
            services.AddSingleton<INotifications>(sp => new Notifications(sp.GetRequiredService<VirtoCommercePlatformRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceCustomerRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<ICustomerModule>(sp => new CustomerModule(sp.GetRequiredService<VirtoCommerceCustomerRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceOrdersRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IOrderModule>(sp => new OrderModule(sp.GetRequiredService<VirtoCommerceOrdersRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceSubscriptionRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<ISubscriptionModule>(sp => new SubscriptionModule(sp.GetRequiredService<VirtoCommerceSubscriptionRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceInventoryRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IInventoryModule>(sp => new InventoryModule(sp.GetRequiredService<VirtoCommerceInventoryRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceMarketingRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IMarketingModulePromotion>(sp => new MarketingModulePromotion(sp.GetRequiredService<VirtoCommerceMarketingRESTAPIdocumentation>()));
            services.AddSingleton<IMarketingModuleDynamicContent>(sp => new MarketingModuleDynamicContent(sp.GetRequiredService<VirtoCommerceMarketingRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommercePricingRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IPricingModule>(sp => new PricingModule(sp.GetRequiredService<VirtoCommercePricingRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceCartRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<ICartModule>(sp => new CartModule(sp.GetRequiredService<VirtoCommerceCartRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceContentRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IMenu>(sp => new Menu(sp.GetRequiredService<VirtoCommerceContentRESTAPIdocumentation>()));
            services.AddSingleton<IContent>(sp => new Content(sp.GetRequiredService<VirtoCommerceContentRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceProductRecommendationsRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IRecommendations>(sp => new Recommendations(sp.GetRequiredService<VirtoCommerceProductRecommendationsRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceSitemapsRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<ISitemapsModuleApiOperations>(sp => new SitemapsModuleApiOperations(sp.GetRequiredService<VirtoCommerceSitemapsRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceCacheRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<ICacheModule>(sp => new CacheModule(sp.GetRequiredService<VirtoCommerceCacheRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient) => new VirtoCommerceQuoteRESTAPIdocumentation(credentials, httpClient, disposeHttpClient));
            services.AddSingleton<IQuoteModule>(sp => new QuoteModule(sp.GetRequiredService<VirtoCommerceQuoteRESTAPIdocumentation>()));

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
