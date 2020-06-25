using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class CartLineItemDto
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

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
        [JsonProperty(PropertyName = "sku")]
        public string Sku { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "productType")]
        public string ProductType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "quantity")]
        public int? Quantity { get; set; }

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
        [JsonProperty(PropertyName = "fulfillmentLocationCode")]
        public string FulfillmentLocationCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shipmentMethodCode")]
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "requiredShipping")]
        public bool? RequiredShipping { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

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
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isReccuring")]
        public bool? IsReccuring { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxIncluded")]
        public bool? TaxIncluded { get; set; }

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
        [JsonProperty(PropertyName = "validationType")]
        public string ValidationType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isReadOnly")]
        public bool? IsReadOnly { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "priceId")]
        public string PriceId { get; set; }

        /// <summary>
        /// </summary>
        //[JsonProperty(PropertyName = "price")]
        //public PriceDto Price { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "listPrice")]
        public double? ListPrice { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "listPriceWithTax")]
        public double? ListPriceWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "salePrice")]
        public double? SalePrice { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "salePriceWithTax")]
        public double? SalePriceWithTax { get; set; }

        /// <summary>
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
        /// </summary>
        [JsonProperty(PropertyName = "fee")]
        public double? Fee { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "feeWithTax")]
        public double? FeeWithTax { get; set; }

        /// <summary>
        /// </summary>
        //[JsonProperty(PropertyName = "discounts")]
        //public IList<DiscountDto> Discounts { get; set; }

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
        //[JsonProperty(PropertyName = "taxDetails")]
        //public IList<TaxDetailDto> TaxDetails { get; set; }

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
