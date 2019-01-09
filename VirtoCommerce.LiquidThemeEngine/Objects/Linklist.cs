using DotLiquid;
using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    /// <summary>
    /// Represents link list in site navigation part
    /// </summary>
    /// <remarks>
    /// https://docs.shopify.com/themes/liquid-documentation/objects/linklist
    /// </remarks>
    public partial class Linklist : Entity, IDictionaryKey
    {
        /// <summary>
        /// Returns the handle of the linklist
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// Returns a collection of links in the linklist
        /// </summary>
        public IList<Link> Links { get; set; }

        /// <summary>
        /// Returns the title of the linklist
        /// </summary>
        public string Title { get; set; }

        public string Key => Handle;
    }
}
