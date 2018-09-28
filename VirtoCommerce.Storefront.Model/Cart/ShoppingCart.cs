using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Subscriptions;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class ShoppingCart : Entity, IDiscountable, IValidatable, IHasLanguage, ITaxable, ICacheKey
    {
        public ShoppingCart(Currency currency, Language language)
        {
            Currency = currency;
            Language = language;
            HandlingTotal = new Money(currency);
            HandlingTotalWithTax = new Money(currency);
            DiscountAmount = new Money(currency);
            Total = new Money(currency);
            SubTotal = new Money(currency);
            SubTotalWithTax = new Money(currency);
            ShippingPrice = new Money(currency);
            ShippingPriceWithTax = new Money(currency);
            ShippingTotal = new Money(currency);
            ShippingTotalWithTax = new Money(currency);
            PaymentPrice = new Money(currency);
            PaymentPriceWithTax = new Money(currency);
            PaymentTotal = new Money(currency);
            PaymentTotalWithTax = new Money(currency);
            HandlingTotal = new Money(currency);
            HandlingTotalWithTax = new Money(currency);
            DiscountTotal = new Money(currency);
            DiscountTotalWithTax = new Money(currency);
            TaxTotal = new Money(currency);

            Addresses = new List<Address>();
            Discounts = new List<Discount>();
            Items = new List<LineItem>();
            Payments = new List<Payment>();
            Shipments = new List<Shipment>();
            TaxDetails = new List<TaxDetail>();
            Coupons = new List<Coupon>();
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

        ///// Gets or sets the shopping cart coupon
        ///// </summary>
        ///// <value>
        ///// Coupon object
        ///// </value>
        //public Coupon Coupon { get; set; }

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
        /// SubTotal + ShippingSubTotal + TaxTotal + PaymentSubTotal + FeeTotal - DiscountTotal;
        /// </summary>
        public Money Total { get; set; }

        /// <summary>
        /// Gets the value of shopping cart subtotal
        /// Items.Sum(x => x.ListPrice * x.Quantity);
        /// </summary>
        public Money SubTotal { get; set; }

        /// <summary>
        /// Gets the value of shopping cart subtotal with taxes
        /// Items.Sum(x => x.ListPriceWithTax * x.Quantity);
        /// </summary>
        public Money SubTotalWithTax { get; set; }

        /// <summary>
        /// Gets the value of sum shipping cost without discount
        /// Shipments.Sum(x => x.Price);
        /// </summary>
        public Money ShippingPrice { get; set; }

        /// <summary>
        /// Shipments.Sum(x => x.PriceWithTax);
        /// </summary>
        public Money ShippingPriceWithTax { get; set; }

        /// <summary>
        /// Gets the value of shipping total cost
        /// Shipments.Sum(x => x.Total)
        /// </summary>
        public Money ShippingTotal { get; set; }

        /// <summary>
        /// Shipments.Sum(x => x.TotalWithTax)
        /// </summary>
        public Money ShippingTotalWithTax { get; set; }

        /// <summary>
        /// Payments.Sum(x => x.Price)
        /// </summary>
        public Money PaymentPrice { get; set; }

        /// <summary>
        /// Payments.Sum(x => x.PriceWithTax)
        /// </summary>
        public Money PaymentPriceWithTax { get; set; }

        /// <summary>
        /// Payments.Sum(x => x.Total)
        /// </summary>
        public virtual Money PaymentTotal { get; set; }

        /// <summary>
        /// Payments.Sum(x => x.TotalWithTax)
        /// </summary>
        public virtual Money PaymentTotalWithTax { get; set; }

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
        /// Gets or sets the value of handling total cost
        /// </summary>
        public Money HandlingTotal { get; set; }
        public Money HandlingTotalWithTax { get; set; }

        public Money DiscountAmount { get; set; }

        /// <summary>
        /// Gets the value of total discount amount
        /// Items.Sum(x => x.DiscountTotal) + Shipments.Sum(x => x.DiscountAmount) + Payments.Sum(x => x.DiscountAmount) + DiscountAmount
        /// </summary>
        public Money DiscountTotal { get; set; }

        /// <summary>
        /// Items.Sum(x => x.DiscountTotalWithTax) + Shipments.Sum(x => x.DiscountAmountWithTax) + Payments.Sum(x => x.DiscountAmountWithTax) + DiscountAmountWithTax
        /// </summary>
        public Money DiscountTotalWithTax { get; set; }

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
        /// Left for backward compatibility with old themes
        /// </summary>
        public Coupon Coupon
        {
            get
            {
                return Coupons.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets or sets the collection of shopping cart coupons
        /// </summary>
        /// <value>
        /// Collection of Coupon objects
        /// </value>
        public IList<Coupon> Coupons { get; set; }

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
        public IList<Discount> Discounts { get; private set; }

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
                    //Use rounded amount for whole cart discount
                    DiscountAmount += discount.Amount.Amount;
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

            foreach (var coupon in Coupons)
            {
                coupon.AppliedSuccessfully = !string.IsNullOrEmpty(coupon.Code) && rewards.Any(x => x.IsValid && x.Coupon.EqualsInvariant(coupon.Code));
            }
        }
        #endregion


        #region ITaxable Members
        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal { get; set; }

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

        /// <summary>
        /// Gets or sets shopping cart type - Cart, Wishlist
        /// </summary>
        public string Type { get; set; }

        public void ApplyTaxRates(IEnumerable<TaxRate> taxRates)
        {
            TaxPercentRate = 0m;
            foreach (var lineItem in Items)
            {
                //Get percent rate from line item
                if (TaxPercentRate == 0)
                {
                    TaxPercentRate = lineItem.TaxPercentRate;
                }
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

        #region ICacheKey
        public override string GetCacheKey()
        {
            return string.Join(":", "Cart", Id, Name, CustomerId);
        }
        #endregion

        public override string ToString()
        {
            var cartId = Id ?? "transient";
            var customer = Customer != null ? Customer.ToString() : "undefined";

            return $"Cart #{cartId}-{Name} {customer}";
        }

        public override object Clone()
        {
            var result = base.Clone() as ShoppingCart;

            result.HandlingTotal = HandlingTotal?.Clone() as Money;
            result.HandlingTotalWithTax = HandlingTotalWithTax?.Clone() as Money;
            result.DiscountAmount = DiscountAmount?.Clone() as Money;
            result.Total = Total?.Clone() as Money;
            result.SubTotal = SubTotal?.Clone() as Money;
            result.SubTotalWithTax = SubTotalWithTax?.Clone() as Money;
            result.ShippingPrice = ShippingPrice?.Clone() as Money;
            result.ShippingPriceWithTax = ShippingPriceWithTax?.Clone() as Money;
            result.ShippingTotal = ShippingTotal?.Clone() as Money;
            result.ShippingTotalWithTax = ShippingTotalWithTax?.Clone() as Money;
            result.PaymentPrice = PaymentPrice?.Clone() as Money;
            result.PaymentPriceWithTax = PaymentPriceWithTax?.Clone() as Money;
            result.PaymentTotal = PaymentTotal?.Clone() as Money;
            result.PaymentTotalWithTax = PaymentTotalWithTax?.Clone() as Money;
            result.HandlingTotal = HandlingTotal?.Clone() as Money;
            result.HandlingTotalWithTax = HandlingTotalWithTax?.Clone() as Money;
            result.DiscountTotal = DiscountTotal?.Clone() as Money;
            result.DiscountTotalWithTax = DiscountTotalWithTax?.Clone() as Money;
            result.TaxTotal = TaxTotal?.Clone() as Money;

            if (Discounts != null)
            {
                result.Discounts = new List<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }
            if (TaxDetails != null)
            {
                result.TaxDetails = new List<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }
            if (DynamicProperties != null)
            {
                result.DynamicProperties = new List<DynamicProperty>(DynamicProperties.Select(x => x.Clone() as DynamicProperty));
            }
            if (ValidationErrors != null)
            {
                result.ValidationErrors = new List<ValidationError>(ValidationErrors.Select(x => x.Clone() as ValidationError));
            }
            if (Addresses != null)
            {
                result.Addresses = new List<Address>(Addresses.Select(x => x.Clone() as Address));
            }
            if (Items != null)
            {
                result.Items = new List<LineItem>(Items.Select(x => x.Clone() as LineItem));
            }
            if (Payments != null)
            {
                result.Payments = new List<Payment>(Payments.Select(x => x.Clone() as Payment));
            }
            if (Shipments != null)
            {
                result.Shipments = new List<Shipment>(Shipments.Select(x => x.Clone() as Shipment));
            }
            if (Coupons != null)
            {
                result.Coupons = new List<Coupon>(Coupons.Select(x => x.Clone() as Coupon));
            }
            if (AvailablePaymentMethods != null)
            {
                result.AvailablePaymentMethods = new List<PaymentMethod>(AvailablePaymentMethods.Select(x => x.Clone() as PaymentMethod));
            }

            return result;
        }
    }
}
