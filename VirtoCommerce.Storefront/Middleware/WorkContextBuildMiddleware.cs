using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Data.Stores;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Cart.Services;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Quote.Services;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Common;

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
            var serviceProvider = context.RequestServices;

            var builder = WorkContextBuilder.FromHttpContext(context);
            builder.WithSettings(_options);
            builder.WithCountries(serviceProvider.GetRequiredService<ICountriesService>());
            await builder.WithAuthAsync(serviceProvider.GetRequiredService<SignInManager<CustomerInfo>>());
            await builder.WithStoresAsync(serviceProvider.GetRequiredService<IStoreService>(), _options.DefaultStore);
            await builder.WithCurrenciesAsync(serviceProvider.GetRequiredService<ICurrencyService>());
            await builder.WithCatalogsAsync(serviceProvider.GetRequiredService<ICatalogService>());
            await builder.WithShoppingCartAsync("default", serviceProvider.GetRequiredService<ICartBuilder>());
            await builder.WithStaticContentAsync(serviceProvider.GetRequiredService<IMenuLinkListService>(), serviceProvider.GetRequiredService<IStaticContentService>());
            await builder.WithPricelistsAsync(serviceProvider.GetRequiredService<IPricingService>());
            await builder.WithQuotesAsync(serviceProvider.GetRequiredService<IQuoteRequestBuilder>());
            builder.WithVendors(serviceProvider.GetRequiredService<ICustomerService>(), serviceProvider.GetRequiredService<ICatalogService>());
            _workContextAccessor.WorkContext = builder.Build();

            await _next(context);
        }
    }

}
