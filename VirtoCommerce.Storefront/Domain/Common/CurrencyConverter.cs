using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CurrencyConverter
    {
        public static Currency ToCurrency(this coreDto.Currency currency, Language language)
        {
            var retVal = new Currency(language, currency.Code, currency.Name, currency.Symbol, (decimal)currency.ExchangeRate.Value)
            {
                CustomFormatting = currency.CustomFormatting
            };
            return retVal;
        }
    }
}
