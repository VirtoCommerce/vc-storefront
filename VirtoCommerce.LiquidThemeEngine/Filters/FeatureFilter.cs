namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    using global::Scriban;

    public class FeatureFilter
    {
        public static string IsFeatureActive(TemplateContext context, string key, params object[] variables)
        {
            bool result;

            if (!(context.TemplateLoader is ShopifyLiquidThemeEngine themeEngine))
            {
                result = false;
            }
            else
            {
                result = themeEngine.IsFeatureActive(key);
            }

            return result ? "true" : "false";
        }
    }
}
