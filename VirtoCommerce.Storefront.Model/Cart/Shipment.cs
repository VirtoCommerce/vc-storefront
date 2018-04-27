using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class Shipment : Entity, IDiscountable, IValidatable, ITaxable
    {
        public Shipment()
        {
            Discounts = new List<Discount>();
            Items = new List<CartShipmentItem>();
            TaxDetails = new List<TaxDetail>();
            ValidationErrors = new List<ValidationError>();
            IsValid = true;
        }

        public Shipment(Currency currency)
            :this()
        {
            Currency = currency;
        
            Price = new Money(currency);
            DiscountAmount = new Money(currency);
        }
     
        /// <summary>
        /// Gets or sets the value of shipping method code
        /// </summary>
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping method option
        /// </summary>
        public string ShipmentMethodOption { get; set; }

        /// <summary>
        /// Gets or sets the value of fulfillment center id
        /// </summary>
        public string FulfilmentCenterId { get; set; }

        /// <summary>
        /// Gets or sets the delivery address
        /// </summary>
        /// <value>
        /// Address object
        /// </value>
        public Address DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets the value of volumetric weight
        /// </summary>
        public decimal? VolumetricWeight { get; set; }

        /// <summary>
        /// Gets or sets the value of weight unit
        /// </summary>
        public string WeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of weight
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Gets or sets the value of measurement units
        /// </summary>
        public string MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets the value of height
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Gets or sets the value of length
        /// </summary>
        public double? Length { get; set; }

        /// <summary>
        /// Gets or sets the value of width
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping price
        /// </summary>
        public Money Price { get; set; }

     
            /// <summary>
        /// Gets or sets the value of shipping price including tax
        /// </summary>
        public Money PriceWithTax
        {
            get
            {
                return Price + Price * TaxPercentRate;
            }
        }

        /// <summary>
        /// Gets the value of total shipping price without taxes
        /// </summary>
        public Money Total
        {
            get
            {
                return Price - DiscountAmount;
            }
        }

        /// <summary>
        /// Gets the value of total shipping price including taxes
        /// </summary>
        public Money TotalWithTax
        {
            get
            {
                return PriceWithTax - DiscountAmountWithTax;
            }
        }

        /// <summary>
        /// Gets the value of total shipping discount amount
        /// </summary>
        public Money DiscountAmount { get; set; }
        public Money DiscountAmountWithTax
        {
            get
            {
                return DiscountAmount + DiscountAmount * TaxPercentRate;
            }
        }

        /// <summary>
        /// Gets or sets the collection of shipping items
        /// </summary>
        /// <value>
        /// Collection of CartShipmentItem objects
        /// </value>
        public IList<CartShipmentItem> Items { get; set; }

        #region ITaxable Members
        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal
        {
            get
            {
                return TotalWithTax - Total;
            }
        }

        public decimal TaxPercentRate { get; set; }

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
            TaxPercentRate = 0m;
            var shipmentTaxRate = taxRates.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(Id ?? ""));
            if(shipmentTaxRate == null)
            {
                shipmentTaxRate = taxRates.FirstOrDefault(x => x.Line.Code.EqualsInvariant(ShipmentMethodCode) && x.Line.Name.EqualsInvariant(ShipmentMethodOption));
            }          
            if (shipmentTaxRate != null && shipmentTaxRate.Rate.Amount > 0)
            {
                var amount = Total.Amount > 0 ? Total.Amount : Price.Amount;
                if (amount > 0)
                {
                    TaxPercentRate = TaxRate.TaxPercentRound(shipmentTaxRate.Rate.Amount / amount);
                }
            }
        }
        #endregion

        #region IValidatable Members
        public bool IsValid { get; set; }
        public IList<ValidationError> ValidationErrors { get; set; }
        #endregion

        #region IDiscountable Members
        public IList<Discount> Discounts { get; private set; }

        public Currency Currency { get; set; }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var shipmentRewards = rewards.Where(r => r.RewardType == PromotionRewardType.ShipmentReward && (string.IsNullOrEmpty(r.ShippingMethodCode) || r.ShippingMethodCode.EqualsInvariant(ShipmentMethodCode)));

            Discounts.Clear();

            DiscountAmount = new Money(0m, Currency);

            foreach (var reward in shipmentRewards)
            {
                var discount = reward.ToDiscountModel(Price - DiscountAmount);

                if (reward.IsValid)
                {
                    Discounts.Add(discount);
                    DiscountAmount += discount.Amount;
                }
            }
        }
        #endregion

        public bool HasSameMethod(ShippingMethod method)
        {           
            // Return true if the fields match:
            return (ShipmentMethodCode.EqualsInvariant(method.ShipmentMethodCode)) && (ShipmentMethodOption.EqualsInvariant(method.OptionName));
        }
    }
}
