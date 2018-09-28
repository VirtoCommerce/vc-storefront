using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class TaxDetail : ValueObject
    {
        public TaxDetail(Currency currency)
        {
            Rate = new Money(currency);
            Amount = new Money(currency);
        }
        public Money Rate { get; set; }

        public Money Amount { get; set; }

        public string Name { get; set; }

        public override object Clone()
        {
            var result = base.Clone() as TaxDetail;
            result.Rate = Rate?.Clone() as Money;
            result.Amount = Amount?.Clone() as Money;
            return result;
        }
    }
}
