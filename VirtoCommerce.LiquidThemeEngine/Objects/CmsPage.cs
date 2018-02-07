using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class CmsPage : Drop
    {
        #region Public Properties
        public DefaultableDictionary Settings { get; set; } 
        public List<IDictionary<string, object>> Blocks { get; set; }
        #endregion
    }
}
