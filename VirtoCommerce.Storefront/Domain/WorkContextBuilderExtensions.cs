using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class WorkContextBuilderExtensions
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

        public static Task WithCurrentLanguageForStore(this IWorkContextBuilder builder, IList<Language> availLanguages, Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            //Try to get language from request
            var currentLanguage = store.DefaultLanguage;
            var regexpPattern = string.Format(@"\/({0})\/?", string.Join("|", availLanguages));
            var match = Regex.Match(builder.HttpContext.Request.Path, regexpPattern, RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                var language = new Language(match.Groups[1].Value);
                //Get store default language if language not in the supported by stores list
                currentLanguage = store.Languages.Contains(language) ? language : currentLanguage;
            }
            builder.WorkContext.CurrentLanguage = currentLanguage;
            return Task.CompletedTask;
        }


        public static Task WithCurrenciesAsync(this IWorkContextBuilder builder, IList<Currency> currencies)
        {
            if (currencies == null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }
            if (builder.WorkContext.AllStores == null)
            {
                throw new StorefrontException("Unable to set currencies without stores");
            }         
            if (builder.WorkContext.CurrentStore == null)
            {
                throw new StorefrontException("Unable to set currencies without current store");
            }

            builder.WorkContext.AllCurrencies = currencies;

            //Sync store currencies with avail in system
            foreach (var store in builder.WorkContext.AllStores)
            {
                store.SyncCurrencies(builder.WorkContext.AllCurrencies, builder.WorkContext.CurrentLanguage);
                store.CurrentSeoInfo = store.SeoInfos.FirstOrDefault(x => x.Language == builder.WorkContext.CurrentLanguage);
            }

            //Set current currency
            //Get currency from request url  
            StringValues currencyCode;
            if (!builder.HttpContext.Request.Query.TryGetValue("currency", out currencyCode))
            {
                //Next try get from claims
                currencyCode = builder.HttpContext.User.FindFirstValue(StorefrontClaims.CurrencyClaimType);
            }
            var currentCurrency = builder.WorkContext.CurrentStore.DefaultCurrency;
            //Get store default currency if currency not in the supported by stores list
            if (!string.IsNullOrEmpty(currencyCode))
            {
                currentCurrency = builder.WorkContext.CurrentStore.Currencies.FirstOrDefault(x => x.Equals(currencyCode)) ?? currentCurrency;
            }

            builder.WorkContext.CurrentCurrency = currentCurrency;

            return Task.CompletedTask;
        }

        public static async Task WithCurrenciesAsync(this IWorkContextBuilder builder)
        {
            if (builder.WorkContext.CurrentLanguage == null)
            {
                throw new StorefrontException("Unable to set currencies without language");
            }

            var serviceProvider = builder.HttpContext.RequestServices;
            var storeService = serviceProvider.GetRequiredService<ICurrencyService>();

            var currencies = await storeService.GetAllCurrenciesAsync(builder.WorkContext.CurrentLanguage);

            await builder.WithCurrenciesAsync(currencies);
        }

    }
}
