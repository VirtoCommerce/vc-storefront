using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Lists
{
    public class GetListsWithProductRequest
    {
        /// <summary>
        /// Gets or sets the value of lists names to search product in
        /// </summary>
        [JsonProperty("listNames")]
        public string[] ListNames { get; set; }

        /// <summary>
        /// list type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of product id to search for
        /// </summary>
        [JsonProperty("productId")]
        public string ProductId { get; set; }
    }
}
