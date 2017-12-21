using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Binders;
using VirtoCommerce.Storefront.DependencyInjection;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.JsonConverters;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Bus;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Security;
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
            services.AddSingleton<IMemberService, MemberService>();
            services.AddSingleton<ICustomerOrderService, CustomerOrderService>();
            services.AddSingleton<IQuoteService, QuoteService>();
            services.AddSingleton<ISubscriptionService, SubscriptionService>();
            services.AddSingleton<ICatalogService, CatalogService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IPricingService, PricingService>();
            services.AddSingleton<ITaxEvaluator, TaxEvaluator>();
            services.AddSingleton<IPromotionEvaluator, PromotionEvaluator>();
            services.AddSingleton<IMarketingService, MarketingService>();
            services.AddSingleton<IStaticContentService, StaticContentService>();
            services.AddSingleton<IMenuLinkListService, MenuLinkListServiceImpl>();
            services.AddSingleton<IStaticContentItemFactory, StaticContentItemFactory>();
            services.AddSingleton<IApiChangesWatcher, ApiChangesWatcher>();
            services.AddSingleton<AssociationRecommendationsProvider>();
            services.AddSingleton<CognitiveRecommendationsProvider>();
            services.AddSingleton<IRecommendationProviderFactory, RecommendationProviderFactory>(provider => new RecommendationProviderFactory(provider.GetService<AssociationRecommendationsProvider>(), provider.GetService<CognitiveRecommendationsProvider>()));
            services.AddTransient<IQuoteRequestBuilder, QuoteRequestBuilder>();
            services.AddTransient<ICartBuilder, CartBuilder>();

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
            services.AddSingleton<IUserStore<User>, UserStoreStub>();
            services.AddSingleton<IUserClaimsPrincipalFactory<User>, UserPrincipalFactory>();
            services.AddScoped<UserManager<User>, CustomUserManager>();

            //Resource-based authorization that requires API permissions for some operations
            services.AddSingleton<IAuthorizationHandler, StorefrontAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanImpersonate",
                                  policy => policy.Requirements.Add(AuthorizationOperations.CanImpersonate));
                options.AddPolicy("CanResetCache",
                                policy => policy.Requirements.Add(AuthorizationOperations.CanResetCache));
            });

            var auth = services.AddAuthentication();

            var facebookSection = Configuration.GetSection("Authentication:Facebook");
            if (facebookSection.GetChildren().Any())
            {
                auth.AddFacebook(facebookOptions =>
                {
                    facebookSection.Bind(facebookOptions);
                });
            }
            var googleSection = Configuration.GetSection("Authentication:Google");
            if (googleSection.GetChildren().Any())
            {
                auth.AddGoogle(googleOptions =>
                {
                    googleSection.Bind(googleOptions);
                });
            }
            var githubSection = Configuration.GetSection("Authentication:Github");
            if (githubSection.GetChildren().Any())
            {
                auth.AddGitHub(GitHubAuthenticationOptions =>
                {
                    githubSection.Bind(GitHubAuthenticationOptions);
                });
            }
            var stackexchangeSection = Configuration.GetSection("Authentication:Stackexchange");
            if (stackexchangeSection.GetChildren().Any())
            {
                auth.AddStackExchange(StackExchangeAuthenticationOptions =>
                {
                    stackexchangeSection.Bind(StackExchangeAuthenticationOptions);
                });
            }
            services.AddIdentity<User, IdentityRole>(options =>
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
                options.AccessDeniedPath = "/error/AccessDenied";
                options.SlidingExpiration = true;
            });
            
            //Add Liquid view engine
            services.AddLiquidViewEngine(options =>
            {
                Configuration.GetSection("VirtoCommerce:LiquidThemeEngine").Bind(options);
            });

            var snapshotProvider = services.BuildServiceProvider();
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default", new CacheProfile()
                {
                    Duration = (int)TimeSpan.FromHours(1).TotalSeconds,
                    VaryByHeader = "host"
                });
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new CartTypesJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new MoneyJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new CurrencyJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new OrderTypesJsonConverter(snapshotProvider.GetService<IWorkContextAccessor>()));
                options.SerializerSettings.Converters.Add(new RecommendationJsonConverter(snapshotProvider.GetService<IRecommendationProviderFactory>()));
                //Converter for providing back compatibility with old themes was used CustomerInfo type which has contained user and contact data in the single type.
                //May be removed when all themes will fixed to new User type with nested Contact property.
                options.SerializerSettings.Converters.Add(new UserBackwardCompatibilityJsonConverter(options.SerializerSettings));
            }).AddViewOptions(options =>
            {
                options.ViewEngines.Add(snapshotProvider.GetService<ILiquidViewEngine>());
            });

            //Register event handlers via reflection
            services.RegisterAssembliesEventHandlers(typeof(Startup));

            services.AddApplicationInsightsTelemetry();
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
            var requireHttpsOptions = new RequireHttpsOptions();
            Configuration.GetSection("VirtoCommerce:RequireHttps").Bind(requireHttpsOptions);
            if (requireHttpsOptions.Enabled)
            {
                options.AddRedirectToHttps(requireHttpsOptions.StatusCode,  requireHttpsOptions.Port);
            }
            app.UseRewriter(options);
            app.UseMvc(routes =>
            {
                routes.MapStorefrontRoutes();
            });


        }
    }
}
