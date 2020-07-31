namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class DiscountDto
    {
        public string PromotionId { get; set; }
        public string Currency { get; set; }
        public double? DiscountAmount { get; set; }
        public double? DiscountAmountWithTax { get; set; }
        public string Coupon { get; set; }
        public string Description { get; set; }
    }
}
