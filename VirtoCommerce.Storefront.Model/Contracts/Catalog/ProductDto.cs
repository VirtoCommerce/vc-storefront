using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class ProductDto
    {

        public ProductDto()
        {
            Variations = Array.Empty<VariationDto>();
            Properties = Array.Empty<PropertyDto>();
            Descriptions = Array.Empty<DescriptionDto>();
            SeoInfos = Array.Empty<SeoInfoDto>();
        }

        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string BrandName { get; set; }

        public string Slug { get; set; }

        public string ImgSrc { get; set; }

        public string ProductType { get; set; }

        public CategoryDto Category { get; set; }

       // public VariationDto MasterVariation { get; set; }

        public DescriptionDto[] Descriptions { get; set; }

        public PropertyDto[] Properties { get; set; }

        public VariationDto[] Variations { get; set; }

        public ProductAssociationConnectionDto Associations { get; set; }

        public string OuterId { get; set; }

        public string CatalogId { get; set; }

        public SeoInfoDto[] SeoInfos { get; set; }

        public TaxCategoryDto Tax { get; set; }

        public AssetDto[] Assets { get; set; }

        public AvailabilityDataDto AvailabilityData { get; set; }

        public ImageDto[] Images { get; set; }

        public PriceDto[] Prices { get; set; }

        public OutlineDto[] Outlines { get; set; }
    }
}
