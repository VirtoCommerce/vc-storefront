using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Common.Exceptions;

namespace VirtoCommerce.Storefront.Data.Stores
{
    public partial class WorkContextBuilder
    {
        private WorkContextBuilder(WorkContext workContext, HttpContext httpContext)
        {
            WorkContext = workContext;
            HttpContext = httpContext;
        }

        public static WorkContextBuilder FromHttpContext(HttpContext httpContext)
        {
            var result = new WorkContextBuilder(new WorkContext(), httpContext);
            var qs = httpContext.Request.Query;
            result.WorkContext.RequestUrl = httpContext.Request.GetUri();
            //TODO:
            //result.WorkContext.QueryString = new System.Collections.Specialized.NameValueCollection(httpContext.Request.Query);
            return result;
        }

        protected HttpContext HttpContext { get; }
        protected WorkContext WorkContext { get; }

        public WorkContext Build()
        {
            return WorkContext;
        }

        public WorkContextBuilder WithCountries(Country[] countries)
        {
            WorkContext.AllCountries = countries ?? throw new ArgumentNullException(nameof(countries));
            return this;
        }

        public WorkContextBuilder WithCountries(ICountriesService countryService)
        {
            if (countryService == null)
            {
                throw new ArgumentNullException(nameof(countryService));
            }

            return WithCountries(countryService.GetCountries().ToArray());
        }

        public WorkContextBuilder WithStores(Store[] stores, string defaultStoreId)
        {
            if (stores.IsNullOrEmpty())
            {
                throw new NoStoresException();
            }

            WorkContext.AllStores = stores;
            //Set current store
            //Try first find by store url (if it defined)
            var currentStore = stores.FirstOrDefault(x => HttpContext.Request.Path.StartsWithSegments(new PathString("/" + x.Id)));
            if (currentStore == null)
            {
                currentStore = stores.Where(x => x.IsStoreUrl(WorkContext.RequestUrl)).FirstOrDefault();
            }
            if (currentStore == null && defaultStoreId != null)
            {
                currentStore = stores.FirstOrDefault(x => x.Id.EqualsInvariant(defaultStoreId));
            }

            WorkContext.CurrentStore = currentStore ?? stores.FirstOrDefault();          

            //TODO: need to use DDD Specification for this conditions
            //Set current language
            var availLanguages = stores.SelectMany(s => s.Languages)
                                  .Union(stores.Select(s => s.DefaultLanguage))
                                  .Select(x => x.CultureName).Distinct().ToArray();

            //Try to get language from request
            var currentLanguage = WorkContext.CurrentStore.DefaultLanguage; 
            var regexpPattern = string.Format(@"\/({0})\/?", string.Join("|", availLanguages));
            var match = Regex.Match(HttpContext.Request.Path, regexpPattern, RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                var language = new Language(match.Groups[1].Value);
                //Get store default language if language not in the supported by stores list
                currentLanguage = WorkContext.CurrentStore.Languages.Contains(language) ? language : currentLanguage;
            }
            WorkContext.CurrentLanguage = currentLanguage;
            return this;
        }

        public async Task<WorkContextBuilder> WithStoresAsync(IStoreService storeService, string defaultStoreId)
        {
            if (storeService == null)
            {
                throw new ArgumentNullException(nameof(storeService));
            }
            var stores = await storeService.GetAllStoresAsync();
            return WithStores(stores, defaultStoreId);
        }


        public WorkContextBuilder WithCurrencies(Currency[] currencies)
        {
            if (currencies == null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }
            if (WorkContext.AllStores == null)
            {
                throw new StorefrontException("Unable to set currencies without stores");
            }
            if (WorkContext.CurrentLanguage == null)
            {
                throw new StorefrontException("Unable to set currencies without language");
            }
            if (WorkContext.CurrentStore == null)
            {
                throw new StorefrontException("Unable to set currencies without current store");
            }

            WorkContext.AllCurrencies = currencies;

            //Sync store currencies with avail in system
            foreach (var store in WorkContext.AllStores)
            {
                store.SyncCurrencies(WorkContext.AllCurrencies, WorkContext.CurrentLanguage);
                store.CurrentSeoInfo = store.SeoInfos.FirstOrDefault(x => x.Language == WorkContext.CurrentLanguage);
            }

            //Set current currency
            //Get currency from request url  
            StringValues currencyCode;
            if (!HttpContext.Request.Query.TryGetValue("currency", out currencyCode))
            {
                //Next try get from Cookies
                currencyCode = HttpContext.Request.Cookies[StorefrontConstants.CurrencyCookie];
            }
            var currentCurrency = WorkContext.CurrentStore.DefaultCurrency;
            //Get store default currency if currency not in the supported by stores list
            if (!string.IsNullOrEmpty(currencyCode))
            {
                currentCurrency = WorkContext.CurrentStore.Currencies.FirstOrDefault(x => x.Equals(currencyCode)) ?? currentCurrency;
            }
            WorkContext.CurrentCurrency = currentCurrency;
            return this;
        }

        public async Task<WorkContextBuilder> WithCurrenciesAsync(ICurrencyService currencyService)
        {
            if (currencyService == null)
            {
                throw new ArgumentNullException(nameof(currencyService));
            }
            if (WorkContext.CurrentLanguage == null)
            {
                throw new StorefrontException("Unable to set currencies without language");
            }
            var currencies = await currencyService.GetAllCurrenciesAsync(WorkContext.CurrentLanguage);
            return WithCurrencies(currencies);
        }
        
    }
}
