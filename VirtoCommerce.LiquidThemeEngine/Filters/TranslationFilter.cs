using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
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
            //TODO: find out more elegant way to access localization resources
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var localization = themeAdaptor.ReadLocalization();

            if (localization != null)
            {
                //Backward compatibility "" | t returns whole localization JSON
                //TODO: remove later
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
