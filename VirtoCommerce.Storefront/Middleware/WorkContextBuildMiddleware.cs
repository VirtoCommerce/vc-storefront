using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Middleware
{
    public class WorkContextBuildMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly StorefrontOptions _options;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly Dictionary<string, object> _applicationSettings;
        public WorkContextBuildMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment,
                                          IOptions<StorefrontOptions> options, IWorkContextAccessor workContextAccessor, IConfiguration configuration)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _workContextAccessor = workContextAccessor;
            _options = options.Value;
            _configuration = configuration;

            //Load a user-defined  settings from the special section.
            //All of these settings are accessible from the themes and through access to WorkContext.ApplicationSettings property
            //Trim of the VirtoCommerce:AppSettings added for backward compatibility with old themes
            _applicationSettings = _configuration.GetSection("VirtoCommerce:AppSettings").AsEnumerable().Where(x => x.Value != null)
                                                                                      .ToDictionary(x => x.Key.Replace("VirtoCommerce:AppSettings:", ""), x => (object)x.Value);
        }

        public async Task Invoke(HttpContext context)
        {
            //Do not process for exist exception 
            var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null)
            {
                await _next(context);
                return;
            }

            var builder = new WorkContextBuilder(context);
            var workContext = builder.WorkContext;

            workContext.ApplicationSettings = _applicationSettings;
            //The important to preserve the order of initialization
            await builder.WithCountriesAsync();

            await builder.WithStoresAsync(_options.DefaultStore);
            await builder.WithCurrentUserAsync();
            await builder.WithCurrenciesAsync(workContext.CurrentLanguage, workContext.CurrentStore);

            await builder.WithCatalogsAsync();
            await builder.WithDefaultShoppingCartAsync("default", workContext.CurrentStore, workContext.CurrentUser, workContext.CurrentCurrency, workContext.CurrentLanguage);
            await builder.WithMenuLinksAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithPagesAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithBlogsAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithPricelistsAsync();
            await builder.WithQuotesAsync(workContext.CurrentStore, workContext.CurrentUser, workContext.CurrentCurrency, workContext.CurrentLanguage);
            await builder.WithUserOrdersAsync();
            await builder.WithUserQuotesAsync();
            await builder.WithUserSubscriptionsAsync();
            await builder.WithVendorsAsync(workContext.CurrentStore, workContext.CurrentLanguage);

            workContext.AvailableRoles = SecurityConstants.Roles.AllRoles;
            _workContextAccessor.WorkContext = workContext;


            await _next(context);
        }
    }

}
