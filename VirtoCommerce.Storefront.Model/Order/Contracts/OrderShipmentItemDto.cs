using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Order
{
    public class OrderShipmentItemDto
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lineItemId")]
        public string LineItemId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lineItem")]
        public OrderLineItemDto LineItem { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "barCode")]
        public string BarCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "outerId")]
        public string OuterId { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "createdDate")]
        //public System.DateTime? CreatedDate { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "modifiedDate")]
        //public System.DateTime? ModifiedDate { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "createdBy")]
        //public string CreatedBy { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "modifiedBy")]
        //public string ModifiedBy { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
