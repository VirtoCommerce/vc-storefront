using Scriban;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// Filter used for localization 
    /// </summary>
    public static partial class TranslationFilter
    {
        public static string T(TemplateContext context, string key, params object[] variables)
        {
            var result = key;
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var localization = themeAdaptor.ReadLocalization();

            if (localization != null)
            {
                //Backward compatibility "" | t returns entire localization JSON
                if (string.IsNullOrEmpty(key))
                {
                    result = localization.ToString();
                }
                else if (key.IsValidJsonPath())
                {
                    result = (localization.SelectToken(key, errorWhenNoMatch: false) ?? key).ToString();
                    if (!variables.IsNullOrEmpty())
                    {
                        result = string.Format(result, variables);
                    }
                }
            }

            return result;
        }
    }




}
