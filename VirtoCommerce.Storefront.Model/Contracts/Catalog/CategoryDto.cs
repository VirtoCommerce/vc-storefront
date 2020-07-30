using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class CategoryDto
    {
        public CategoryDto()
        {
            Outlines = Array.Empty<OutlineDto>();
            SeoInfos = Array.Empty<SeoInfoDto>();
            Images = Array.Empty<ImageDto>();
        }

        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool HasParent { get; set; }

        public string Slug { get; set; }

        public CategoryDto Parent { get; set; }

        public OutlineDto[] Outlines { get; set; }

        public SeoInfoDto[] SeoInfos { get; set; }

        public ImageDto[] Images { get; set; }
    }
}
