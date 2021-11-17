using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CountriesWorkContextBuilderExtensions
    {
        public static Task WithCountriesAsync(this IWorkContextBuilder builder, IList<Country> countries)
        {
            builder.WorkContext.AllCountries = countries;
            return Task.CompletedTask;
        }

        public static async Task WithCountriesAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var countryService = serviceProvider.GetRequiredService<ICountriesService>();
            var countries = await countryService.GetCountriesAsync();
            await builder.WithCountriesAsync(countries);
        }


    }
}
