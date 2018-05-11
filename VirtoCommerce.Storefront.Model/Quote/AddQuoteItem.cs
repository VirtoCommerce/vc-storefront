using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Quote
{
    public partial class AddQuoteItem
    {
        public AddQuoteItem()
        {
            Quantity = 1;
        }
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
