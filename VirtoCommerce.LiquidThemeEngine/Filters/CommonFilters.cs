using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scriban;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public class CommonFilters
    {
        #region Public Methods and Operators

        public static object Default(object input, object value)
        {
            return input ?? value;
        }

        public static string Json(object input)
        {
            if (input == null)
            {
                return null;
            }

            var serializedString = JsonConvert.SerializeObject(
               input,
               new JsonSerializerSettings()
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                   //TODO:
                   //ContractResolver = new RubyContractResolver(),
               });

            return serializedString;
        }

        public static string Render(TemplateContext context, string input)
        {
            if (input == null)
            {
                return null;
            }
            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var result = themeEngine.RenderTemplate(input, null, context.CurrentGlobal);
            return result;

        }

        #endregion


    }
}
