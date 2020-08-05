using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class SearchCartDescriptionResponseDto
    {
        [JsonProperty(PropertyName = "carts")]
        public List<CartDescriptionDto> Carts { get; set; }
    }
}
