namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class TermFacet
    {
        public FacetTypes FacetType { get; set; }

        public string Name { get; set; }

        public FacetTermDto[] Terms { get; set; }
    }
}
