using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Lists
{
    public class FilterListsByProductRequest
    {
        [JsonProperty("listNames")]
        public string[] ListNames { get; set; }

        [JsonProperty("productId")]
        public string ProdcutId { get; set; }
    }
}
