using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class ShoppingCartSearchResultDto
    {
        [JsonProperty(PropertyName = "totalCount")]
        public int? TotalCount { get; set; }

        [JsonProperty(PropertyName = "results")]
        public IList<ShoppingCartDto> Results { get; set; }
    }
}
