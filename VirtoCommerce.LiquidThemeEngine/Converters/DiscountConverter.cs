using VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class DiscountConverter
    {
        public static Discount ToShopifyModel(this Storefront.Model.Marketing.Discount discount)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidDiscount(discount);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Discount ToLiquidDiscount(Storefront.Model.Marketing.Discount discount)
        {
            var result = new Discount
            {
                Amount = discount.Amount.Amount * 100,
                Code = discount.PromotionId,
                Id = discount.PromotionId,
                Savings = -discount.Amount.Amount * 100
            };

            return result;
        }
    }
}