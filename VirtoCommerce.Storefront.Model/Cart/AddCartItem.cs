using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class AddCartItem
    {
        public AddCartItem()
        {
            Quantity = 1;
        }
        [JsonProperty("Id")]
        public string Id
        {
            get { return ProductId; }
            set { ProductId = value; }
        }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Cart type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Cart name
        /// </summary>
        [JsonProperty("listName")]
        public string ListName { get; set; }

        /// <summary>
        /// Manual price
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }


        /// <summary>
        /// Comment
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Dynamic properties
        /// </summary>
        [JsonProperty("dynamicProperties")]
        public Dictionary<string, string> DynamicProperties { get; set; }
    }
}
