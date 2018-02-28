using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Lists
{
    public class AddListItem
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("listName")]
        public string ListName { get; set; }
    }
}
