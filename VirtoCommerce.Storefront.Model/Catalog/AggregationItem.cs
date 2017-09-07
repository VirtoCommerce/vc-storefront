namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class AggregationItem
    {
        public int Count { get; set; }
        public bool IsApplied { get; set; }
        public string Label { get; set; }
        public object Value { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }
    }
}
