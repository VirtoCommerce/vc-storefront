using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public partial class ContentPage : ContentItem
    {
        public string Template { get; set; }

        public override void LoadContent(string content, IDictionary<string, IEnumerable<string>> metaInfoMap)
        {
            Template = "page";

            if (metaInfoMap.ContainsKey("template"))
            {
                Template = metaInfoMap["template"].FirstOrDefault();
            }

            base.LoadContent(content, metaInfoMap);
        }
    }
}
