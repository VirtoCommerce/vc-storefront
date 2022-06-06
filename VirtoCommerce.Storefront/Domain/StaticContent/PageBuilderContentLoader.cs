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
            var page = JsonConvert.DeserializeObject<JObject>(content);
            var settings = page.GetValue("settings");
            var items = settings.AsJEnumerable();
            foreach (var item in items.OfType<JProperty>())
            {
                metadata.Add(item.Name, new List<string> { item.Value.Value<string>() });
            }
        }
    }
}
