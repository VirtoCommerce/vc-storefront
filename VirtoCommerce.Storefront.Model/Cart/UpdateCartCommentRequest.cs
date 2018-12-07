using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class UpdateCartCommentRequest
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
