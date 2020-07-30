namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class SeoInfoDto
    {
        public string Id { get; set; }

        public string ImageAltDescription { get; set; }

        public bool? IsActive { get; set; }

        public string LanguageCode { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string Name { get; set; }


        public string ObjectId { get; set; }

        public string ObjectType { get; set; }

        public string PageTitle { get; set; }

        public string SemanticUrl { get; set; }

        public string StoreId { get; set; }
    }
}
