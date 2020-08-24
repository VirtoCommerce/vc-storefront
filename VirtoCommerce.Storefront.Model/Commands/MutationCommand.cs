using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Commands
{
    public class MutationCommand
    {
        [JsonProperty(PropertyName = "storeId")]
        public string StoreId { get; set; }

        [JsonProperty(PropertyName = "cartName")]
        public string CartName { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "cultureName")]
        public string CultureName { get; set; }

        [JsonProperty(PropertyName = "currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonProperty(PropertyName = "cartType")]
        public string CartType { get; set; }
    }
}
