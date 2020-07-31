namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class MoneyDto
    {
        public decimal? Amount { get; set; }

        public int DecimalDigits { get; set; }

        public string FormattedAmount { get; set; }

        public string FormattedAmountWithoutCurrency { get; set; }

        public string FormattedAmountWithoutPoint { get; set; }

        public string FormattedAmountWithoutPointAndCurrency { get; set; }
    }
}
