using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Contracts.Catalog;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class GetProductsResponseDto
    {
        [JsonProperty(PropertyName = "products")]
        public ProductConnection Products { get; set; }
    }
}
