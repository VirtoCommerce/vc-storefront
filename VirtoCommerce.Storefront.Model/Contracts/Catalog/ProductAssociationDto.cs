using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class ProductAssociationDto
    {
        public ProductAssociationDto()
        {
            Tags = Array.Empty<string>();
        }

        public string AssociatedObjectId { get; set; }

        public string AssociatedObjectType { get; set; }

        public int Priority { get; set; }

        public ProductDto Product { get; set; }

        public int? Quantity { get; set; }

        public string Type { get; set; }

        public string[] Tags { get; set; }
    }
}
