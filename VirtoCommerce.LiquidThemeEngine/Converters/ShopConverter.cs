using PagedList.Core;
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

            result.Metafields = new MetaFieldNamespacesCollection(new[]
            {
                new MetafieldsCollection("dynamic_properties", workContext.CurrentLanguage, store.DynamicProperties),
                new MetafieldsCollection("settings", store.Settings)
            });

            if (workContext.Categories != null)
            {
                result.Collections = new MutablePagedList<Collection>((pageNumber, pageSize, sortInfos) =>
                {
                    workContext.Categories.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<Collection>(workContext.Categories.Select(x => ToLiquidCollection(x, workContext)), workContext.Categories);
                }, 1, workContext.Categories.PageSize);
            }

            return result;
        }
    }
}
