using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class CartShipmentDto
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shipmentMethodCode")]
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shipmentMethodOption")]
        public string ShipmentMethodOption { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fulfillmentCenterId")]
        public string FulfillmentCenterId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fulfillmentCenterName")]
        public string FulfillmentCenterName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "warehouseLocation")]
        public string WarehouseLocation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "volumetricWeight")]
        public double? VolumetricWeight { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "weightUnit")]
        public string WeightUnit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "weight")]
        public double? Weight { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "measureUnit")]
        public string MeasureUnit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "height")]
        public double? Height { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "length")]
        public double? Length { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public double? Width { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public MoneyDto Price { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "priceWithTax")]
        public MoneyDto PriceWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public MoneyDto Total { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "totalWithTax")]
        public MoneyDto TotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discountAmount")]
        public MoneyDto DiscountAmount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discountAmountWithTax")]
        public MoneyDto DiscountAmountWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fee")]
        public double? Fee { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "feeWithTax")]
        public double? FeeWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxType")]
        public string TaxType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxTotal")]
        public double? TaxTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxPercentRate")]
        public double? TaxPercentRate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deliveryAddress")]
        public CartAddressDto DeliveryAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<CartShipmentItem> Items { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discounts")]
        public IList<Discount> Discounts { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxDetails")]
        public IList<TaxDetail> TaxDetails { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "dynamicProperties")]
        public IList<DynamicObjectPropertyDto> DynamicProperties { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdDate")]
        public System.DateTime? CreatedDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedDate")]
        public System.DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedBy")]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

    }
}
