using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;

namespace VirtoCommerce.Storefront.Model
{
    public partial class TierPrice : ValueObject, ITaxable
    {
        public TierPrice(Currency currency)
            :this (new Money(currency), 0)
        {
        }
        public TierPrice(Money price, long quantity)
        {
            Currency = price.Currency;
            TaxDetails = new List<TaxDetail>();
            Price = price;
            DiscountAmount = new Money(price.Currency);

            Quantity = quantity;
        }

        public Money Price { get; set; }

        private Money _price;
        public Money PriceWithTax
        {
            get
            {
                return _price ?? Price;
            }
            set
            {
                _price = value;
            }
        }      

        public Money DiscountAmount { get; set; }
        public Money DiscountAmountWithTax
        {
            get
            {
                return DiscountAmount + DiscountAmount * TaxPercentRate;
            }
        }

        /// <summary>
        /// Actual price includes all kind of discounts
        /// </summary>
        public Money ActualPrice
        {
            get
            {
                return Price - DiscountAmount;
            }
        }

        public Money ActualPriceWithTax
        {
            get
            {
                return PriceWithTax - DiscountAmountWithTax;
            }
        }

        public long Quantity { get; set; }



        #region ITaxable Members
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the value of total shipping tax amount
        /// </summary>
        public Money TaxTotal
        {
            get
            {
                return ActualPriceWithTax - ActualPrice;
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
            var shipmentTaxRate = taxRates.FirstOrDefault(x => x.Line.Quantity == Quantity);
            if (shipmentTaxRate != null && ActualPrice.Amount > 0 && shipmentTaxRate.Rate.Amount > 0)
            {
                TaxPercentRate = TaxRate.TaxPercentRound(shipmentTaxRate.Rate.Amount / ActualPrice.Amount);
            }
        }
        #endregion

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Price;
            yield return DiscountAmount;
            yield return TaxPercentRate;
            yield return Quantity;           

        }
    }
}