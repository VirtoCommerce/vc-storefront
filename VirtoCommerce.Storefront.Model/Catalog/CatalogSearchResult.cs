using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class CatalogSearchResult
    {
        public IPagedList<Product> Products { get; set; }
        public Aggregation[] Aggregations { get; set; }
    }
}
