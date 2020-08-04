using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Contracts.Catalog;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class GetCategoriestResponseDto
    {
        [JsonProperty(PropertyName = "categories")]
        public CategoryConnectionDto Categories { get; set; }
    }
}
