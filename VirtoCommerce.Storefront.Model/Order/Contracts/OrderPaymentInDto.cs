using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.Model.Order
{
    public class OrderPaymentInDto
    {
        public OrderPaymentInDto()
        {
            TaxDetails = new List<TaxDetailDto>();
            Discounts = new List<DiscountDto>();
        }
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "purpose")]
        public string Purpose { get; set; }

        /// <summary>
        /// Gets or sets payment method (gateway) code
        /// </summary>
        [JsonProperty(PropertyName = "gatewayCode")]
        public string GatewayCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentMethod")]
        public PaymentMethod PaymentMethod { get; set; }

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
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerName")]
        public string CustomerName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "incomingDate")]
        public System.DateTime? IncomingDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "billingAddress")]
        public AddressDto BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'New', 'Pending',
        /// 'Authorized', 'Paid', 'PartiallyRefunded', 'Refunded', 'Voided',
        /// 'Custom', 'Cancelled'
        /// </summary>
        [JsonProperty(PropertyName = "paymentStatus")]
        public int PaymentStatus { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "authorizedDate")]
        public System.DateTime? AuthorizedDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "capturedDate")]
        public System.DateTime? CapturedDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "voidedDate")]
        public System.DateTime? VoidedDate { get; set; }

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
        [JsonProperty(PropertyName = "discounts")]
        public IList<DiscountDto> Discounts { get; set; }

        //TODO
        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "transactions")]
        //public IList<PaymentGatewayTransaction> Transactions { get; set; }

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
