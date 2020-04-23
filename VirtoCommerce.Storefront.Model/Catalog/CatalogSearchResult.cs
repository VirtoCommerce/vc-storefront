using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class CatalogSearchResult : IHasBreadcrumbs
    {
        [JsonIgnore]
        public ProductSearchCriteria Criteria { get; private set; }

        [JsonIgnore]
        public Category Category { get; set; }

        public CatalogSearchResult()
            : this(new ProductSearchCriteria())
        {
        }
        public CatalogSearchResult(ProductSearchCriteria criteria)
        {
            Criteria = criteria;
        }
        public IMutablePagedList<Product> Products { get; set; }
        // Represent bucket, aggregated data based on a search query resulted by current search criteria CurrentCatalogSearchCriteria(example color 33, gr
        public IList<Aggregation> Aggregations { get; set; } = new List<Aggregation>();
    
        public IEnumerable<Breadcrumb> GetBreadcrumbs()
        {
            if (Category != null)
            {
                foreach (var breadCrumb in Category.GetBreadcrumbs())
                {
                    yield return breadCrumb;
                }
            }
            foreach (var appliedItem in Aggregations.SelectMany(x => x.Items).Where(x => x.IsApplied))
            {

                yield return new AggregationItemBreadcrumb(appliedItem)
                {
                    Title = appliedItem.Label
                };
            }
        }
    }
}

