using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class DynamicPropertyDictionaryItem : Entity
    {
        public string PropertyId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IList<LocalizedString> DisplayNames { get; set; } = new List<LocalizedString>();
    }
}
