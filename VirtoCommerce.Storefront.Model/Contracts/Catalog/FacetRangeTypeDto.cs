namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class FacetRangeTypeDto
    {
        public long? Count { get; set; }

        public long? From { get; set; }

        public string FromStr { get; set; }

        public bool IncludeFrom { get; set; }

        public bool IncludeTo { get; set; }

        public long? Max { get; set; }

        public long? Min { get; set; }

        public long? To { get; set; }

        public string ToStr { get; set; }

        public long? Total { get; set; }

        public bool? IsSelected { get; set; }

        public string Label { get; set; }
    }
}
