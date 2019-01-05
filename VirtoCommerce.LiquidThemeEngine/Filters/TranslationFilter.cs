using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using Scriban;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// Filter used for localization 
    /// </summary>
    public class TranslationFilter
    {
        #region Public Methods and Operators
        public static string T(TemplateContext context, string key, params object[] variables)
        {
            var retVal = key;
            //TODO: find out more elegant way to access localization resources
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var localization = themeAdaptor.ReadLocalization();

            if (localization != null)
            {
                retVal = (localization.SelectToken(key) ?? key).ToString();
            }
            return retVal;
        }
        #endregion


    }




}
