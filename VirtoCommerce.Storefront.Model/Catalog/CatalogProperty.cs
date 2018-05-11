using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class CatalogProperty : Entity
    {
        public CatalogProperty()
        {
            LocalizedValues = new List<LocalizedString>();
            DisplayNames = new List<LocalizedString>();
            Values = new List<string>();
        }
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of object this property is applied to.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Property value type
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// Dictionary value id
        /// </summary>
        public string ValueId { get; set; }
        /// <summary>
        /// Property values for all languages
        /// </summary>
        public IList<LocalizedString> LocalizedValues { get; set; }
        /// <summary>
        /// Property value in current language
        /// </summary>
        public string Value { get; set; }

        public string DisplayName { get; set; }

        public IList<LocalizedString> DisplayNames { get; set; }

        public bool IsMultivalue { get; set; }

        public IList<string> Values { get; set; }
    }
}
