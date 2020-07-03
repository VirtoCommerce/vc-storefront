using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Commands
{
    public class AddCartItemCommand : MutationCommand
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
    }
}
