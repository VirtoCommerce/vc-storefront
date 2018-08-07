using System;
using System.IO;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Binders;
using VirtoCommerce.Storefront.DependencyInjection;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Domain.Cart;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Infrastructure.ApplicationInsights;
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

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

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
            services.AddSingleton<IBlobChangesWatcher, BlobChangesWatcher>();
            services.AddTransient<ICartBuilder, CartBuilder>();
            services.AddTransient<ICartService, CartService>();

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
                var azureBlobOptions = new AzureBlobContentOptions();
                Configuration.GetSection("VirtoCommerce:AzureBlobStorage").Bind(azureBlobOptions);

                services.AddAzureBlobContent(options =>
                {
                    options.Container = contentConnectionString.RootPath;
                    options.ConnectionString = contentConnectionString.ConnectionString;
                    options.PollForChanges = azureBlobOptions.PollForChanges;
                    options.ChangesPoolingInterval = azureBlobOptions.ChangesPoolingInterval;
                });
            }
            else
            {
                var fileSystemBlobOptions = new FileSystemBlobContentOptions();
                Configuration.GetSection("VirtoCommerce:FileSystemBlobStorage").Bind(fileSystemBlobOptions);
                services.AddFileSystemBlobContent(options =>
                {
                    options.Path = HostingEnvironment.MapPath(contentConnectionString.RootPath);
                });
            }

            //Identity overrides for use remote user storage
            services.AddScoped<IUserStore<User>, UserStoreStub>();
            services.AddScoped<IRoleStore<Role>, UserStoreStub>();
            services.AddScoped<UserManager<User>, CustomUserManager>();

            //Resource-based authorization that requires API permissions for some operations
            services.AddSingleton<IAuthorizationHandler, CanImpersonateAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanReadContentItemAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, OnlyRegisteredUserAuthorizationHandler>();
            // register the AuthorizationPolicyProvider which dynamically registers authorization policies for each permission defined in the platform 
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            //Storefront authorization handler for policy based on permissions 
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanEditOrganizationResourceAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CanImpersonateAuthorizationRequirement.PolicyName,
                                policy => policy.Requirements.Add(new CanImpersonateAuthorizationRequirement()));
                options.AddPolicy(CanReadContentItemAuthorizeRequirement.PolicyName,
                                policy => policy.Requirements.Add(new CanReadContentItemAuthorizeRequirement()));
                options.AddPolicy(CanEditOrganizationResourceAuthorizeRequirement.PolicyName,
                                policy => policy.Requirements.Add(new CanEditOrganizationResourceAuthorizeRequirement()));
                options.AddPolicy(OnlyRegisteredUserAuthorizationRequirement.PolicyName,
                                policy => policy.Requirements.Add(new OnlyRegisteredUserAuthorizationRequirement()));
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

            //This line is required in order to use the old Identity V2 hashes to prevent rehashes passwords for platform users which login in the storefront
            //and it can lead to platform access denied for them. (TODO: Need to remove after platform migration to .NET Core)
            services.Configure<PasswordHasherOptions>(option => option.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2);
            services.Configure<IdentityOptions>(Configuration.GetSection("IdentityOptions"));
            services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, Configuration.GetSection("CookieAuthenticationOptions"));
            services.AddIdentity<User, Role>(options => { }).AddDefaultTokenProviders();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // The Tempdata provider cookie is not essential. Make it essential
            // so Tempdata is functional when tracking is disabled.
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            //Add Liquid view engine
            services.AddLiquidViewEngine(options =>
            {
                Configuration.GetSection("VirtoCommerce:LiquidThemeEngine").Bind(options);
            });

            var snapshotProvider = services.BuildServiceProvider();
            services.AddMvc(options =>
            {
                //Workaround to avoid 'Null effective policy causing exception' (on logout)
                //https://github.com/aspnet/Mvc/issues/7809
                //TODO: Try to remove in ASP.NET Core 2.2
                options.AllowCombiningAuthorizeFilters = false;


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
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
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
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            //Register event handlers via reflection
            services.RegisterAssembliesEventHandlers(typeof(Startup));

            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsExtensions();
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
                app.UseHsts();
            }
            //Do not write telemetry to debug output 
            TelemetryDebugWriter.IsTracingDisabled = true;

            app.UseResponseCaching();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            //WorkContextBuildMiddleware must  always be registered first in  the Middleware chain
            app.UseMiddleware<WorkContextBuildMiddleware>();
            app.UseMiddleware<StoreMaintenanceMiddleware>();
            app.UseMiddleware<NoLiquidThemeMiddleware>();
            app.UseMiddleware<CreateStorefrontRolesMiddleware>();
            app.UseMiddleware<AntiforgeryTokenMiddleware>();
            app.UseMiddleware<ApiErrorHandlingMiddleware>();


            app.UseStatusCodePagesWithReExecute("/error/{0}");

            var rewriteOptions = new RewriteOptions();
            //Load IIS url rewrite rules from external file
            if (File.Exists("IISUrlRewrite.xml"))
            {
                using (var iisUrlRewriteStreamReader = File.OpenText("IISUrlRewrite.xml"))
                {
                    rewriteOptions.AddIISUrlRewrite(iisUrlRewriteStreamReader);
                }
            }
            rewriteOptions.Add(new StorefrontUrlNormalizeRule());

            var requireHttpsOptions = new RequireHttpsOptions();
            Configuration.GetSection("VirtoCommerce:RequireHttps").Bind(requireHttpsOptions);
            if (requireHttpsOptions.Enabled)
            {
                rewriteOptions.AddRedirectToHttps(requireHttpsOptions.StatusCode, requireHttpsOptions.Port);
            }
            app.UseRewriter(rewriteOptions);
            //Enable browser XSS protection
            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Xss-Protection"] = "1";
                await next();
            });
            app.UseMvc(routes =>
            {
                routes.MapStorefrontRoutes();
            });

        }
    }
}
