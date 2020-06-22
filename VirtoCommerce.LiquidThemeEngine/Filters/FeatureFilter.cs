using System.Collections.Generic;
using global::Scriban;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public class FeatureFilter
    {
        public static bool IsFeatureActive(TemplateContext context, string key, params object[] variables)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (context.TemplateLoader is ShopifyLiquidThemeEngine themeEngine)
            {
                return themeEngine.IsFeatureActive(key);
            }

            return false;
        }

        public static string IsFeaturesActive(TemplateContext context, string key, params object[] featureNames)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            switch (context.TemplateLoader)
            {
                case ShopifyLiquidThemeEngine themeEngine:
                    {
                        var featuresStateJsonObject = BuildFeaturesStateJsonObject(themeEngine, featureNames);

                        return featuresStateJsonObject.ToString();
                    }

                default:
                    return string.Empty;
            }
        }

        private static JObject BuildFeaturesStateJsonObject(ShopifyLiquidThemeEngine themeEngine, IEnumerable<object> featureNames)
        {
            var result = new JObject();

            foreach (string featureName in featureNames)
            {
                var featureActive = themeEngine.IsFeatureActive(featureName);
                result.Add(featureName, featureActive);
            }

            return result;
        }
    }
}
