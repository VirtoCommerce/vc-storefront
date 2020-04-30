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

        public override Dictionary<string, IEnumerable<string>> ReadMetadata()
        {
            var result = new Dictionary<string, IEnumerable<string>>();
            var page = JsonConvert.DeserializeObject<JArray>(Content) ?? new JArray();
            var settings = page.FirstOrDefault(x => (x as JObject)?.GetValue("type")?.Value<string>() == "settings");
            if (settings != null)
            {
                var items = settings.AsJEnumerable();
                foreach (JProperty prop in items)
                {
                    result.Add(prop.Name, new List<string> { prop.Value.Value<string>() });
                }
            }
            AddPropertiesFromFilename(result);
            return result;
        }
    }
}
