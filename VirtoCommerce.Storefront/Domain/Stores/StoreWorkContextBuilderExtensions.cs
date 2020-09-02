using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class StoreWorkContextBuilderExtensions
    {
        public static Task WithStoresAsync(this IWorkContextBuilder builder, IEnumerable<Store> stores, string defaultStoreId)
        {
            if (stores == null)
            {
                throw new NoStoresException();
            }

            builder.WorkContext.AllStores = stores.ToArray();
            var currentStore = builder.HttpContext.GetCurrentStore(stores, defaultStoreId);

            currentStore.ThemeName = builder.WorkContext.QueryString.Get("previewtheme") ?? currentStore.ThemeName;

            //Very important workaround. If left  Null or Empty as Url for default store with condition of multiple stores present, will be generated a relative url '/store_name/' instead of
            // '/' thats can  leads to invalid urls to default store and other issue with  <base href=""> that contains invalid relative  url
            if (defaultStoreId != null && string.IsNullOrEmpty(currentStore.Url) && currentStore.Id.EqualsInvariant(defaultStoreId))
            {
                currentStore.Url = "/";
            }
            builder.WorkContext.CurrentStore = currentStore;
            builder.WorkContext.CurrentLanguage = builder.HttpContext.GetCurrentLanguage(builder.WorkContext.CurrentStore);

            // SEO for category, product and blogs is set inside corresponding controllers
            // there we default SEO for requested store 
            var seoInfo = builder.WorkContext.CurrentStore.SeoInfos?.FirstOrDefault(x => x.Language == builder.WorkContext.CurrentLanguage);
            if (seoInfo != null && builder.WorkContext.RequestUrl != null)
            {
                var htmlEncoder = builder.HttpContext.RequestServices.GetRequiredService<HtmlEncoder>();
                seoInfo.Slug = htmlEncoder.Encode(builder.WorkContext.RequestUrl.ToString());
                builder.WorkContext.CurrentPageSeo = seoInfo;
            }
            return Task.CompletedTask;
        }

        public static async Task WithStoresAsync(this IWorkContextBuilder builder, string defaultStoreId)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var storeService = serviceProvider.GetRequiredService<IStoreService>();

            var stores = await storeService.GetAllStoresAsync();

            if (stores.IsNullOrEmpty())
            {
                throw new NoStoresException();
            }

            await builder.WithStoresAsync(stores, defaultStoreId);
        }

        public static Task WithCurrenciesAsync(this IWorkContextBuilder builder, IList<Currency> availCurrencies, Store store)
        {
            if (availCurrencies == null)
            {
                throw new ArgumentNullException(nameof(availCurrencies));
            }
            //Filter all avail currencies, leave only currencies define for store 
            var storeCurrencies = availCurrencies.Where(x => store.CurrenciesCodes.Any(y => x.Equals(y))).ToList();
            builder.WorkContext.AllCurrencies = storeCurrencies;
            builder.WorkContext.CurrentCurrency = builder.HttpContext.GetCurrentCurrency(availCurrencies, store);
            return Task.CompletedTask;
        }

        public static async Task WithCurrenciesAsync(this IWorkContextBuilder builder, Language language, Store store)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            var serviceProvider = builder.HttpContext.RequestServices;
            var currencyService = serviceProvider.GetRequiredService<ICurrencyService>();

            var currencies = await currencyService.GetAllCurrenciesAsync(language);

            await builder.WithCurrenciesAsync(currencies, store);
        }
    }
}

