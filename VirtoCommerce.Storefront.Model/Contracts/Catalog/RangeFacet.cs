namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class RangeFacet
    {
        public FacetTypes FacetTypes { get; set; }

        public string Name { get; set; }

        public FacetRangeTypeDto[] Ranges { get; set; }
    }
}
