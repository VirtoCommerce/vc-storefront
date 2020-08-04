namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class TierPriceDto
    {
        public MoneyDto Price { get; set; }
        public MoneyDto PriveWithTax { get; set; }
        public long? Quantity { get; set; }
    }
}
