using System;
using System.Collections.Generic;
using System.Text;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class JsonPage: Drop
    {
        public IDictionary<string, object> Settings { set; get; }
        public List<IDictionary<string, object>> Blocks { set; get; }
    }
}
