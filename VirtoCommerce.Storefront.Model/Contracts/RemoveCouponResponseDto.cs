using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class RemoveCouponResponseDto
    {
        [JsonProperty(PropertyName = "removeCoupon")]
        public ShoppingCartDto RemoveCoupon { get; set; }
    }
}
