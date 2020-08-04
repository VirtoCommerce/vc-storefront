using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class GetWishListResponseDto
    {
        [JsonProperty(PropertyName = "wishLists")]
        public List<WishListDto> WishLists { get; set; }
    }
}
