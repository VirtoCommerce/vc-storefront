using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class GetCartResponseDto
    {
        [JsonProperty(PropertyName = "cart")]
        public ShoppingCartDto Cart { get; set; }
    }
}
