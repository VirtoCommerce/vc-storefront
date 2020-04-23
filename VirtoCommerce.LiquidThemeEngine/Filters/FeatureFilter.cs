namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    using System;

    using global::Scriban;

    using Newtonsoft.Json.Linq;

    public class FeatureFilter
    {
        public static string IsFeatureActive(TemplateContext context, string key, params object[] variables)
        {
            if (string.IsNullOrEmpty(key))
            {
                return IsFeaturesActive(context, key, variables);
            }

            string result;
            if (!(context.TemplateLoader is ShopifyLiquidThemeEngine themeEngine))
            {
                result = Convert.ToString(false);
            }
            else
            {
                result = IsFeatureActive(themeEngine, key);
            }

            return result;
        }

        public static string IsFeaturesActive(TemplateContext context, string key, params object[] variables)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return IsFeatureActive(context, key, variables);
            }

            string result;
            if (!(context.TemplateLoader is ShopifyLiquidThemeEngine themeEngine))
            {
                result = Convert.ToString(false);
            }
            else
            {
                var jObject = new JObject();

                foreach (string featureName in variables)
                {
                    var isFeatureActiveResult = IsFeatureActive(themeEngine, featureName);
                    jObject.Add(featureName, isFeatureActiveResult);
                }

                result = jObject.ToString();
            }

            return result;
        }

        private static string IsFeatureActive(ShopifyLiquidThemeEngine themeEngine, string key)
        {
            var isActive = themeEngine.IsFeatureActive(key);
            return Convert.ToString(isActive);
        }
    }
}
