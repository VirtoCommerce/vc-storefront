using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class AddCartItems
    {
        [JsonProperty("id")]
        public string ProductIds { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        public AddCartItems()
        {
            Quantity = Quantity;
        }
    }
}
