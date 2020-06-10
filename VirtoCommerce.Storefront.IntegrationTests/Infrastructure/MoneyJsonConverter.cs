using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public class MoneyJsonConverter : JsonConverter<Money>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override Money ReadJson(JsonReader reader, Type objectType, Money existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var currencyCode = obj["currency"]["code"].Value<string>();
            var amount = obj["amount"].Value<decimal>();

            // TODO: here we should try to find incoming currency code inside available currencies,
            // but workContextAccessor is not accessible here, so just use default values for currency

            //var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(currencyCode));
            //if (currency == null)
            //{
            //    throw new NotSupportedException("Unknown currency code: " + currencyCode);
            //}
            var currency = new Currency(
                new Language(TestEnvironment.DefaultCultureName),
                currencyCode,
                TestEnvironment.DefaultCurrencyName,
                TestEnvironment.DefaultCurrencySymbol,
                TestEnvironment.DefaultExchangeRate);

            return new Money(amount, currency);
        }

        public override void WriteJson(JsonWriter writer, Money value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            JObject.FromObject(
                    new
                    {
                        value.Amount,
                        value.FormattedAmount,
                        value.FormattedAmountWithoutPointAndCurrency,
                        value.FormattedAmountWithoutPoint,
                        value.FormattedAmountWithoutCurrency,
                        value.Currency
                    }, serializer)
                .WriteTo(writer);
        }
    }
}
