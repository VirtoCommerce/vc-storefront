using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class SettingEntry : IAccessibleByIndexKey
    {
        public SettingEntry()
        {
            AllowedValues = new List<object>();
            ArrayValues = new List<object>();
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public string ValueType { get; set; }
        public IList<object> AllowedValues { get; set; }
        public object DefaultValue { get; set; }
        public bool IsArray { get; set; }
        public IList<object> ArrayValues { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string IndexKey => Name;
    }
}
