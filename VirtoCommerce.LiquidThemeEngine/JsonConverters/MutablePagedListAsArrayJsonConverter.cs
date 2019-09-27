using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.JsonConverters
{
    /// <summary>
    ///This converter is designed to force serialize all derived types from IMutablePagedLists as array.
    ///Because the main type MutablePagedList<> also derived from IDitionary that  causes the default Json serializer to tries to serialize as a dictionary first.
    /// </summary>
    public class MutablePagedListAsArrayJsonConverter : JsonConverter
    {
        private readonly JsonSerializerSettings _jsonSettings;

        public MutablePagedListAsArrayJsonConverter()
        {

        }

        public MutablePagedListAsArrayJsonConverter(JsonSerializerSettings jsonSettings)
        {
            _jsonSettings = jsonSettings;
        }
        public override bool CanConvert(Type objectType)
        {
            return typeof(IMutablePagedList).IsAssignableFrom(objectType);
        }

        public override bool CanWrite => true;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (_jsonSettings != null)
            {
                serializer = JsonSerializer.Create(_jsonSettings);
            }
            var toListMethod = typeof(Enumerable).GetMethod("ToList");
            var constructedToList = toListMethod.MakeGenericMethod(value.GetType().GetGenericArguments()[0]);
            var list = constructedToList.Invoke(null, new object[] { value });
            //Force serialize MutablePagedList type as array, instead of dictionary
            var result = JArray.FromObject(list, serializer);
            result.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
