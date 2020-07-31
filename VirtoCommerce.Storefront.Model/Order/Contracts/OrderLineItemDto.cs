using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Contracts;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Order
{
    public class OrderLineItemDto
    {
        public OrderLineItemDto()
        {
            Discounts = new List<DiscountDto>();
            TaxDetails = new List<TaxDetailDto>();
        }


        /// <summary>
        /// Gets or sets price id
        /// </summary>
        [JsonProperty(PropertyName = "priceId")]
        public string PriceId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets unit price without discount and tax
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public double? Price { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "priceWithTax")]
        public double? PriceWithTax { get; set; }

        /// <summary>
        /// Gets or sets resulting price with discount for one unit
        /// </summary>
        [JsonProperty(PropertyName = "placedPrice")]
        public double? PlacedPrice { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "placedPriceWithTax")]
        public double? PlacedPriceWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "extendedPrice")]
        public double? ExtendedPrice { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "extendedPriceWithTax")]
        public double? ExtendedPriceWithTax { get; set; }

        /// <summary>
        /// Gets the value of the single qty line item discount amount
        /// </summary>
        [JsonProperty(PropertyName = "discountAmount")]
        public double? DiscountAmount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discountAmountWithTax")]
        public double? DiscountAmountWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discountTotal")]
        public double? DiscountTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discountTotalWithTax")]
        public double? DiscountTotalWithTax { get; set; }

        /// <summary>
        /// Gets or sets tax category or type
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
        /// Gets or sets reserve quantity
        /// </summary>
        [JsonProperty(PropertyName = "reserveQuantity")]
        public int? ReserveQuantity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "sku")]
        public string Sku { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "productType")]
        public string ProductType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "catalogId")]
        public string CatalogId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "categoryId")]
        public string CategoryId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isGift")]
        public bool? IsGift { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingMethodCode")]
        public string ShippingMethodCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fulfillmentLocationCode")]
        public string FulfillmentLocationCode { get; set; }

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
        [JsonProperty(PropertyName = "outerId")]
        public string OuterId { get; set; }

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
        [JsonProperty(PropertyName = "isCancelled")]
        public bool? IsCancelled { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "cancelledDate")]
        public DateTime? CancelledDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "cancelReason")]
        public string CancelReason { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "objectType")]
        //public string ObjectType { get; private set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "dynamicProperties")]
        //public IList<DynamicObjectProperty> DynamicProperties { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discounts")]
        public IList<DiscountDto> Discounts { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxDetails")]
        public IList<TaxDetailDto> TaxDetails { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "createdDate")]
        //public DateTime? CreatedDate { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "modifiedDate")]
        //public DateTime? ModifiedDate { get; set; }

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
