using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class AddCartItem
    {
        public AddCartItem()
        {
            Quantity = 1;
        }
        [JsonProperty("id")]
        public string ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
