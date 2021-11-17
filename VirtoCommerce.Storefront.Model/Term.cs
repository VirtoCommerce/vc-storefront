using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class Term : ValueObject
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
