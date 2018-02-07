using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cms;

namespace VirtoCommerce.Storefront.JsonConverters
{
    public class CmsPageJsonConverter : JsonConverter
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public CmsPageJsonConverter(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public CmsPageJsonConverter()
        {
        }


        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return typeof(CmsPageDefinition).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var blocks = obj["blocks"];

            var retVal = new CmsPageDefinition();
            retVal.Blocks = ReadBlocks(retVal, blocks);

            return retVal;
        }

        private List<IDictionary<string, object>> ReadBlocks(CmsPageDefinition cmsPage, JToken jblocks)
        {
            var retVal = new List<IDictionary<string, object>>();

            foreach (var jblock in jblocks.Children())
            {
                var type = jblock["type"].Value<string>();

                if (type == "settings")
                {
                    var dict = jblock.ToObject<Dictionary<string, object>>().ToDictionary(x => x.Key, x => x.Value);
                    cmsPage.Settings = new DefaultableDictionary(dict, null);
                }
                else
                {
                    var dict = jblock.ToObject<Dictionary<string, object>>().ToDictionary(x => x.Key, x => x.Value);

                    if (type == "container")
                    {
                        var innerBlocks = dict["blocks"];

                        dict["blocks"] = ReadBlocks(cmsPage, (JToken)innerBlocks);
                    }

                    retVal.Add(new Dictionary<string, object>(dict));
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
