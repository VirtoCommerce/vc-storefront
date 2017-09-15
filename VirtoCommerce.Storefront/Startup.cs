using CacheManager.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Builders;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.DependencyInjection;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Filters;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Order.Events;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Quote.Events;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using VirtoCommerce.Storefront.Model.Tax.Services;
using VirtoCommerce.Storefront.Models;
using VirtoCommerce.Storefront.Routing;
using VirtoCommerce.Storefront.Services;
using VirtoCommerce.Storefront.Services.Identity;
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
            //The IHttpContextAccessor service is not registered by default
            //https://github.com/aspnet/Hosting/issues/793
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IWorkContextAccessor, WorkContextAccessor>();
            services.AddSingleton<IUrlBuilder, UrlBuilder>();
            services.AddSingleton<IStorefrontUrlBuilder, StorefrontUrlBuilder>();

            services.AddSingleton<ICountriesService, JsonCountriesService>(provider => new JsonCountriesService(provider.GetService<ICacheManager<object>>(), HostingEnvironment.MapPath("~/countries.json")));
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

            //TODO: replace to  Event bus publisher
            //Register domain events
            services.AddSingleton<IEventPublisher<OrderPlacedEvent>, EventPublisher<OrderPlacedEvent>>();
            services.AddSingleton<IEventPublisher<UserLoginEvent>, EventPublisher<UserLoginEvent>>();
            services.AddSingleton<IEventPublisher<QuoteRequestUpdatedEvent>, EventPublisher<QuoteRequestUpdatedEvent>>();

          

            //Register platform API clients
            services.AddPlatformApi(Configuration, HostingEnvironment);

            services.AddCache(Configuration, HostingEnvironment);

            services.AddContentBlobServices(Configuration, HostingEnvironment);

            services.AddLiquidViewEngine(Configuration);

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

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(WorkContextAuthPopulateFilter)); // by type
            }
            );


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

            app.UseStaticFiles();

            app.UseMiddleware<WorkContextPopulateMiddleware>();
            app.UseMiddleware<StoreMaintenanceMiddleware>();
            app.UseMiddleware<NoLiquidThemeMiddleware>();

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseAuthentication();

            var options = new RewriteOptions().Add(new StorefrontUrlNormalizeRule());
            app.UseRewriter(options);
            app.UseMvc(routes =>
            {
                routes.MapStorefrontRoutes();
            });


        }
    }
}
