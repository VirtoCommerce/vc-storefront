using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Order
{
    /// <summary>
    /// Represent customer order
    /// </summary>
    public partial class CustomerOrder : Entity
    {
        public CustomerOrder(Currency currency)
        {
            Addresses = new List<Address>();
            InPayments = new List<PaymentIn>();
            Items = new List<LineItem>();
            TaxDetails = new List<TaxDetail>();
            DynamicProperties = new List<DynamicProperty>();
            Currency = currency;
            Total = new Money(currency);
            SubTotal = new Money(currency);
            SubTotalWithTax = new Money(currency);
            DiscountTotal = new Money(currency);
            DiscountTotalWithTax = new Money(currency);
            ShippingTotal = new Money(currency);
            ShippingTotalWithTax = new Money(currency);
            TaxTotal = new Money(currency);
            Discounts = new List<Discount>();
            ShippingTaxTotal = new Money(currency);
            ShippingDiscountTotal = new Money(currency);
            ShippingDiscountTotalWithTax = new Money(currency);
            SubTotalTaxTotal = new Money(currency);
            SubTotalDiscount = new Money(currency);
            SubTotalDiscountWithTax = new Money(currency);
            DiscountAmount = new Money(currency);
            DiscountAmountWithTax = new Money(currency);

        }
        public string PaymentMethodType { get; set; }
        /// <summary>
        /// Gets or Sets CustomerName
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or Sets CustomerId
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Chanel (Web site, mobile application etc)
        /// </summary>
        /// <value>Chanel (Web site, mobile application etc)</value>
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or Sets StoreId
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or Sets StoreName
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or Sets OrganizationName
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or Sets OrganizationId
        /// </summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// Employee who should handle that order
        /// </summary>
        /// <value>Employee who should handle that order</value>
        public string EmployeeName { get; set; }

        /// <summary>
        /// Gets or Sets EmployeeId
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// All shipping and billing order addresses
        /// </summary>
        /// <value>All shipping and billing order addresses</value>
        public IList<Address> Addresses { get; set; }
        [JsonIgnore]
        public Address ShippingAddress => Addresses?.FirstOrDefault(a => (a.Type & AddressType.Shipping) == AddressType.Shipping);
        [JsonIgnore]
        public Address BillingAddress => Addresses?.FirstOrDefault(a => (a.Type & AddressType.Billing) == AddressType.Billing) ?? ShippingAddress;
        [JsonIgnore]
        public string Email => Addresses?.Where(a => !string.IsNullOrEmpty(a.Email)).Select(a => a.Email).FirstOrDefault();

        //TODO: Can be evaluated for each 
        public string FinancialStatus => Status;


        /// <summary>
        /// Incoming payments operations
        /// </summary>
        /// <value>Incoming payments operations</value>
        public IList<PaymentIn> InPayments { get; set; }

        /// <summary>
        /// All customer order line items
        /// </summary>
        /// <value>All customer order line items</value>
        public IList<LineItem> Items { get; set; }

        /// <summary>
        /// All customer order shipments
        /// </summary>
        /// <value>All customer order shipments</value>
        public List<Shipment> Shipments { get; set; }

        /// <summary>
        /// All customer order discount
        /// </summary>
        /// <value>All customer order discount</value>
        public Discount Discount { get; set; }

        /// <summary>
        /// Tax details
        /// </summary>
        /// <value>Tax details</value>
        public IList<TaxDetail> TaxDetails { get; set; }

        /// <summary>
        /// Unique user friendly document number (generate automatically based on special algorithm realization)
        /// </summary>
        /// <value>Unique user friendly document number (generate automatically based on special algorithm realization)</value>
        public string Number { get; set; }

        /// <summary>
        /// A Purchase Order (PO) is a document generated by the buyer in order to authorize a purchase transaction.
        /// A PO Number uniquely identifies a purchase order and is generally defined by the buyer.
        /// The buyer will match the PO number in the invoice to the Purchase Order.
        /// </summary>
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// Flag can be used to refer to a specific order status in a variety of user scenarios with combination of Status\r\n            (Order completion, Shipment send etc)
        /// </summary>
        /// <value>Flag can be used to refer to a specific order status in a variety of user scenarios with combination of Status\r\n            (Order completion, Shipment send etc)</value>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Current operation status may have any values defined by concrete business process
        /// </summary>
        /// <value>Current operation status may have any values defined by concrete business process</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or Sets Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Currency code
        /// </summary>
        /// <value>Currency code</value>
        public Currency Currency { get; set; }
        public string CurrencyCode => Currency?.Code;

        /// <summary>
        /// Gets or Sets IsCancelled
        /// </summary>
        public bool? IsCancelled { get; set; }
        public bool? Cancelled => IsCancelled;

        /// <summary>
        /// Gets or Sets CancelledDate
        /// </summary>
        public DateTime? CancelledDate { get; set; }

        /// <summary>
        /// Gets or Sets CancelReason
        /// </summary>
        public string CancelReason { get; set; }

        /// <summary>
        /// Dynamic properties collections
        /// </summary>
        /// <value>Dynamic properties collections</value>
        public IList<DynamicProperty> DynamicProperties { get; set; }

        /// <summary>
        /// Gets or Sets CreatedDate
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or Sets ModifiedDate
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or Sets CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or Sets ModifiedBy
        /// </summary>
        public string ModifiedBy { get; set; }


        public IList<Discount> Discounts { get; set; }

        public Money Total { get; set; }

        public Money DiscountAmount { get; set; }
        public Money DiscountAmountWithTax { get; set; }

        public Money SubTotal { get; set; }
        public Money SubTotalWithTax { get; set; }

        public Money ShippingTotal { get; set; }
        public Money ShippingTotalWithTax { get; set; }
        public Money ShippingTaxTotal { get; set; }
        public Money ShippingPrice { get; set; }
        public Money ShippingPriceWithTax { get; set; }

        public Money PaymentTotal { get; set; }
        public Money PaymentTotalWithTax { get; set; }
        public Money PaymentPrice { get; set; }
        public Money PaymentPriceWithTax { get; set; }
        public Money PaymentDiscountTotal { get; set; }
        public Money PaymentDiscountTotalWithTax { get; set; }
        public Money PaymentTaxTotal { get; set; }

        public Money DiscountTotal { get; set; }
        public Money DiscountTotalWithTax { get; set; }
        public Money TaxTotal { get; set; }
        public Money ShippingDiscountTotalWithTax { get; set; }
        public Money ShippingDiscountTotal { get; set; }
        public Money SubTotalTaxTotal { get; set; }
        public Money SubTotalDiscount { get; set; }
        public Money SubTotalDiscountWithTax { get; set; }
        public string SubscriptionNumber { get; set; }

    }
}
