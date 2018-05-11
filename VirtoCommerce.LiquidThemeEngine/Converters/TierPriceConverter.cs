using VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class TierPriceConverter
    {
        public static TierPrice ToShopifyModel(this Storefront.Model.TierPrice tierPrice)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidTierPrice(tierPrice);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual TierPrice ToLiquidTierPrice(Storefront.Model.TierPrice tierPrice)
        {
            var result = new TierPrice();
            result.Price = tierPrice.Price.Amount * 100;
            result.Quantity = tierPrice.Quantity;
            return result;
        }
    }
}