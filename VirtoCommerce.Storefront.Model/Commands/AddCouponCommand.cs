using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Commands
{
    public class AddCouponCommand : MutationCommand
    {
        [JsonProperty(PropertyName = "couponCode")]
        public string CouponCode { get; set; }
    }
}
