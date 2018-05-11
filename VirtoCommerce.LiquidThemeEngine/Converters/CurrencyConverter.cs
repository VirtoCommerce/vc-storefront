using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class CurrencyConverter
    {
        public static Currency ToShopifyModel(this StorefrontModel.Common.Currency currency)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidCurrency(currency);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Currency ToLiquidCurrency(StorefrontModel.Common.Currency currency)
        {
            var result = new Currency();

            result.Code = currency.Code;
            result.CurrencyCode = currency.Code;
            result.EnglishName = currency.EnglishName;
            result.Symbol = currency.Symbol;

            return result;
        }
    }
}