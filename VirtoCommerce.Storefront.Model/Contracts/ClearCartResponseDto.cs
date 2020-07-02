using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class ClearCartResponseDto
    {
        [JsonProperty(PropertyName = "clearCart")]
        public ShoppingCartDto ClearCart { get; set; }
    }
}
