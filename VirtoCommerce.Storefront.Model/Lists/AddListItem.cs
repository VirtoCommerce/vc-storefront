using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Lists
{
    public class AddListItem
    {
        /// <summary>
        /// Gets or sets the value of product id to add to wishlist
        /// </summary>
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name of wishlist to add a product to
        /// </summary>
        [JsonProperty("listName")]
        public string ListName { get; set; }

        /// <summary>
        /// List type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
