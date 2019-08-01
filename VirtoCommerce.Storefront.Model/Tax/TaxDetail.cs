using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class TaxDetail : ValueObject
    {
        public TaxDetail(Currency currency)
        {
            Amount = new Money(currency);
        }
        public decimal Rate { get; set; }

        public Money Amount { get; set; }
        public string Title => Name;
        public decimal Price
        {
            get
            {
                return Amount.Amount * 100;
            }
        }

        public string Name { get; set; }

        public override object Clone()
        {
            var result = base.Clone() as TaxDetail;
            result.Rate = Rate;
            result.Amount = Amount?.Clone() as Money;
            return result;
        }
    }
}
