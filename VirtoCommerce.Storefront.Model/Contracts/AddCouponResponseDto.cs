using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class AddCouponResponseDto
    {
        [JsonProperty(PropertyName = "addCoupon")]
        public ShoppingCartDto AddCoupon { get; set; }
    }
}
