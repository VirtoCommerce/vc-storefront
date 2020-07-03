using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.JsonConverters
{
    public class GraphQlMoneyJsonConverter : JsonConverter<Money>
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public GraphQlMoneyJsonConverter(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public override Money ReadJson(JsonReader reader, Type objectType, [AllowNull] Money existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var currencyCode = obj["currency"]?["code"]?.Value<string>() ?? _workContextAccessor.WorkContext.CurrentCurrency.Code;
            if (currencyCode == null)
            {
                throw new NotSupportedException("Unknown currency code: " + currencyCode);
            }
            var amount = obj["amount"].Value<decimal>();
            var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(currencyCode));
            if (currency == null)
            {
                throw new NotSupportedException("Unknown currency code: " + currencyCode);
            }

            return new Money(amount, currency);
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] Money value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var result = JObject.FromObject(new { value.Amount, value.FormattedAmount, value.FormattedAmountWithoutPointAndCurrency, value.FormattedAmountWithoutPoint, value.FormattedAmountWithoutCurrency, value.Currency }, serializer);
            result.WriteTo(writer);
        }
    }
}
