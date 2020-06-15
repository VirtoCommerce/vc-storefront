using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public class CurrencyJsonConverter : JsonConverter<Currency>
    {
        public override Currency ReadJson(JsonReader reader, Type objectType, Currency existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var code = obj["code"].Value<string>();
            return new Currency(Language.InvariantLanguage, code ?? "en-US");
        }

        public override void WriteJson(JsonWriter writer, Currency value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var result = JObject.FromObject(new { value.Code, value.Symbol }, serializer);
            result.WriteTo(writer);
        }
    }
}
