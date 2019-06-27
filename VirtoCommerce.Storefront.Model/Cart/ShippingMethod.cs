using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model
{
    public partial class ShippingMethod : ValueObject, ITaxable, IDiscountable
    {
        public ShippingMethod()
        {
            Discounts = new List<Discount>();
        }

        public ShippingMethod(Currency currency)
            : this()
        {
            Currency = currency;
            Price = new Money(currency);
            DiscountAmount = new Money(currency);
        }

        /// <summary>
        /// Gets or sets the value of shipping method priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping method code
        /// </summary>
        public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping method name
        /// </summary>
        public string Name { get; set; }
        public string Title => Name;

        public string Handle => ShipmentMethodCode;
        /// <summary>
        /// Gets or sets the value of shipping method option name
        /// </summary>
        public string OptionName { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping method option description
        /// </summary>
        public string OptionDescription { get; set; }

        /// <summary>
        /// Gets or sets the value of shipping method logo absolute URL
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// price without discount and taxes
        /// </summary>
        public Money Price { get; set; }

        /// <summary>
        ///  price with tax but without discount
        /// </summary>
        public Money PriceWithTax
        {
            get
            {
                return Price + Price * TaxPercentRate;
            }
        }

        /// <summary>
        /// Resulting price with discount but without tax
        /// </summary>
        public Money Total
        {
            get
            {
                return Price - DiscountAmount;
            }
        }
        /// <summary>
        /// Resulting price with discount and tax
        /// </summary>
        public Money TotalWithTax
        {
            get
            {
                return PriceWithTax - DiscountAmountWithTax;
            }
        }

        /// <summary>
        /// Total discount amount without tax
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
        /// Custom properties for shipping method
        /// </summary>
        public List<SettingEntry> Settings { get; set; }

        #region ITaxable Members
        /// <summary>
        /// Gets the value of total shipping method tax 
        /// </summary>
        public Money TaxTotal
        {
            get
            {
                return TotalWithTax - Total;
            }
        }

        /// <summary>
        /// Gets or sets the value of shipping tax type
        /// </summary>
        public string TaxType { get; set; }

        public decimal TaxPercentRate { get; private set; }

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
            var taxLineId = BuildTaxLineId();
            var taxRate = taxRates.FirstOrDefault(x => x.Line.Id == taxLineId);

            if (taxRate != null && taxRate.Rate.Amount > 0)
            {
                var amount = Total.Amount > 0 ? Total.Amount : Price.Amount;
                if (amount > 0)
                {
                    TaxPercentRate = TaxRate.TaxPercentRound(taxRate.Rate.Amount / amount);
                }
            }
        }

        public virtual string BuildTaxLineId()
        {
            return string.Join("&", ShipmentMethodCode, OptionName);
        }

        #endregion

        #region IDiscountable Members
        public IList<Discount> Discounts { get; private set; }

        public Currency Currency { get; set; }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var shipmentRewards = rewards.Where(r => r.RewardType == PromotionRewardType.ShipmentReward && (r.ShippingMethodCode.IsNullOrEmpty() || r.ShippingMethodCode.EqualsInvariant(ShipmentMethodCode)));

            Discounts.Clear();
            DiscountAmount = new Money(0m, Currency);

            foreach (var reward in shipmentRewards)
            {
                var discount = reward.ToDiscountModel(Price);

                if (reward.IsValid)
                {
                    Discounts.Add(discount);
                    DiscountAmount += discount.Amount;
                }
            }
        }
        #endregion


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ShipmentMethodCode;
            yield return OptionName;
        }


        public override object Clone()
        {
            var result = base.Clone() as ShippingMethod;
            result.Price = Price?.Clone() as Money;
            result.DiscountAmount = DiscountAmount?.Clone() as Money;
            return result;
        }


    }
}
