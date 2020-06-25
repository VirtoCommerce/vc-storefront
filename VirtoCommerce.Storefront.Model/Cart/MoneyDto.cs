using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class MoneyDto
    {
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }
        [JsonProperty(PropertyName = "decimalDigits")]
        public int DecimalDigits { get; set; }
        [JsonProperty(PropertyName = "formattedAmount")]
        public string FormattedAmount { get; set; }
        [JsonProperty(PropertyName = "formattedAmountWithoutPoint")]
        public string FormattedAmountWithoutPoint { get; set; }
        [JsonProperty(PropertyName = "formattedAmountWithoutCurrency")]
        public string FormattedAmountWithoutCurrency { get; set; }
        [JsonProperty(PropertyName = "formattedAmountWithoutPointAndCurrency")]
        public string FormattedAmountWithoutPointAndCurrency { get; set; }
    }
}
