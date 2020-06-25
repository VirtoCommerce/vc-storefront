using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class ShoppingCartDto
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "storeId")]
        public string StoreId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isAnonymous")]
        public bool? IsAnonymous { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerName")]
        public string CustomerName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "organizationId")]
        public string OrganizationId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public CurrencyDto Currency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxIncluded")]
        public bool? TaxIncluded { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isRecuring")]
        public bool? IsRecuring { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

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
        [JsonProperty(PropertyName = "validationType")]
        public string ValidationType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "volumetricWeight")]
        public double? VolumetricWeight { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public Money Total { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subTotal")]
        public MoneyDto SubTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subTotalWithTax")]
        public MoneyDto SubTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingTotal")]
        public MoneyDto ShippingTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingTotalWithTax")]
        public MoneyDto ShippingTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingSubTotal")]
        public MoneyDto ShippingSubTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingSubTotalWithTax")]
        public double? ShippingSubTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingDiscountTotal")]
        public double? ShippingDiscountTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingDiscountTotalWithTax")]
        public double? ShippingDiscountTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentTotal")]
        public double? PaymentTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentTotalWithTax")]
        public double? PaymentTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentSubTotal")]
        public double? PaymentSubTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentSubTotalWithTax")]
        public double? PaymentSubTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentDiscountTotal")]
        public double? PaymentDiscountTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentDiscountTotalWithTax")]
        public double? PaymentDiscountTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "handlingTotal")]
        public double? HandlingTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "handlingTotalWithTax")]
        public double? HandlingTotalWithTax { get; set; }

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
        [JsonProperty(PropertyName = "feeTotal")]
        public double? FeeTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "feeTotalWithTax")]
        public double? FeeTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "addresses")]
        public IList<CartAddressDto> Addresses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<CartLineItemDto> Items { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "payments")]
        public IList<Payment> Payments { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shipments")]
        public IList<CartShipmentDto> Shipments { get; set; }

        [JsonProperty(PropertyName = "shippingPrice")]
        public MoneyDto ShippingPrice { get; set; }

        [JsonProperty(PropertyName = "shippingPriceWithTax")]
        public MoneyDto ShippingPriceWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "coupons")]
        public IList<string> Coupons { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "coupon")]
        public string Coupon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discounts")]
        public IList<Discount> Discounts { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxType")]
        public string TaxType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxTotal")]
        public MoneyDto TaxTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxPercentRate")]
        public double? TaxPercentRate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxDetails")]
        public IList<TaxDetailDto> TaxDetails { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; private set; }

        [JsonProperty(PropertyName = "isValid")]
        public bool IsValid { get; set; }

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

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "hasPhysicalProducts")]
        public bool? HasPhysicalProducts { get; set; }

        /// <summary>
        /// Gets or sets the value of measurement unit
        /// </summary>
        [JsonProperty(PropertyName = "measureUnit")]
        public string MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of height
        /// </summary>
        [JsonProperty(PropertyName = "height")]
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the value of length
        /// </summary>
        [JsonProperty(PropertyName = "length")]
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the value of width
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public decimal Width { get; set; }

        [JsonProperty(PropertyName = "itemsCount")]
        public int ItemsCount { get; set; }

        /// <summary>
        /// Gets or sets shopping cart items quantity (sum of each line item quantity * items count)
        /// </summary>
        [JsonProperty(PropertyName = "itemsQuantity")]
        public int ItemsQuantity { get; set; }
    }
}
