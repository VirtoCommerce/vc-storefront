using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.Model.Order
{
    public class OrderShipmentDto
    {
        public OrderShipmentDto()
        {
            Discounts = new List<DiscountDto>();
            TaxDetails = new List<TaxDetailDto>();
            Items = new List<OrderShipmentItemDto>();
            Packages = new List<ShipmentPackageDto>();
            InPayments = new List<OrderPaymentInDto>();
        }

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
        [JsonProperty(PropertyName = "fulfillmentCenterId")]
        public string FulfillmentCenterId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fulfillmentCenterName")]
        public string FulfillmentCenterName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "employeeId")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "employeeName")]
        public string EmployeeName { get; set; }

        /// <summary>
        /// Gets or sets current shipment method code
        /// </summary>
        [JsonProperty(PropertyName = "shipmentMethodCode")]
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets current shipment option code
        /// </summary>
        [JsonProperty(PropertyName = "shipmentMethodOption")]
        public string ShipmentMethodOption { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "shippingMethod")]
        public ShippingMethod ShippingMethod { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerOrderId")]
        public string CustomerOrderId { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "customerOrder")]
        //public CustomerOrderRequestDto CustomerOrder { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<OrderShipmentItemDto> Items { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "packages")]
        public IList<ShipmentPackageDto> Packages { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "inPayments")]
        public IList<OrderPaymentInDto> InPayments { get; set; }

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
        [JsonProperty(PropertyName = "discounts")]
        public IList<DiscountDto> Discounts { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deliveryAddress")]
        public AddressDto DeliveryAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public double? Price { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "priceWithTax")]
        public double? PriceWithTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public double? Total { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "totalWithTax")]
        public double? TotalWithTax { get; set; }

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
        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; set; }

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
        [JsonProperty(PropertyName = "taxDetails")]
        public IList<TaxDetailDto> TaxDetails { get; set; }

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
        public bool IsCancelled { get; set; }

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
        //[JsonProperty(PropertyName = "operationsLog")]
        //public IList<OperationLog> OperationsLog { get; set; }

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
