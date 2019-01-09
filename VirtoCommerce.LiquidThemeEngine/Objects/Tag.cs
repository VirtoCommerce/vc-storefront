using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Tag : Entity, IDictionaryKey
    {
        public Tag(string groupName, string value)
        {
            GroupName = groupName;
            Value = value;
            Id = string.Concat(GroupName, "_", Value).ToLowerInvariant();
        }

        public string GroupType { get; set; }
        public string GroupName { get; set; }
        public string GroupLabel { get; set; }
        public string Label { get; set; }
        public int Count { get; set; }
        public string Value { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }

        public string Key => Id;
    }
}
