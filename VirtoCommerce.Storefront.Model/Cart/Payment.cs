using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class Payment : Entity, ITaxable, IDiscountable
    {
        public Payment(Currency currency)
        {
            Discounts = new List<Discount>();
            TaxDetails = new List<TaxDetail>();

            Currency = currency;
            Price = new Money(currency);
            DiscountAmount = new Money(currency);
            Amount = new Money(currency);
        }

        /// <summary>
        /// Gets or sets the value of payment outer id
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// Gets or sets the value of payment gateway code
        /// </summary>
        public string PaymentGatewayCode { get; set; }

        /// <summary>
        /// Gets or sets the value of payment currency
        /// </summary>
        /// <value>
        /// Currency code in ISO 4217 format
        /// </value>
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the value of payment amount
        /// </summary>
        public Money Amount { get; set; }

        /// <summary>
        /// Gets or sets the billing address
        /// </summary>
        /// <value>
        /// Address object
        /// </value>
        public Address BillingAddress { get; set; }


        /// <summary>
        /// Gets or sets the value of payment service price
        /// </summary>
        public Money Price { get; set; }


        /// <summary>
        /// Gets or sets the value of payment service price including tax
        /// </summary>
        public Money PriceWithTax
        {
            get
            {
                return Price + Price * TaxPercentRate;
            }
        }

        /// <summary>
        /// Gets the value of total payment service price without taxes
        /// </summary>
        public Money Total
        {
            get
            {
                return Price - DiscountAmount;
            }
        }

        /// <summary>
        /// Gets the value of total payment service price including taxes
        /// </summary>
        public Money TotalWithTax
        {
            get
            {
                return PriceWithTax - DiscountAmountWithTax;
            }
        }

        /// <summary>
        /// Gets the value of total payment service discount amount
        /// </summary>
        public Money DiscountAmount { get; set; }
        public Money DiscountAmountWithTax
        {
            get
            {
                return DiscountAmount + DiscountAmount * TaxPercentRate;
            }
        }

        #region ITaxable Members
        /// <summary>
        /// Gets or sets the value of total payment service tax amount
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
        /// Gets or sets the value of payment tax type
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
            var paymentTaxRate = taxRates.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(Id ?? ""));
            if (paymentTaxRate == null)
            {
                paymentTaxRate = taxRates.FirstOrDefault(x => x.Line.Code.EqualsInvariant(PaymentGatewayCode));
            }
            if (paymentTaxRate != null && paymentTaxRate.Rate.Amount > 0)
            {
                var amount = Total.Amount > 0 ? Total.Amount : Price.Amount;
                if (amount > 0)
                {
                    TaxPercentRate = TaxRate.TaxPercentRound(paymentTaxRate.Rate.Amount / amount);
                }
            }            
        }
        #endregion

        #region IDiscountable Members
        public IList<Discount> Discounts { get; private set; }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var paymentRewards = rewards.Where(r => r.RewardType == PromotionRewardType.PaymentReward && (r.PaymentMethodCode.IsNullOrEmpty() || r.PaymentMethodCode.EqualsInvariant(PaymentGatewayCode)));

            Discounts.Clear();

            DiscountAmount = new Money(0m, Currency);

            foreach (var reward in paymentRewards)
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
    }
}