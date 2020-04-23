using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class SettingEntry : IAccessibleByIndexKey
    {
        public SettingEntry()
        {
            AllowedValues = new List<string>();
            ArrayValues = new List<string>();
        }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public IList<string> AllowedValues { get; set; }
        public string DefaultValue { get; set; }
        public bool IsArray { get; set; }
        public IList<string> ArrayValues { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string IndexKey => Name;
    }
}
