using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using storefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ShopStaticConverter
    {
        public static Shop ToShopifyModel(this Store store, storefrontModel.WorkContext workContext)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidShop(store, workContext);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Shop ToLiquidShop(Store store, storefrontModel.WorkContext workContext)
        {
            var result = new Shop();

            result.Catalog = store.Catalog;
            result.Id = store.Id;
            result.Name = store.Name;
            result.SubscriptionEnabled = store.SubscriptionEnabled;
            result.QuotesEnabled = store.QuotesEnabled;
            result.CustomerAccountsEnabled = true;
            result.CustomerAccountsOptional = true;
            result.Currency = workContext.CurrentCurrency.Code;
            result.Description = store.Description;
            result.Domain = store.Url;
            result.Email = store.Email;
            result.MoneyFormat = "";
            result.MoneyWithCurrencyFormat = "";
            result.Url = store.Url ?? "~/";
            result.Currencies = workContext.AllCurrencies.Select(x => x.Code).ToArray();
            result.Languages = store.Languages.Select(x => x.ToShopifyModel()).ToArray();
            result.Catalog = store.Catalog;
            result.Status = store.StoreState.ToString();


            result.Metafields = new Dictionary<string, IDictionary<string, object>>
            {
                ["dynamic_properties"] = store.DynamicProperties.ToDictionary(prop => prop.Name, prop =>
                {
                    return (object)prop.Values.GetLocalizedStringsForLanguage(workContext.CurrentLanguage).Select(x => x.Value).ToArray();
                }),
                ["settings"] = store.Settings.ToDictionary(setting => setting.Name, setting => (object)setting.Value)
            };

            if (workContext.Categories != null)
            {
                result.Collections = new MutablePagedList<Collection>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    workContext.Categories.Slice(pageNumber, pageSize, sortInfos, @params);
                    return new StaticPagedList<Collection>(workContext.Categories.Select(x => ToLiquidCollection(x, workContext)), workContext.Categories);
                }, 1, workContext.Categories.PageSize);
            }

            return result;
        }
    }
}
