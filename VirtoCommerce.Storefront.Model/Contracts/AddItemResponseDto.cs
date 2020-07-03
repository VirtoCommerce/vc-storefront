using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class AddItemResponseDto
    {
        [JsonProperty(PropertyName = "addItem")]
        public ShoppingCartDto AddItem { get; set; }
    }
}
