using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class PageMetadataVisitor : IContentItemVisitor
    {
        public bool Suit(ContentItem item)
        {
            return item.FileName.EndsWith(".page");
        }

        public ContentItem Parse(string path, string content, ContentItem item)
        {
            var page = JsonConvert.DeserializeObject<JArray>(content);
            var settings = page.FirstOrDefault(x => (x as JObject)?.GetValue("type")?.Value<string>() == "settings");
            var items = settings.AsJEnumerable();
            item.MetaInfo = new Dictionary<string, IEnumerable<string>>();
            foreach (JProperty prop in items)
            {
                item.MetaInfo.Add(prop.Name, new List<string> { prop.Value.Value<string>() });
            }
            return item;
        }
    }
}
