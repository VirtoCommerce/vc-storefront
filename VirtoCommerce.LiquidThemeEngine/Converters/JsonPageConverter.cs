using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class JsonPageConverter
    {
        public static JsonPage ToShopifyModel(this StorefrontModel.JsonPage page)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidCmsPage(page);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual JsonPage ToLiquidCmsPage(StorefrontModel.JsonPage source)
        {
            var result = new JsonPage
            {
                Settings = source.Settings,
                Blocks = new List<IDictionary<string, object>>()
            };

            foreach (var block in source.Blocks)
            {
                var collection = ProcessBlockRecursively(block);
                result.Blocks.Add(collection);
            }

            return result;
        }

        private MetafieldsCollection ProcessBlockRecursively(IDictionary<string, object> block)
        {
            var result = new Dictionary<string, object>();
            foreach (var key in block.Keys)
            {
                if (block[key] is Newtonsoft.Json.Linq.JArray)
                {
                        var array = (Newtonsoft.Json.Linq.JArray)block[key];
                        var resultArray = new List<IDictionary<string, object>>();
                        foreach (Newtonsoft.Json.Linq.JObject item in array)
                        {
                            var listItem = item.ToObject<Dictionary<string, object>>();
                            resultArray.Add(ProcessBlockRecursively(listItem));
                        }
                        result.Add(key, resultArray);
                }
                else
                {
                    result.Add(key, block[key]);
                }
            }
            return new MetafieldsCollection(string.Empty, result);
        }
    }
}
