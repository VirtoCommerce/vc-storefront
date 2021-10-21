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

        public void ReadMetaData(string content, IDictionary<string, object> metadata)
        {
            var page = JsonConvert.DeserializeObject<JArray>(content);
            var settings = page.FirstOrDefault(x => (x as JObject)?.GetValue("type")?.Value<string>() == "settings");
            var items = settings.AsJEnumerable();

            foreach (JProperty item in items)
            {
                if (item.Value.HasValues)
                {
                    if (item.Value.Type == JTokenType.Array)
                    {
                        metadata.Add(item.Name, item.Value.ToArray());
                        continue;
                    }

                    if (item.Value.Type == JTokenType.Object)
                    {
                        var objectProperties = item.Value.Select(i => i.ToObject<JProperty>()).ToList();
                        var propertiesNames = objectProperties.Select(i => i.Name).ToList();
                        if (propertiesNames.Contains("url") && propertiesNames.Contains("altText"))
                        {
                            metadata.Add(item.Name, item.Value);
                        }
                        else
                        {
                            foreach (var property in objectProperties)
                            {
                                metadata.Add($"{item.Name}.{property.Name}", property.Value.Value<string>());
                            }
                        }
                    }

                    if (item.Value.Type == JTokenType.Property)
                    {
                        var value = item.Value.Value<string>();
                        if (value != null)
                        {
                            metadata.Add(item.Name, value);
                        }
                    }
                }
                else
                {
                    var value = item.Value.Value<string>();
                    if (value != null)
                    {
                        metadata.Add(item.Name, value);
                    }
                }
            }

            metadata.Add("template", "json-page");
        }
    }
}
