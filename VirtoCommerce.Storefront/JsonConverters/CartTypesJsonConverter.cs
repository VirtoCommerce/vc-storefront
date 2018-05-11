using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;

namespace VirtoCommerce.Storefront.Binders
{
    public class CartTypesJsonConverter : JsonConverter
    {
        private static Type[] _knowTypes = new[] { typeof(Shipment), typeof(Payment)};


        private readonly IWorkContextAccessor _workContextAccessor;
        public CartTypesJsonConverter(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }
        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var workContext = _workContextAccessor.WorkContext;
            var obj = JObject.Load(reader);
            var currencyCode = obj.SelectToken("currency.code");
            if (currencyCode == null)
            {
                currencyCode = obj.SelectToken("currency");
            }
            var currency = workContext.CurrentCart.Value.Currency;
            if (currencyCode != null)
            {
                currency = workContext.AllCurrencies.FirstOrDefault(x => x.Equals(currencyCode.Value<string>()));
            }
            if (objectType == typeof(Shipment))
            {
                retVal = new Shipment(currency);
            }
            else if (objectType == typeof(Payment))
            {
                retVal = new Payment(currency);
            }
         
            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}