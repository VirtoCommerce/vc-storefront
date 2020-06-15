using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public class TierPriceJsonConverter : JsonConverter<TierPrice>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override TierPrice ReadJson(JsonReader reader, Type objectType, TierPrice existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var price = obj["price"].ToObject<Money>();
            var quantity = obj["quantity"].Value<long>();

            return new TierPrice(price, quantity);
        }

        public override void WriteJson(JsonWriter writer, TierPrice value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
