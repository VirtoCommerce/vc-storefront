using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class AddCartItem
    {
        public AddCartItem()
        {
            Quantity = 1;
        }
        [JsonProperty("Id")]
        public string Id
        {
            get { return ProductId; }
            set { ProductId = value; }
        }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Cart type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Cart name
        /// </summary>
        [JsonProperty("listName")]
        public string ListName { get; set; }
    }
}
