using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class PageBuilderContentLoader : IStaticContentLoader
    {
        public string PrepareContent(string content)
        {
            return content;
        }

        public void ReadMetaData(string content, IDictionary<string, IEnumerable<string>> metadata)
        {
            var page = JsonConvert.DeserializeObject<JArray>(content);
            var settings = page.FirstOrDefault(x => (x as JObject)?.GetValue("type")?.Value<string>() == "settings");
            var items = settings.AsJEnumerable();

            foreach (JProperty item in items)
            {
                if (item.Value.HasValues)
                {
                    foreach (var itemProp in item.Value)
                    {
                        var property = itemProp.ToObject<JProperty>();
                        metadata.Add($"{item.Name}.{property.Name}", new List<string> { property.Value.Value<string>() });
                    }
                }
                else
                {
                    var value = item.Value.Value<string>();
                    if (value != null)
                    {
                        metadata.Add(item.Name, new List<string> { value });
                    }
                }
            }

            metadata.Add("template", new List<string> { "json-page" });
        }
    }
}
