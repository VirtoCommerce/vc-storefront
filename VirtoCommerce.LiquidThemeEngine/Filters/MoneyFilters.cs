using System;
using System.Globalization;
using DotLiquid;
using Scriban;
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
        public static string Money(TemplateContext context, object input, string currencyCode = null)
        {
            var money = GetMoney(context, input, currencyCode);
            return money == null ? null : money.ToString();
        }

        public static string MoneyWithoutDecimalPart(TemplateContext context, object input, string currencyCode = null)
        {
            var money = GetMoney(context, input, currencyCode);
            return money == null ? null : money.FormattedAmountWithoutPoint;
        }


        private static Money GetMoney(TemplateContext context, object input, string currencyCode = null)
        {
            if (input == null)
            {
                return null;
            }
            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var amount = Convert.ToDecimal(input, CultureInfo.InvariantCulture);
            var currency = currencyCode == null ? themeEngine.WorkContext.CurrentCurrency : new Currency(themeEngine.WorkContext.CurrentLanguage, currencyCode);
            return new Money(amount / 100, currency);
        }
    }
}
