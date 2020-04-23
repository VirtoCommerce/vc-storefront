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
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.AutoRestClients.CacheModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.ContentModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.NotificationsModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PaymentModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.ShippingModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.SitemapsModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.TaxModuleApi;
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
                   .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseCookies = false })
                   .AddHttpMessageHandler(sp => sp.GetService<AuthenticationHandlerFactory>().CreateAuthHandler());


            return services;
        }
        /// <summary>
        ///  init autorest generated ServiceClient instance with platform enpoint HttpClient and add it into DI services as singlton
        /// </summary>
        /// <typeparam name="TServiceClient"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceClientFactory"></param>
        /// <returns></returns>
        private static IServiceCollection AddAutoRestClient<TServiceClient>(this IServiceCollection services, Func<ServiceClientCredentials, HttpClient, bool, Uri, TServiceClient> serviceClientFactory)
            where TServiceClient : ServiceClient<TServiceClient>
        {
            services.AddSingleton(sp =>
            {
                var platformEndpointOptions = sp.GetRequiredService<IOptions<PlatformEndpointOptions>>().Value;
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(PlatformEndpointHttpClientName);
                var serviceClient = serviceClientFactory(new EmptyServiceClientCredentials(), httpClient, true, platformEndpointOptions.Url);
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
            services.AddTransient<ClientCredentialsAuthHandler>();
            services.AddSingleton<AuthenticationHandlerFactory>();
            services.AddHttpClient();
            services.AddPlatformEnpointHttpClient();
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new StoreModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IStoreModule>(sp => new StoreModule(sp.GetRequiredService<StoreModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new CoreModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ICommerce>(sp => new Commerce(sp.GetRequiredService<CoreModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new CatalogModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ICatalogModuleCategories>(sp => new CatalogModuleCategories(sp.GetRequiredService<CatalogModuleClient>()));
            services.AddSingleton<ICatalogModuleProducts>(sp => new CatalogModuleProducts(sp.GetRequiredService<CatalogModuleClient>()));
            services.AddSingleton<ICatalogModuleIndexedSearch>(sp => new CatalogModuleIndexedSearch(sp.GetRequiredService<CatalogModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new PlatformModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ISecurity>(sp => new Security(sp.GetRequiredService<PlatformModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new NotificationsModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<INotifications>(sp => new Notifications(sp.GetRequiredService<NotificationsModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new CustomerModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ICustomerModule>(sp => new CustomerModule(sp.GetRequiredService<CustomerModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new OrdersModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IOrderModule>(sp => new OrderModule(sp.GetRequiredService<OrdersModuleClient>()));
            services.AddSingleton<IOrderModulePayments>(sp => new OrderModulePayments(sp.GetRequiredService<OrdersModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new SubscriptionModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ISubscriptionModule>(sp => new SubscriptionModule(sp.GetRequiredService<SubscriptionModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new InventoryModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IInventoryModule>(sp => new InventoryModule(sp.GetRequiredService<InventoryModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new MarketingModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IMarketingModulePromotion>(sp => new MarketingModulePromotion(sp.GetRequiredService<MarketingModuleClient>()));
            services.AddSingleton<IMarketingModuleDynamicContent>(sp => new MarketingModuleDynamicContent(sp.GetRequiredService<MarketingModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new PricingModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IPricingModule>(sp => new PricingModule(sp.GetRequiredService<PricingModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new CartModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ICartModule>(sp => new CartModule(sp.GetRequiredService<CartModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new ContentModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IMenu>(sp => new Menu(sp.GetRequiredService<ContentModuleClient>()));
            services.AddSingleton<IContent>(sp => new Content(sp.GetRequiredService<ContentModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new VirtoCommerceProductRecommendationsRESTAPIdocumentation(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IRecommendations>(sp => new Recommendations(sp.GetRequiredService<VirtoCommerceProductRecommendationsRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new SitemapsModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ISitemapsModuleApiOperations>(sp => new SitemapsModuleApiOperations(sp.GetRequiredService<SitemapsModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new VirtoCommerceCacheRESTAPIdocumentation(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ICacheModule>(sp => new CacheModule(sp.GetRequiredService<VirtoCommerceCacheRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new VirtoCommerceQuoteRESTAPIdocumentation(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IQuoteModule>(sp => new QuoteModule(sp.GetRequiredService<VirtoCommerceQuoteRESTAPIdocumentation>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new PaymentModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IPaymentModule>(sp => new PaymentModule(sp.GetRequiredService<PaymentModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new ShippingModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<IShippingModule>(sp => new ShippingModule(sp.GetRequiredService<ShippingModuleClient>()));
            services.AddAutoRestClient((credentials, httpClient, disposeHttpClient, baseUri) => new TaxModuleClient(credentials, httpClient, disposeHttpClient) { BaseUri = baseUri });
            services.AddSingleton<ITaxModule>(sp => new TaxModule(sp.GetRequiredService<TaxModuleClient>()));

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

            services.AddSingleton<ISassFileManager, SassFileManager>();
            services.AddSingleton<ILiquidThemeEngine, ShopifyLiquidThemeEngine>();
            services.AddSingleton<ILiquidViewEngine, LiquidThemedViewEngine>();
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
