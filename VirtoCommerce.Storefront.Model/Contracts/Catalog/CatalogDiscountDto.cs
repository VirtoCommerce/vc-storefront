namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class CatalogDiscountDto
    {
        public MoneyDto Amount { get; set; }
        public MoneyDto AmountWitTax { get; set; }
        public string Coupon { get; set; }
        public string Description { get; set; }
        public PromotionDto Promotion { get; set; }
        public string PromotionId { get; set; }
    }
}
