using System;
using System.Globalization;
using DotLiquid;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/money-filters
    /// </summary>
    public class MoneyFilters
    {
        /// <summary>
        /// Formats the price based on the shop's HTML without currency setting.
        /// {{ 145 | money }}
        /// </summary>
        /// <param name="input"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public static string Money(object input, string currencyCode = null)
        {
            var money = GetMoney(input, currencyCode);
            return money == null ? null : money.ToString();
        }

        public static string MoneyWithoutDecimalPart(object input, string currencyCode = null)
        {
            var money = GetMoney(input, currencyCode);
            return money == null ? null : money.FormattedAmountWithoutPoint;
        }


        private static Money GetMoney(object input, string currencyCode = null)
        {
            if (input == null)
            {
                return null;
            }
            var themeEngine = (ShopifyLiquidThemeEngine)Template.FileSystem;
            var amount = Convert.ToDecimal(input, CultureInfo.InvariantCulture);
            var currency = currencyCode == null ? themeEngine.WorkContext.CurrentCurrency : new Currency(themeEngine.WorkContext.CurrentLanguage, currencyCode);
            return new Money(amount / 100, currency);
        }
    }
}
