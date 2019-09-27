using System.Collections.Generic;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class SearchProductsResult
    {
        public IPagedList<Product> Products { get; set; }
        public IList<Aggregation> Aggregations { get; set; }
        public IPagedList MetaData { get; set; }
    }
}
