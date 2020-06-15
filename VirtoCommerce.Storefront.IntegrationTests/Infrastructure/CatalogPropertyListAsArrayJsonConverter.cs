using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public class CatalogPropertyListAsArrayJsonConverter : JsonConverter<IMutablePagedList<CatalogProperty>>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override IMutablePagedList<CatalogProperty> ReadJson(JsonReader reader, Type objectType, IMutablePagedList<CatalogProperty> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JArray.Load(reader);
            return new MutablePagedList<CatalogProperty>(obj.Select(v => v.Value<CatalogProperty>()));
        }

        public override void WriteJson(JsonWriter writer, IMutablePagedList<CatalogProperty> value, JsonSerializer serializer)
        {
            var toListMethod = typeof(Enumerable).GetMethod("ToList");
            var constructedToList = toListMethod.MakeGenericMethod(value.GetType().GetGenericArguments()[0]);
            var list = constructedToList.Invoke(null, new object[] { value });
            var result = JArray.FromObject(list);
            result.WriteTo(writer);
        }
    }
}
