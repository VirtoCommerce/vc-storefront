using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class ShipmentDto
    {
        [JsonProperty(PropertyName = "fulfillmentCenterId")]
        public string FulfillmentCenterId { get; set; }
        [JsonProperty(PropertyName = "height")]
        public decimal? Height { get; set; }
        [JsonProperty(PropertyName = "length")]
        public decimal? Length { get; set; }
        [JsonProperty(PropertyName = "measureUnit")]
        public string MeasureUnit { get; set; }
        [JsonProperty(PropertyName = "shipmentMethodCode")]
        public string ShipmentMethodCode { get; set; }
        [JsonProperty(PropertyName = "shipmentMethodOption")]
        public string ShipmentMethodOption { get; set; }
        [JsonProperty(PropertyName = "volumetricWeight")]
        public decimal? VolumetricWeight { get; set; }
        [JsonProperty(PropertyName = "weight")]
        public decimal? Weight { get; set; }
        [JsonProperty(PropertyName = "weightUnit")]
        public string WeightUnit { get; set; }
        [JsonProperty(PropertyName = "width")]
        public decimal? Width { get; set; }
        [JsonProperty(PropertyName = "deliveryAddress")]
        public AddressDto DeliveryAddress { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
    }
}
