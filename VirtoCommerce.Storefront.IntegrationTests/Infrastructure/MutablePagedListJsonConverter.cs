using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public class MutablePagedListJsonConverter<T> : JsonConverter<IMutablePagedList<T>>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override IMutablePagedList<T> ReadJson(JsonReader reader, Type objectType, IMutablePagedList<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JArray.Load(reader);
            return new MutablePagedList<T>(obj.Select(v => v.Value<T>()));
        }

        public override void WriteJson(JsonWriter writer, IMutablePagedList<T> value, JsonSerializer serializer)
        {
            var toListMethod = typeof(Enumerable).GetMethod("ToList");
            var constructedToList = toListMethod.MakeGenericMethod(value.GetType().GetGenericArguments()[0]);
            var list = constructedToList.Invoke(null, new object[] { value });
            var result = JArray.FromObject(list, serializer);
            result.WriteTo(writer);
        }
    }
}
