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
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.DependencyInjection;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
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
