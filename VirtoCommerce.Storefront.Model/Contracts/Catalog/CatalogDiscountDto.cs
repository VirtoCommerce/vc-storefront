namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class CatalogDiscountDto
    {
        public decimal Amount { get; set; }
        public decimal AmountWitTax { get; set; }
        public string Coupon { get; set; }
        public string Description { get; set; }
        public PromotionDto Promotion { get; set; }
        public string PromotionId { get; set; }
    }
}
