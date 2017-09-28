using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Authentication;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Middleware
{
    public class WorkContextBuildMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly StorefrontOptions _options;
        private readonly IWorkContextAccessor _workContextAccessor;
        public WorkContextBuildMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment,
                                          IOptions<StorefrontOptions> options, IWorkContextAccessor workContextAccessor)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _workContextAccessor = workContextAccessor;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var builder = new WorkContextBuilder(context);
            var workContext = builder.WorkContext;

            workContext.ApplicationSettings = _options.Settings;

            await builder.WithCountriesAsync();

            await builder.WithCurrentUserAsync();
            await builder.WithStoresAsync(_options.DefaultStore);

            //Set current language
            var availLanguages = workContext.AllStores.SelectMany(s => s.Languages)
                                  .Union(workContext.AllStores.Select(s => s.DefaultLanguage)).Distinct().ToList();
            await builder.WithCurrentLanguageForStore(availLanguages, workContext.CurrentStore);

            await builder.WithCurrenciesAsync();
            await builder.WithCatalogsAsync();
            await builder.WithDefaultShoppingCartAsync("default", workContext.CurrentStore, workContext.CurrentCustomer, workContext.CurrentCurrency, workContext.CurrentLanguage);
            await builder.WithMenuLinksAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithPagesAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithBlogsAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithPricelistsAsync();
            await builder.WithQuotesAsync(workContext.CurrentStore, workContext.CurrentCustomer, workContext.CurrentCurrency, workContext.CurrentLanguage);
            await builder.WithVendorsAsync(workContext.CurrentStore, workContext.CurrentLanguage);

            _workContextAccessor.WorkContext = workContext;

            await _next(context);
        }
    }

}
