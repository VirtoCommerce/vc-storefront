using VirtoCommerce.LiquidThemeEngine.Objects;
using storefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ImageStaticConverter
    {
        public static Image ToShopifyModel(this storefrontModel.Image image)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidImage(image);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Image ToLiquidImage(storefrontModel.Image image)
        {
            var result = new Image();
            result.Alt = image.Alt;
            result.Name = image.Title;
            result.Src = image.Url;

            return result;
        }
    }

}
