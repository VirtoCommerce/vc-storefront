using System;
using System.Collections.Generic;
using Scriban;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// Filter used for localization 
    /// </summary>
    public partial class TranslationFilter
    {
        private static string[] _countSuffixes = new[] { ".zero", ".one", ".two" };


        #region Public Methods and Operators
        public static string T(TemplateContext context, string key, params object[] variables)
        {
            var result = key;
            //TODO: find out more elegant way to access localization resources
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var localization = themeAdaptor.ReadLocalization();

            if (localization != null)
            {
                result = (localization.SelectToken(key) ?? key).ToString();
                if (!variables.IsNullOrEmpty())
                {
                    result = string.Format(result, variables);
                }
            }

            return result;
        }
        #endregion

        private static string TryTransformKey(string input, Dictionary<string, object> variables)
        {
            var retVal = input;

            object countValue;
            if (variables.TryGetValue("count", out countValue) && countValue != null)
            {
                var count = Convert.ToUInt16(countValue);
                retVal += count < 2 ? _countSuffixes[count] : ".other";
            }
            return retVal;
        }
    }




}
