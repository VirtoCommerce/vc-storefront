using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class AggregationPostLoadContext
    {
        public ProductSearchCriteria ProductSearchCriteria { get; set; }
        public IDictionary<string, Category> CategoryByIdDict { get; set; }
    }
}
