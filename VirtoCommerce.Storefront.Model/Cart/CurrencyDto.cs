using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class CurrencyDto
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "cultureName")]
        public string CultureName { get; set; }
        [JsonProperty(PropertyName = "symbol")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "englishName")]
        public string EnglishName { get; set; }
        [JsonProperty(PropertyName = "exchangeRate")]
        public decimal ExchangeRate { get; set; }
        [JsonProperty(PropertyName = "customFormatting", NullValueHandling = NullValueHandling.Include)]
        public string CustomFormatting { get; set; }
    }
}
