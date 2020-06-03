using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Infrastructure.Swagger;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Cart
{
    [SwaggerSchemaId("CartShipment")]
    public partial class Shipment : Entity, IDiscountable, IValidatable, ITaxable
    {
        public Shipment()
        {
            Discounts = new List<Discount>();
            Items = new List<CartShipmentItem>();
            TaxDetails = new List<TaxDetail>();
            ValidationErrors = new List<ValidationError>();
        }

        public Shipment(Currency currency)
            : this()
        {
            Currency = currency;

            Price = new Money(currency);
            PriceWithTax = new Money(currency);
            DiscountAmount = new Money(currency);
            DiscountAmountWithTax = new Money(currency);
            Total = new Money(currency);
            TotalWithTax = new Money(currency);
            TaxTotal = new Money(currency);
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
        public string FulfillmentCenterId { get; set; }

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
        /// Price * TaxPercentRate
        /// </summary>
        public Money PriceWithTax { get; set; }

        /// <summary>
        /// Gets the value of total shipping price without taxes
        /// Price + Fee - DiscountAmount;
        /// </summary>
        public Money Total { get; set; }

        /// <summary>
        /// Gets the value of total shipping price including taxes
        /// Total * TaxPercentRate
        /// </summary>
        public Money TotalWithTax { get; set; }

        /// <summary>
        /// Gets the value of total shipping discount amount
        /// </summary>
        public Money DiscountAmount { get; set; }
        /// <summary>
        /// DiscountAmount * TaxPercentRate
        /// </summary>
        public Money DiscountAmountWithTax { get; set; }

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
        public Money TaxTotal { get; set; }

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
            if (shipmentTaxRate == null)
            {
                shipmentTaxRate = taxRates.FirstOrDefault(x => x.Line.Code.EqualsInvariant(ShipmentMethodCode) && x.Line.Name.EqualsInvariant(ShipmentMethodOption));
            }
            if (shipmentTaxRate != null && shipmentTaxRate.Rate.Amount > 0)
            {
                if (shipmentTaxRate.PercentRate > 0)
                {
                    TaxPercentRate = shipmentTaxRate.PercentRate;
                }
                else
                {
                    var amount = Total.Amount > 0 ? Total.Amount : Price.Amount;
                    if (amount > 0)
                    {
                        TaxPercentRate = TaxRate.TaxPercentRound(shipmentTaxRate.Rate.Amount / amount);
                    }
                }

                TaxDetails = shipmentTaxRate.Line.TaxDetails;
            }
        }
        #endregion

        #region IValidatable Members
        public bool IsValid => ValidationErrors?.Any() ?? true;
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

                if (reward.IsValid && discount.Amount.InternalAmount > 0)
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

        public override object Clone()
        {
            var result = base.Clone() as Shipment;

            result.Price = Price?.Clone() as Money;
            result.PriceWithTax = PriceWithTax?.Clone() as Money;
            result.DiscountAmount = DiscountAmount?.Clone() as Money;
            result.DiscountAmountWithTax = DiscountAmountWithTax?.Clone() as Money;
            result.Total = Total?.Clone() as Money;
            result.TotalWithTax = TotalWithTax?.Clone() as Money;
            result.TaxTotal = TaxTotal?.Clone() as Money;


            if (Discounts != null)
            {
                result.Discounts = new List<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }
            if (TaxDetails != null)
            {
                result.TaxDetails = new List<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }
            if (Items != null)
            {
                result.Items = new List<CartShipmentItem>(Items.Select(x => x.Clone() as CartShipmentItem));
            }
            if (ValidationErrors != null)
            {
                result.ValidationErrors = new List<ValidationError>(ValidationErrors.Select(x => x.Clone() as ValidationError));
            }

            return result;
        }
    }
}
