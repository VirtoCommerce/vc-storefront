using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class CategoryConnectionDto
    {
        public CategoryDto[] Items { get; set; }

        [JsonProperty(PropertyName = "filter_facets")]
        public FilterFacet[] FilterFacets { get; set; }

        [JsonProperty(PropertyName = "range_facets")]
        public RangeFacet[] RangeFacets { get; set; }

        [JsonProperty(PropertyName = "term_facets")]
        public TermFacet[] TermFacets { get; set; }

        public int? TotalCount { get; set; }
    }
}
