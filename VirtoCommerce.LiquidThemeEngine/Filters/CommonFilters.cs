using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

            var contents = Hash.FromAnonymousObject(new { input });
            var serializedString = JsonConvert.SerializeObject(
                contents["input"],
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new RubyContractResolver(),
                });

            return serializedString;
        }

        public static string Render(Context context, string input)
        {
            if (input == null)
            {
                return null;
            }
            var themeEngine = (ShopifyLiquidThemeEngine)Template.FileSystem;
            var renderParams = context.Environments[0].ToDictionary(x => x.Key, x => x.Value);
            var result = themeEngine.RenderTemplate(input.ToString(), renderParams);
            return result;
        }

        #endregion


    }
}
