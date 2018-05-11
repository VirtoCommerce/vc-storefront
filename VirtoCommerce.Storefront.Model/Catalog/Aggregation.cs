namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class Aggregation
    {
        public string AggregationType { get; set; }
        public string Field { get; set; }
        public string Label { get; set; }
        public AggregationItem[] Items { get; set; }
    }
}
