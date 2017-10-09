﻿using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Subscriptions;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class ShoppingCart : Entity, IDiscountable, IValidatable, IHasLanguage, ITaxable
    {
        public ShoppingCart(Currency currency, Language language)
        {
            Currency = currency;
            Language = language;
            HandlingTotal = new Money(currency);
            HandlingTotalWithTax = new Money(currency);
            DiscountAmount = new Money(currency);
            Addresses = new List<Address>();
            Discounts = new List<Discount>();
            Items = new List<LineItem>();
            Payments = new List<Payment>();
            Shipments = new List<Shipment>();
            TaxDetails = new List<TaxDetail>();
            DynamicProperties = new List<DynamicProperty>();
            ValidationErrors = new List<ValidationError>();
            AvailablePaymentMethods = new List<PaymentMethod>();
            IsValid = true;
        }

        /// <summary>
        /// Gets or sets the value of shopping cart name
        /// </summary>
        public string Name { get; set; }


        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the value of store id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the value of channel id
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the sign that shopping cart contains line items which require shipping
        /// </summary>
        public bool HasPhysicalProducts { get; set; }

        /// <summary>
        /// Gets or sets the flag of shopping cart is anonymous
        /// </summary>
        public bool IsAnonymous { get; set; }

        public User Customer { get; set; }

        /// <summary>
        /// Gets or sets the value of shopping cart customer id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the value of shopping cart customer name
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the value of shopping cart organization id
        /// </summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart coupon
        /// </summary>
        /// <value>
        /// Coupon object
        /// </value>
        public Coupon Coupon { get; set; }

        /// <summary>
        /// Gets or sets the flag of shopping cart is recurring
        /// </summary>
        public bool IsRecuring { get; set; }

        /// <summary>
        /// Gets or sets the value of shopping cart text comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the value of volumetric weight
        /// </summary>
        public decimal VolumetricWeight { get; set; }

        /// <summary>
        /// Gets or sets the value of weight unit
        /// </summary>
        public string WeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of shopping cart weight
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the value of measurement unit
        /// </summary>
        public string MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of height
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the value of length
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the value of width
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Gets the value of shopping cart total cost
        /// </summary>
        public Money Total
        {
            get
            {
                return SubTotal + TaxTotal + ShippingPrice + PaymentPrice - DiscountTotal;
            }
        }

        /// <summary>
        /// Gets the value of shopping cart subtotal
        /// </summary>
        public Money SubTotal
        {
            get
            {
                var subtotal = Items.Sum(i => i.ListPrice.Amount * i.Quantity);
                return new Money(subtotal, Currency);
            }
        }

        /// <summary>
        /// Gets the value of shopping cart subtotal with taxes
        /// </summary>
        public Money SubTotalWithTax
        {
            get
            {
                var subtotalWithTax = Items.Sum(i => i.ListPriceWithTax.Amount * i.Quantity);
                return new Money(subtotalWithTax, Currency);
            }
        }

        /// <summary>
        /// Gets the value of shopping cart items total extended price (product price includes all kinds of discounts)
        /// </summary>
        public Money ExtendedPriceTotal
        {
            get
            {
                var extendedPriceTotal = Items.Sum(i => i.ExtendedPrice.Amount);
                return new Money(extendedPriceTotal, Currency);
            }
        }

        public Money ExtendedPriceTotalWithTax
        {
            get
            {
                var extendedPriceWithTaxTotal = Items.Sum(i => i.ExtendedPriceWithTax.Amount);
                return new Money(extendedPriceWithTaxTotal, Currency);
            }
        }

        /// <summary>
        /// Gets the value of sum shipping cost without discount
        /// </summary>
        public Money ShippingPrice
        {
            get
            {
                var shippingPrice = Shipments.Sum(s => s.Price.Amount);
                return new Money(shippingPrice, Currency);
            }
        }

        public Money ShippingPriceWithTax
        {
            get
            {
                var shippingPriceWithTax = Shipments.Sum(s => s.PriceWithTax.Amount);
                return new Money(shippingPriceWithTax, Currency);
            }
        }


        /// <summary>
        /// Gets the value of shipping total cost
        /// </summary>
        public Money ShippingTotal
        {
            get
            {
                var shippingTotal = Shipments.Sum(s => s.Total.Amount);
                return new Money(shippingTotal, Currency);
            }
        }

        public Money ShippingTotalWithTax
        {
            get
            {
                var shippingTotalWithTax = Shipments.Sum(s => s.TotalWithTax.Amount);
                return new Money(shippingTotalWithTax, Currency);
            }
        }

        public Money PaymentPrice
        {
            get
            {
                var paymentPrice = Payments.Sum(s => s.Price.Amount);
                return new Money(paymentPrice, Currency);
            }
        }

        public Money PaymentPriceWithTax
        {
            get
            {
                var paymentPriceWithTax = Payments.Sum(s => s.PriceWithTax.Amount);
                return new Money(paymentPriceWithTax, Currency);
            }
        }

        public virtual Money PaymentTotal
        {
            get
            {
                var paymentTotal = Payments.Sum(s => s.Total.Amount);
                return new Money(paymentTotal, Currency);
            }
        }

        public virtual Money PaymentTotalWithTax
        {
            get
            {
                var paymentTotalWithTax = Payments.Sum(s => s.TotalWithTax.Amount);
                return new Money(paymentTotalWithTax, Currency);
            }
        }

        /// <summary>
        /// Gets or sets the value of handling total cost
        /// </summary>
        public Money HandlingTotal { get; set; }
        public Money HandlingTotalWithTax { get; set; }

        public Money DiscountAmount { get; set; }

        /// <summary>
        /// Gets the value of total discount amount
        /// </summary>
        public Money DiscountTotal
        {
            get
            {
                var itemDiscountTotal = Items.Sum(i => i.DiscountTotal.InternalAmount);
                var shipmentDiscountTotal = Shipments.Sum(s => s.DiscountAmount.InternalAmount);
                var paymentDiscountTotal = Payments.Sum(s => s.DiscountAmount.InternalAmount);

                return new Money(DiscountAmount.InternalAmount + itemDiscountTotal + shipmentDiscountTotal + paymentDiscountTotal, Currency);
            }
        }


        public Money DiscountTotalWithTax
        {
            get
            {
                var itemDiscountTotalWithTax = Items.Sum(i => i.DiscountTotalWithTax.Amount);
                var shipmentDiscountTotalWithTax = Shipments.Sum(s => s.DiscountAmountWithTax.Amount);
                var paymentDiscountTotalWithTax = Payments.Sum(s => s.DiscountAmountWithTax.Amount);

                return new Money(DiscountAmount.Amount + itemDiscountTotalWithTax + shipmentDiscountTotalWithTax + paymentDiscountTotalWithTax, Currency);
            }
        }

        /// <summary>
        /// Gets or sets the collection of shopping cart addresses
        /// </summary>
        /// <value>
        /// Collection of Address objects
        /// </value>
        public IList<Address> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the value of shopping cart line items
        /// </summary>
        /// <value>
        /// Collection of LineItem objects
        /// </value>
        public IList<LineItem> Items { get; set; }

        public int ItemsCount => Items.Count;

        /// <summary>
        /// Gets or sets shopping cart items quantity (sum of each line item quantity * items count)
        /// </summary>
        public int ItemsQuantity => Items.Sum(i => i.Quantity);

        /// <summary>
        /// Gets or sets the collection of shopping cart payments
        /// </summary>
        /// <value>
        /// Collection of Payment objects
        /// </value>
        public IList<Payment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the collection of shopping cart shipments
        /// </summary>
        /// <value>
        /// Collection of Shipment objects
        /// </value>
        public IList<Shipment> Shipments { get; set; }


        /// <summary>
        /// Used for dynamic properties management, contains object type string
        /// </summary>
        /// <value>Used for dynamic properties management, contains object type string</value>

        public string ObjectType { get; set; }

        /// <summary>
        /// Dynamic properties collections
        /// </summary>
        /// <value>Dynamic properties collections</value>
        public IList<DynamicProperty> DynamicProperties { get; set; }


        public IList<PaymentMethod> AvailablePaymentMethods { get; set; }

        public LineItem RecentlyAddedItem
        {
            get
            {
                return Items.OrderByDescending(i => i.CreatedDate).FirstOrDefault();
            }
        }

        /// <summary>
        /// If the cart is issued as an order by subscription  this property contains the future subscription payment plan
        /// </summary>
        public PaymentPlan PaymentPlan { get; set; }

        #region IValidatable Members
        public bool IsValid { get; set; }
        public IList<ValidationError> ValidationErrors { get; set; }
        #endregion

        #region IDiscountable Members
        public IList<Discount> Discounts { get; }

        public Currency Currency { get; }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            Discounts.Clear();
            DiscountAmount = new Money(Currency);

            var cartRewards = rewards.Where(x => x.RewardType == PromotionRewardType.CartSubtotalReward);
            foreach (var reward in cartRewards)
            {
                //When a discount is applied to the cart subtotal, the tax calculation has already been applied, and is reflected in the tax subtotal.
                //Therefore, a discount applying to the cart subtotal will occur after tax.
                //For instance, if the cart subtotal is $100, and $15 is the tax subtotal, a cart - wide discount of 10 % will yield a total of $105($100 subtotal – $10 discount + $15 tax on the original $100).
                if (reward.IsValid)
                {
                    var discount = reward.ToDiscountModel(ExtendedPriceTotal);
                    Discounts.Add(discount);
                    DiscountAmount = discount.Amount;
                }
            }

            var lineItemRewards = rewards.Where(x => x.RewardType == PromotionRewardType.CatalogItemAmountReward);
            foreach (var lineItem in Items)
            {
                lineItem.ApplyRewards(lineItemRewards);
            }

            var shipmentRewards = rewards.Where(x => x.RewardType == PromotionRewardType.ShipmentReward);
            foreach (var shipment in Shipments)
            {
                shipment.ApplyRewards(shipmentRewards);
            }

            var paymentRewards = rewards.Where(x => x.RewardType == PromotionRewardType.PaymentReward);
            foreach (var payment in Payments)
            {
                payment.ApplyRewards(paymentRewards);
            }

            if (Coupon != null && !string.IsNullOrEmpty(Coupon.Code))
            {
                Coupon.AppliedSuccessfully = rewards.Any(x => x.IsValid && x.Coupon != null);
            }

        }
        #endregion


        #region ITaxable Members
        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal
        {
            get
            {
                var retVal = new Money(0m, Currency);

                foreach (var lineItem in Items)
                {
                    retVal += lineItem.TaxTotal;
                }
                foreach (var shipment in Shipments)
                {
                    retVal += shipment.TaxTotal;
                }
                foreach (var payment in Payments)
                {
                    retVal += payment.TaxTotal;
                }
                return retVal;
            }
        }

        public decimal TaxPercentRate { get; private set; }

        /// <summary>
        /// Gets or sets the value of shipping tax type
        /// </summary>
        public string TaxType { get; set; }

        /// <summary>
        /// Gets or sets the collection of line item tax details lines
        /// </summary>
        /// <value>
        /// Collection of TaxDetail objects
        /// </value>
        public IList<TaxDetail> TaxDetails { get; set; }

        public void ApplyTaxRates(IEnumerable<TaxRate> taxRates)
        {
            foreach (var lineItem in Items)
            {
                lineItem.ApplyTaxRates(taxRates);
            }
            foreach (var shipment in Shipments)
            {
                shipment.ApplyTaxRates(taxRates);
            }
            foreach (var payment in Payments)
            {
                payment.ApplyTaxRates(taxRates);
            }
        }
        #endregion

        #region IHasLanguage Members
        public Language Language { get; set; }
        #endregion

        public override string ToString()
        {
            var cartId = Id ?? "transient";
            var customer = Customer != null ? Customer.ToString() : "undefined";

            return $"Cart #{cartId}-{Name} {customer}";
        }
    }
}
