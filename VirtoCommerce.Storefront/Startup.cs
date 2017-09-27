using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Authentication;
using VirtoCommerce.Storefront.Binders;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.DependencyInjection;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.JsonConverters;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Bus;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using VirtoCommerce.Storefront.Model.Tax.Services;
using VirtoCommerce.Storefront.Routing;
using VirtoCommerce.Tools;

namespace VirtoCommerce.Storefront
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnviroment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnviroment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();

            services.Configure<StorefrontOptions>(Configuration.GetSection("VirtoCommerce"));

            //The IHttpContextAccessor service is not registered by default
            //https://github.com/aspnet/Hosting/issues/793
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IWorkContextAccessor, WorkContextAccessor>();
            services.AddSingleton<IUrlBuilder, UrlBuilder>();
            services.AddSingleton<IStorefrontUrlBuilder, StorefrontUrlBuilder>();

            services.AddSingleton<IStoreService, StoreService>();
            services.AddSingleton<ICurrencyService, CurrencyService>();
            services.AddSingleton<ISlugRouteService, SlugRouteService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<ICustomerOrderService, CustomerOrderService>();
            services.AddSingleton<IQuoteService, QuoteService>();
            services.AddSingleton<ISubscriptionService, SubscriptionService>();
            services.AddSingleton<ICatalogService, CatalogService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IPricingService, PricingService>();
            services.AddSingleton<ITaxEvaluator, TaxEvaluator>();
            services.AddSingleton<IPromotionEvaluator, PromotionEvaluator>();
            services.AddSingleton<IMarketingService, MarketingService>();
            services.AddSingleton<IProductAvailabilityService, ProductAvailabilityService>();
            services.AddSingleton<IQuoteRequestBuilder, QuoteRequestBuilder>();
            services.AddSingleton<ICartBuilder, CartBuilder>();
            services.AddSingleton<IStaticContentService, StaticContentService>();
            services.AddSingleton<IMenuLinkListService, MenuLinkListServiceImpl>();
            services.AddSingleton<IStaticContentItemFactory, StaticContentItemFactory>();
            services.AddSingleton<AssociationRecommendationsProvider>();
            services.AddSingleton<CognitiveRecommendationsProvider>();
            services.AddSingleton<IRecommendationProviderFactory, RecommendationProviderFactory>(provider => new RecommendationProviderFactory(provider.GetService<AssociationRecommendationsProvider>(), provider.GetService<CognitiveRecommendationsProvider>()));

            //Register events framework dependencies
            services.AddSingleton(new InProcessBus());
            services.AddSingleton<IEventPublisher>(provider => provider.GetService<InProcessBus>());
            services.AddSingleton<IHandlerRegistrar>(provider => provider.GetService<InProcessBus>());

            //Register platform API clients
            services.AddPlatformEndpoint(options =>
            {
                Configuration.GetSection("VirtoCommerce:Endpoint").Bind(options);
            });

            services.AddSingleton<ICountriesService, FileSystemCountriesService>();
            services.Configure<FileSystemCountriesOptions>(options =>
            {
               options.FilePath = HostingEnvironment.MapPath("~/countries.json");
            });

            var contentConnectionString = BlobConnectionString.Parse(Configuration.GetConnectionString("ContentConnectionString"));
            if (contentConnectionString.Provider.EqualsInvariant("AzureBlobStorage"))
            {
                services.AddAzureBlobContent(options =>
                {
                    options.Container = contentConnectionString.RootPath;
                    options.ConnectionString = contentConnectionString.ConnectionString;
                });
            }
            else
            {
                services.AddFileSystemBlobContent(options =>
                {
                    options.Path = HostingEnvironment.MapPath(contentConnectionString.RootPath);
                });
            }

            //Identity overrides for use remote user storage
            services.AddSingleton<IUserStore<CustomerInfo>, CustomUserStore>();
            services.AddSingleton<IUserPasswordStore<CustomerInfo>, CustomUserStore>();
            services.AddSingleton<IUserEmailStore<CustomerInfo>, CustomUserStore>();
            services.AddSingleton<IUserClaimsPrincipalFactory<CustomerInfo>, CustomerInfoPrincipalFactory>();
            services.AddScoped<UserManager<CustomerInfo>, CustomUserManager>();

            services.AddAuthentication();
            services.AddIdentity<CustomerInfo, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            }).AddDefaultTokenProviders();


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(30);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout               
                options.SlidingExpiration = true;
            });

            //Add Liquid view engine
            services.AddLiquidViewEngine(options =>
            {
                Configuration.GetSection("VirtoCommerce:LiquidThemeEngine").Bind(options);
            });

            var snapshotProvider = services.BuildServiceProvider();
            //Register JSON converters to 
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default", new CacheProfile()
                {
                    Duration = 60,
                    VaryByHeader = "host"
                });
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new CartTypesJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new MoneyJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new CurrencyJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new OrderTypesJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new RecommendationJsonConverter(snapshotProvider.GetService<IRecommendationProviderFactory>()));
            }).AddViewOptions(options =>
            {
                options.ViewEngines.Add(snapshotProvider.GetService<ILiquidViewEngine>());
            });


            //Register event handlers via reflection
            services.RegisterAssembliesEventHandlers(typeof(Startup));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
            }

            app.UseResponseCaching();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMiddleware<WorkContextBuildMiddleware>();
            app.UseMiddleware<StoreMaintenanceMiddleware>();
            app.UseMiddleware<NoLiquidThemeMiddleware>();
            app.UseMiddleware<ApiErrorHandlingMiddleware>();

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            var options = new RewriteOptions().Add(new StorefrontUrlNormalizeRule());
            app.UseRewriter(options);
            app.UseMvc(routes =>
            {
                routes.MapStorefrontRoutes();
            });


        }
    }
}
