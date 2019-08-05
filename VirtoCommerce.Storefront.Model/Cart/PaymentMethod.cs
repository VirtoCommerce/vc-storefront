using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model
{
    public partial class PaymentMethod : ValueObject, ITaxable, IDiscountable
    {

        public PaymentMethod(Currency currency)
        {
            Currency = currency;

            Price = new Money(currency);
            DiscountAmount = new Money(currency);
            TaxDetails = new List<TaxDetail>();
            Discounts = new List<Discount>();
        }


        /// <summary>
        /// Gets or sets the value of payment gateway code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the value of payment method name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of payment method logo absolute URL
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the value of payment method description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value of payment method type
        /// </summary>
        /// <value>
        /// "Unknown", "Standard", "Redirection", "PreparedForm"
        /// </value>
        public string PaymentMethodType { get; set; }

        /// <summary>
        /// Gets or sets the value of payment method group type
        /// </summary>
        /// <value>
        /// "Paypal", "BankCard", "Alternative", "Manual"
        /// </value>
        public string PaymentMethodGroupType { get; set; }

        /// <summary>
        /// Gets or sets the value of payment method priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Is payment method available for partial payments
        /// </summary>
        public bool IsAvailableForPartial { get; set; }

        /// <summary>
        /// Custom properties for payment method
        /// </summary>
        public List<SettingEntry> Settings { get; set; }

        public Currency Currency { get; set; }

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
            var paymentTaxRate = taxRates.FirstOrDefault(x => x.Line.Id != null && x.Line.Id.EqualsInvariant(Code ?? ""));
            if (paymentTaxRate != null)
            {
                if (paymentTaxRate.PercentRate > 0)
                {
                    TaxPercentRate = paymentTaxRate.PercentRate;
                }
                else
                {
                    var amount = Total.Amount > 0 ? Total.Amount : Price.Amount;
                    if (amount > 0)
                    {
                        TaxPercentRate = TaxRate.TaxPercentRound(paymentTaxRate.Rate.Amount / amount);
                    }
                }

                TaxDetails = paymentTaxRate.Line.TaxDetails;
            }
        }
        #endregion

        #region IDiscountable Members
        public IList<Discount> Discounts { get; private set; }

        public void ApplyRewards(IEnumerable<PromotionReward> rewards)
        {
            var paymentRewards = rewards.Where(r => r.RewardType == PromotionRewardType.PaymentReward && (r.PaymentMethodCode.IsNullOrEmpty() || r.PaymentMethodCode.EqualsInvariant(Code)));

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

        public override object Clone()
        {
            var result = base.Clone() as PaymentMethod;
            result.Price = Price?.Clone() as Money;
            result.DiscountAmount = DiscountAmount?.Clone() as Money;
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
