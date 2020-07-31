namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class AssetDto
    {
        public string Id { get; set; }

        public string MimeType { get; set; }

        public string Name { get; set; }

        public string RelativeUrl { get; set; }

        public long Size { get; set; }

        public string TypeId { get; set; }

        public string Url { get; set; }

        public string Group { get; set; }
    }
}
