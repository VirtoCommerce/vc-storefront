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
            PriceWithTax = new Money(currency);
            DiscountAmount = new Money(currency);
            DiscountAmountWithTax = new Money(currency);
            Amount = new Money(currency);
            Total = new Money(currency);
            TotalWithTax = new Money(currency);
            TaxTotal = new Money(currency);
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
        /// Price * TaxPercentRate
        /// </summary>
        public Money PriceWithTax { get; set; }

        /// <summary>
        /// Gets the value of total payment service price without taxes
        /// Price - DiscountAmount;
        /// </summary>
        public Money Total { get; set; }

        /// <summary>
        /// Gets the value of total payment service price including taxes
        /// Total * TaxPercentRate
        /// </summary>
        public Money TotalWithTax { get; set; }

        /// <summary>
        /// Gets the value of total payment service discount amount
        /// </summary>
        public Money DiscountAmount { get; set; }
        /// <summary>
        /// DiscountAmount * TaxPercentRate
        /// </summary>
        public Money DiscountAmountWithTax { get; set; }

        #region ITaxable Members
        /// <summary>
        /// Gets or sets the value of total payment service tax amount
        /// </summary>
        public Money TaxTotal { get; set; }

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

                if (reward.IsValid && discount.Amount.InternalAmount > 0)
                {
                    Discounts.Add(discount);
                    DiscountAmount += discount.Amount;
                }
            }
        }
        #endregion

        public override object Clone()
        {
            var result = base.Clone() as Payment;

            result.Currency = result.Currency?.Clone() as Currency;
            result.Price = result.Price?.Clone() as Money;
            result.PriceWithTax = result.PriceWithTax?.Clone() as Money;
            result.DiscountAmount = result.DiscountAmount?.Clone() as Money;
            result.DiscountAmountWithTax = result.DiscountAmountWithTax?.Clone() as Money;
            result.Amount = result.Amount?.Clone() as Money;
            result.Total = result.Total?.Clone() as Money;
            result.TotalWithTax = result.TotalWithTax?.Clone() as Money;
            result.TaxTotal = result.TaxTotal?.Clone() as Money;

            if (Discounts != null)
            {
                result.Discounts = new List<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }
            if (TaxDetails != null)
            {
                result.TaxDetails = new List<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }

            return result;
        }
    }
}
