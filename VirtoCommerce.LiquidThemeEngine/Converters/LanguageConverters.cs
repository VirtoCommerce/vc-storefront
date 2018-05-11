using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class LanguageConverter
    {
        public static Language ToShopifyModel(this StorefrontModel.Language language)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidLanguage(language);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Language ToLiquidLanguage(StorefrontModel.Language language)
        {
            var result = new Language();
            result.CultureName = language.CultureName;
            result.NativeName = language.NativeName;
            result.ThreeLeterLanguageName = language.ThreeLeterLanguageName;
            result.ThreeLetterRegionName = language.ThreeLetterRegionName;
            result.TwoLetterLanguageName = language.TwoLetterLanguageName;
            result.TwoLetterRegionName = language.TwoLetterRegionName;

            return result;
        }
    }
}