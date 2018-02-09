using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cms
{
    public class CmsPageDefinition
    {
        public DefaultableDictionary Settings { get; set; }
        public List<IDictionary<string, object>> Blocks { set; get; }

        public CmsPageDefinition()
        {
            Settings = new DefaultableDictionary(null);
            Blocks = new List<IDictionary<string, object>>();
        }
    }
}
