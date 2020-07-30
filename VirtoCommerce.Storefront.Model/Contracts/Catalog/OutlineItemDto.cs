using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class OutlineItemDto
    {
        public OutlineItemDto()
        {
            SeoInfos = Array.Empty<SeoInfoDto>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string SeoObjectType { get; set; }

        public SeoInfoDto[] SeoInfos { get; set; }
    }
}
