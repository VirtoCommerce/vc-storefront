using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class ProductAssociationConnectionDto
    {
        public ProductAssociationConnectionDto()
        {
            Items = Array.Empty<ProductAssociationDto>();
        }

        public ProductAssociationDto[] Items { get; set; }

        public PageInfoDto PageInfo { get; set; }

        public int? TotalCount { get; set; }
    }
}
