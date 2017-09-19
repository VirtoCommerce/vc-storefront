using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Models
{
    public class AddCartItem
    {
        public AddCartItem()
        {
            Quantity = 1;
        }
        [JsonProperty("id")]
        public string ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
