using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class EditorialReview : LocalizedString, IAccessibleByIndexKey
    {
        public string ReviewType { get; set; }
        public string Content => Value;

        public string IndexKey => ReviewType;
    }
}
