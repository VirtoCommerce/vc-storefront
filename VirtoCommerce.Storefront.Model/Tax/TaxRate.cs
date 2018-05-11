using System;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class TaxRate
    {
        public TaxRate(Currency currency)
        {
            Rate = new Money(currency);
        }
        public Money Rate { get; set; }
        public TaxLine Line { get; set; }

        public static decimal TaxPercentRound(decimal percent)
        {
            return Math.Round(percent, 4, MidpointRounding.AwayFromZero);
        }
    }
}
