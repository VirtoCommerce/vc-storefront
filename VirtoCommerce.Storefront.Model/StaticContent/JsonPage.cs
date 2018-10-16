using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public class JsonPage
    {
        public IDictionary<string, object> Settings { set; get; }
        public List<JObject> Blocks { set; get; }
    }
}
