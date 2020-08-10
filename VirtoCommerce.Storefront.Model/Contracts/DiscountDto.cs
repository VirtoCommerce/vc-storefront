namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class DiscountDto
    {
        public string PromotionId { get; set; }
        public string Currency { get; set; }
        public double? Amount { get; set; }
        public double? AmountWithTax { get; set; }
        public string Coupon { get; set; }
        public string Description { get; set; }
    }
}
