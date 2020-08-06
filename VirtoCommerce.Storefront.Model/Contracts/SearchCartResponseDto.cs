using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class SearchCartResponseDto
    {
        [JsonProperty(PropertyName = "carts")]
        public CartsListDto Carts { get; set; }
    }

    public class CartsListDto
    {
        public List<ShoppingCartDto> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
