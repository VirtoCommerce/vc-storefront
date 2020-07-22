using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.Model.Order
{
    public class CustomerOrderRequestDto
    {
        public CustomerOrderRequestDto()
        {
            Addresses = new List<AddressDto>();
            InPayments = new List<OrderPaymentInDto>();
            Items = new List<OrderLineItemDto>();
            Shipments = new List<OrderShipmentDto>();
            Discounts = new List<DiscountDto>();
            TaxDetails = new List<TaxDetailDto>();
        }

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
        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "storeId")]
        public string StoreId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "storeName")]
        public string StoreName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "organizationId")]
        public string OrganizationId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "employeeId")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "employeeName")]
        public string EmployeeName { get; set; }

        /// <summary>
        /// Gets or sets the basis shopping cart id of which the order was
        /// created
        /// </summary>
        [JsonProperty(PropertyName = "shoppingCartId")]
        public string ShoppingCartId { get; set; }

        /// <summary>
        /// Gets or sets flag determines that the order is the prototype
        /// </summary>
        [JsonProperty(PropertyName = "isPrototype")]
        public bool? IsPrototype { get; set; }

        /// <summary>
        /// Gets or sets number for subscription  associated with this order
        /// </summary>
        [JsonProperty(PropertyName = "subscriptionNumber")]
        public string SubscriptionNumber { get; set; }

        /// <summary>
        /// Gets or sets identifier for subscription  associated with this
        /// order
        /// </summary>
        [JsonProperty(PropertyName = "subscriptionId")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "addresses")]
        public IList<AddressDto> Addresses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "inPayments")]
        public IList<OrderPaymentInDto> InPayments { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<OrderLineItemDto> Items { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shipments")]
        public IList<OrderShipmentDto> Shipments { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discounts")]
        public IList<DiscountDto> Discounts { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "discountAmount")]
        public double? DiscountAmount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxDetails")]
        public IList<TaxDetailDto> TaxDetails { get; set; }

        /// <summary>
        /// Gets or sets grand order total
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public double? Total { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subTotal")]
        public double? SubTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subTotalWithTax")]
        public double? SubTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subTotalDiscount")]
        public double? SubTotalDiscount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subTotalDiscountWithTax")]
        public double? SubTotalDiscountWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subTotalTaxTotal")]
        public double? SubTotalTaxTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingTotal")]
        public double? ShippingTotal { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingTotalWithTax")]
        public double? ShippingTotalWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingSubTotal")]
        public double? ShippingSubTotal { get; set; }

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
        [JsonProperty(PropertyName = "shippingTaxTotal")]
        public double? ShippingTaxTotal { get; set; }

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
        [JsonProperty(PropertyName = "paymentTaxTotal")]
        public double? PaymentTaxTotal { get; set; }

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
        /// </summary>
        [JsonProperty(PropertyName = "languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "operationType")]
        public string OperationType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "parentOperationId")]
        public string ParentOperationId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isApproved")]
        public bool? IsApproved { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "sum")]
        public double? Sum { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "outerId")]
        public string OuterId { get; set; }

        //TODO
        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "childrenOperations")]
        //public IList<IOperation> ChildrenOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isCancelled")]
        public bool? IsCancelled { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "cancelledDate")]
        public System.DateTime? CancelledDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "cancelReason")]
        public string CancelReason { get; set; }

        //TODO
        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "dynamicProperties")]
        //public IList<DynamicObjectProperty> DynamicProperties { get; set; }

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
