using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.JsonConverters
{
    public class CurrencyJsonConverter : JsonConverter
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        public CurrencyJsonConverter(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Currency).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);
            var pt = obj["code"];
            if (pt != null)
            {
                var currencyCode = pt.Value<string>();
                retVal = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(currencyCode));
                if (retVal == null)
                {
                    throw new NotSupportedException("Unknown currency code: " + currencyCode);
                }
            }         
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
