using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class AggregationItemBreadcrumb : Breadcrumb
    {
        public AggregationItemBreadcrumb(AggregationItem item) : base("Tag")
        {
            AggregationItem = item;
        }

        public AggregationItem AggregationItem { get; private set; }
    }
}
