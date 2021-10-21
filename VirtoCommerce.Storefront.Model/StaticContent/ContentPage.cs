using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public partial class ContentPage : ContentItem
    {
        public string Template { get; set; }

        public override void LoadContent(string content, IDictionary<string, object> metaInfoMap)
        {
            Template = "page";

            if (metaInfoMap.ContainsKey("template"))
            {
                Template = metaInfoMap["template"].ToString();
            }

            base.LoadContent(content, metaInfoMap);
        }
    }
}
