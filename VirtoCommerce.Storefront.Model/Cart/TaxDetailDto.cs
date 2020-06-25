using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class TaxDetailDto
    {
        [JsonProperty(PropertyName = "rate")]
        public MoneyDto Rate { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public MoneyDto Amount { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
    }
}
