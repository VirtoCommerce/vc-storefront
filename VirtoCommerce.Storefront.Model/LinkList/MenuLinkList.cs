using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Represents site navigation menu link list object
    /// </summary>
    public class MenuLinkList : Entity, IHasLanguage, IAccessibleByIndexKey
    {
        /// <summary>
        /// Gets or sets the name of site navigation menu link list
        /// </summary>
        public string Name { get; set; }
        public string Title => Name;
        public string Handle => Name?.Handelize();

        /// <summary>
        /// Gets or sets the site navigation menu link list store ID
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the collection of site navigation menu link for link list
        /// </summary>
        public IList<MenuLink> MenuLinks { get; set; } = new List<MenuLink>();
        [JsonIgnore]
        public IList<MenuLink> Links => MenuLinks;

        #region IHasLanguage Members
        /// <summary>
        /// Gets or sets the locale of site navigation menu link list
        /// </summary>
        public Language Language { get; set; }

        #endregion

        public string IndexKey => Handle;
    }
}
