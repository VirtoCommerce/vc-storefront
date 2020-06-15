using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.Storefront.Domain
{
    public class JsonPageReader : ContentItemReader
    {
        public JsonPageReader(string path, string content) : base(path, content) { }

        public override string ReadContent()
        {
            return Content;
        }

        public override Dictionary<string, object> ReadMetadata()
        {
            var result = new Dictionary<string, object>();
            var page = JsonConvert.DeserializeObject<JArray>(Content) ?? new JArray();
            var settings = page.FirstOrDefault(x => (x as JObject)?.GetValue("type")?.Value<string>() == "settings");
            if (settings != null)
            {
                result = settings.ToObject<Dictionary<string, object>>().ToDictionary(x => x.Key, x => x.Value);
            }
            AddPropertiesFromFilename(result);
            return result;
        }
    }
}
