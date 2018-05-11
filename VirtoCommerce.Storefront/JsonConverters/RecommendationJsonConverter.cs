using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Recommendations;

namespace VirtoCommerce.Storefront.Binders
{
    public class RecommendationJsonConverter : JsonConverter
    {
        private static Type[] _knowTypes = new[] { typeof(RecommendationEvalContext)};


        private readonly IRecommendationProviderFactory _providerFactory;
        public RecommendationJsonConverter(IRecommendationProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }
        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = new RecommendationEvalContext(); 
            var obj = JObject.Load(reader);
            var providerName = obj.SelectToken("provider");
            if (providerName != null)
            {
                var provider = _providerFactory.GetProvider(providerName.Value<string>());
                if(provider != null)
                {
                    retVal = provider.CreateEvalContext();
                }
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